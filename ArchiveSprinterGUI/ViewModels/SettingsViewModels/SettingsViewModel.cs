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
using Newtonsoft.Json;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using AS.SampleDataManager;
using System.Windows;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Configuration _model;
        [JsonIgnore]
        public Configuration Model
        {
            get { return _model; }
        }
        [JsonIgnore]
        public SampleDataManagerViewModel SampleDataMngr { get; set; }
        private StepViewModel _selectedStep;
        [JsonIgnore]
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
            //_previousOutputDirectory = Environment.CurrentDirectory;

            SampleDataMngr = new SampleDataManagerViewModel();
            SampleDataMngr.SignalCheckStatusChanged += _signalCheckStatusChanged;

            DataConfigStepSelected = new RelayCommand(_dataConfigStepSelected);
            DataConfigStepAdded = new RelayCommand(_dataConfigStepAdded);
            DeleteDataConfigStep = new RelayCommand(_deleteDataConfigStep);
            SetCurrentFocusedTextbox = new RelayCommand(_setCurrentlySelectedTextbox);
            SetCurrentFocusedTextboxUnarySteps = new RelayCommand(_setCurrentFocusedTextboxUnarySteps);
            SetCurrentPhasorCreationFocusedTextBox = new RelayCommand(_setCurrentPhasorCreationFocusedTextBox);

            SignatureCalAdded = new RelayCommand(_addASignatureStep);
            SignatureStepSelected = new RelayCommand(_signatureStepSelected);
            DeleteASignatureStep = new RelayCommand(_deleteASignatureStep);
            DeSelectAllSteps = new RelayCommand(_deSelectAllSteps);
            //SelectSignatureOutputDir = new RelayCommand(_selectSignatureOutputDir);

            AddDataWriter = new RelayCommand(_addDataWriter);
            DataWriterSelected = new RelayCommand(_dataWriterSelected);
            DeleteDataWriter = new RelayCommand(_deleteAdatawriter);
        }
        private DataSourceSettingViewModel _dataSourceVM = new DataSourceSettingViewModel();
        public DataSourceSettingViewModel DataSourceVM 
        {
            get { return _dataSourceVM; }
            set
            {
                _dataSourceVM = value;
                OnPropertyChanged();
            }
        }
        private List<DataSourceSettingViewModel> _dataSourceVMList = new List<DataSourceSettingViewModel>();
        public List<DataSourceSettingViewModel> DataSourceVMList
        {  // List of input file information
            get{return _dataSourceVMList; }
            set{ _dataSourceVMList = value; }
        }
        [JsonIgnore]
        public ICommand DataConfigStepSelected { get; set; }
        private void _dataConfigStepSelected(object obj)
        {
            PreProcessStepViewModel newStep = obj as PreProcessStepViewModel;
            // Set step to edit
            _stepSelectedToEdit(newStep);
            //OnPropertyChanged();

        }
        [JsonIgnore]
        public ICommand DataConfigStepAdded { get; set; }
        private void _dataConfigStepAdded(object obj)
        {
            string stepName = (string)obj;
            PreProcessStepViewModel newStep = new PreProcessStepViewModel(stepName);
            newStep.SampleDataMngr = SampleDataMngr;
            newStep.StepCounter = PreProcessSteps.Count + 1;
            newStep.ThisStepInputsGroupedByType = new SignalTree("Step " + newStep.StepCounter.ToString() + " - " + newStep.Name);
            newStep.ThisStepInputsGroupedByType.SignalCheckStatusChanged += _signalCheckStatusChanged;
            newStep.ThisStepOutputsGroupedByPMU = new SignalTree("Step " + newStep.StepCounter.ToString() + " - " + newStep.Name);
            newStep.ThisStepOutputsGroupedByPMU.SignalCheckStatusChanged += _signalCheckStatusChanged;
            PreProcessSteps.Add(newStep);
            SampleDataMngr.GroupedSignalByPreProcessStepsInput.Add(newStep.ThisStepInputsGroupedByType);
            if (newStep.Model is Customization)
            {
                SampleDataMngr.GroupedSignalByPreProcessStepsOutput.Add(newStep.ThisStepOutputsGroupedByPMU);
            }
            // Set step to edit
            _stepSelectedToEdit(newStep);
            //OnPropertyChanged();
        }
        [JsonIgnore]
        public ICommand DeleteDataConfigStep { get; set; }
        private void _deleteDataConfigStep(object obj)
        {
            // Delete step
            PreProcessStepViewModel stepRemove = (PreProcessStepViewModel)obj;
            if (MessageBox.Show("Delete step " + stepRemove.StepCounter.ToString() + " in Data Configuration: " + stepRemove.Name + "?", "Warning!", MessageBoxButton.OKCancel) == MessageBoxResult.OK){
                // Try to delete current step
                try
                {
                    foreach (var step in PreProcessSteps)
                    {
                        if (step.StepCounter > stepRemove.StepCounter)
                        {
                            step.StepCounter -= 1;
                            step.ThisStepInputsGroupedByType.Label = "Step " + step.StepCounter.ToString() + " - " + step.Name;
                            step.ThisStepOutputsGroupedByPMU.Label = "Step " + step.StepCounter.ToString() + " - " + step.Name;
                            for (int i = step.InputChannels.Count - 1; i >= 0 ; i--)
                            {
                                if (stepRemove.OutputChannels.Contains(step.InputChannels[i]))
                                {
                                    step.InputChannels.RemoveAt(i);
                                }
                            }
                            for (int i = step.OneToOneSignalPairs.Count - 1; i >= 0; i--)
                            {
                                if (stepRemove.OutputChannels.Contains(step.OneToOneSignalPairs[i].Input))
                                {
                                    step.OneToOneSignalPairs.RemoveAt(i);
                                }
                            }
                            for (int i = step.OneToTwoSignalPairs.Count - 1; i >= 0; i--)
                            {
                                if (stepRemove.OutputChannels.Contains(step.OneToTwoSignalPairs[i].Ang))
                                {
                                    step.OneToTwoSignalPairs[i].Ang = null;
                                }
                                if (stepRemove.OutputChannels.Contains(step.OneToTwoSignalPairs[i].Mag))
                                {
                                    step.OneToTwoSignalPairs[i].Mag = null;
                                }
                                if (step.OneToTwoSignalPairs[i].Mag == null && step.OneToTwoSignalPairs[i].Ang == null)
                                {
                                    step.OneToTwoSignalPairs.RemoveAt(i);
                                }
                            }
                            if (stepRemove.OutputChannels.Contains(step.Subtrahend))
                            {
                                step.Subtrahend = new SignalViewModel();
                            }
                            if (stepRemove.OutputChannels.Contains(step.Minuend))
                            {
                                step.Minuend = new SignalViewModel();
                            }
                            if (stepRemove.OutputChannels.Contains(step.Dividend))
                            {
                                step.Dividend = new SignalViewModel();
                            }
                            if (stepRemove.OutputChannels.Contains(step.Divisor))
                            {
                                step.Divisor = new SignalViewModel();
                            }
                            if (stepRemove.OutputChannels.Contains(step.PhasorPowerInput.CurrentPhasor))
                            {
                                step.PhasorPowerInput.CurrentPhasor = null;
                            }
                            if (stepRemove.OutputChannels.Contains(step.PhasorPowerInput.VoltagePhasor))
                            {
                                step.PhasorPowerInput.VoltagePhasor = null;
                            }
                            if (stepRemove.OutputChannels.Contains(step.MagAngPowerInput.CurrentAng))
                            {
                                step.MagAngPowerInput.CurrentAng = null;
                            }
                            if (stepRemove.OutputChannels.Contains(step.MagAngPowerInput.CurrentMag))
                            {
                                step.MagAngPowerInput.CurrentMag = null;
                            }
                            if (stepRemove.OutputChannels.Contains(step.MagAngPowerInput.VoltageAng))
                            {
                                step.MagAngPowerInput.VoltageAng = null;
                            }
                            if (stepRemove.OutputChannels.Contains(step.MagAngPowerInput.VoltageMag))
                            {
                                step.MagAngPowerInput.VoltageMag = null;
                            }
                        }
                    }
                    foreach (var step in SignatureSettings)
                    {
                        for (int i = step.InputChannels.Count - 1; i >= 0; i--)
                        {
                            if (stepRemove.OutputChannels.Contains(step.InputChannels[i]))
                            {
                                step.InputChannels.RemoveAt(i);
                            }
                        }
                    }
                    foreach (var step in DataWriters)
                    {
                        for (int i = step.InputChannels.Count - 1; i >= 0; i--)
                        {
                            if (stepRemove.OutputChannels.Contains(step.InputChannels[i]))
                            {
                                step.InputChannels.RemoveAt(i);
                            }
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
                    _deSelectAllSteps(null);
                }
                foreach (var item in stepRemove.OutputChannels)
                {

                }
                //var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                //var stepsOutputAsSignalHierachy = new ObservableCollection<SignalTree>();
                //foreach (var stp in PreProcessSteps)
                //{
                //    stepsInputAsSignalHierachy.Add(stp.ThisStepInputsGroupedByType);
                //    if (stp.Model is Customization)
                //    {
                //        stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsGroupedByPMU);
                //    }
                //}
                //SampleDataMngr.GroupedSignalByPreProcessStepsInput = stepsInputAsSignalHierachy;
                //SampleDataMngr.GroupedSignalByPreProcessStepsOutput = stepsOutputAsSignalHierachy;
            }
        }
        private void _stepSelectedToEdit(PreProcessStepViewModel step)
        {
            //PreProcessStepViewModel step = obj as PreProcessStepViewModel;
            if (SelectedStep != step)
            {
                if (SelectedStep != null)
                {
                    //try
                    //{
                    //    SelectedStep.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(SelectedStep.InputChannels);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.Message);
                    //}
                    //if (((PreProcessStepViewModel)SelectedStep).Model is Customization)
                    //{
                    //    try
                    //    {
                    //        SelectedStep.ThisStepOutputsGroupedByPMU.SignalList = SampleDataMngr.SortSignalByPMU(SelectedStep.OutputChannels);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        MessageBox.Show(ex.Message);
                    //    }
                    //}
                    // Deselect previously selected step
                    SelectedStep.IsSelected = false;
                    try
                    {
                        SelectedStep.UpdateInputOutputTree();
                        if (SelectedStep is PreProcessStepViewModel)
                        {
                            var st = SelectedStep as PreProcessStepViewModel;
                            st.SetupOutputSignals();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    // Check if this step is complete
                    // if (!step.IsComplete)
                    //{
                    //   MessageBox.Show("Missing field(s) in this step, please double check!", "Error!", MessageBoxButtons.OK);
                    //}
                }
                var lastNmberOfSteps = step.StepCounter;
                var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                var stepsOutputAsSignalHierachy = new ObservableCollection<SignalTree>();
                foreach (var stp in PreProcessSteps)
                {
                    if (stp.StepCounter < lastNmberOfSteps)
                    {
                        stepsInputAsSignalHierachy.Add(stp.ThisStepInputsGroupedByType);
                        if (stp.Model is Customization)
                        {
                            stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsGroupedByPMU);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                // Set this step to selected
                step.IsSelected = true;
                SelectedStep = step;
                SampleDataMngr.GroupedSignalByPreProcessStepsInput = stepsInputAsSignalHierachy;
                SampleDataMngr.GroupedSignalByPreProcessStepsOutput = stepsOutputAsSignalHierachy;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();

                if (step.Name == "Scalar Repetition")
                {
                    SampleDataMngr.SignalSelectionTreeViewVisibility = Visibility.Collapsed;
                }
                else
                {
                    SampleDataMngr.SignalSelectionTreeViewVisibility = Visibility.Visible;
                }
            }
            else
            {
                foreach (var s in SelectedStep.InputChannels)
                {
                    s.IsChecked = true;
                }
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        private void _signalCheckStatusChanged(SignalTree e)
        {
            _updateSignals(e);
            SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            if (SelectedStep != null)
            {
                //try
                //{
                //    SelectedStep.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(SelectedStep.InputChannels);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                //if (SelectedStep is PreProcessStepViewModel)
                //{
                //    var st = SelectedStep as PreProcessStepViewModel;
                //    st.SetupOutputSignals();
                //    //try
                //    //{
                //    //    SelectedStep.ThisStepOutputsGroupedByPMU.SignalList = SampleDataMngr.SortSignalByPMU(SelectedStep.OutputChannels);
                //    //}
                //    //catch (Exception ex)
                //    //{
                //    //    MessageBox.Show(ex.Message);
                //    //}
                //}
                //if (SelectedStep is SignatureSettingViewModel)
                //{
                //    SampleDataMngr.GroupedSignalBySignatureStepsInput.Add(SelectedStep.ThisStepInputsGroupedByType);
                //}
                //if (SelectedStep is DataWriterViewModel)
                //{
                //    SampleDataMngr.GroupedSignalByDataWriterStepsInput.Add(SelectedStep.ThisStepInputsGroupedByType);
                //}
            }
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
                try
                {
                    SelectedStep.AddSignal(tree.Signal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
                try
                {
                    SelectedStep.RemoveSignal(tree.Signal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
        [JsonIgnore]
        public List<String> DQFilterList => _model.DQFilterList;
        [JsonIgnore]
        public List<String> CustomizationList => _model.CustomizationList;
        [JsonIgnore]
        public List<SignatureCalMenu> SignatureList => _model.SignatureList;
        [JsonIgnore]
        public Dictionary<string, List<string>> TypeUnitDictionary => _model.TypeUnitDictionary;
        [JsonIgnore]
        public ICommand SignatureCalAdded { get; set; }
        private void _addASignatureStep(object obj)
        {
            var sig = ((SignatureCalMenu)obj).Signature;
            var newSig = new SignatureSettingViewModel(sig);
            newSig.SampleDataMngr = SampleDataMngr;
            newSig.WindowOverlapStr = WindowOverlapStr;
            newSig.WindowSizeStr = WindowSizeStr;
            newSig.StepCounter = SignatureSettings.Count + 1;
            newSig.ThisStepInputsGroupedByType = new SignalTree("Step " + newSig.StepCounter.ToString() + " - " + newSig.SignatureName);
            newSig.ThisStepInputsGroupedByType.SignalCheckStatusChanged += _signalCheckStatusChanged;
            newSig.ThisStepOutputsGroupedByPMU = new SignalTree("Step " + newSig.StepCounter.ToString() + " - " + newSig.SignatureName);
            newSig.ThisStepOutputsGroupedByPMU.SignalCheckStatusChanged += _signalCheckStatusChanged;
            SampleDataMngr.GroupedSignalBySignatureStepsInput.Add(newSig.ThisStepInputsGroupedByType);
            SignatureSettings.Add(newSig);
            Model.SignatureSettings.Add(newSig.Model);
            _signatureStepSelected(newSig);
        }
        [JsonIgnore]
        public ICommand SignatureStepSelected { get; set; }
        private void _signatureStepSelected(object obj)
        {
            SignatureSettingViewModel step = obj as SignatureSettingViewModel;
            if (SelectedStep != step)
            {
                if (SelectedStep != null)
                {
                    //SelectedStep.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(SelectedStep.InputChannels);
                    SelectedStep.IsSelected = false;
                    try
                    {
                        SelectedStep.UpdateInputOutputTree();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                var lastNmberOfSteps = step.StepCounter;
                var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                foreach (var sig in SignatureSettings)
                {
                    if (sig.StepCounter < lastNmberOfSteps)
                    {
                        stepsInputAsSignalHierachy.Add(sig.ThisStepInputsGroupedByType);
                    }
                    else
                    {
                        break;
                    }
                }
                step.IsSelected = true;
                SelectedStep = step;
                SampleDataMngr.GroupedSignalBySignatureStepsInput = stepsInputAsSignalHierachy;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        [JsonIgnore]
        public ICommand DeleteASignatureStep { get; set; }
        private void _deleteASignatureStep(object obj)
        {
            // Delete step
            SignatureSettingViewModel stepRemove = (SignatureSettingViewModel)obj;
            if (MessageBox.Show("Delete step " + stepRemove.StepCounter.ToString() + " in signature calculation: " + stepRemove.SignatureName + "?", "Warning!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
                            step.ThisStepInputsGroupedByType.Label = "Step " + step.StepCounter.ToString() + " - " + step.SignatureName;
                        }
                    }
                    Model.SignatureSettings.Remove(stepRemove.Model);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error deleting step");
                }
                // Select a different step?
                if (SelectedStep != null)
                {
                    _deSelectAllSteps(null);
                }
            }
        }
        [JsonIgnore]
        public ICommand DeSelectAllSteps { get; set; }
        private void _deSelectAllSteps(object obj)
        {
            if (SelectedStep != null)
            {
                //foreach (var s in SelectedStep.InputChannels)
                //{
                //    s.IsChecked = false;
                //}
                SelectedStep.IsSelected = false;
                try
                {
                    SelectedStep.UpdateInputOutputTree();
                    if (SelectedStep is PreProcessStepViewModel)
                    {
                        var st = SelectedStep as PreProcessStepViewModel;
                        st.SetupOutputSignals();
                        var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                        var stepsOutputAsSignalHierachy = new ObservableCollection<SignalTree>();
                        foreach (var stp in PreProcessSteps)
                        {
                            stepsInputAsSignalHierachy.Add(stp.ThisStepInputsGroupedByType);
                            if (stp.Model is Customization)
                            {
                                stepsOutputAsSignalHierachy.Add(stp.ThisStepOutputsGroupedByPMU);
                            }
                        }
                        SelectedStep = null;
                        SampleDataMngr.GroupedSignalByPreProcessStepsInput = stepsInputAsSignalHierachy;
                        SampleDataMngr.GroupedSignalByPreProcessStepsOutput = stepsOutputAsSignalHierachy;
                        SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
                        SampleDataMngr.SignalSelectionTreeViewVisibility = Visibility.Visible;
                    }
                    if (SelectedStep is SignatureSettingViewModel)
                    {
                        var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                        foreach (var stp in SignatureSettings)
                        {
                            stepsInputAsSignalHierachy.Add(stp.ThisStepInputsGroupedByType);
                        }
                        SelectedStep = null;
                        SampleDataMngr.GroupedSignalBySignatureStepsInput = stepsInputAsSignalHierachy;
                        SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
                    }
                    if (SelectedStep is DataWriterViewModel)
                    {
                        var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                        foreach (var stp in DataWriters)
                        {
                            stepsInputAsSignalHierachy.Add(stp.ThisStepInputsGroupedByType);
                        }
                        SelectedStep = null;
                        SampleDataMngr.GroupedSignalByDataWriterStepsInput = stepsInputAsSignalHierachy;
                        SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private ObservableCollection<SignatureSettingViewModel> _signatureSettings = new ObservableCollection<SignatureSettingViewModel>();
        public ObservableCollection<SignatureSettingViewModel> SignatureSettings 
        {
            get { return _signatureSettings; }
            set { _signatureSettings = value;
                OnPropertyChanged();
            } 
        }
        //private int _oldTabIndex;
        private int _currentTabIndex;
        [JsonIgnore]
        public int CurrentTabIndex 
        {
            get { return _currentTabIndex; }
            set {
                if (_currentTabIndex != value)
                {
                    if (_currentTabIndex == 1 || (_currentTabIndex == 0 && value == 2) || (_currentTabIndex == 0 && value == 3))
                    {
                        var allDataPreprocessOutputSignals = _getAllPreprocessOutput();
                        try
                        {
                            SampleDataMngr.AllPreProcessOutputGroupedByType = SampleDataMngr.SortSignalsByType(allDataPreprocessOutputSignals);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        try
                        {
                            SampleDataMngr.AllPreProcessOutputGroupedByPMU = SampleDataMngr.SortSignalByPMU(allDataPreprocessOutputSignals);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
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
        private ObservableCollection<SignalViewModel> _getAllPreprocessOutput()
        {
            var results = new ObservableCollection<SignalViewModel>();
            foreach (var stp in PreProcessSteps)
            {
                foreach (var sig in stp.OutputChannels)
                {
                    if (!results.Contains(sig))
                    {
                        results.Add(sig);
                    }
                }
            }
            return results;
        }
        [JsonProperty("DatawriteOutFrequency")]
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
        [JsonProperty("DatawriteOutFrequencyUnit")]
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
        [JsonIgnore]
        public List<string> DatawriteOutFrequencyUnits { get { return _model.DatawriteOutFrequencyUnits; } }
        [JsonProperty("WindowSize")]
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
                    //do I need to do this for data writers too?
                }
            }
        }
        [JsonProperty("WindowOverlap")]
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
        [JsonIgnore]
        private string _previousSaveFileDirectory;
        internal void SaveConfigFile(string configFilePath)
        {
            //_model.SaveConfigFile();
            var config = JsonConvert.SerializeObject(this, Formatting.Indented);
#if DEBUG
            Console.WriteLine(config);
#endif
            //using (var fbd = new CommonSaveFileDialog())
            //{
            //    fbd.InitialDirectory = _previousSaveFileDirectory;
            //    fbd.AddToMostRecentlyUsedList = true;
            //    fbd.DefaultDirectory = _previousSaveFileDirectory;
            //    fbd.EnsureFileExists = true;
            //    fbd.EnsurePathExists = true;
            //    fbd.EnsureReadOnly = false;
            //    fbd.EnsureValidNames = true;
            //    fbd.ShowPlacesList = true;
            //    fbd.RestoreDirectory = true;
            //    fbd.Title = "Please Select a Directory to Save Config file.";
            //    fbd.Filters.Add(new CommonFileDialogFilter("Json files", "*.json"));
            //    fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
            //    CommonFileDialogResult result = fbd.ShowDialog();
            //    if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
            //    {
            //        ConfigFile = fbd.FileName;
            //        _previousSaveFileDirectory = Path.GetDirectoryName(fbd.FileName);
            //        using (StreamWriter outputFile = new StreamWriter(ConfigFile))
            //        {
            //            outputFile.WriteLine(config);
            //        }
            //    }
            //}
            using (StreamWriter outputFile = new StreamWriter(configFilePath))
            {
                outputFile.WriteLine(config);
            }
        }
        private string _configFile;
        [JsonIgnore]
        public string ConfigFile 
        {
            get { return _configFile; }
            set 
            {
                _configFile = value;
            }
        }
        [JsonIgnore]
        private string _previousFileDirectory;
        internal void OpenConfigFile()
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = _previousFileDirectory;
                fbd.IsFolderPicker = false;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = _previousFileDirectory;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select Archive Sprinter Config file.";
                fbd.Filters.Add(new CommonFileDialogFilter("Json files", "*.json"));
                fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();
                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    ConfigFile = fbd.FileName;
                    _previousFileDirectory = Path.GetDirectoryName(fbd.FileName);
                }
            }
            if (File.Exists(ConfigFile))
            {
                ReadConfigFile(ConfigFile);
                //_model.ReadConfigFile(ConfigFile);
            }
        }
        public void ReadConfigFile(string configFile)
        {
            using (StreamReader reader = File.OpenText(configFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                var config = (SettingsViewModel)serializer.Deserialize(reader, typeof(SettingsViewModel));
                WindowOverlapStr = config.WindowOverlapStr;
                WindowSizeStr = config.WindowSizeStr;
                DataSourceVM = config.DataSourceVM;
                DatawriteOutFrequencyStr = config.DatawriteOutFrequencyStr;
                DatawriteOutFrequencyUnit = config.DatawriteOutFrequencyUnit;
                //SignatureOutputDir = config.SignatureOutputDir;
                PreProcessSteps = new ObservableCollection<PreProcessStepViewModel>();
                SignatureSettings = new ObservableCollection<SignatureSettingViewModel>();
                DataWriters = new ObservableCollection<DataWriterViewModel>();
                DateTimeStart = config.DateTimeStart;
                DateTimeEnd = config.DateTimeEnd;
                foreach (var pre in config.PreProcessSteps)
                {
                    var newStep = new PreProcessStepViewModel(pre.Name);
                    newStep.SampleDataMngr = SampleDataMngr;
                    newStep.StepCounter = PreProcessSteps.Count + 1;
                    PreProcessSteps.Add(newStep);
                    if (newStep.Model is VoltPhasorFilt)
                    {
                        newStep.NomVoltage = pre.NomVoltage;
                        newStep.VoltMin = pre.VoltMin;
                        newStep.VoltMax = pre.VoltMax;
                    }
                    if (newStep.Model is FreqFilt)
                    {
                        newStep.FreqMaxChan = pre.FreqMaxChan;
                        newStep.FreqMinChan = pre.FreqMinChan;
                        newStep.FreqPctChan = pre.FreqPctChan;
                        newStep.FreqMaxSamp = pre.FreqMaxSamp;
                        newStep.FreqMinSamp = pre.FreqMinSamp;
                    }
                    if (newStep.Model is OutlierFilt)
                    {
                        newStep.StdDevMult = pre.StdDevMult;
                    }
                    if (newStep.Model is StaleDQFilt)
                    {
                        newStep.StaleThresh = pre.StaleThresh;
                    }
                    if (newStep.Model is DataFrameDQFilt)
                    {
                        newStep.PercentBadThresh = pre.PercentBadThresh;
                        newStep.KeepDiffPMUSeparate = pre.KeepDiffPMUSeparate;
                    }
                    if (newStep.Model is Customization)
                    {
                        newStep.CustPMUname = pre.CustPMUname;
                        newStep.SignalName = pre.SignalName;
                        if (pre.Subtrahend != null)
                        {
                            var subtrahend = SampleDataMngr.FindSignal(pre.Subtrahend.PMUName, pre.Subtrahend.SignalName);
                            if (subtrahend != null)
                            {
                                newStep.Subtrahend = subtrahend;
                            }
                        }
                        if (pre.Minuend != null)
                        {
                            var minuend = SampleDataMngr.FindSignal(pre.Minuend.PMUName, pre.Minuend.SignalName);
                            if (minuend != null)
                            {
                                newStep.Minuend = minuend;
                            }
                        }
                        newStep.Scalar = pre.Scalar;
                        newStep.TypeAbbreviation = pre.TypeAbbreviation;
                        newStep.Unit = pre.Unit;
                        if (pre.TimeSourcePMU != null)
                        {
                            newStep.TimeSourcePMU = new PMUWithSamplingRate(pre.TimeSourcePMU.PMU, pre.TimeSourcePMU.SamplingRate, pre.TimeSourcePMU.SignalLength);
                        }
                        newStep.SamplingRate = pre.SamplingRate;
                        if (pre.Dividend != null)
                        {
                            var divident = SampleDataMngr.FindSignal(pre.Dividend.PMUName, pre.Dividend.SignalName);
                            if (divident != null)
                            {
                                newStep.Dividend = divident;
                            }
                        }
                        if (pre.Divisor != null)
                        {
                            var divisor = SampleDataMngr.FindSignal(pre.Divisor.PMUName, pre.Divisor.SignalName);
                            if (divisor != null)
                            {
                                newStep.Divisor = divisor;
                            }
                        }
                        newStep.Exponent = pre.Exponent;
                        foreach (var item in pre.OneToOneSignalPairs)
                        {
                            var input = SampleDataMngr.FindSignal(item.Input.PMUName, item.Input.SignalName);
                            if (input != null)
                            {
                                if (newStep.Model is ExponentialCust)
                                {
                                    var output = newStep.ExponentAddOutputSignalOfthisInput(input);
                                    output.SignalName = item.Output.SignalName;
                                }
                                else if (newStep.Model is AngleCust)
                                {
                                    var output = newStep.AngleCalculationAddOutputSignalOfthisInput(input);
                                    output.SignalName = item.Output.SignalName;
                                }
                                else if (newStep.Model is MetricPrefixCust)
                                {
                                    var output = newStep.UnaryAddOutputSignalOfthisInput(input);
                                    output.SignalName = item.Output.SignalName;
                                    output.Unit = item.Output.Unit;
                                }
                                else
                                {
                                    var output = newStep.UnaryAddOutputSignalOfthisInput(input);
                                    output.SignalName = item.Output.SignalName;
                                }
                            }
                        }
                        foreach (var item in pre.OneToTwoSignalPairs)
                        {
                            var mag = SampleDataMngr.FindSignal(item.Mag.PMUName, item.Mag.SignalName);
                            var ang = SampleDataMngr.FindSignal(item.Ang.PMUName, item.Ang.SignalName);
                            if (mag != null || ang != null)
                            {
                                newStep.CreatPhasorAddSignalPair(mag, ang, item.Output.SignalName);
                            }
                        }
                        newStep.IsFromPhasor = pre.IsFromPhasor;
                        newStep.PowType = pre.PowType;
                        if (pre.PhasorPowerInput != null)
                        {
                            if (pre.PhasorPowerInput.VoltagePhasor != null)
                            {
                                var vphasor = SampleDataMngr.FindSignal(pre.PhasorPowerInput.VoltagePhasor.PMUName, pre.PhasorPowerInput.VoltagePhasor.SignalName);
                                if (vphasor != null)
                                {
                                    newStep.PhasorPowerInput.VoltagePhasor = vphasor;
                                    //newStep.InputChannels.Add(vphasor);
                                }
                            }
                            if (pre.PhasorPowerInput.CurrentPhasor != null)
                            {
                                var iphasor = SampleDataMngr.FindSignal(pre.PhasorPowerInput.CurrentPhasor.PMUName, pre.PhasorPowerInput.CurrentPhasor.SignalName);
                                if (iphasor != null)
                                {
                                    newStep.PhasorPowerInput.CurrentPhasor = iphasor;
                                    //newStep.InputChannels.Add(iphasor);
                                }
                            }
                        }
                        if (pre.MagAngPowerInput != null)
                        {
                            if (pre.MagAngPowerInput.VoltageMag != null)
                            {
                                var vm = SampleDataMngr.FindSignal(pre.MagAngPowerInput.VoltageMag.PMUName, pre.MagAngPowerInput.VoltageMag.SignalName);
                                if (vm != null)
                                {
                                    newStep.MagAngPowerInput.VoltageMag = vm;
                                    //newStep.InputChannels.Add(vm);
                                }
                            }
                            if (pre.MagAngPowerInput.CurrentMag != null)
                            {
                                var im = SampleDataMngr.FindSignal(pre.MagAngPowerInput.CurrentMag.PMUName, pre.MagAngPowerInput.CurrentMag.SignalName);
                                if (im != null)
                                {
                                    newStep.MagAngPowerInput.CurrentMag = im;
                                    //newStep.InputChannels.Add(im);
                                }
                            }
                            if (pre.MagAngPowerInput.VoltageAng != null)
                            {
                                var va = SampleDataMngr.FindSignal(pre.MagAngPowerInput.VoltageAng.PMUName, pre.MagAngPowerInput.VoltageAng.SignalName);
                                if (va != null)
                                {
                                    newStep.MagAngPowerInput.VoltageAng = va;
                                    //newStep.InputChannels.Add(va);
                                }
                            }
                            if (pre.MagAngPowerInput.CurrentAng != null)
                            {
                                var ia = SampleDataMngr.FindSignal(pre.MagAngPowerInput.CurrentAng.PMUName, pre.MagAngPowerInput.CurrentAng.SignalName);
                                if (ia != null)
                                {
                                    newStep.MagAngPowerInput.CurrentAng = ia;
                                    //newStep.InputChannels.Add(ia);
                                }
                            }
                        }
                        if (newStep.Model is SpecifySignalTypeUnitCust)
                        {
                            var input = SampleDataMngr.FindSignal(pre.InputChannels[0].PMUName, pre.InputChannels[0].SignalName);
                            if (input !=null)
                            {
                                newStep.AddinputToSpecifyTypeUnit(input);
                            }
                        }
                    }
                    // take care of all input output signals
                    foreach (var sig in pre.InputChannels)
                    {
                        var foundSig = SampleDataMngr.FindSignal(sig.PMUName, sig.SignalName);
                        if (foundSig != null)
                        {
                            newStep.InputChannels.Add(foundSig);
                        }
                    }
                    //if (newStep.Model is Filter)
                    //{
                    //    newStep.OutputChannels = newStep.InputChannels;
                    //}
                    //else
                    //{
                    //    foreach (var sig in newStep.InputChannels)
                    //    {
                    //        // need to make up output signals from each input, which might depends on different customization and all differ
                    //    }
                    //}
                    newStep.ThisStepInputsGroupedByType = new SignalTree("Step " + newStep.StepCounter.ToString() + " - " + newStep.Name);
                    newStep.ThisStepInputsGroupedByType.SignalCheckStatusChanged += _signalCheckStatusChanged;
                    newStep.ThisStepOutputsGroupedByPMU = new SignalTree("Step " + newStep.StepCounter.ToString() + " - " + newStep.Name);
                    newStep.ThisStepOutputsGroupedByPMU.SignalCheckStatusChanged += _signalCheckStatusChanged;
                    newStep.ThisStepOutputsGroupedByPMU.SignalList = SampleDataMngr.SortSignalByPMU(newStep.OutputChannels);
                    newStep.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(newStep.InputChannels);
                    SampleDataMngr.GroupedSignalByPreProcessStepsInput.Add(newStep.ThisStepInputsGroupedByType);
                    if (newStep.Model is Customization)
                    {
                        SampleDataMngr.GroupedSignalByPreProcessStepsOutput.Add(newStep.ThisStepOutputsGroupedByPMU);
                    }
                }
                foreach (var signature in config.SignatureSettings)
                {
                    var newSig = new SignatureSettingViewModel(signature.SignatureName);
                    newSig.SampleDataMngr = SampleDataMngr;
                    newSig.WindowOverlapStr = WindowOverlapStr;
                    newSig.WindowSizeStr = WindowSizeStr;
                    newSig.StepCounter = SignatureSettings.Count + 1;
                    newSig.OmitNan = signature.OmitNan;
                    if (newSig.Model is RootMeanSquare)
                    {
                        newSig.RemoveMean = signature.RemoveMean;
                    }
                    if (newSig.Model is FrequencyBandRMS)
                    {
                        newSig.CalculateFull = signature.CalculateFull;
                        newSig.CalculateBand2 = signature.CalculateBand2;
                        newSig.CalculateBand3 = signature.CalculateBand3;
                        newSig.CalculateBand4 = signature.CalculateBand4;
                        newSig.Threshold = signature.Threshold;
                    }
                    if (newSig.Model is Percentile)
                    {
                        newSig.PercentileStr = signature.PercentileStr;
                    }
                    if (newSig.Model is Histogram)
                    {
                        newSig.Minimum = signature.Minimum;
                        newSig.Maximum = signature.Maximum;
                        newSig.NumberOfBins = signature.NumberOfBins;
                    }
                    SignatureSettings.Add(newSig);
                    Model.SignatureSettings.Add(newSig.Model);
                    foreach (var sig in signature.InputChannels)
                    {
                        var foundSig = SampleDataMngr.FindSignal(sig.PMUName, sig.SignalName);
                        if (foundSig != null)
                        {
                            newSig.InputChannels.Add(foundSig);
                        }
                    }
                    newSig.ThisStepInputsGroupedByType = new SignalTree("Step " + newSig.StepCounter.ToString() + " - " + newSig.SignatureName);
                    newSig.ThisStepInputsGroupedByType.SignalCheckStatusChanged += _signalCheckStatusChanged;
                    //newStep.ThisStepOutputsGroupedByPMU.SignalList = SampleDataMngr.SortSignalByPMU(newStep.OutputChannels);
                    newSig.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(newSig.InputChannels);
                    SampleDataMngr.GroupedSignalBySignatureStepsInput.Add(newSig.ThisStepInputsGroupedByType);
                    //if (newStep.Model is Customization)
                    //{
                    //    SampleDataMngr.GroupedSignalByPreProcessStepsOutput.Add(newStep.ThisStepOutputsGroupedByPMU);
                    //}
                }
                foreach (var writer in config.DataWriters)
                {
                    var newWriter = new DataWriterViewModel(writer.Name);
                    newWriter.SampleDataMngr = SampleDataMngr;
                    newWriter.StepCounter = DataWriters.Count + 1;
                    newWriter.Mnemonic = writer.Mnemonic;
                    newWriter.SeparatePMUs = writer.SeparatePMUs;
                    newWriter.SavePath = writer.SavePath;
                    DataWriters.Add(newWriter);
                    Model.DataWriters.Add(newWriter.Model);
                    foreach (var sig in writer.InputChannels)
                    {
                        var foundSig = SampleDataMngr.FindSignal(sig.PMUName, sig.SignalName);
                        if (foundSig != null)
                        {
                            newWriter.InputChannels.Add(foundSig);
                        }
                    }
                    newWriter.ThisStepInputsGroupedByType = new SignalTree(newWriter.Name + " " + newWriter.StepCounter.ToString());
                    newWriter.ThisStepInputsGroupedByType.SignalCheckStatusChanged += _signalCheckStatusChanged;
                    newWriter.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(newWriter.InputChannels);
                    SampleDataMngr.GroupedSignalByDataWriterStepsInput.Add(newWriter.ThisStepInputsGroupedByType);
                }
            }
        }
        //public string SignatureOutputDir
        //{
        //    get { return _model.SignatureOutputDir; }
        //    set
        //    {
        //        if (_model.SignatureOutputDir != value)
        //        {
        //            _model.SignatureOutputDir = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        //private string _previousOutputDirectory;
        //[JsonIgnore]
        //public ICommand SelectSignatureOutputDir { get; set; }
        //private void _selectSignatureOutputDir(object obj)
        //{
        //    using (var fbd = new CommonOpenFileDialog())
        //    {
        //        fbd.InitialDirectory = _previousOutputDirectory;
        //        fbd.IsFolderPicker = true;
        //        fbd.AddToMostRecentlyUsedList = true;
        //        fbd.AllowNonFileSystemItems = false;
        //        fbd.DefaultDirectory = _previousOutputDirectory;
        //        fbd.EnsureFileExists = true;
        //        fbd.EnsurePathExists = true;
        //        fbd.EnsureReadOnly = false;
        //        fbd.EnsureValidNames = true;
        //        fbd.Multiselect = false;
        //        fbd.ShowPlacesList = true;
        //        fbd.RestoreDirectory = true;
        //        fbd.Title = "Please Select Signature Output Directory.";
        //        //fbd.Filters.Add(new CommonFileDialogFilter("CSV files", "*.csv"));
        //        //fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
        //        CommonFileDialogResult result = fbd.ShowDialog();
        //        if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
        //        {
        //            SignatureOutputDir = fbd.FileName;
        //            _previousOutputDirectory = fbd.FileName;
        //        }
        //    }
        //}
        //string jsonTypeNameAll = JsonConvert.SerializeObject(SignatureSettings, Formatting.Indented, new JsonSerializerSettings
        //{
        //    TypeNameHandling = TypeNameHandling.All
        //});

        //Console.WriteLine(jsonTypeNameAll);
        public string DateTimeStart 
        {
            get { return _model.DateTimeStart; }
            set
            {
                _model.DateTimeStart = value;
                OnPropertyChanged();
            }
        }
        public string DateTimeEnd
        {
            get { return _model.DateTimeEnd; }
            set
            {
                _model.DateTimeEnd = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<DataWriterViewModel> _dataWriters = new ObservableCollection<DataWriterViewModel>();
        public ObservableCollection<DataWriterViewModel> DataWriters 
        {
            get { return _dataWriters; }
            set
            {
                _dataWriters = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public ICommand AddDataWriter { get; set; }
        private void _addDataWriter(object obj)
        {
            var newWriter = new DataWriterViewModel();
            newWriter.SampleDataMngr = SampleDataMngr;
            newWriter.StepCounter = DataWriters.Count + 1;
            newWriter.ThisStepInputsGroupedByType = new SignalTree("Step " + newWriter.StepCounter.ToString() + " - " + newWriter.Name);
            newWriter.ThisStepInputsGroupedByType.SignalCheckStatusChanged += _signalCheckStatusChanged;
            newWriter.ThisStepOutputsGroupedByPMU = new SignalTree("Step " + newWriter.StepCounter.ToString() + " - " + newWriter.Name);
            newWriter.ThisStepOutputsGroupedByPMU.SignalCheckStatusChanged += _signalCheckStatusChanged;
            SampleDataMngr.GroupedSignalByDataWriterStepsInput.Add(newWriter.ThisStepInputsGroupedByType);
            DataWriters.Add(newWriter);
            Model.DataWriters.Add(newWriter.Model);
            _dataWriterSelected(newWriter);
        }
        [JsonIgnore]
        public ICommand DataWriterSelected { get; set; }
        private void _dataWriterSelected(object obj)
        {
            DataWriterViewModel step = obj as DataWriterViewModel;
            if (SelectedStep != step)
            {
                if (SelectedStep != null)
                {
                    //SelectedStep.ThisStepInputsGroupedByType.SignalList = SampleDataMngr.SortSignalsByType(SelectedStep.InputChannels);
                    SelectedStep.IsSelected = false;
                    try
                    {
                        SelectedStep.UpdateInputOutputTree();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                var lastNmberOfSteps = step.StepCounter;
                var stepsInputAsSignalHierachy = new ObservableCollection<SignalTree>();
                foreach (var sig in DataWriters)
                {
                    if (sig.StepCounter < lastNmberOfSteps)
                    {
                        stepsInputAsSignalHierachy.Add(sig.ThisStepInputsGroupedByType);
                    }
                    else
                    {
                        break;
                    }
                }
                step.IsSelected = true;
                SelectedStep = step;
                SampleDataMngr.GroupedSignalByDataWriterStepsInput = stepsInputAsSignalHierachy;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        [JsonIgnore]
        public ICommand DeleteDataWriter { get; set; }
        private void _deleteAdatawriter(object obj)
        {
            // Delete step
            DataWriterViewModel stepRemove = (DataWriterViewModel)obj;
            if (MessageBox.Show("Delete data writer " + stepRemove.StepCounter.ToString() + "?", "Warning!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Try to delete current step
                try
                {
                    DataWriters.Remove(stepRemove);
                    foreach (var step in DataWriters)
                    {
                        if (step.StepCounter > stepRemove.StepCounter)
                        {
                            step.StepCounter -= 1;
                            step.ThisStepInputsGroupedByType.Label = "Step " + step.StepCounter.ToString() + " - " + step.Name;
                        }
                    }
                    Model.DataWriters.Remove(stepRemove.Model);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error deleting a data writer");
                }
                // Select a different step?
                if (SelectedStep != null)
                {
                    _deSelectAllSteps(null);
                }
            }
        }
        [JsonIgnore]
        public ICommand SetCurrentFocusedTextbox { get; set; }
        private void _setCurrentlySelectedTextbox(object obj)
        {
            if (SelectedStep != null)
            {
                foreach (var s in SelectedStep.InputChannels)
                {
                    s.IsChecked = false;
                }
                if (obj != null)
                {
                    var signal = obj as SignalViewModel;
                    if (!string.IsNullOrEmpty(signal.TypeAbbreviation) && !string.IsNullOrEmpty(signal.PMUName))
                    {
                        signal.IsChecked = true;
                    }
                }
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        public ICommand SetCurrentFocusedTextboxUnarySteps { get; set; }
        private void _setCurrentFocusedTextboxUnarySteps(object obj)
        {
            if (SelectedStep != null)
            {
                foreach (var s in SelectedStep.InputChannels)
                {
                    s.IsChecked = false;
                }
                if (obj != null)
                {
                    UnaryInputOutputPair pair = (UnaryInputOutputPair)obj;
                    if (pair.Input != null && pair.Output != null)
                    {
                        pair.Input.IsChecked = true;
                        if (SelectedStep is PreProcessStepViewModel)
                        {
                            var st = SelectedStep as PreProcessStepViewModel;
                            st.SelectedOneToOneInputOutputPair = pair;
                        }
                    }
                }
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        public ICommand SetCurrentPhasorCreationFocusedTextBox { get; set; }
        private void _setCurrentPhasorCreationFocusedTextBox(object obj)
        {
            if (SelectedStep != null)
            {
                foreach (var s in SelectedStep.InputChannels)
                {
                    s.IsChecked = false;
                }
                if (obj != null)
                {
                    object[] parameters = obj as object[];
                    CreatPhasorInputOutputSignalsViewModel pair = (CreatPhasorInputOutputSignalsViewModel)parameters[0];
                    if (parameters[1] != null)
                    {
                        //pair.Value.IsChecked = true;
                    }
                    if (pair != null)
                    {
                        if (pair.Ang != null)
                        {
                            pair.Ang.IsChecked = true;
                        }
                        if (pair.Mag != null)
                        {
                            pair.Mag.IsChecked = true;
                        }
                        if (SelectedStep is PreProcessStepViewModel)
                        {
                            var st = SelectedStep as PreProcessStepViewModel;
                            st.SelectedOneToTwoInputOutputPair = pair;
                        }
                    }
                }
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        public ICommand PowerPhasorTextBoxGotFocus { get; set; }
    }
}
