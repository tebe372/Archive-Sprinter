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

        public abstract void Process(List<Signal> e);

        public abstract bool CheckStepIsComplete();

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

        public override void Process(List<Signal> e)
        {
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

        public override void Process(List<Signal> e)
        {

        }
    }

    public class DropOutZeroFilt : Filter
    {
        public int FlagBit { get; set; }
        public override void Process(List<Signal> e)
        {
            //do some parameter process
            //according to the input channels that is selected, call the actual function and process each signal.
            foreach (var signal in e)
            {
                Filters.DropOutZeroFilt(signal, SetToNaN);
            }
        }
    }
    public class SubtractionCustomization : Customization
    {
        public Signal Subtrahend { get; set; }
        public Signal Minuend { get; set; }
        public string SignalName { get; set; } //validation of signal name and pmu name should have been done in the view model?
        public string PMUName { get; set; }
        public override void Process(List<Signal> e)
        {
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
        }
    }
}
