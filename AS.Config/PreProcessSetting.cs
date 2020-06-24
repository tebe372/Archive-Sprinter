using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.ComputationManager.Calculations;
using AS.Core.Models;
using Newtonsoft.Json;


namespace AS.Config
{
    public abstract class PreProcessStep
    {
        public PreProcessStep()
        {
            Name = "Undefined";
        }
        public PreProcessStep(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        [JsonExtensionData]
        private Dictionary<string, object> Parameters = new Dictionary<string, object> { };

        public void AddParameter(string paramName, string paramValue)
        {
            Parameters[paramName] = paramValue;
        }

        public abstract List<Signal> Process(List<Signal> e);

        public abstract bool CheckStepIsComplete();
        public List<string> InputSignals { get; set; }

    }
    // Data Quality Filter Class
    public class Filter: PreProcessStep
    {
        public Filter(string filterName) : base(filterName)
        {
            
        }

        public Filter() : base()
        {

        }
       
        public IList<SignalSignature> PMUs { get; set; }

        public override List<Signal> Process(List<Signal> e)
        {
            return new List<Signal>();
        }

        public override bool CheckStepIsComplete()
        {
            throw new NotImplementedException();
        }
        public bool SetToNaN { get; set; }
    }

    // Customization Class
    public class Customization : PreProcessStep
    {
        public Customization()
        {

        }
        public Customization(string customName) : base (customName)
        {
 
        }

        public override bool CheckStepIsComplete()
        {
            throw new NotImplementedException();
        }

        public override List<Signal> Process(List<Signal> e)
        {
            return new List<Signal>();
        }
    }
    public class PMUflagFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Status Flags"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            var groupbyPMU = e.GroupBy(x => x.PMUName).ToDictionary(y => y.Key, y => y.ToList());
            foreach (var gr in groupbyPMU)
            {
                if (InputSignals.Contains(gr.Key))
                {
                    for (int i = 0; i < gr.Value.Count(); i++)
                    {
                        if (i == 0)
                        {
                            Filters.PMUflagFilt(gr.Value[i]);
                        }
                        else
                        {
                            gr.Value[i].Flags = new List<bool>(gr.Value[0].Flags);
                        }
                        filteredSignal.Add(gr.Value[i]);
                    }
                }
            }
            return filteredSignal;
        }
    }
    public class DropOutZeroFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Zeros"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    Filters.DropOutZeroFilt(signal);
                    filteredSignal.Add(signal);
                }
            }
            return filteredSignal;
        }
    }
    public class DropOutMissingFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Missing"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    Filters.DropOutMissingFilt(signal);
                    filteredSignal.Add(signal);
                }
            }
            return filteredSignal;
        }
    }
    public class VoltPhasorFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Nominal Voltage"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            var groupbyPMU = e.GroupBy(x => x.TypeAbbreviation).ToDictionary(y => y.Key.Length < 3 ? y.Key : y.Key.Substring(0, 2), y => y.ToList());
            List<Signal> vms = null;
            if (groupbyPMU.ContainsKey("VM"))
            {
                vms = groupbyPMU["VM"];
                foreach (var signal in vms)
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.VoltPhasorFilt(signal, "VM", VoltMax, VoltMin, NomVoltage);
                        filteredSignal.Add(signal);
                    }
                }
            }
            if (groupbyPMU.ContainsKey("VP"))
            {
                var vps = groupbyPMU["VP"];
                foreach (var signal in vps)
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.VoltPhasorFilt(signal, "VP", VoltMax, VoltMin, NomVoltage);
                        filteredSignal.Add(signal);
                    }
                }
            }
            if (groupbyPMU.ContainsKey("VA"))
            {
                if (vms != null)
                {
                    var vas = groupbyPMU["VA"];
                    //var vpsNames = vps.Select(x => x.SignalName.Substring(0, x.SignalName.LastIndexOf('.')));
                    foreach (var signal in vas)
                    {
                        var name = signal.PMUName + "_" + signal.SignalName;
                        if (InputSignals.Contains(name))
                        {
                            var vpFound = false;
                            foreach (var vm in vms)
                            {
                                if (signal.SignalName.Substring(0, signal.SignalName.LastIndexOf('.')) == vm.SignalName.Substring(0, vm.SignalName.LastIndexOf('.')))
                                {
                                    signal.Flags = new List<bool>(vm.Flags);
                                    vpFound = true;
                                    break;
                                }
                            }
                            //how to find the corresponding magnitude?
                            if (vpFound)
                            {
                                filteredSignal.Add(signal);
                            }
                            else
                            {
                                throw new Exception("Cannot pass voltage angle signal: " + signal.SignalName + " through VoltPhasorFilt as corresponding voltage magnitude signal was not found.");
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Cannot pass voltage angle signal through VoltPhasorFilt as corresponding voltage magnitude signal was not found.");
                }
            }
            //foreach (var signal in e)
            //{
            //    var type = signal.TypeAbbreviation;
            //    if (type.Length > 1)
            //    {
            //        var tp = type.Substring(0, 2);
            //        var name = signal.PMUName + "_" + signal.SignalName;
            //        if (InputSignals.Contains(name))
            //        {
            //            if (tp == "VM" || tp == "VP")
            //            {
            //                Filters.VoltPhasorFilt(signal, tp, VoltMax, VoltMin, NomVoltage);
            //                filteredSignal.Add(signal);
            //            }
            //            else if (tp == "VA")
            //            {

            //            }
            //        }
            //    }
            //}
            return filteredSignal;
        }
        public double VoltMin { get; set; }
        public double VoltMax { get; set; }
        public double NomVoltage { get; set; }
    }
    public class FreqFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Nominal Frequency"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.FreqFilt(signal, FreqMaxChan, FreqMinChan, FreqPctChan, FreqMinSamp, FreqMaxSamp);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
        public double FreqMaxChan { get; set; }
        public double FreqMinChan { get; set; }
        public double FreqPctChan { get; set; }
        public double FreqMinSamp { get; set; }
        public double FreqMaxSamp { get; set; }
    }
    public class OutlierFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Outliers"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.OutlierFilt(signal, StdDevMult);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
        public double StdDevMult { get; set; }
    }
    public class StaleDQFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Stale Data"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.StaleDQFilt(signal, StaleThresh, FlagAllByFreq);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
        public string StaleThresh { get; set; }
        public bool FlagAllByFreq { get; set; }
    }
    public class DataFrameDQFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Data Frame"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.DataFrameDQFilt(signal, PercentBadThresh);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
        public string PercentBadThresh { get; set; }
    }
    public class PMUchanDQFilt : DataFrameDQFilt
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Channel"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.PMUchanDQFilt(signal, PercentBadThresh);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
    }
    public class PMUallDQFilt : DataFrameDQFilt
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Entire PMU"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.PMUallDQFilt(signal, PercentBadThresh);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
    }
    public class WrappingFailureDQFilt : Filter
    {
        public int FlagBit { get; set; }
        public new string Name { get { return "Angle Wrapping"; } }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> filteredSignal = new List<Signal>();
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                var type = signal.TypeAbbreviation;
                if (type.Length == 1 && type == "F")
                {
                    var name = signal.PMUName + "_" + signal.SignalName;
                    if (InputSignals.Contains(name))
                    {
                        Filters.WrappingFailureDQFilt(signal, AngleThresh);
                        filteredSignal.Add(signal);
                    }
                }
            }
            return filteredSignal;
        }
        public string AngleThresh { get; set; }
    }
    public class SubtractionCustomization : Customization
    {
        public Signal Subtrahend { get; set; }
        public Signal Minuend { get; set; }
        public string SignalName { get; set; } //validation of signal name and pmu name should have been done in the view model?
        public string PMUName { get; set; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            //find the 2 input channels according to the customer selection for the

            //validate the subtrahend and minuend in the ViewModel when user first select the signal

            var newSignal = new Signal(PMUName, SignalName);
            if (Subtrahend.TypeAbbreviation == Minuend.TypeAbbreviation)
            {
                newSignal.TypeAbbreviation = Subtrahend.TypeAbbreviation;
            }
            else
            {
                newSignal.TypeAbbreviation = "OTHER";
            }


            if (Subtrahend.Data.Count == Minuend.Data.Count && Subtrahend.Unit == Minuend.Unit)
            {
                newSignal.Data = Customizations.SubtractionCustomization(Subtrahend.Data, Minuend.Data);
                newSignal.SamplingRate = Subtrahend.SamplingRate;
                newSignal.Unit = Subtrahend.Unit;
                //flag?
            }
            else
            {
                newSignal.Unit = "OTHER";
                newSignal.Data = null;  //matlab set it to NaN
                newSignal.SamplingRate = -1;
                //flag?
            }
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
}
