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

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class PreProcessStepViewModel : ViewModelBase
    {
        public virtual PreProcessStep Model { get; } // might need to be get rid of if possible

        //public virtual string Name { get; set; } // need to get rid of
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

        public string Name { get; set; }

        public int StepCounter { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isComplete;
        public bool IsComplete
        {
            get { return _isComplete; }
            set
            {
                _isComplete = value;
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

        public PreProcessStepViewModel()
        {
            _isSelected = false;
            _isComplete = false;
            StepCounter = 0;
        }
    }
    public class DropOutZeroFiltViewModel : PreProcessStepViewModel
    {
        public DropOutZeroFiltViewModel(DropOutZeroFilt m)
        {
            _model = m;
        }
        private DropOutZeroFilt _model;
        public override PreProcessStep Model
        {
            get { return _model; }
        }
        private bool _setToNaN;
        public bool SetToNaN 
        {
            get
            {
                return _model.SetToNaN;
            }
            set { _model.SetToNaN = value; } 
        }

        //private DropOutZeroFilt _model;
        //public override PreProcessStep Model { get { return _model; } }
    }
}
