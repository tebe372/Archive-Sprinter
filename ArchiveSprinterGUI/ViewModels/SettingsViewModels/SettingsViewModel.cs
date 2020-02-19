using AS.Config;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Configuration _model;

        private PreProcessStepViewModel _selectedStep;
        public PreProcessStepViewModel SelectedStep
        {
            get { return _selectedStep; }
            set
            {
                _selectedStep = value;
                OnPropertyChanged();
            }
        }

        private List<PreProcessStepViewModel> _preProcessSteps;
        public List<PreProcessStepViewModel> PreProcessSteps
        {
           get { return _preProcessSteps; }
            set
            {
                _preProcessSteps = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            _model = new Configuration();
            _selectedStep = new PreProcessStepViewModel();
            _preProcessSteps = new List<PreProcessStepViewModel>();
            
            DataConfigStepSelected = new RelayCommand(_dataConfigStepSelected);
            DataConfigStepAdded = new RelayCommand(_dataConfigStepAdded);
        }
        public int CurrentTabIndex { get; set; } = 0;
        public DataSourceSettingViewModel DataSourceVM { get; set; } = new DataSourceSettingViewModel();
        private List<DataSourceSettingViewModel> _dataSourceVMList = new List<DataSourceSettingViewModel>();
        public List<DataSourceSettingViewModel> DataSourceVMList
        {  // List of input file information
            get{return _dataSourceVMList; }
            set{ _dataSourceVMList = value; }
        }

        public ICommand DataConfigStepSelected { get; set; }
        private void _dataConfigStepSelected(object obj)
        {
            PreProcessStepViewModel newStep = obj as PreProcessStepViewModel;
            // Set step to edit
            _stepSelectedToEdit(newStep);
            OnPropertyChanged();

        }

        public ICommand DataConfigStepAdded { get; set; }
        private void _dataConfigStepAdded(object obj)
        {
            string stepName = (string)obj;
            PreProcessStepViewModel newStep = new PreProcessStepViewModel();
            newStep.Name = stepName;
     
            newStep.StepCounter = PreProcessSteps.Count + 1;
            PreProcessSteps.Add(newStep);
            // Set step to edit
            _stepSelectedToEdit(newStep);
            OnPropertyChanged();

        }

        private void _stepSelectedToEdit(object obj)
        {
            PreProcessStepViewModel step = obj as PreProcessStepViewModel;
            if (SelectedStep != step && step != null)
            {
                // Check if this step is complete
               // if (!step.IsComplete)
                //{
                 //   MessageBox.Show("Missing field(s) in this step, please double check!", "Error!", MessageBoxButtons.OK);
                //}
            }
            // Select step if not already selected
            if (!step.IsSelected)
            {
                // Find old step and deselect it
                // Select this step
                step.IsSelected = true;
                SelectedStep = step;
            }
        }

        public List<String> DQFilterList => _model.DQFilterList;
        public List<String> CustomizationList => _model.CustomizationList;

        public List<SignatureSettingViewModel> SignatureSettings { get; internal set; }
    }


}
