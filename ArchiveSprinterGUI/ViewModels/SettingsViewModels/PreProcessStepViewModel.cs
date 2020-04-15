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

        public PreProcessStepViewModel(string name) : this()
        {
            _isComplete = false;
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
                case "Subtraction":
                    _model = new SubtractionCustomization();
                    break;
            }

        }
         
        public override void AddSignal(SignalViewModel signal)
        {
            switch (Name)
            {
                case "Status Flags":
                case "Zeros":
                case "Missing":
                case "Nominal Voltage":
                case "Nominal Frequency":
                    InputChannels.Add(signal); // redundant with baseclass function
                    break;
                case "Subtraction":
                    // Set parameter
                    SetFocusedTextBox(signal);
                    break;

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
            _model.InputSignals = InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList();
        }
    }
    
}
