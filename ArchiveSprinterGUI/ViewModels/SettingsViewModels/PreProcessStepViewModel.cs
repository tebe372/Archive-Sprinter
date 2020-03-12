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
    public class PreProcessStepViewModel : StepViewModel
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
        public string Name { get; set; }

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

        public PreProcessStepViewModel() : base()
        {
            _isComplete = false;
        }

        public PreProcessStepViewModel(string name) : this()
        {
            _isComplete = false;
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
