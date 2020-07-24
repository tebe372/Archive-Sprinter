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
                    if (m != null && m.NomVoltage != v)
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
                    if (m != null && m.VoltMin != v)
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
                    if (m != null && m.VoltMax != v)
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
                    if (m != null && m.FreqMaxChan != v)
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
                    if (m != null && m.FreqMinChan != v)
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
                    if (m != null && m.FreqPctChan != v)
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
                    if (m != null && m.FreqMinSamp != v)
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
                    if (m != null && m.FreqMaxSamp != v)
                    {
                        m.FreqMaxSamp = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string StaleThresh
        {
            get
            {
                if (_model is StaleDQFilt)
                {
                    var m = _model as StaleDQFilt;
                    return m.StaleThresh.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int v))
                {
                    var m = _model as StaleDQFilt;
                    if (m != null && m.StaleThresh != v)
                    {
                        m.StaleThresh = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string StdDevMult
        {
            get
            {
                if (_model is OutlierFilt)
                {
                    var m = _model as OutlierFilt;
                    return m.StdDevMult.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int v))
                {
                    var m = _model as OutlierFilt;
                    if (m != null && m.StdDevMult != v)
                    {
                        m.StdDevMult = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public string PercentBadThresh
        {
            get
            {
                if (_model is DataFrameDQFilt)
                {
                    var m = _model as DataFrameDQFilt;
                    return m.PercentBadThresh.ToString();
                }
                //else if (_model is PMUchanDQFilt)
                //{
                //    var m = _model as PMUchanDQFilt;
                //    return m.PercentBadThresh.ToString();
                //}
                //else if (_model is PMUallDQFilt)
                //{
                //    var m = _model as PMUallDQFilt;
                //    return m.PercentBadThresh.ToString();
                //}
                else
                {
                    return null;
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double v))
                {
                    var m = _model as DataFrameDQFilt;
                    if (m != null && m.PercentBadThresh != v)
                    {
                        m.PercentBadThresh = v;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public bool KeepDiffPMUSeparate
        {
            get
            {
                if (_model is DataFrameDQFilt)
                {
                    var m = _model as DataFrameDQFilt;
                    return m.KeepDiffPMUSeparate;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                var m = _model as DataFrameDQFilt;
                if (m != null && m.KeepDiffPMUSeparate != value)
                {
                    m.KeepDiffPMUSeparate = value;
                    OnPropertyChanged();
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
                case "Division":
                    _model = new DivisionCust(); 
                    break;
                case "Exponential":
                    _model = new ExponentialCust(); 
                    break;
                case "Sign Reversal":
                    _model = new SignReversalCust();
                    break;
                case "Absolute Value":
                    _model = new AbsValCust();
                    break;
                case "Real Component":
                    _model = new RealComponentCust();
                    break;
                case "Imaginary Component":
                    _model = new ImagComponentCust();
                    break;
                case "Angle Calculation":
                    _model = new AngleCust();
                    break;
                case "Complex Conjugate":
                    _model = new ComplexConjCust();
                    break;
                case "Phasor Creation":
                    _model = new CreatePhasorCust();
                    break;
                case "Power Calculation":
                    _model = new PowerCalcCust();
                    break;
                case "Signal Type/Unit":
                    _model = new SpecifySignalTypeUnitCust();
                    break;
                case "Metric Prefix":
                    _model = new MetricPrefixCust();
                    break;
                case "Angle Conversion":
                    _model = new AngleConversionCust();
                    break;
                case "Duplicate Signals":
                    _model = new SignalReplicationCust();
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
                    case "Multiplication":

                        break;
                    case "Division":

                        break;
                    case "Exponential":

                        break;
                    case "Sign Reversal":

                        break;
                    case "Absolute Value":

                        break;
                    case "Real Component":

                        break;
                    case "Imaginary Component":

                        break;
                    case "Angle Calculation":

                        break;
                    case "Complex Conjugate":

                        break;
                    case "Phasor Creation":

                        break;
                    case "Power Calculation":

                        break;
                    case "Signal Type/Unit":

                        break;
                    case "Metric Prefix":

                        break;
                    case "Angle Conversion":

                        break;
                    case "Duplicate Signals":

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
