using AS.Config;
using AS.Utilities;
using AS.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public SampleDataManagerViewModel SampleDataMngr { get; set; }

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

        private ObservableCollection<PreProcessStepViewModel> _preProcessSteps;
        public ObservableCollection<PreProcessStepViewModel> PreProcessSteps
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
            _preProcessSteps = new ObservableCollection<PreProcessStepViewModel>();

            SampleDataMngr = new SampleDataManagerViewModel();
            SampleDataMngr.SignalCheckStatusChanged += _signalCheckStatusChanged;

            DataConfigStepSelected = new RelayCommand(_dataConfigStepSelected);
            DataConfigStepAdded = new RelayCommand(_dataConfigStepAdded);
            DeleteDataConfigStep = new RelayCommand(_deleteDataConfigStep);
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

        public ICommand DeleteDataConfigStep { get; set; }
        private void _deleteDataConfigStep(object obj)
        {
            // Delete step
            PreProcessStepViewModel stepRemove = (PreProcessStepViewModel)obj;
            if (MessageBox.Show("Delete step " + stepRemove.StepCounter.ToString() + " in Data Configuration: " + stepRemove.Name + "?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.OK){
                // Try to delete current step
                try
                {
                    foreach (var step in PreProcessSteps)
                    {
                        if (step.StepCounter > stepRemove.StepCounter)
                        {
                            step.StepCounter -= 1;
                        }
                    }
                    PreProcessSteps.Remove(stepRemove);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error deleting step");
                }
                // Select a different step?
                if (SelectedStep != null)
                {
                    SelectedStep.IsSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private void _stepSelectedToEdit(object obj)
        {
            PreProcessStepViewModel step = obj as PreProcessStepViewModel;
            if (SelectedStep != step)
            {
                if (SelectedStep != null)
                {
                    // Deselect previously selected step
                    SelectedStep.IsSelected = false;
                    // Set this step to selected
                    SelectedStep = step;
                    SelectedStep.IsSelected = true;

                    // Check if this step is complete
                    // if (!step.IsComplete)
                    //{
                    //   MessageBox.Show("Missing field(s) in this step, please double check!", "Error!", MessageBoxButtons.OK);
                    //}
                }
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }

        private void _signalCheckStatusChanged(SignalTree e)
        {
            _updateSignals(e);
        }

        private void _updateSignals(SignalTree e)
        {
            SignalTree tree = e as SignalTree;
            if (SelectedStep != null)
            {
                if ((bool)tree.IsChecked)
                {
                     _addSignalsToStep(tree);
                }
                else
                {
                    //remove signals from plot's signal list
                    _removeSignalsFromStep(tree);
                }
               
            }
            else
            {
                MessageBox.Show("No step is selected to add signal.");
                tree.ChangeIsCheckedStatus(false);
                tree.CheckDirectParent();
            }
        }

        private void _addSignalsToStep(SignalTree tree)
        {
            if (tree.Signal != null)
            {
                SelectedStep.InputChannels.Add(tree.Signal);
            }
            else
            {
                foreach (var tr in tree.SignalList)
                {
                    _addSignalsToStep(tr);
                }
            }
        }

        private void _removeSignalsFromStep(SignalTree tree)
        {
            if (tree.Signal != null && SelectedStep.InputChannels.Contains(tree.Signal))
            {
                SelectedStep.InputChannels.Remove(tree.Signal);
            }
            else
            {
                if (tree.SignalList != null && tree.SignalList.Count > 0)
                {
                    foreach (var tr in tree.SignalList)
                    {
                        _removeSignalsFromStep(tr);
                    }
                }
            }
        }
        public List<String> DQFilterList => _model.DQFilterList;
        public List<String> CustomizationList => _model.CustomizationList;

        public List<SignatureSettingViewModel> SignatureSettings { get; internal set; } = new List<SignatureSettingViewModel>();
    }


}
