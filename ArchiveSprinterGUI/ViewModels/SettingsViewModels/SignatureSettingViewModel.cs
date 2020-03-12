using AS.Config;
using AS.Core.ViewModels;
using AS.Utilities;
using System;
using System.Collections.ObjectModel;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class SignatureSettingViewModel : ViewModelBase
    {
        private SignatureSetting _model;

        public SignatureSettingViewModel(string sig)
        {
            switch (sig)
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
                default:
                    break;
            }
            _inputChannels = new ObservableCollection<SignalViewModel>();
        }

        public SignatureSetting Model
        {
            get { return _model; }
        }
        public string SignatureName
        {
            get { return _model.SignatureName; }
        }
        public string WindowSize 
        {
            get { return _model.WindowSize.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _model.WindowSize = Convert.ToInt32(value);
                    OnPropertyChanged();
                }
            }
        }
        public string WindowOverlap
        {
            get { return _model.WindowOverlap.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _model.WindowOverlap = Convert.ToInt32(value);
                    OnPropertyChanged();
                }
            }
        }
        public bool OmitNan
        {
            get { return _model.OmitNan; }
            set
            {
                _model.OmitNan = value;
                OnPropertyChanged();
            }
        }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                // Go through and set all the input channels 
                foreach (var s in InputChannels)
                {
                    s.IsChecked = value;
                }
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SignalViewModel> _inputChannels;
        public ObservableCollection<SignalViewModel> InputChannels
        {
            get
            {
                return _inputChannels;
            }
            set
            {
                _inputChannels = value;
                OnPropertyChanged();
            }
        }
        public int StepCounter { get; set; }
    }
}