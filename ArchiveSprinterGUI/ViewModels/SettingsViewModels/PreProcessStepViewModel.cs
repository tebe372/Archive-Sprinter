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
        private PreProcessStep _model;
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
                // Go through and set all the input channels 
                foreach (var s in InputChannels)
                {
                    s.IsChecked = value;
                }
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
            _inputChannels = new ObservableCollection<SignalViewModel>();
        }

        public PreProcessStepViewModel(string name)
        {
            _isSelected = false;
            _isComplete = false;
            StepCounter = 0;
            _inputChannels = new ObservableCollection<SignalViewModel>();
            Name = name;
            switch (name)
            {
                case "Zeros":
                    _model = new DropOutZeroFilt();
                    break;
                case "Subtraction":
                    _model = new SubtractionCustomization();
                    break;
            }

        }
    }
    /*
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
   
    public class SubtractionCustViewModel : PreProcessStepViewModel
    {
        private SignalViewModel _minuend;
        public SignalViewModel Minuend
        {
            get { return _minuend; }
            set {
                _minuend = value;
                OnPropertyChanged();
            }
        }
        private SignalViewModel _subtrahend;
        public SignalViewModel Subtrahend
        {
            get { return _subtrahend; }
            set
            {
                _subtrahend = value;
                OnPropertyChanged();
            }
        }
    }
    */
}
