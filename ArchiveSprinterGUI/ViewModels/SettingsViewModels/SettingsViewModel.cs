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
        public Configuration Model
        {
            get { return _model; }
        }
        public SampleDataManagerViewModel SampleDataMngr { get; set; }

        private StepViewModel _selectedStep;
        public StepViewModel SelectedStep
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
            //_selectedStep = new PreProcessStepViewModel();
            _preProcessSteps = new ObservableCollection<PreProcessStepViewModel>();
            _currentTabIndex = 0;
            //_oldTabIndex = 0;

            SampleDataMngr = new SampleDataManagerViewModel();
            SampleDataMngr.SignalCheckStatusChanged += _signalCheckStatusChanged;

            DataConfigStepSelected = new RelayCommand(_dataConfigStepSelected);
            DataConfigStepAdded = new RelayCommand(_dataConfigStepAdded);
            DeleteDataConfigStep = new RelayCommand(_deleteDataConfigStep);

            SignatureCalAdded = new RelayCommand(_addASignatureStep);
            SignatureStepSelected = new RelayCommand(_signatureStepSelected);
            DeleteASignatureStep = new RelayCommand(_deleteASignatureStep);
            DeSelectAllSteps = new RelayCommand(_deSelectAllSteps);
        }
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
            PreProcessStepViewModel newStep = new PreProcessStepViewModel(stepName);
     
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

                    // Check if this step is complete
                    // if (!step.IsComplete)
                    //{
                    //   MessageBox.Show("Missing field(s) in this step, please double check!", "Error!", MessageBoxButtons.OK);
                    //}
                }
                // Set this step to selected
                step.IsSelected = true;
                SelectedStep = step;
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
        public List<SignatureCalMenu> SignatureList => _model.SignatureList;
        public ICommand SignatureCalAdded { get; set; }
        private void _addASignatureStep(object obj)
        {
            var sig = ((SignatureCalMenu)obj).Signature;
            var newSig = new SignatureSettingViewModel(sig);
            newSig.WindowOverlapStr = WindowOverlapStr;
            newSig.WindowSizeStr = WindowSizeStr;
            newSig.StepCounter = SignatureSettings.Count + 1;
            SignatureSettings.Add(newSig);
            _signatureStepSelected(newSig);
        }
        public ICommand SignatureStepSelected { get; set; }
        private void _signatureStepSelected(object obj)
        {
            SignatureSettingViewModel step = obj as SignatureSettingViewModel;
            if (SelectedStep != step)
            {
                if (SelectedStep != null)
                {
                    SelectedStep.IsSelected = false;
                }
                step.IsSelected = true;
                SelectedStep = step;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        public ICommand DeleteASignatureStep { get; set; }
        private void _deleteASignatureStep(object obj)
        {
            // Delete step
            SignatureSettingViewModel stepRemove = (SignatureSettingViewModel)obj;
            if (MessageBox.Show("Delete step " + stepRemove.StepCounter.ToString() + " in signature calculation: " + stepRemove.SignatureName + "?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                // Try to delete current step
                try
                {
                    SignatureSettings.Remove(stepRemove);
                    foreach (var step in SignatureSettings)
                    {
                        if (step.StepCounter > stepRemove.StepCounter)
                        {
                            step.StepCounter -= 1;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error deleting step");
                }
                // Select a different step?
                if (SelectedStep != null)
                {
                    SelectedStep.IsSelected = false;
                    SelectedStep = null;
                }
            }
        }
        public ICommand DeSelectAllSteps { get; set; }
        private void _deSelectAllSteps(object obj)
        {
            if (SelectedStep != null)
            {
                foreach (var s in SelectedStep.InputChannels)
                {
                    s.IsChecked = false;
                }
                SelectedStep.IsSelected = false;
                SelectedStep = null;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        public ObservableCollection<SignatureSettingViewModel> SignatureSettings { get; internal set; } = new ObservableCollection<SignatureSettingViewModel>();
        //private int _oldTabIndex;
        private int _currentTabIndex;
        public int CurrentTabIndex 
        {
            get { return _currentTabIndex; }
            set {
                if (_currentTabIndex != value)
                {
                    _currentTabIndex = value;
                    _deSelectAllSteps(null);
                    //if (_oldTabIndex == 1)
                    //{

                    //}
                    //_oldTabIndex = _currentTabIndex;
                    OnPropertyChanged();
                }
            }
        }
        public string DatawriteOutFrequencyStr
        {
            get { return _model.DatawriteOutFrequencyStr; }
            set
            {
                if (_model.DatawriteOutFrequencyStr != value)
                {
                    _model.DatawriteOutFrequencyStr = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DatawriteOutFrequencyUnit
        {
            get { return _model.DatawriteOutFrequencyUnit; }
            set
            {
                if (_model.DatawriteOutFrequencyUnit != value)
                {
                    _model.DatawriteOutFrequencyUnit = value;
                    OnPropertyChanged();
                    if (value == "Month")
                    {
                        DatawriteOutFrequencyStr = "1";
                    }
                }
            }
        }
        public List<string> DatawriteOutFrequencyUnits { get { return _model.DatawriteOutFrequencyUnits; } }
        public string WindowSizeStr
        {
            get { return _model.WindowSizeStr; }
            set
            {
                if (_model.WindowSizeStr != value)
                {
                    _model.WindowSizeStr = value;
                    OnPropertyChanged();
                    foreach (var sigSetting in SignatureSettings)
                    {
                        sigSetting.WindowSizeStr = value;
                    }
                }
            }
        }
        public string WindowOverlapStr
        {
            get { return _model.WindowOverlapStr; }
            set
            {
                if (_model.WindowOverlapStr != value)
                {
                    _model.WindowOverlapStr = value;
                    OnPropertyChanged();
                    foreach (var sigSetting in SignatureSettings)
                    {
                        sigSetting.WindowOverlapStr = value;
                    }
                }
            }
        }

    }


}
