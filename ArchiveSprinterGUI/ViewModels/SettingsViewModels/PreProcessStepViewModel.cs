using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Config;
using AS.Utilities;
using System.Collections.ObjectModel;
using AS.Core.ViewModels;
using AS.Core.Models;
using Newtonsoft.Json;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class PreProcessStepViewModel : StepViewModel
    {
        private PreProcessStep _model;
        [JsonIgnore]
        public PreProcessStep Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }      
        public string Name { get; set; }
        public SignalViewModel Minuend
        {
            get
            {
                if (parameters.ContainsKey("Minuend"))
                {
                    return parameters["Minuend"];
                }
                return null;
            }
            set
            {
                parameters["Minuend"] = value;
                OnPropertyChanged();
            }
        }
        public SignalViewModel Subtrahend
        {
            get
            {
                if (parameters.ContainsKey("Subtrahend"))
                {
                    return parameters["Subtrahend"];
                }
                return new SignalViewModel();
            }
            set
            {
                parameters["Subtrahend"] = value;
                OnPropertyChanged();
            }
        }
        public string NomVoltage 
        {
            get 
            {
                if (_model is VoltPhasorFilt)
                {
                    var m = _model as VoltPhasorFilt;
                    return m.NomVoltage.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as VoltPhasorFilt;
                    if (m.NomVoltage != v)
                    {
                        m.NomVoltage = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string VoltMin
        {
            get
            {
                if (_model is VoltPhasorFilt)
                {
                    var m = _model as VoltPhasorFilt;
                return m.VoltMin.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as VoltPhasorFilt;
                    if (m.VoltMin != v)
                    {
                        m.VoltMin = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string VoltMax
        {
            get
            {
                if (_model is VoltPhasorFilt)
                {
                    var m = _model as VoltPhasorFilt;
                return m.VoltMax.ToString();
            }
                else
                {
                return null;
            }
        }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as VoltPhasorFilt;
                    if (m.VoltMax != v)
                    {
                        m.VoltMax = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string FreqMaxChan
        {
            get
            {
                if (_model is FreqFilt)
                {
                    var m = _model as FreqFilt;
                    return m.FreqMaxChan.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as FreqFilt;
                    if (m.FreqMaxChan != v)
                    {
                        m.FreqMaxChan = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string FreqMinChan
        {
            get
            {
                if (_model is FreqFilt)
                {
                    var m = _model as FreqFilt;
                    return m.FreqMinChan.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as FreqFilt;
                    if (m.FreqMinChan != v)
                    {
                        m.FreqMinChan = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string FreqPctChan
        {
            get
            {
                if (_model is FreqFilt)
                {
                    var m = _model as FreqFilt;
                    return m.FreqPctChan.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as FreqFilt;
                    if (m.FreqPctChan != v)
                    {
                        m.FreqPctChan = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string FreqMinSamp
        {
            get
            {
                if (_model is FreqFilt)
                {
                    var m = _model as FreqFilt;
                    return m.FreqMinSamp.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as FreqFilt;
                    if (m.FreqMinSamp != v)
                    {
                        m.FreqMinSamp = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string FreqMaxSamp
        {
            get
            {
                if (_model is FreqFilt)
                {
                    var m = _model as FreqFilt;
                    return m.FreqMaxSamp.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as FreqFilt;
                    if (m.FreqMaxSamp != v)
                    {
                        m.FreqMaxSamp = v;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private Dictionary<string, SignalViewModel> parameters = new Dictionary<string,SignalViewModel>();

        private bool _isComplete;
        [JsonIgnore]
        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                _isComplete = value;
                OnPropertyChanged();
            }
        }
        public PreProcessStepViewModel() : base()
        {
            _isComplete = false;
        }
        [JsonConstructor]
        public PreProcessStepViewModel(string name) : this()
        {
            Name = name;
            switch (name)
            {
                case "Status Flags":
                    _model = new PMUflagFilt();
                    break;
                case "Zeros":
                    _model = new DropOutZeroFilt();
                    break;
                case "Missing":
                    _model = new DropOutMissingFilt();
                    break;
                case "Nominal Voltage":
                    _model = new VoltPhasorFilt();
                    break;
                case "Nominal Frequency":
                    _model = new FreqFilt();
                    break;
                case "Outliers":
                    _model = new OutlierFilt();
                    break;
                case "Stale Data":
                    _model = new StaleDQFilt();
                    break;
                case "Data Frame":
                    _model = new DataFrameDQFilt();
                    break;
                case "Channel":
                    _model = new PMUchanDQFilt();
                    break;
                case "Entire PMU":
                    _model = new PMUallDQFilt();
                    break;
                //case "Angle Wrapping":
                //    _model = new WrappingFailureDQFilt();
                //    break;
                case "Scalar Repetition":
                    _model = new ScalarRepCust();
                    break;
                case "Addition":
                    _model = new AdditionCust();
                    break;
                case "Subtraction":
                    _model = new SubtractionCustomization();
                    break;
                case "Multiplication":
                    _model = new MultiplicationCust();
                    break;
            }

        }
         
        public override void AddSignal(SignalViewModel signal)
        {
            if (_model is Filter)
            {
                InputChannels.Add(signal); // Override baseclass function
                OutputChannels.Add(signal);
            }
            else
            {
                switch (Name)
                {
                    case "Scalar Repetition":

                        break;
                    case "Addition":

                        break;
                    case "Subtraction":
                        // Set parameter
                        SetFocusedTextBox(signal); // if step is a customization, need to make up the output signal from input signal depends on type of customizaion
                        break;

                }
            }
        }
        public override void RemoveSignal(SignalViewModel signal)
        {
            if (_model is Filter)
            {
                InputChannels.Remove(signal);
                OutputChannels.Remove(signal);
            }
            else
            {//need to remove the output signal depends on type of customization
                switch (Name)
                {
                    case "Subtraction":
                        break;
                    default:
                        break;
                }
            }
        }

        internal bool SetFocusedTextBox(SignalViewModel signal)
        {
            if (CurrentCursor == "")
            {
                //No textbox selected
                throw new Exception("Error! Please select a valid text box for this input signal!");
            } else if (CurrentCursor == "Minuend")
            {
                // Subtraction 
                if (signal.IsChecked)
                {
                    // Check to make sure subtrahend and minuend aren't the same signal
                    if (Subtrahend != null && Subtrahend == signal)
                    {
                        throw new Exception("Minuend cannot be the same as the subtrahend!");
                    }
                    else // Set this as the minuend
                    {
                        Minuend = signal;
                        Minuend.IsChecked = false;
                    }
                }
                else
                {
                    // Check box unchecked, want to delete content in box
                    if (signal == Minuend)
                    {
                        Minuend = new SignalViewModel(); // Set as blank value
                    }
                }
                
                CurrentCursor = "";
                return true;
            } else if (CurrentCursor == "Subtrahend")
            {
                // Subtraction 
                if (signal.IsChecked)
                {
                    // Check to make sure subtrahend and minuend aren't the same signal
                    if (Minuend != null && Minuend == signal)
                    {
                        throw new Exception("Subtrahend cannot be the same as the minuend!");
                    }
                    else // Set this as the minuend
                    {
                        Subtrahend = signal;
                        Subtrahend.IsChecked = false;
                    }
                }
                else
                {
                    // Check box unchecked, want to delete content in box
                    if (signal == Minuend)
                    {
                        Subtrahend = new SignalViewModel(); // Set as blank value
                    }
                }

                CurrentCursor = "";
                return true;
            }
                return false;
        }

        internal void GetSignalNameList()
        {
            if (_model is PMUflagFilt)
            {
                _model.InputSignals = InputChannels.Select(x => x.PMUName).ToList();
            }
            else
            {
                _model.InputSignals = InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList();
            }
        }
    }    
}
