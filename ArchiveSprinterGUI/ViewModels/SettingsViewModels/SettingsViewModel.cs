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
using Newtonsoft.Json;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            _previousOutputDirectory = Environment.CurrentDirectory;

            SampleDataMngr = new SampleDataManagerViewModel();
            SampleDataMngr.SignalCheckStatusChanged += _signalCheckStatusChanged;

            DataConfigStepSelected = new RelayCommand(_dataConfigStepSelected);
            DataConfigStepAdded = new RelayCommand(_dataConfigStepAdded);
            DeleteDataConfigStep = new RelayCommand(_deleteDataConfigStep);

            SignatureCalAdded = new RelayCommand(_addASignatureStep);
            SignatureStepSelected = new RelayCommand(_signatureStepSelected);
            DeleteASignatureStep = new RelayCommand(_deleteASignatureStep);
            DeSelectAllSteps = new RelayCommand(_deSelectAllSteps);
            SelectSignatureOutputDir = new RelayCommand(_selectSignatureOutputDir);
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
            OnPropertyChanged();

        }

        [JsonIgnore]
        public ICommand DataConfigStepAdded { get; set; }
        private void _dataConfigStepAdded(object obj)
        {
            string stepName = (string)obj;
            PreProcessStepViewModel newStep = new PreProcessStepViewModel(stepName);
     
            newStep.StepCounter = PreProcessSteps.Count + 1;
            newStep.ThisStepOutputsGroupedByPMU = new SignalTree("Step " + newStep.StepCounter.ToString() + " _ " + newStep.Name);
            PreProcessSteps.Add(newStep);
            // Set step to edit
            _stepSelectedToEdit(newStep);
            OnPropertyChanged();

        }

        [JsonIgnore]
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
            if (SelectedStep != null)
            {
                SelectedStep.UpdateInputOutputTree();
                if (SelectedStep is PreProcessStepViewModel)
                {
                    SelectedStep.ThisStepOutputsGroupedByPMU.SignalList = SampleDataMngr.SortSignalByPMU(SelectedStep.InputChannels);
                }
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
                SelectedStep.AddSignal(tree.Signal);
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
        [JsonIgnore]
        public List<String> DQFilterList => _model.DQFilterList;
        [JsonIgnore]
        public List<String> CustomizationList => _model.CustomizationList;
        [JsonIgnore]
        public List<SignatureCalMenu> SignatureList => _model.SignatureList;
        [JsonIgnore]
        public ICommand SignatureCalAdded { get; set; }
        private void _addASignatureStep(object obj)
        {
            var sig = ((SignatureCalMenu)obj).Signature;
            var newSig = new SignatureSettingViewModel(sig);
            newSig.WindowOverlapStr = WindowOverlapStr;
            newSig.WindowSizeStr = WindowSizeStr;
            newSig.StepCounter = SignatureSettings.Count + 1;
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
                    SelectedStep.IsSelected = false;
                }
                step.IsSelected = true;
                SelectedStep = step;
                SampleDataMngr.DetermineCheckStatusOfGroupedSignals();
            }
        }
        [JsonIgnore]
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
                    Model.SignatureSettings.Remove(stepRemove.Model);
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
        [JsonIgnore]
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
        internal void SaveConfigFile()
        {
            //_model.SaveConfigFile();
            var config = JsonConvert.SerializeObject(this, Formatting.Indented);
            Console.WriteLine(config);

            using (var fbd = new CommonSaveFileDialog())
            {
                fbd.InitialDirectory = _previousSaveFileDirectory;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.DefaultDirectory = _previousSaveFileDirectory;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select a Directory to Save Config file.";
                fbd.Filters.Add(new CommonFileDialogFilter("Json files", "*.json"));
                fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();
                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    ConfigFile = fbd.FileName;
                    _previousSaveFileDirectory = Path.GetDirectoryName(fbd.FileName);
                    using (StreamWriter outputFile = new StreamWriter(ConfigFile))
                    {
                        outputFile.WriteLine(config);
                    }
                }
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
                _readConfigFile(ConfigFile);
                //_model.ReadConfigFile(ConfigFile);
            }
        }
        private void _readConfigFile(string configFile)
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
                SignatureOutputDir = config.SignatureOutputDir;
                PreProcessSteps = new ObservableCollection<PreProcessStepViewModel>();
                SignatureSettings = new ObservableCollection<SignatureSettingViewModel>();
                foreach (var pre in config.PreProcessSteps)
                {
                    var newStep = new PreProcessStepViewModel(pre.Name);
                    newStep.StepCounter = PreProcessSteps.Count + 1;
                    newStep.ThisStepOutputsGroupedByPMU = new SignalTree("Step " + newStep.StepCounter.ToString() + " _ " + newStep.Name);
                    PreProcessSteps.Add(newStep);
                    foreach (var sig in pre.InputChannels)
                    {
                        var foundSig = SampleDataMngr.FindSignal(sig.PMUName, sig.SignalName);
                        if (foundSig != null)
                        {
                            newStep.InputChannels.Add(foundSig);
                        }
                    }
                    newStep.ThisStepOutputsGroupedByPMU.SignalList = SampleDataMngr.SortSignalByPMU(newStep.InputChannels);
                    if (newStep.Model is VoltPhasorFilt)
                    {
                        newStep.NomVoltage = pre.NomVoltage;
                        newStep.VoltMin = pre.VoltMin;
                        newStep.VoltMax = pre.VoltMax;
                    }
                }
                foreach (var signature in config.SignatureSettings)
                {
                    var newSig = new SignatureSettingViewModel(signature.SignatureName);
                    newSig.WindowOverlapStr = WindowOverlapStr;
                    newSig.WindowSizeStr = WindowSizeStr;
                    newSig.StepCounter = SignatureSettings.Count + 1;
                    newSig.OmitNan = signature.OmitNan;
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
                }
            }
        }
        public string SignatureOutputDir
        {
            get { return _model.SignatureOutputDir; }
            set
            {
                if (_model.SignatureOutputDir != value)
                {
                    _model.SignatureOutputDir = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _previousOutputDirectory;
        [JsonIgnore]
        public ICommand SelectSignatureOutputDir { get; set; }
        private void _selectSignatureOutputDir(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = _previousOutputDirectory;
                fbd.IsFolderPicker = true;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = _previousOutputDirectory;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select Signature Output Directory.";
                //fbd.Filters.Add(new CommonFileDialogFilter("CSV files", "*.csv"));
                //fbd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();
                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    SignatureOutputDir = fbd.FileName;
                    _previousOutputDirectory = fbd.FileName;
                }
            }
        }


        //string jsonTypeNameAll = JsonConvert.SerializeObject(SignatureSettings, Formatting.Indented, new JsonSerializerSettings
        //{
        //    TypeNameHandling = TypeNameHandling.All
        //});

        //Console.WriteLine(jsonTypeNameAll);
    }
}
