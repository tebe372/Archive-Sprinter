using AS.Core.ViewModels;
using AS.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class StepViewModel : ViewModelBase
    {
        public StepViewModel()
        {
            StepCounter = 0;
            _isSelected = false;
            _isExpanded = false;
            _inputChannels = new ObservableCollection<SignalViewModel>();
            _outputChannels = new ObservableCollection<SignalViewModel>();
            //ThisStepOutputsGroupedByPMU = new SignalTree();
        }
        private int _stepCounter;
        [JsonIgnore]
        public int StepCounter 
        {
            get { return _stepCounter; }
            set { _stepCounter = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;
        [JsonIgnore]
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
        private bool _isExpanded;
        [JsonIgnore]
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

        [JsonIgnore]
        private string _currentCursor;
        [JsonIgnore]
        public string CurrentCursor
        {
            get
            {
                return _currentCursor;
            }
            set
            {
                _currentCursor = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalViewModel> _inputChannels;
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
        public virtual void AddSignal(SignalViewModel signal)
        {
            InputChannels.Add(signal);
        }
        [JsonIgnore]
        public SignalTree ThisStepInputsGroupedByType { get; set; }
        [JsonIgnore]
        public SignalTree ThisStepOutputsGroupedByPMU { get; set; }
        private ObservableCollection<SignalViewModel> _outputChannels;
        [JsonIgnore]
        public ObservableCollection<SignalViewModel> OutputChannels
        {
            get
            {
                return _outputChannels;
            }
            set
            {
                _outputChannels = value;
                OnPropertyChanged();
            }
        }

        public void UpdateInputOutputTree()
        {
        }

        public virtual void RemoveSignal(SignalViewModel signal)
        {
            InputChannels.Remove(signal);
        }
    }
}
