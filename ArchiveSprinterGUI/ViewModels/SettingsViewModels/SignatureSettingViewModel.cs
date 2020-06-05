using AS.Config;
using AS.Core.ViewModels;
using AS.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class SignatureSettingViewModel : StepViewModel
    {
        private SignatureSetting _model;
        //public SignatureSettingViewModel() : base() {
        //}
        [JsonConstructor]
        public SignatureSettingViewModel(string signatureName) : base()
        {
            switch (signatureName)
            {
                case "Mean":
                    _model = new Mean();
                    break;
                case "Variance":
                    _model = new Variance();
                    break;
                case "Standard Deviation":
                    _model = new StandardDeviation();
                    break;
                case "Kurtosis":
                    _model = new Kurtosis();
                    break;
                case "Skewness":
                    _model = new Skewness();
                    break;
                case "Correlation Coefficient":
                    _model = new CorrelationCoefficient();
                    break;
                case "Covariance":
                    _model = new Covariance();
                    break;
                case "Periodogram":
                    _model = new Periodogram();
                    break;
                case "Generalized Magnitude Squared Coherence (GMSC) Spectrum":
                    _model = new GMSCSpectrum();
                    break;
                case "Percentile":
                    _model = new Percentile();
                    break;
                case "Quartiles":
                    _model = new Quartiles();
                    break;
                case "Median":
                    _model = new Median();
                    break;
                case "Maximum":
                    _model = new Maximum();
                    break;
                case "Minimum":
                    _model = new Minimum();
                    break;
                case "Range":
                    _model = new Range();
                    break;
                case "Rise":
                    _model = new Rise();
                    break;
                case "Histogram":
                    _model = new Histogram();
                    break;
                case "Root Mean Squared Value":
                    _model = new RootMeanSquare();
                    break;
                case "Frequency Band Root Mean Squared Value":
                    _model = new FrequencyBandRMS();
                    break;
                default:
                    _model = null;
                    break;
            }
        }

        [JsonIgnore]
        public SignatureSetting Model
        {
            get { return _model; }
        }
        public string SignatureName
        {
            get { return _model.SignatureName; }
        }
        [JsonProperty("WindowSize")]
        public string WindowSizeStr
        {
            get { return _model.WindowSizeStr; }
            set
            {
                if (_model != null && _model.WindowSizeStr != value)
                {
                    _model.WindowSizeStr = value;
                    OnPropertyChanged();
                }
            }
        }
        [JsonProperty("WindowOverlap")]
        public string WindowOverlapStr
        {
            get { return _model.WindowOverlapStr; }
            set
            {
                if (_model != null && _model.WindowOverlapStr != value)
                {
                    _model.WindowOverlapStr = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool OmitNan
        {
            get { return _model.OmitNan; }
            set
            {
                if (_model != null && _model.OmitNan != value)
                {
                    _model.OmitNan = value;
                    OnPropertyChanged();
                }
            }
        }
        
        internal void GetSignalNameList()
        {
            _model.InputSignals = InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList();
        }

        internal void GetSamplingRAte()
        {
            _model.SamplingRate = InputChannels.FirstOrDefault().SamplingRate;
        }
        public bool? RemoveMean 
        {
            get 
            {
                if (_model is RootMeanSquare)
                {
                    var m = _model as RootMeanSquare;
                    return m.RemoveMean;
                }
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }
            }
            set
            {
                if (_model is RootMeanSquare)
                {
                    var m = _model as RootMeanSquare;
                    if (value != null && m.RemoveMean != (bool)value)
                    {
                        m.RemoveMean = (bool)value;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public bool? CalculateFull
        {
            get
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    return m.CalculateFull;
                }
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }
            }
            set
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    if (value != null && m.CalculateFull != (bool)value)
                    {
                        m.CalculateFull = (bool)value;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public bool? CalculateBand2
        {
            get
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    return m.CalculateBand2;
                }
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }
            }
            set
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    if (value != null && m.CalculateBand2 != (bool)value)
                    {
                        m.CalculateBand2 = (bool)value;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public bool? CalculateBand3
        {
            get
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    return m.CalculateBand3;
                }
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }
            }
            set
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    if (value != null && m.CalculateBand3 != (bool)value)
                    {
                        m.CalculateBand3 = (bool)value;
                        OnPropertyChanged();
                    }
                }
            }
        }
        public bool? CalculateBand4
        {
            get
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    return m.CalculateBand4;
                }
                else
                {
                    //throw new NotImplementedException();
                    return null;
                }
            }
            set
            {
                if (_model is FrequencyBandRMS)
                {
                    var m = _model as FrequencyBandRMS;
                    if (value != null && m.CalculateBand4 != (bool)value)
                    {
                        m.CalculateBand4 = (bool)value;
                        OnPropertyChanged();
                    }
                }
            }
        }
    }
}