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
    }
}
