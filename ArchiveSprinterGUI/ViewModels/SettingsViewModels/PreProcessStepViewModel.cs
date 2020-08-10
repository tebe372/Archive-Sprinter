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
using AS.SampleDataManager;

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
        public override string Name
        {
            get { return _model.Name; }
        }
        public SignalViewModel Minuend
        {
            get
            {
                if (parameters.ContainsKey("Minuend"))
                {
                    return parameters["Minuend"];
                }
                //return new SignalViewModel();
                return null;
            }
            set
            {
                var m = _model as SubtractionCust;
                if (m != null)
                {
                    m.Minuend = value.Model;
                }
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
                //return new SignalViewModel();
                return null;
            }
            set
            {
                var m = _model as SubtractionCust;
                if (m != null)
                {
                    m.Subtrahend = value.Model;
                }
                parameters["Subtrahend"] = value;
                OnPropertyChanged();
            }
        }
        public SignalViewModel Dividend
        {
            get
            {
                if (parameters.ContainsKey("Dividend"))
                {
                    return parameters["Dividend"];
                }
                //return new SignalViewModel();
                return null;
            }
            set
            {
                var m = _model as DivisionCust;
                if (m != null)
                {
                    m.Dividend = value.Model;
                }
                parameters["Dividend"] = value;
                OnPropertyChanged();
            }
        }
        public SignalViewModel Divisor
        {
            get
            {
                if (parameters.ContainsKey("Divisor"))
                {
                    return parameters["Divisor"];
                }
                //return new SignalViewModel();
                return null;
            }
            set
            {
                var m = _model as DivisionCust;
                if (m != null)
                {
                    m.Divisor = value.Model;
                }
                parameters["Divisor"] = value;
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
        public string CustPMUname
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.PMUName;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.PMUName = value;
                    foreach (var s in OutputChannels)
                    {
                        s.PMUName = value;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public string SignalName
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.SignalName;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.SignalName = value;
                    if (OutputChannels.Count == 1)
                    {
                        OutputChannels[0].SignalName = value;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public string TypeAbbreviation
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.TypeAbbreviation;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.TypeAbbreviation = value;
                    if (OutputChannels.Count == 1)
                    {
                        OutputChannels[0].TypeAbbreviation = value;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public string Unit
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.Unit;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.Unit = value;
                    if (OutputChannels.Count == 1)
                    {
                        OutputChannels[0].Unit = value;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public int SamplingRate
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.SamplingRate;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.SamplingRate = value;
                    if (OutputChannels.Count == 1)
                    {
                        OutputChannels[0].SamplingRate = value;
                    }
                    OnPropertyChanged();
                }
            }
        }
        public double Scalar
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.Scalar;
                }
                else
                {
                    return double.NaN;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.Scalar = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Exponent
        {
            get
            {
                if (_model is ExponentialCust)
                {
                    var m = _model as ExponentialCust;
                    return m.Exponent;
                }
                else
                {
                    return double.NaN;
                }
            }
            set
            {
                var m = _model as ExponentialCust;
                if (m != null)
                {
                    m.Exponent = value;
                    OnPropertyChanged();
                }
            }
        }
        public PMUWithSamplingRate TimeSourcePMU
        {
            get
            {
                if (_model is Customization)
                {
                    var m = _model as Customization;
                    return m.TimeSourcePMU;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var m = _model as Customization;
                if (m != null)
                {
                    m.TimeSourcePMU = value;
                    if (value != null)
                    {
                        SamplingRate = value.SamplingRate;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Dictionary<string, SignalViewModel> parameters = new Dictionary<string, SignalViewModel>();
        public ObservableCollection<UnaryInputOutputPair> OneToOneSignalPairs { get; set; } = new ObservableCollection<UnaryInputOutputPair>();
        [JsonIgnore]
        public UnaryInputOutputPair SelectedOneToOneInputOutputPair { get; internal set; }
        public ObservableCollection<CreatPhasorInputOutputSignalsViewModel> OneToTwoSignalPairs { get; set; } = new ObservableCollection<CreatPhasorInputOutputSignalsViewModel>();
        [JsonIgnore]
        public CreatPhasorInputOutputSignalsViewModel SelectedOneToTwoInputOutputPair { get; internal set; }
        public bool IsFromPhasor
        {
            get
            {
                if (_model is PowerCalcCust)
                {
                    var m = _model as PowerCalcCust;
                    return m.IsFromPhasor;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                var m = _model as PowerCalcCust;
                if (m != null)
                {
                    m.IsFromPhasor = value;
                    OnPropertyChanged();
                }
            }
        }
        public PowerType PowType
        {
            get
            {
                if (_model is PowerCalcCust)
                {
                    var m = _model as PowerCalcCust;
                    return m.PowType;
                }
                else
                {
                    return PowerType.CP;
                }
            }
            set
            {
                var m = _model as PowerCalcCust;
                if (m != null)
                {
                    m.PowType = value;
                    TypeAbbreviation = value.ToString();
                    Unit = _powerUnitFromType(value);
                    OnPropertyChanged();
                }
            }
        }
        private PowerInputByPhasors _phasorPowerInput = new PowerInputByPhasors();
        public PowerInputByPhasors PhasorPowerInput
        {
            get { return _phasorPowerInput; }
            set
            {
                _phasorPowerInput = value;
                OnPropertyChanged();
            }
        }
        private PowerInputByMagAng _magAngPowerInput = new PowerInputByMagAng();
        public PowerInputByMagAng MagAngPowerInput
        {
            get { return _magAngPowerInput; }
            set
            {
                _magAngPowerInput = value;
                OnPropertyChanged();
            }
        }
        private string _powerUnitFromType(PowerType type)
        {
            switch (type)
            {
                case PowerType.CP:
                    return "MVA";
                case PowerType.P:
                    return "MW";
                case PowerType.Q:
                    return "MVAR";
                case PowerType.S:
                    return "MVA";
                default:
                    return null;
            }
        }

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
                    var sclr = new SignalViewModel(SignalName, CustPMUname);
                    OutputChannels.Add(sclr);
                    break;
                case "Addition":
                    _model = new AdditionCust();
                    var add = new SignalViewModel(SignalName, CustPMUname);
                    OutputChannels.Add(add);
                    break;
                case "Subtraction":
                    _model = new SubtractionCust();
                    var sub = new SignalViewModel(SignalName, CustPMUname);
                    OutputChannels.Add(sub);
                    break;
                case "Multiplication":
                    _model = new MultiplicationCust();
                    var mul = new SignalViewModel(SignalName, CustPMUname);
                    OutputChannels.Add(mul);
                    break;
                case "Division":
                    _model = new DivisionCust();
                    var div = new SignalViewModel(SignalName, CustPMUname);
                    OutputChannels.Add(div);
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
                    IsFromPhasor = true;
                    var power = new SignalViewModel(SignalName, CustPMUname);
                    power.TypeAbbreviation = PowType.ToString();
                    power.Unit = _powerUnitFromType(PowType);
                    OutputChannels.Add(power);
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
                        //_addSignalsToAdditionCustomization(signal);
                        InputChannels.Add(signal);
                        break;
                    case "Subtraction":
                        // Set parameter
                        try
                        {
                            SubtractionSetFocusedTextBox(signal); // if step is a customization, need to make up the output signal from input signal depends on type of customizaion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;
                    case "Multiplication":
                        InputChannels.Add(signal);
                        break;
                    case "Division":
                        try
                        {
                            DivisionSetFocusedTextBox(signal); // if step is a customization, need to make up the output signal from input signal depends on type of customizaion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;
                    case "Exponential":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            ExponentExchangeInputSignal(signal);
                        }
                        else
                        {
                            ExponentAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Sign Reversal":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Absolute Value":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Real Component":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Imaginary Component":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Angle Calculation":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            AngleCalculationExchangeInputSignal(signal);
                        }
                        else
                        {
                            AngleCalculationAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Complex Conjugate":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Phasor Creation":
                        if (SelectedOneToTwoInputOutputPair != null)
                        {
                            try
                            {
                                CreatPhasorExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            CreatPhasorAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Power Calculation":
                        _addInputForPowerCalculation(signal);
                        break;
                    case "Signal Type/Unit":
                        // cannot change the order of these two steps
                        AddinputToSpecifyTypeUnit(signal);
                        InputChannels.Add(signal);
                        break;
                    case "Metric Prefix":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Angle Conversion":
                        if (SelectedOneToOneInputOutputPair != null)
                        {
                            try
                            {
                                UnaryExchangeInputSignal(signal);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            UnaryAddOutputSignalOfthisInput(signal);
                            InputChannels.Add(signal);
                        }
                        break;
                    case "Duplicate Signals":
                        UnaryAddOutputSignalOfthisInput(signal);
                        InputChannels.Add(signal);
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
                    case "Scalar Repetition":

                        break;
                    case "Addition":
                        //_addSignalsToAdditionCustomization(signal);
                        InputChannels.Remove(signal);
                        break;
                    case "Subtraction":
                        try
                        {
                            SubtractionSetFocusedTextBox(signal); // if step is a customization, need to make up the output signal from input signal depends on type of customizaion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;
                    case "Multiplication":
                        InputChannels.Remove(signal);
                        break;
                    case "Division":
                        try
                        {
                            DivisionSetFocusedTextBox(signal); // if step is a customization, need to make up the output signal from input signal depends on type of customizaion
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        break;
                    case "Exponential":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Sign Reversal":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Absolute Value":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Real Component":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Imaginary Component":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Angle Calculation":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Complex Conjugate":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Phasor Creation":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfthisPhasorInput(signal);
                        break;
                    case "Power Calculation":
                        _removeInputForPowerCalculation(signal);
                        break;
                    case "Signal Type/Unit":
                        InputChannels.Remove(signal);
                        OutputChannels.Clear();
                        //_removeSpecifyTypeUnitOutput();
                        break;
                    case "Metric Prefix":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Angle Conversion":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    case "Duplicate Signals":
                        InputChannels.Remove(signal);
                        _removeOutputSignalOfThisUnaryInput(signal);
                        break;
                    default:
                        break;
                }
            }
        }
        internal bool SubtractionSetFocusedTextBox(SignalViewModel signal)
        {
            if (signal.IsChecked)
            {
                if (CurrentCursor == "")
                {
                    //No textbox selected
                    signal.IsChecked = false;
                    throw new Exception("Error! Please select a valid text box for this input signal!");
                }
                if (CurrentCursor == "Minuend")
                {
                    //// Check to make sure subtrahend and minuend aren't the same signal
                    //if (Subtrahend != null && Subtrahend == signal)
                    //{
                    //    throw new Exception("Minuend cannot be the same as the subtrahend!");
                    //}
                    //else // Set this as the minuend
                    //{
                    InputChannels.Remove(Minuend);
                    if (Minuend != null && Minuend != Subtrahend)
                    {
                        Minuend.IsChecked = false;
                    }
                    Minuend = signal;
                    InputChannels.Add(Minuend);
                    //}
                }
                if (CurrentCursor == "Subtrahend")
                {
                    //// Check to make sure subtrahend and minuend aren't the same signal
                    //if (Minuend != null && Minuend == signal)
                    //{
                    //    throw new Exception("Subtrahend cannot be the same as the minuend!");
                    //}
                    //else // Set this as the Subtrahend
                    //{
                    InputChannels.Remove(Subtrahend);
                    if (Subtrahend != null && Subtrahend != Minuend)
                    {
                        Subtrahend.IsChecked = false;
                    }
                    Subtrahend = signal;
                    InputChannels.Add(Subtrahend);
                    //}
                }
                CurrentCursor = "";
                return true;
            }
            else
            {
                // Check box unchecked, want to delete content in box
                if (signal == Minuend)
                {
                    InputChannels.Remove(Minuend);
                    Minuend = null; // Set as blank value
                }
                if (signal == Subtrahend)
                {
                    InputChannels.Remove(Subtrahend);
                    Subtrahend = null; // Set as blank value
                }
                CurrentCursor = "";
                return true;
            }
        }
        private bool DivisionSetFocusedTextBox(SignalViewModel signal)
        {
            if (signal.IsChecked)
            {
                if (CurrentCursor == "")
                {
                    //No textbox selected
                    signal.IsChecked = false;
                    throw new Exception("Error! Please select a valid text box for this input signal!");
                }
                if (CurrentCursor == "Dividend")
                {
                    //// Check to make sure subtrahend and minuend aren't the same signal
                    //if (Divisor != null && Divisor == signal)
                    //{
                    //    throw new Exception("Minuend cannot be the same as the subtrahend!");
                    //}
                    //else // Set this as the minuend
                    //{
                    InputChannels.Remove(Dividend);
                    if (Dividend != null && Dividend != Divisor)
                    {
                        Dividend.IsChecked = false;
                    }
                    Dividend = signal;
                    InputChannels.Add(Dividend);
                    //}
                }
                if (CurrentCursor == "Divisor")
                {
                    //// Check to make sure subtrahend and minuend aren't the same signal
                    //if (Minuend != null && Minuend == signal)
                    //{
                    //    throw new Exception("Subtrahend cannot be the same as the minuend!");
                    //}
                    //else // Set this as the Subtrahend
                    //{
                    InputChannels.Remove(Divisor);
                    if (Divisor != null && Divisor != Dividend)
                    {
                        Divisor.IsChecked = false;
                    }
                    Divisor = signal;
                    InputChannels.Add(Divisor);
                    //}
                }
                CurrentCursor = "";
                return true;
            }
            else
            {
                // Check box unchecked, want to delete content in box
                if (signal == Dividend)
                {
                    InputChannels.Remove(Dividend);
                    Dividend = null; // Set as blank value
                }
                if (signal == Divisor)
                {
                    InputChannels.Remove(Divisor);
                    Divisor = null; // Set as blank value
                }
                CurrentCursor = "";
                return true;
            }
        }
        public SignalViewModel ExponentAddOutputSignalOfthisInput(SignalViewModel signal)
        {
            var output = new SignalViewModel(signal.SignalName, CustPMUname);
            output.SamplingRate = signal.SamplingRate;
            //if (Exponent == 1) // how about == 0?
            //{
            //    output.TypeAbbreviation = signal.TypeAbbreviation;
            //    output.Unit = signal.Unit;
            //}
            //else if (Exponent == 0 || (signal.TypeAbbreviation == "SC" && signal.Unit == "SC"))
            //{
            //    sig.TypeAbbreviation = "SC";
            //    sig.Unit = "SC";
            //}
            //else
            //{
            output.TypeAbbreviation = "OTHER";
            output.Unit = "O";
            //}
            OneToOneSignalPairs.Add(new UnaryInputOutputPair(signal, output));
            OutputChannels.Add(output);
            return output;
        }
        private void ExponentExchangeInputSignal(SignalViewModel signal)
        {
            var output = SelectedOneToOneInputOutputPair.Output;
            var input = SelectedOneToOneInputOutputPair.Input;
            input.IsChecked = false;
            InputChannels.Remove(input);
            InputChannels.Add(signal);
            output.SamplingRate = signal.SamplingRate;
            //if (Exponent == 1)
            //{
            //    output.TypeAbbreviation = signal.TypeAbbreviation;
            //    output.Unit = signal.Unit;
            //}
            //else if (Exponent == 0 || (signal.TypeAbbreviation == "SC" && signal.Unit == "SC"))
            //{
            //    output.TypeAbbreviation = "SC";
            //    output.Unit = "SC";
            //}
            //else
            //{
            output.TypeAbbreviation = "OTHER";
            output.Unit = "O";
            //}
            var idx = OneToOneSignalPairs.IndexOf(SelectedOneToOneInputOutputPair);
            OneToOneSignalPairs.Remove(SelectedOneToOneInputOutputPair);
            OneToOneSignalPairs.Insert(idx, new UnaryInputOutputPair(signal, output));
            SelectedOneToOneInputOutputPair = null;
        }
        public SignalViewModel UnaryAddOutputSignalOfthisInput(SignalViewModel signal)
        {
            if (_model is MetricPrefixCust)
            {
                if (signal.TypeAbbreviation == "D" || signal.TypeAbbreviation == "OTHER" || signal.TypeAbbreviation == "SC" || (signal.TypeAbbreviation.Length == 3 && signal.TypeAbbreviation.Substring(1, 1) == "A"))
                {
                    signal.IsChecked = false;
                    throw new Exception("Metric prefix customization cannot apply to this signal type: " + signal.TypeAbbreviation + " .");
                }
            }
            if (_model is AngleConversionCust)
            {
                if (signal.TypeAbbreviation.Length != 3 || signal.TypeAbbreviation.Substring(1, 1) != "A")
                {
                    signal.IsChecked = false;
                    throw new Exception("Angle unit conversion customization can only apply to signals of angle type.");
                }
            }
            var output = new SignalViewModel(signal.SignalName, CustPMUname);
            output.SamplingRate = signal.SamplingRate;
            output.TypeAbbreviation = signal.TypeAbbreviation;
            output.Unit = signal.Unit;
            OneToOneSignalPairs.Add(new UnaryInputOutputPair(signal, output));
            OutputChannels.Add(output);
            return output;
        }
        private void UnaryExchangeInputSignal(SignalViewModel signal)
        {
            if (_model is MetricPrefixCust)
            {
                if (signal.TypeAbbreviation == "D" || signal.TypeAbbreviation == "OTHER" || signal.TypeAbbreviation == "SC" || (signal.TypeAbbreviation.Length == 3 && signal.TypeAbbreviation.Substring(1, 1) == "A"))
                {
                    signal.IsChecked = false;
                    throw new Exception("Metric prefix customization cannot apply to this signal type: " + signal.TypeAbbreviation + " .");
                }
            }
            if (_model is AngleConversionCust)
            {
                if (signal.TypeAbbreviation.Length != 3 || signal.TypeAbbreviation.Substring(1, 1) != "A")
                {
                    signal.IsChecked = false;
                    throw new Exception("Angle unit conversion customization can only apply to signals of angle type.");
                }
            }
            var output = SelectedOneToOneInputOutputPair.Output;
            var input = SelectedOneToOneInputOutputPair.Input;
            input.IsChecked = false;
            InputChannels.Remove(input);
            InputChannels.Add(signal);
            output.SamplingRate = signal.SamplingRate;
            output.TypeAbbreviation = signal.TypeAbbreviation;
            output.Unit = signal.Unit;
            var idx = OneToOneSignalPairs.IndexOf(SelectedOneToOneInputOutputPair);
            OneToOneSignalPairs.Remove(SelectedOneToOneInputOutputPair);
            OneToOneSignalPairs.Insert(idx, new UnaryInputOutputPair(signal, output));
            SelectedOneToOneInputOutputPair = null;
        }
        public SignalViewModel AngleCalculationAddOutputSignalOfthisInput(SignalViewModel signal)
        {
            var output = new SignalViewModel(signal.SignalName, CustPMUname);
            output.SamplingRate = signal.SamplingRate;
            output.TypeAbbreviation = _getAngleType(signal.TypeAbbreviation);
            if (output.TypeAbbreviation == "OTHER")
            {
                output.Unit = "O";
            }
            else
            {
                output.Unit = "RAD";
            }
            OneToOneSignalPairs.Add(new UnaryInputOutputPair(signal, output));
            OutputChannels.Add(output);
            return output;
        }
        private void AngleCalculationExchangeInputSignal(SignalViewModel signal)
        {
            var output = SelectedOneToOneInputOutputPair.Output;
            var input = SelectedOneToOneInputOutputPair.Input;
            input.IsChecked = false;
            InputChannels.Remove(input);
            InputChannels.Add(signal);
            output.SamplingRate = signal.SamplingRate;
            output.TypeAbbreviation = _getAngleType(signal.TypeAbbreviation);
            if (output.TypeAbbreviation == "OTHER")
            {
                output.Unit = "O";
            }
            else
            {
                output.Unit = "RAD";
            }
            var idx = OneToOneSignalPairs.IndexOf(SelectedOneToOneInputOutputPair);
            OneToOneSignalPairs.Remove(SelectedOneToOneInputOutputPair);
            OneToOneSignalPairs.Insert(idx, new UnaryInputOutputPair(signal, output));
            SelectedOneToOneInputOutputPair = null;
        }
        private string _getAngleType(string type)
        {
            switch (type)
            {
                case "VPP":
                    return "VAP";
                case "VPA":
                    return "VAA";
                case "VPB":
                    return "VAB";
                case "VPC":
                    return "VAC";
                case "IPP":
                    return "IAP";
                case "IPA":
                    return "IAA";
                case "IPB":
                    return "IAB";
                case "IPC":
                    return "IAC";
                default:
                    return "OTHER";
            }
        }
        private void _removeOutputSignalOfThisUnaryInput(SignalViewModel signal)
        {
            foreach (var item in OneToOneSignalPairs)
            {
                if (item.Input == signal)
                {
                    OneToOneSignalPairs.Remove(item);
                    OutputChannels.Remove(item.Output);
                    break;
                }
            }
            if (SelectedOneToOneInputOutputPair != null)
            {
                SelectedOneToOneInputOutputPair = null;
            }
        }
        private void CreatPhasorAddOutputSignalOfthisInput(SignalViewModel signal)
        {
            var output = new SignalViewModel(signal.SignalName, CustPMUname);
            var inputType = signal.TypeAbbreviation.ToArray();
            if (inputType.Length != 3 || (inputType[1] != 'M' && inputType[1] != 'A'))
            {
                signal.IsChecked = false;
                throw new Exception("Only signals of current/voltage magnitude/angle type is allowed for phasor creation customization from magnitude and angle.");
            }
            output.TypeAbbreviation = inputType[0] + "P" + inputType[2];
            output.SamplingRate = signal.SamplingRate;
            if (inputType.Length == 3 && inputType[1] == 'M')
            {
                output.Unit = signal.Unit;
                var newPair = new CreatPhasorInputOutputSignalsViewModel(signal, null, output);
                OneToTwoSignalPairs.Add(newPair);
            }
            if (inputType.Length == 3 && inputType[1] == 'A')
            {
                var newPair = new CreatPhasorInputOutputSignalsViewModel(null, signal, output);
                OneToTwoSignalPairs.Add(newPair);
            }
            OutputChannels.Add(output);
        }
        private void CreatPhasorExchangeInputSignal(SignalViewModel signal)
        {
            var output = SelectedOneToTwoInputOutputPair.Output;
            var inputMag = SelectedOneToTwoInputOutputPair.Mag;
            var inputAng = SelectedOneToTwoInputOutputPair.Ang;
            if (output.SamplingRate != signal.SamplingRate)
            {
                throw new Exception("The sampling rate of magnitude and angle signals must match to create a phasor.");
            }
            var inputType = signal.TypeAbbreviation.ToArray();
            if (inputType.Length != 3 || (inputType[1] != 'M' && inputType[1] != 'A'))
            {
                signal.IsChecked = false;
                throw new Exception("Only signals of current/voltage magnitude/angle type is allowed for phasor creation customization from magnitude and angle.");
            }
            output.TypeAbbreviation = inputType[0] + "P" + inputType[2];
            if (inputType.Length == 3 && inputType[1] == 'M')
            {
                SelectedOneToTwoInputOutputPair.Mag = signal;
                output.Unit = signal.Unit;
                if (inputMag != null)
                {
                    inputMag.IsChecked = false;
                    InputChannels.Remove(inputMag);
                }
            }
            if (inputType.Length == 3 && inputType[1] == 'A')
            {
                SelectedOneToTwoInputOutputPair.Ang = signal;
                if (inputAng != null)
                {
                    inputAng.IsChecked = false;
                    InputChannels.Remove(inputAng);
                }
            }
            SelectedOneToTwoInputOutputPair = null;
            InputChannels.Add(signal);
        }
        internal void CreatPhasorAddSignalPair(SignalViewModel mag, SignalViewModel ang, string signalName)
        {
            var output = new SignalViewModel(signalName, CustPMUname);
            if (mag != null)
            {
                var inputType = mag.TypeAbbreviation.ToArray();
                output.TypeAbbreviation = inputType[0] + "P" + inputType[2];
                output.Unit = mag.Unit;
                output.SamplingRate = mag.SamplingRate;
            }
            else if (ang != null)
            {
                var inputType = ang.TypeAbbreviation.ToArray();
                output.TypeAbbreviation = inputType[0] + "P" + inputType[2];
                output.SamplingRate = ang.SamplingRate;
            }
            var pair = new CreatPhasorInputOutputSignalsViewModel(mag, ang, output);
            OneToTwoSignalPairs.Add(pair);
            OutputChannels.Add(output);
        }
        private void _removeOutputSignalOfthisPhasorInput(SignalViewModel signal)
        {
            if (SelectedOneToTwoInputOutputPair != null)
            {
                if (SelectedOneToTwoInputOutputPair.Mag == signal)
                {
                    SelectedOneToTwoInputOutputPair.Mag = null;
                }
                if (SelectedOneToTwoInputOutputPair.Ang == signal)
                {
                    SelectedOneToTwoInputOutputPair.Ang = null;
                }
                if (SelectedOneToTwoInputOutputPair.Mag == null && SelectedOneToTwoInputOutputPair.Ang == null)
                {
                    OutputChannels.Remove(SelectedOneToTwoInputOutputPair.Output);
                    OneToTwoSignalPairs.Remove(SelectedOneToTwoInputOutputPair);
                }
                SelectedOneToTwoInputOutputPair = null;
            }
            else
            {
                foreach (var item in OneToTwoSignalPairs)
                {
                    if (item.Mag == signal)
                    {
                        item.Mag = null;
                        if (item.Ang == null)
                        {
                            OutputChannels.Remove(item.Output);
                            OneToTwoSignalPairs.Remove(item);
                        }
                        break;
                    }
                    if (item.Ang == signal)
                    {
                        item.Ang = null;
                        if (item.Mag == null)
                        {
                            OutputChannels.Remove(item.Output);
                            OneToTwoSignalPairs.Remove(item);
                        }
                        break;
                    }
                }
            }
        }
        private void _addInputForPowerCalculation(SignalViewModel signal)
        {
            if (SamplingRate <= 0 && signal.SamplingRate > 0)
            {
                SamplingRate = signal.SamplingRate;
            }
            else if (SamplingRate != signal.SamplingRate || signal.SamplingRate <= 0)
            {
                return;
            }
            var inputType = signal.TypeAbbreviation.ToArray();
            if (IsFromPhasor)
            {
                if (inputType.Length == 3 && inputType[0] == 'V' && inputType[1] == 'P')
                {
                    if (PhasorPowerInput.VoltagePhasor != null)
                    {
                        PhasorPowerInput.VoltagePhasor.IsChecked = false;
                        InputChannels.Remove(PhasorPowerInput.VoltagePhasor);
                    }
                    PhasorPowerInput.VoltagePhasor = signal;
                }
                else if (inputType.Length == 3 && inputType[0] == 'I' && inputType[1] == 'P')
                {
                    if (PhasorPowerInput.CurrentPhasor != null)
                    {
                        PhasorPowerInput.CurrentPhasor.IsChecked = false;
                        InputChannels.Remove(PhasorPowerInput.CurrentPhasor);
                    }
                    PhasorPowerInput.CurrentPhasor = signal;
                }
                else
                {
                    signal.IsChecked = false;
                    throw new Exception("Only signals of phasor type is allowed for power creation customization from phasor.");
                }
            }
            else
            {
                if (inputType.Length == 3 && inputType[0] == 'V' && inputType[1] == 'M')
                {
                    if (MagAngPowerInput.VoltageMag != null)
                    {
                        MagAngPowerInput.VoltageMag.IsChecked = false;
                        InputChannels.Remove(MagAngPowerInput.VoltageMag);
                    }
                    MagAngPowerInput.VoltageMag = signal;
                }
                else if (inputType.Length == 3 && inputType[0] == 'I' && inputType[1] == 'M')
                {
                    if (MagAngPowerInput.CurrentMag != null)
                    {
                        MagAngPowerInput.CurrentMag.IsChecked = false;
                        InputChannels.Remove(MagAngPowerInput.CurrentMag);
                    }
                    MagAngPowerInput.CurrentMag = signal;
                }
                else if (inputType.Length == 3 && inputType[0] == 'V' && inputType[1] == 'A')
                {
                    if (MagAngPowerInput.VoltageAng != null)
                    {
                        MagAngPowerInput.VoltageAng.IsChecked = false;
                        InputChannels.Remove(MagAngPowerInput.VoltageAng);
                    }
                    MagAngPowerInput.VoltageAng = signal;
                }
                else if (inputType.Length == 3 && inputType[0] == 'I' && inputType[1] == 'A')
                {
                    if (MagAngPowerInput.CurrentAng != null)
                    {
                        MagAngPowerInput.CurrentAng.IsChecked = false;
                        InputChannels.Remove(MagAngPowerInput.CurrentAng);
                    }
                    MagAngPowerInput.CurrentAng = signal;
                }
                else
                {
                    signal.IsChecked = false;
                    throw new Exception("Only signals of current/voltage magnitude/angle type is allowed for power creation customization from magnitude and angle.");
                }
            }
            InputChannels.Add(signal);
        }
        private void _removeInputForPowerCalculation(SignalViewModel signal)
        {
            InputChannels.Remove(signal);
            var inputType = signal.TypeAbbreviation.ToArray();
            if (IsFromPhasor)
            {
                if (inputType.Length == 3 && inputType[0] == 'V' && inputType[1] == 'P')
                {
                    if (PhasorPowerInput.VoltagePhasor == signal)
                    {
                        PhasorPowerInput.VoltagePhasor = null;
                    }
                }
                if (inputType.Length == 3 && inputType[0] == 'I' && inputType[1] == 'P')
                {
                    if (PhasorPowerInput.CurrentPhasor == signal)
                    {
                        PhasorPowerInput.CurrentPhasor = null;
                    }
                }
                if (PhasorPowerInput.CurrentPhasor == null && PhasorPowerInput.VoltagePhasor == null)
                {
                    SamplingRate = -1;
                }
            }
            else
            {
                if (inputType.Length == 3 && inputType[0] == 'V' && inputType[1] == 'M')
                {
                    if (MagAngPowerInput.VoltageMag == signal)
                    {
                        MagAngPowerInput.VoltageMag = null;
                    }
                }
                if (inputType.Length == 3 && inputType[0] == 'I' && inputType[1] == 'M')
                {
                    if (MagAngPowerInput.CurrentMag == signal)
                    {
                        MagAngPowerInput.CurrentMag = null;
                    }
                }
                if (inputType.Length == 3 && inputType[0] == 'V' && inputType[1] == 'A')
                {
                    if (MagAngPowerInput.VoltageAng == signal)
                    {
                        MagAngPowerInput.VoltageAng = null;
                    }
                }
                if (inputType.Length == 3 && inputType[0] == 'I' && inputType[1] == 'A')
                {
                    if (MagAngPowerInput.CurrentAng == signal)
                    {
                        MagAngPowerInput.CurrentAng = null;
                    }
                }
                if (MagAngPowerInput.VoltageMag == null && MagAngPowerInput.CurrentMag == null && MagAngPowerInput.VoltageAng == null && MagAngPowerInput.CurrentAng == null)
                {
                    SamplingRate = -1;
                }
            }
        }
        public void AddinputToSpecifyTypeUnit(SignalViewModel signal)
        {
            if (InputChannels.Count > 0)
            {
                InputChannels[0].IsChecked = false;
                InputChannels.Clear();
                OutputChannels.Clear();
            }
            SignalViewModel stu = new SignalViewModel(SignalName, CustPMUname);
            stu.TypeAbbreviation = TypeAbbreviation;
            stu.Unit = Unit;
            OutputChannels.Add(stu);
            SamplingRate = signal.SamplingRate;
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
            if (_model is Customization)
            {
                _model.OutputSignals = OutputChannels.Select(x => x.Model).ToList();
                if (OneToOneSignalPairs.Count > 0)
                {
                    Dictionary<string, Signal> pairs = new Dictionary<string, Signal>();
                    foreach (var item in OneToOneSignalPairs)
                    {
                        var key = item.Input.PMUName + "_" + item.Input.SignalName;
                        pairs[key] = item.Output.Model;
                    }
                    var m = _model as Customization;
                    m.OneToOneSignalPairs = pairs;
                }
                if (OneToTwoSignalPairs.Count > 0)
                {
                    Dictionary<string, CreatPhasorInputOutputSignals> pairs = new Dictionary<string, CreatPhasorInputOutputSignals>();
                    foreach (var item in OneToTwoSignalPairs)
                    {
                        var pair = new CreatPhasorInputOutputSignals(item.Mag.Model, item.Ang.Model, item.Output.Model);
                        var key1 = item.Mag.PMUName + "_" + item.Mag.SignalName;
                        var key2 = item.Ang.PMUName + "_" + item.Ang.SignalName;
                        pairs[key1] = pair;
                        pairs[key2] = pair;
                    }
                    var m = _model as Customization;
                    m.OneToTwoSignalPairs = pairs;
                }
                if (_model is PowerCalcCust)
                {
                    var dict = new Dictionary<string, string>();
                    if (IsFromPhasor)
                    {
                        var vp = PhasorPowerInput.VoltagePhasor.PMUName + "_" + PhasorPowerInput.VoltagePhasor.SignalName;
                        dict[vp] = "vp";
                        var ip = PhasorPowerInput.CurrentPhasor.PMUName + "_" + PhasorPowerInput.CurrentPhasor.SignalName;
                        dict[ip] = "ip";
                    }
                    else
                    {
                        var vm = MagAngPowerInput.VoltageMag.PMUName + "_" + MagAngPowerInput.VoltageMag.SignalName;
                        dict[vm] = "vm";
                        var va = MagAngPowerInput.VoltageAng.PMUName + "_" + MagAngPowerInput.VoltageAng.SignalName;
                        dict[va] = "va";
                        var im = MagAngPowerInput.CurrentMag.PMUName + "_" + MagAngPowerInput.CurrentMag.SignalName;
                        dict[im] = "im";
                        var ia = MagAngPowerInput.CurrentAng.PMUName + "_" + MagAngPowerInput.CurrentAng.SignalName;
                        dict[ia] = "ia";
                    }
                    var m = _model as PowerCalcCust;
                    m.SignalNameTypeDict = dict;
                }
            }
        }
        internal void SetupOutputSignals()
        {
            try
            {
                switch (Name)
                {
                    case "Scalar Repetition":
                        _checkOutputNameValidity();
                        break;
                    case "Addition":
                        _checkOutputNameValidity();
                        _setupOutputForAdditionCustomization();
                        break;
                    case "Subtraction":
                        _checkOutputNameValidity();
                        _setupOutputForSubtractionCustomization();
                        break;
                    case "Multiplication":
                        _checkOutputNameValidity();
                        _setupOutputForMultiplicationCustomization();
                        break;
                    case "Division":
                        _checkOutputNameValidity();
                        _setupOutputForDivisionCustomization();
                        break;
                    case "Exponential":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Sign Reversal":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Absolute Value":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Real Component":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Imaginary Component":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Angle Calculation":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Complex Conjugate":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Phasor Creation":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Power Calculation":
                        _checkOutputNameValidity();
                        break;
                    case "Signal Type/Unit":
                        _checkOutputNameValidity();
                        break;
                    case "Metric Prefix":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Angle Conversion":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                    case "Duplicate Signals":
                        foreach (var item in OutputChannels)
                        {
                            _checkOutputNameValidity(item.SignalName);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void _checkOutputNameValidity(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = SignalName;
            }
            var foundSignal = SampleDataMngr.FindSignal(CustPMUname, name);
            if (foundSignal != null)
            {
                throw new Exception("Signal " + name + " in PMU " + CustPMUname + " already exists.");
            }
        }
        private void _setupOutputForAdditionCustomization()
        {
            string type = null;
            string unit = null;
            var rate = -1;
            foreach (var signal in InputChannels)
            {
                if (string.IsNullOrEmpty(type))
                    type = signal.TypeAbbreviation;
                else if (type != signal.TypeAbbreviation)
                {
                    TypeAbbreviation = "OTHER";
                    Unit = "O";
                    return;
                }
                if (string.IsNullOrEmpty(unit))
                    unit = signal.TypeAbbreviation;
                else if (unit != signal.TypeAbbreviation)
                {
                    Unit = "O";
                    TypeAbbreviation = "OTHER";
                    return;
                }
                if (rate == -1)
                    rate = signal.SamplingRate;
                else if (rate != signal.SamplingRate)
                {
                    SamplingRate = -1;
                    return;
                }
            }
            if (!string.IsNullOrEmpty(type))
                TypeAbbreviation = type;
            if (!string.IsNullOrEmpty(unit))
                Unit = unit;
            if (rate != -1)
                SamplingRate = rate;
        }
        private void _setupOutputForSubtractionCustomization()
        {
            if (Subtrahend.TypeAbbreviation == Minuend.TypeAbbreviation && Subtrahend.Unit == Minuend.Unit)
            {
                TypeAbbreviation = Subtrahend.TypeAbbreviation;
                Unit = Subtrahend.Unit;
            }
            else
            {
                TypeAbbreviation = "OTHER";
                Unit = "O";
            }
            if (Subtrahend.SamplingRate == Minuend.SamplingRate)
            {
                SamplingRate = Subtrahend.SamplingRate;
            }
            else
            {
                SamplingRate = -1;
            }
        }
        private void _setupOutputForMultiplicationCustomization()
        {
            string type = null;
            string unit = null;
            var rate = -1;
            var countNonScalarType = 0;
            foreach (var signal in InputChannels)
            {
                if (signal.TypeAbbreviation != "SC")
                {
                    countNonScalarType += 1;
                    if (string.IsNullOrEmpty(type))
                        type = signal.TypeAbbreviation;
                    if (string.IsNullOrEmpty(unit))
                        unit = signal.TypeAbbreviation;
                }
                if (rate == -1)
                    rate = signal.SamplingRate;
                else if (rate != signal.SamplingRate)
                {
                    SamplingRate = -1;
                    return;
                }
            }
            if (countNonScalarType == 0)
            {
                TypeAbbreviation = "SC";
                Unit = "SC";
            }
            else if (countNonScalarType == 1)
            {
                TypeAbbreviation = type;
                Unit = unit;
            }
            else
            {
                TypeAbbreviation = "OTHER";
                Unit = "O";
            }
            if (rate != -1)
                SamplingRate = rate;
        }
        private void _setupOutputForDivisionCustomization()
        {
            if (Dividend.SamplingRate == Divisor.SamplingRate && Dividend.Data.Count == Divisor.Data.Count)
            {
                SamplingRate = Divisor.SamplingRate;
                if (Divisor.TypeAbbreviation == "SC")
                {
                    TypeAbbreviation = Dividend.TypeAbbreviation;
                    Unit = Dividend.Unit;
                }
                else if (Divisor.TypeAbbreviation == Dividend.TypeAbbreviation && Dividend.Unit == Divisor.Unit)
                {
                    TypeAbbreviation = "SC";
                    Unit = "SC";
                }
                else
                {
                    TypeAbbreviation = "OTHER";
                    Unit = "O";
                }
            }
            else
            {
                SamplingRate = -1;
            }
        }

    }
    public class UnaryInputOutputPair : ViewModelBase
    {
        public UnaryInputOutputPair()
        {
            Input = null;
            Output = null;
        }
        public UnaryInputOutputPair(SignalViewModel input, SignalViewModel output)
        {
            Input = input;
            Output = output;
        }
        private SignalViewModel _input;
        public SignalViewModel Input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _output;
        public SignalViewModel Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }
    }
    public class CreatPhasorInputOutputSignalsViewModel : ViewModelBase
    {
        public CreatPhasorInputOutputSignalsViewModel()
        {

        }
        public CreatPhasorInputOutputSignalsViewModel(SignalViewModel mag, SignalViewModel ang, SignalViewModel output)
        {
            _mag = mag;
            _ang = ang;
            _output = output;
        }
        private SignalViewModel _mag;
        public SignalViewModel Mag 
        {
            get { return _mag; }
            set
            {
                _mag = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _ang;
        public SignalViewModel Ang
        {
            get { return _ang; }
            set
            {
                _ang = value;
                OnPropertyChanged();
            }
        }

        private SignalViewModel _output;
        public SignalViewModel Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }
    }
    public class PowerInputByPhasors : ViewModelBase
    {
        public PowerInputByPhasors()
        {
            VoltagePhasor = null;
            CurrentPhasor = null;
        }
        public PowerInputByPhasors(SignalViewModel vphasor, SignalViewModel iphasor)
        {
            VoltagePhasor = vphasor;
            CurrentPhasor = iphasor;
        }
        private SignalViewModel _voltagePhasor;
        public SignalViewModel VoltagePhasor 
        { 
            get { return _voltagePhasor; }
            set
            {
                _voltagePhasor = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _currentPhasor;
        public SignalViewModel CurrentPhasor 
        {
            get { return _currentPhasor; }
            set
            {
                _currentPhasor = value;
                OnPropertyChanged();
            }
            }
    }
    public class PowerInputByMagAng : ViewModelBase
    {
        public PowerInputByMagAng()
        {
            VoltageMag = null;
            CurrentMag = null;
            VoltageAng = null;
            CurrentAng = null;
        }
        public PowerInputByMagAng(SignalViewModel vMag, SignalViewModel iMag, SignalViewModel vAng, SignalViewModel iAng)
        {
            VoltageMag = vMag;
            CurrentMag = iMag;
            VoltageAng = vAng;
            CurrentAng = iAng;
        }
        private SignalViewModel _voltageMag;
        public SignalViewModel VoltageMag
        {
            get { return _voltageMag; }
            set
            {
                _voltageMag = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _currentMag;
        public SignalViewModel CurrentMag
        {
            get { return _currentMag; }
            set
            {
                _currentMag = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _voltageAng;
        public SignalViewModel VoltageAng
        {
            get { return _voltageAng; }
            set
            {
                _voltageAng = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _currentAng;
        public SignalViewModel CurrentAng
        {
            get { return _currentAng; }
            set
            {
                _currentAng = value;
                OnPropertyChanged();
            }
        }
    }
}
