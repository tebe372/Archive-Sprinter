using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.ComputationManager.Calculations;
using AS.Core.Models;
using Newtonsoft.Json;
using AS.SampleDataManager;
using AS.Core.ViewModels;
using System.Numerics;

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
        public virtual string Name { get; set; }
        [JsonExtensionData]
        private Dictionary<string, object> Parameters = new Dictionary<string, object> { };

        public void AddParameter(string paramName, string paramValue)
        {
            Parameters[paramName] = paramValue;
        }

        public abstract List<Signal> Process(List<Signal> e);

        public abstract bool CheckStepIsComplete();
        public List<string> InputSignals { get; set; }
        public List<Signal> OutputSignals { get; set; }
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
        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public string TypeAbbreviation { get; set; }
        public string Unit { get; set; }
        public int SamplingRate { get; set; }
        public double Scalar { get; set; }
        public PMUWithSamplingRate TimeSourcePMU { get; set; }
        public Dictionary<string, Signal> OneToOneSignalPairs { get; set; }
        public Dictionary<string, CreatPhasorInputOutputSignals> OneToTwoSignalPairs { get; set; }

        public override List<Signal> Process(List<Signal> e)
        {
            return new List<Signal>();
        }
    }
    public class PMUflagFilt : Filter
    {
        public int FlagBit { get; set; }
        public override string Name { get { return "Status Flags"; } }
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
        public override string Name { get { return "Zeros"; } }
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
        public override string Name { get { return "Missing"; } }
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
        public override string Name { get { return "Nominal Voltage"; } }
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
        public override string Name { get { return "Nominal Frequency"; } }
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
        public override string Name { get { return "Outliers"; } }
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
                    Filters.OutlierFilt(signal, StdDevMult);
                    filteredSignal.Add(signal);
                }               
            }
            return filteredSignal;
        }
        public double StdDevMult { get; set; }
    }
    public class StaleDQFilt : Filter
    {
        public int FlagBit { get; set; }
        public override string Name { get { return "Stale Data"; } }
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
                    Filters.StaleDQFilt(signal, StaleThresh);
                    filteredSignal.Add(signal);
                }
            }
            return filteredSignal;
        }
        public int StaleThresh { get; set; }
        //public bool FlagAllByFreq { get; set; }
    }
    public class DataFrameDQFilt : Filter
    {
        public int FlagBit { get; set; }
        public override string Name { get { return "Data Frame"; } }
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
                    filteredSignal.Add(signal);
                }
            }
            if (KeepDiffPMUSeparate)
            {
                var groupbyPMU = filteredSignal.GroupBy(x => x.PMUName).ToDictionary(y => y.Key, y => y.ToList());
                foreach (var g in groupbyPMU)
                {
                    Filters.DataFrameDQFilt(g.Value, PercentBadThresh);
                }
            }
            else
            {
                Filters.DataFrameDQFilt(filteredSignal, PercentBadThresh);
            }

            return filteredSignal;
        }
        public double PercentBadThresh { get; set; }
        public bool KeepDiffPMUSeparate { get; set; }
    }
    public class PMUchanDQFilt : DataFrameDQFilt
    {
        public int FlagBit { get; set; }
        public override string Name { get { return "Channel"; } }
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
                    Filters.PMUchanDQFilt(signal, PercentBadThresh);
                    filteredSignal.Add(signal);
                }                
            }
            return filteredSignal;
        }
    }
    public class PMUallDQFilt : DataFrameDQFilt
    {
        public int FlagBit { get; set; }
        public override string Name { get { return "Entire PMU"; } }
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
                        filteredSignal.Add(signal);
                    }
            }
            if (KeepDiffPMUSeparate)
            {
                var groupbyPMU = filteredSignal.GroupBy(x => x.PMUName).ToDictionary(y => y.Key, y => y.ToList());
                foreach (var g in groupbyPMU)
                {
                    Filters.PMUallDQFilt(g.Value, PercentBadThresh);
                }
            }
            else
            {
                Filters.PMUallDQFilt(filteredSignal, PercentBadThresh);
            }
            return filteredSignal;
        }
    }
    //public class WrappingFailureDQFilt : Filter
    //{
    //    public int FlagBit { get; set; }
    //    public new string Name { get { return "Angle Wrapping"; } }
    //    public override List<Signal> Process(List<Signal> e)
    //    {
    //        List<Signal> filteredSignal = new List<Signal>();
    //        //do some parameter process
    //        //according to the input channels that is selected, call the actual function and process each signal.
    //        foreach (var signal in e)
    //        {
    //            var type = signal.TypeAbbreviation;
    //            if (type.Length == 1 && type == "F")
    //            {
    //                var name = signal.PMUName + "_" + signal.SignalName;
    //                if (InputSignals.Contains(name))
    //                {
    //                    Filters.WrappingFailureDQFilt(signal, AngleThresh);
    //                    filteredSignal.Add(signal);
    //                }
    //            }
    //        }
    //        return filteredSignal;
    //    }
    //    public string AngleThresh { get; set; }
    //}
    public class ScalarRepCust : Customization
    {
        public override string Name { get => "Scalar Repetition"; }
        public List<Signal> Process(List<Signal> e, DateTime t)
        {
            List<Signal> customizedSignal = new List<Signal>();
            var newSignal = new Signal(PMUName, SignalName);
            newSignal.TypeAbbreviation = TypeAbbreviation;
            newSignal.Unit = Unit;
            newSignal.SamplingRate = TimeSourcePMU.SamplingRate;
            OutputSignals[0] = Customizations.ScalarRepCustomization(newSignal, Scalar, TimeSourcePMU.SignalLength, t);
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
    public class AdditionCust : Customization
    {
        public override string Name { get => "Addition"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            var newSignal = new Signal(PMUName, SignalName);
            newSignal.TypeAbbreviation = TypeAbbreviation;
            newSignal.Unit = Unit;
            newSignal.SamplingRate = SamplingRate;
            bool firstSignal = true;
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    if (firstSignal)
                    {
                        newSignal.Data = signal.Data;
                        newSignal.TimeStamps = signal.TimeStamps;
                        newSignal.TimeStampNumber = signal.TimeStampNumber;
                        firstSignal = false;
                    }
                    else
                    {
                        if (newSignal.Data.Count == signal.Data.Count)
                        {
                            newSignal.Data = Customizations.AdditionCustomization(newSignal.Data, signal.Data);
                        }
                        else
                        {
                            newSignal.Data = null;
                            newSignal.SamplingRate = -1;
                            //throw new Exception("Signals do not have the same length in addition customization.");
                        }
                    }
                }
            }
            OutputSignals[0] = newSignal;
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
    public class SubtractionCust : Customization
    {
        public override string Name { get { return "Subtraction"; } }
        public Signal Subtrahend { get; set; }
        public Signal Minuend { get; set; }

        //public string SignalName { get; set; } //validation of signal name and pmu name should have been done in the view model?
        //public string PMUName { get; set; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            //find the 2 input channels according to the customer selection for the

            //validate the subtrahend and minuend in the ViewModel when user first select the signal

            var newSignal = new Signal(PMUName, SignalName);
            newSignal.TypeAbbreviation = TypeAbbreviation;
            newSignal.Unit = Unit;
            newSignal.SamplingRate = SamplingRate;

            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    if (signal.SignalName == Minuend.SignalName && signal.PMUName == Minuend.PMUName)
                    {
                        Minuend = signal;
                    }
                    if (signal.SignalName == Subtrahend.SignalName && signal.PMUName == Subtrahend.PMUName)
                    {
                        Subtrahend = signal;
                    }
                }
            }
            if (Subtrahend.Data.Count == Minuend.Data.Count)
            {
                newSignal.Data = Customizations.SubtractionCustomization(Subtrahend.Data, Minuend.Data);
                newSignal.TimeStampNumber = Subtrahend.TimeStampNumber;
                newSignal.TimeStamps = Subtrahend.TimeStamps;
                //flag?
            }
            else
            {
                newSignal.Data = null;  //matlab set it to NaN
                newSignal.SamplingRate = -1;
                //flag?
            }
            OutputSignals[0] = newSignal;
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
    public class MultiplicationCust : Customization
    {
        public override string Name { get => "Multiplication"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            var newSignal = new Signal(PMUName, SignalName);
            newSignal.TypeAbbreviation = TypeAbbreviation;
            newSignal.Unit = Unit;
            newSignal.SamplingRate = SamplingRate;
            bool firstSignal = true;
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    if (firstSignal)
                    {
                        newSignal.Data = signal.Data;
                        newSignal.TimeStamps = signal.TimeStamps;
                        newSignal.TimeStampNumber = signal.TimeStampNumber;
                        firstSignal = false;
                    }
                    else
                    {
                        if (newSignal.Data.Count == signal.Data.Count)
                        {
                            newSignal.Data = Customizations.MultiplicationCustomization(newSignal.Data, signal.Data);
                        }
                        else
                        {
                            newSignal.Data = null;
                            newSignal.SamplingRate = -1;
                            //throw new Exception("Signals do not have the same length in addition customization.");
                        }
                    }
                }
            }
            OutputSignals[0] = newSignal;
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
    public class DivisionCust : Customization
    {
        public override string Name { get => "Division"; }
        public Signal Dividend { get; set; }
        public Signal Divisor { get; set; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            var newSignal = new Signal(PMUName, SignalName);
            newSignal.TypeAbbreviation = TypeAbbreviation;
            newSignal.Unit = Unit;
            newSignal.SamplingRate = SamplingRate;

            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    if (signal.SignalName == Dividend.SignalName && signal.PMUName == Dividend.PMUName)
                    {
                        Dividend = signal;
                    }
                    if (signal.SignalName == Divisor.SignalName && signal.PMUName == Divisor.PMUName)
                    {
                        Divisor = signal;
                    }
                }
            }
            if (Divisor.Data.Count == Dividend.Data.Count)
            {
                newSignal.Data = Customizations.DivisionCustomization(Dividend.Data, Divisor.Data);
                newSignal.TimeStampNumber = Divisor.TimeStampNumber;
                newSignal.TimeStamps = Divisor.TimeStamps;
                //flag?
            }
            else
            {
                newSignal.Data = null;  //matlab set it to NaN
                newSignal.SamplingRate = -1;
                //flag?
            }
            OutputSignals[0] = newSignal;
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
    public class ExponentialCust : Customization
    {
        public override string Name { get => "Exponential"; }
        public double Exponent { get; set; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();

            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    newSignal.Data = Customizations.ExpCustomization(signal.Data, Exponent);
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class SignReversalCust : Customization
    {
        public override string Name { get => "Sign Reversal"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.SignReversalCustomization(signal.Data);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.ComplexData = Customizations.SignReversalCustomization(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class AbsValCust : Customization
    {
        public override string Name { get => "Absolute Value"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.AbsValCustomization(signal.Data);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.Data = Customizations.AbsValCustomization(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class RealComponentCust : Customization
    {
        public override string Name { get => "Real Component"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.RealComponentCustomization(signal.Data);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.Data = Customizations.RealComponentCustomization(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class ImagComponentCust : Customization
    {
        public override string Name { get => "Imaginary Component"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.ImagComponentCustomization(signal.Data);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.Data = Customizations.ImagComponentCustomization(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class AngleCust : Customization
    {
        public override string Name { get => "Angle Calculation"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.AngleCalCustomization(signal.Data);
                    }
                    else
                    {
                        newSignal.Data = Customizations.AngleCalCustomization(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class ComplexConjCust : Customization
    {
        public override string Name { get => "Complex Conjugate"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.ComplexConjugateCustomization(signal.Data);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.ComplexData = Customizations.ComplexConjugateCustomization(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class CreatePhasorCust : Customization
    {
        public override string Name { get => "Phasor Creation"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            List<CreatPhasorInputOutputSignals> pairs = new List<CreatPhasorInputOutputSignals>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var pair = OneToTwoSignalPairs[name];
                    var type = signal.TypeAbbreviation.ToArray();
                    if (type[1] == 'M')
                    {
                        pair.Mag = signal;
                    }
                    if (type[1] == 'A')
                    {
                        pair.Ang = signal;
                    }
                    if (!pairs.Contains(pair))
                    {
                        pairs.Add(pair);
                    }
                } 
            }
            foreach (var item in pairs)
            {
                var newSignal = new Signal(PMUName, item.Output.SignalName);
                if (item.Ang.Unit == "DEG")
                {
                    item.Ang.Data = Customizations.AngleUnitConversionCustomizationForDeg(item.Ang.Data);
                    item.Ang.Unit = "RAD";
                }
                if (item.Mag.Data.Count == item.Ang.Data.Count)
                {
                    newSignal.ComplexData = Customizations.CreatePhasorCustomization(item.Mag.Data, item.Ang.Data);
                    newSignal.TimeStampNumber = item.Mag.TimeStampNumber;
                    newSignal.TimeStamps = item.Mag.TimeStamps;
                    newSignal.SamplingRate = item.Output.SamplingRate;
                }
                else
                {
                    newSignal.ComplexData = null;
                    newSignal.SamplingRate = -1;
                }
                newSignal.TypeAbbreviation = item.Output.TypeAbbreviation;
                newSignal.Unit = item.Output.Unit;
                OutputSignals.Remove(item.Output);
                OutputSignals.Add(newSignal);
                customizedSignal.Add(newSignal);
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class PowerCalcCust : Customization
    {
        public override string Name { get => "Power Calculation"; }
        public PowerType PowType { get; set; }
        public bool IsFromPhasor { get; set; }
        public Dictionary<string, string> SignalNameTypeDict { get; set; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            Dictionary<string, Signal> TypeSignalDict = new Dictionary<string, Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    TypeSignalDict[SignalNameTypeDict[name]] = signal;
                }
            }
            var newSignal = new Signal(PMUName, SignalName);
            List<Complex> complexPower = null;
            if (IsFromPhasor)
            {
                var vp = TypeSignalDict["vp"];
                if (vp.Unit == "V")
                {
                    for (int i = 0; i < vp.Data.Count; i++)
                    {
                        vp.Data[i] = vp.Data[i] / 1000;
                    }
                }
                var ip = TypeSignalDict["ip"];
                if (ip.Unit == "A")
                {
                    for (int i = 0; i < ip.Data.Count; i++)
                    {
                        ip.Data[i] = ip.Data[i] / 1000;
                    }
                }
                if (vp.ComplexData.Count == ip.ComplexData.Count)
                {
                    complexPower = Customizations.PowerFromPhasorCustomization(vp.ComplexData, ip.ComplexData);
                    newSignal.TimeStampNumber = vp.TimeStampNumber;
                    newSignal.TimeStamps = vp.TimeStamps;
                }
                else
                {
                    SamplingRate = -1;
                }
            }
            else
            {
                List<Complex> vp = null;
                var vm = TypeSignalDict["vm"];
                var va = TypeSignalDict["va"];
                if (vm.Unit == "V")
                {
                    for (int i = 0; i < vm.Data.Count; i++)
                    {
                        vm.Data[i] = vm.Data[i] / 1000;
                    }
                }
                if (va.Unit == "DEG")
                {
                    va.Data = Customizations.AngleUnitConversionCustomizationForDeg(va.Data);
                    va.Unit = "RAD";
                }
                if (vm.Data.Count == va.Data.Count)
                {
                    vp = Customizations.CreatePhasorCustomization(vm.Data, va.Data);
                }
                List<Complex> ip = null;
                var im = TypeSignalDict["im"];
                var ia = TypeSignalDict["ia"];
                if (im.Unit == "A")
                {
                    for (int i = 0; i < im.Data.Count; i++)
                    {
                        im.Data[i] = im.Data[i] / 1000;
                    }
                }
                if (ia.Unit == "DEG")
                {
                    ia.Data = Customizations.AngleUnitConversionCustomizationForDeg(ia.Data);
                    ia.Unit = "RAD";
                }
                if (im.Data.Count == ia.Data.Count)
                {
                    ip = Customizations.CreatePhasorCustomization(im.Data, ia.Data);
                }
                if (vp != null && ip != null && vp.Count == ip.Count)
                {
                    complexPower = Customizations.PowerFromPhasorCustomization(vp, ip);
                    newSignal.TimeStampNumber = vm.TimeStampNumber;
                    newSignal.TimeStamps = vm.TimeStamps;
                }
                else
                {
                    SamplingRate = -1;
                }
            }
            if (complexPower != null)
            {
                switch (PowType)
                {
                    case PowerType.CP:
                        newSignal.ComplexData = complexPower;
                        break;
                    case PowerType.S:
                        newSignal.Data = Customizations.AbsValCustomization(complexPower);
                        break;
                    case PowerType.P:
                        newSignal.Data = Customizations.RealComponentCustomization(complexPower);
                        break;
                    case PowerType.Q:
                        newSignal.Data = Customizations.ImagComponentCustomization(complexPower);
                        break;
                    default:
                        break;
                }
            }
            newSignal.TypeAbbreviation = TypeAbbreviation;
            newSignal.Unit = Unit;
            newSignal.SamplingRate = SamplingRate;
            OutputSignals[0] = newSignal;
            e.Add(newSignal);
            customizedSignal.Add(newSignal);
            return customizedSignal;
        }
    }
    public class SpecifySignalTypeUnitCust : Customization
    {
        public override string Name { get => "Signal Type/Unit"; }
        public Signal Input { get; set; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var newSignal = new Signal(PMUName, SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = new List<double>(signal.Data);
                    }
                    else
                    {
                        newSignal.ComplexData = new List<Complex>(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = TypeAbbreviation;
                    newSignal.Unit = Unit;
                    newSignal.SamplingRate = SamplingRate;
                    OutputSignals[0] = newSignal;
                    e.Add(newSignal);
                    customizedSignal.Add(newSignal);
                    break;
                }
            }
            return customizedSignal;
        }
    }
    public class MetricPrefixCust : Customization
    {
        public override string Name { get => "Metric Prefix"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, output.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = Customizations.MetricPrefixCustomization(signal.Data, signal.Unit, output.Unit);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.ComplexData = Customizations.MetricPrefixCustomization(signal.ComplexData, signal.Unit, output.Unit);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.Unit = output.Unit;
                    newSignal.SamplingRate = output.SamplingRate;
                    newSignal.PMUName = output.PMUName;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class AngleConversionCust : Customization
    {
        public override string Name { get => "Angle Conversion"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(output.PMUName, output.SignalName);
                    switch (signal.Unit)
                    {
                        case "DEG":
                            newSignal.Data = Customizations.AngleUnitConversionCustomizationForDeg(signal.Data);
                            newSignal.Unit = "RAD";
                            break;
                        case "RAD":
                            newSignal.Data = Customizations.AngleUnitConversionCustomizationForRad(signal.Data);
                            newSignal.Unit = "DEG";
                            break;
                        default:
                            throw new Exception("Signal in " + signal.PMUName + "with name: " + signal.SignalName + "does not have an appropriate unit for angle conversion customization.");
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = output.TypeAbbreviation;
                    newSignal.SamplingRate = output.SamplingRate;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class SignalReplicationCust : Customization
    {
        public override string Name { get => "Duplicate Signals"; }
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, signal.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        newSignal.Data = new List<double>(signal.Data);
                    }
                    if (signal.ComplexData.Count > 0)
                    {
                        newSignal.ComplexData = new List<Complex>(signal.ComplexData);
                    }
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = signal.TypeAbbreviation;
                    newSignal.Unit = signal.Unit;
                    newSignal.SamplingRate = signal.SamplingRate;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class UnWrapCust : Customization
    {
        public override string Name { get => "UnWrap Angles"; }
        private Dictionary<string, double> _initValues = new Dictionary<string, double>();
        public override List<Signal> Process(List<Signal> e)
        {
            List<Signal> customizedSignal = new List<Signal>();
            foreach (var signal in e)
            {
                var name = signal.PMUName + "_" + signal.SignalName;
                if (InputSignals.Contains(name))
                {
                    if (!_initValues.ContainsKey(name))
                    {
                        _initValues[name] = double.NaN;
                    }
                    var initValue = _initValues[name];
                    var output = OneToOneSignalPairs[name];
                    OutputSignals.Remove(output);
                    var newSignal = new Signal(PMUName, signal.SignalName);
                    if (signal.Data.Count > 0)
                    {
                        if (signal.Unit == "DEG")
                        {
                            signal.Data = Customizations.AngleUnitConversionCustomizationForDeg(signal.Data);
                            signal.Unit = "RAD";
                        }
                        newSignal.Data = Customizations.UnWrapCustomization(signal.Data, initValue);
                    }
                    _initValues[name] = newSignal.Data.Last();
                    newSignal.TimeStampNumber = signal.TimeStampNumber;
                    newSignal.TimeStamps = signal.TimeStamps;
                    newSignal.TypeAbbreviation = signal.TypeAbbreviation;
                    newSignal.Unit = signal.Unit;
                    newSignal.SamplingRate = signal.SamplingRate;
                    OutputSignals.Add(newSignal);
                    customizedSignal.Add(newSignal);
                }
            }
            foreach (var item in customizedSignal)
            {
                e.Add(item);
            }
            return customizedSignal;
        }
    }
    public class CreatPhasorInputOutputSignals
    {
        public CreatPhasorInputOutputSignals(Signal mag, Signal ang, Signal output)
        {
            Mag = mag;
            Ang = ang;
            Output = output;
        }
        public Signal Mag { get; set; }
        public Signal Ang { get; set; }
        public Signal Output { get; set; }
    }
}
