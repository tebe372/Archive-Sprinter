using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
using ArchiveSprinterGUI.ViewModels.SignalInspectionViewModels;
using AS.Config;
using AS.Core.Models;
using AS.DataManager;
using AS.IO;
using AS.SampleDataManager;
using AS.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ArchiveSprinterGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            _sampleDataMgr = SampleDataMngr.Instance;
            _settingsVM = new SettingsViewModel();
            _sgnlInspctVM = new SignalInspectionViewModel();
            _projectControlVM = new ProjectManagerViewModel();
            _projectControlVM.RunSelected += _onRunSelected;
            _currentView = _settingsVM;
            MainViewSelected = new RelayCommand(_switchView);
            StartArchiveSprinter = new RelayCommand(_startArchiveSprinter);
            PauseArchiveSprinter = new RelayCommand(_pauseArchiveSprinter);
            ResumeArchiveSprinter = new RelayCommand(_resumeArchiveSprinter);
            StopArchiveSprinter = new RelayCommand(_stopArchiveSprinter);
            DataMngr = new DataStore();
            DataMngr.ResultsWrittenDone += _onResultsWrittenDone;
            SaveConfigFile = new RelayCommand(_saveConfigFile);
            //OpenConfigFile = new RelayCommand(_openConfigFile);
            _numberOfFilesRead = 0;
            _noTaskingIsRunning = true;
        }
        private SampleDataMngr _sampleDataMgr;
        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private SettingsViewModel _settingsVM;
        public SettingsViewModel SettingsVM
        {
            get { return _settingsVM; }
            set
            {
                _settingsVM = value;
                OnPropertyChanged();
            }
        }
        private SignalInspectionViewModel _sgnlInspctVM;
        public SignalInspectionViewModel SgnlInspctVM
        {
            get { return _sgnlInspctVM; }
            set
            {
                _sgnlInspctVM = value;
                OnPropertyChanged();
            }
        }
        private ProjectManagerViewModel _projectControlVM;
        public ProjectManagerViewModel ProjectControlVM 
        {
            get { return _projectControlVM; }
            set
            {
                _projectControlVM = value;
                OnPropertyChanged();
            }
        }
        private FileReadingManager _reader;
        //public int NumberOfSignatures { get; set; }
        public ICommand MainViewSelected { get; set; }
        private void _switchView(object obj)
        {
            if ((string)obj == "Settings")
            {
                if (CurrentView is SignalInspectionViewModel)
                {
                    var view = (SignalInspectionViewModel)CurrentView;
                    view.DeSelectAllPlots(null);
                }
                CurrentView = SettingsVM;
            }
            else
            {
                CurrentView = _sgnlInspctVM;
            }
        }
        public ICommand StartArchiveSprinter { get; set; }
        private void _startArchiveSprinter(object obj)
        {
            ProjectControlVM.SelectedProject.SelectedRun.CheckTaskDirIntegrity();
            _numberOfFilesRead = 0;
            try
            {
                _setupFileManager();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            var numberOfDataWriters = SettingsVM.DataWriters.Count();
            var numberOfSignatures = SettingsVM.SignatureSettings.Count();
            _setupDataManager(numberOfDataWriters, numberOfSignatures);

            if (numberOfDataWriters > 0 || SettingsVM.SignatureSettings.Count > 0)
            {
                ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning = true;
                _currentRunningTask = ProjectControlVM.SelectedProject.SelectedRun;
                ProjectControlVM.CanRun = false;
                NoTaskingIsRunning = false;
                _startAS(numberOfDataWriters, numberOfSignatures);
            }
            else
            {
                MessageBox.Show("Archive Sprinter didn't run as there's no signature need to be calculated and no data need to be written out.");
            }
        }

        private async void _startAS(int numberOfDataWriters, int numberOfSignatures)
        {
            // this need to be put on a thread
            try
            {
                Task.Run(async () => { await _startReadingFile(); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //data.Start();
            // call another function that start the signature calculation, might be the computation/data manager, on a thread too.
            while (_numberOfFilesRead < 1 && !DataMngr.DataCompleted)
            {
                Thread.Sleep(1000);
            }
            if (numberOfDataWriters > 0)
            {
                try
                {
                    Task.Run(async () => { await _startDataWriters(); });
                }
                catch (Exception ex)
                {
                    //_reader.DataCompleted = true;
                    //DataMngr.DataCompleted = true;
                    //ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning = false;
                    MessageBox.Show(ex.Message);
                }
            }
            if (numberOfSignatures > 0)
            {
                while (!DataMngr.SignatureCanStart)
                {
                    Thread.Sleep(1000);
                }
                try
                {
                    await Task.Run(async () => { await _signatureCalculation(); });
                }
                catch (Exception ex)
                {
                    _reader.DataCompleted = true;
                    DataMngr.DataCompleted = true;
                    ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning = false;
                    NoTaskingIsRunning = true;
                    MessageBox.Show(ex.Message);
                }
                Thread.Sleep(500);
                try
                {
                    Task.Run(async () => { await _writeSignatureResults(); });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            // to delete signals that has been used for signature calculation and datawriter.
            try
            {
                Task.Run(async () => { await _deleteUsedSignals(); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task _deleteUsedSignals()
        {
            await Task.Factory.StartNew(() => DataMngr.DeleteUsedSignals());
        }
        private void _setupFileManager()
        {
            //scan through all the steps of configuration and figure out needed signals.
            List<string> neededSignalList = new List<string>();
            try
            {
                neededSignalList = _getAllNeededSignals();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //send list of signals to reader control

            //read files first by sending this source parameter
            var source = SettingsVM.DataSourceVM.Model;
            _reader = new FileReadingManager();
            _reader.SourceDirectory = source.FileDirectory;
            _reader.FileType = source.FileType;
            _reader.SamplingRate = source.SamplingRate;
            _reader.NumberOfDataPointInFile = source.NumberOfDataPointInFile;
            _reader.Mnemonic = source.Mnemonic;
            _reader.NeededSignalList = neededSignalList;
            _reader.FileReadingDone += _fileReadingDone;
            _reader.DataReadingDone += DataReadingDone;
            _reader.DateTimeStart = SettingsVM.DateTimeStart;
            _reader.DateTimeEnd = SettingsVM.DateTimeEnd;
            _reader.WindowSize = _settingsVM.Model.WindowSize;
        }
        private void _setupDataManager(int numberOfDataWriters, int numberOfSignatures)
        {
            DataMngr.Clean();
            DataMngr.NumberOfDataWriters = numberOfDataWriters;
            DataMngr.NumberOfSignatures = numberOfSignatures;
            DataMngr.DatawriteOutFrequency = _settingsVM.Model.DatawriteOutFrequency;
            DataMngr.DatawriteOutFrequencyUnit = _settingsVM.Model.DatawriteOutFrequencyUnit;
            DataMngr.WindowSize = _settingsVM.Model.WindowSize;
            DataMngr.WindowOverlap = _settingsVM.Model.WindowOverlap;
            //DataMngr.NumberOfSignatures = SettingsVM.SignatureSettings.Count();
            DataMngr.SignatureOutputDir = ProjectControlVM.SelectedProject.SelectedRun.SignaturePath;
            DataMngr.NumberOfDataPointInFile = SettingsVM.DataSourceVM.Model.NumberOfDataPointInFile;
            DataMngr.SamplingRate = SettingsVM.DataSourceVM.Model.SamplingRate;
            DataMngr.DateTimeEnd = SettingsVM.DateTimeEnd;
            DataMngr.DateTimeStart = SettingsVM.DateTimeStart;
        }
        private async Task _startDataWriters()
        {
            foreach (var item in SettingsVM.DataWriters)
            {
                item.GetSignalNameList();
                try
                {
                    Task.Factory.StartNew(() => item.Model.Process(DataMngr));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private List<string> _getAllNeededSignals()
        {
            var signalList = new List<string>();
            var signals = SettingsVM.SampleDataMngr.Model.Signals;
            foreach (var item in SettingsVM.PreProcessSteps)
            {
                if (item.Model is PMUflagFilt)
                {
                    signalList.AddRange(item.InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList());
                    //foreach (var ch in item.InputChannels)
                    //{
                    //    var pmu = ch.PMUName;
                    //    foreach (var g in SettingsVM.SampleDataMngr.GroupedRawSignalsByPMU)
                    //    {
                    //        if (g.Label == pmu)
                    //        {
                    //            signalList.AddRange(g.SignalList.Select(x => x.Signal.PMUName + "_" + x.Signal.SignalName).ToList());
                    //        }
                    //    }
                    //}
                }
                else if (item.Model is VoltPhasorFilt)
                {
                    signalList.AddRange(item.InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList());
                    //foreach (var ch in item.InputChannels)
                    //{
                    //    signalList.Add(ch.PMUName + "_" + ch.SignalName);
                    //    if (ch.TypeAbbreviation.Substring(0, 2) == "VM")
                    //    {
                    //        // if there a way to find corresponding angle signal? for pdat, we can substitute .m by .a to find it. but how about other data source such as csv?
                    //    }
                    //}
                }
                else
                {
                    signalList.AddRange(item.InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList());
                }
                //if (item.Model is Customization)
                //{
                //    try
                //    {
                //        item.SetupOutputSignals();
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}
            }
            foreach (var item in SettingsVM.SignatureSettings)
            {
                signalList.AddRange(item.InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList());
            }
            foreach (var item in SettingsVM.DataWriters)
            {
                signalList.AddRange(item.InputChannels.Select(x => x.PMUName + "_" + x.SignalName).ToList());
            }
            return signalList.Distinct().ToList();
        }
        private void DataReadingDone(object sender, DateTime e)
        {
            DataMngr.DataCompleted = true;
            DataMngr.FinalTimeStamp = e;
            if (DataMngr.NumberOfSignatures == 0)
            {
                //ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning = false;
                _currentRunningTask.IsTaskRunning = false;
                NoTaskingIsRunning = true;
            }
        }
        public DataStore DataMngr { get; set; }
        private int _numberOfFilesRead;
        private void _fileReadingDone(object sender, List<Signal> e, DateTime t)
        {
            // this function call have been put on thread, and put the data that have gone through pre process steps into a container in time order
            _preprocessData(e, t);
            _numberOfFilesRead++;
            CurrentFileTime = _getFileDateTime(_reader.CurrentFile).ToString("MM/dd/yyyy HH:mm:ss");
        }
        private async Task _startReadingFile()
        {
            try
            {
                _reader.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void _preprocessData(List<Signal> e, DateTime t)
        {
            bool newStage = true;
            List<Signal> filteredSignal = new List<Signal>();
            foreach (var item in SettingsVM.PreProcessSteps)
            {
                item.GetSignalNameList();
                if (item.Model is Customization)
                {
                    if (newStage)
                    {
                        newStage = false;
                        // check flags and change values to NAN.
                        foreach (var sig in filteredSignal)
                        {
                            // don't need all of them, only the ones was in previous filters
                            sig.ChangeFlaggedValueToNAN();
                        }
                        filteredSignal = new List<Signal>();
                    }
                    List<Signal> sigs = null;
                    try
                    {
                        if (item.Model is ScalarRepCust)
                        {
                            var m = item.Model as ScalarRepCust;
                            sigs = m.Process(e, t);
                        }
                        else
                        {
                            sigs = item.Model.Process(e);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    if (sigs != null)
                    {
                        foreach (var sig in sigs)
                        {
                            if (!filteredSignal.Contains(sig))
                            {
                                filteredSignal.Add(sig);
                            }
                        }
                    }
                }
                if (item.Model is Filter)
                {
                    if (!newStage)
                    {
                        newStage = true;
                    }
                    List<Signal> sigs = null;
                    try
                    {
                        sigs = item.Model.Process(e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    if (sigs != null)
                    {
                        foreach (var sig in sigs)
                        {
                            if (!filteredSignal.Contains(sig))
                            {
                                filteredSignal.Add(sig);
                            }
                        }
                    }
                }
            }
            // need change the last batch of filter to Nan if the last one is not customization
            if (newStage)
            {
                foreach (var sig in filteredSignal)
                {
                    // don't need all of them, only the ones was in previous filters
                    sig.ChangeFlaggedValueToNAN();
                }
            }
            try
            {
                DataMngr.AddData(e);  //?? shouldn't it be the filteredSignal??
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // concat data, different signature concat data differently

            //foreach (var item in SettingsVM.SignatureSettings)
            //{
            //    item.Model.Process(e);
            //}
        }
        private async Task _signatureCalculation()
        {
            foreach (var item in SettingsVM.SignatureSettings)
            {
                try
                {
                    item.GetSignalNameList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                item.GetSamplingRate();
                Task.Factory.StartNew(() => item.Model.Process(DataMngr));
            }
        }
        private async Task _writeSignatureResults()
        {
            int columnCount = 0;
            foreach (var item in SettingsVM.SignatureSettings)
            {
                switch (item.SignatureName)
                {
                    case "Correlation Coefficient":
                        columnCount += 1;
                        break;
                    case "Covariance":
                        var c = item.InputChannels.Count;
                        columnCount += c * (c - 1) / 2;
                        break;
                    case "Quartiles":
                        foreach (var signal in item.InputChannels)
                        {
                            columnCount += 3;
                        }
                        break;
                    case "Histogram":
                        var hist = (Histogram)item.Model;
                        columnCount += item.InputChannels.Count * hist.NumberOfBins;
                        break;
                    default:
                        foreach (var signal in item.InputChannels)
                        {
                            columnCount++;
                        }
                        break;
                }
            }
            DataMngr.NumberOfColumns = columnCount;
            await Task.Factory.StartNew(() => DataMngr.WriteResults());
        }
        private void _onResultsWrittenDone(object sender, EventArgs e)
        {
            //ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning = false;
            _currentRunningTask.IsTaskRunning = false;
            NoTaskingIsRunning = true;
        }
        public ICommand SaveConfigFile { get; set; }
        private void _saveConfigFile(object obj)
        {
            if (ProjectControlVM.CanRun)
            {
                SettingsVM.SaveConfigFile(ProjectControlVM.SelectedProject.SelectedRun.ConfigFilePath);
            }
            else
            {
                var config = JsonConvert.SerializeObject(SettingsVM, Formatting.Indented);
                ProjectControlVM.AddTask(ProjectControlVM.SelectedProject);
                using (StreamWriter outputFile = new StreamWriter(ProjectControlVM.SelectedProject.SelectedRun.ConfigFilePath))
                {
                    outputFile.WriteLine(config);
                }
                if (File.Exists(ProjectControlVM.SelectedProject.SelectedRun.ConfigFilePath))
                {
                    try
                    {
                        SettingsVM.ReadConfigFile(ProjectControlVM.SelectedProject.SelectedRun.ConfigFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (CurrentView is SettingsViewModel)
                {
                    CurrentView = SettingsVM;
                }
            }
        }
        //public ICommand OpenConfigFile { get; set; }
        //private void _openConfigFile(object obj)
        //{
        //    SettingsVM.OpenConfigFile();
        //}
        private void _onRunSelected(object sender, ProjectViewModel e)
        {
            SettingsVM = new SettingsViewModel();
            string configFile = e.SelectedRun.ConfigFilePath;
            if (File.Exists(configFile))
            {
                try
                {
                    SettingsVM.ReadConfigFile(configFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //SettingsVM.SignatureOutputDir = e.SelectedRun.SignaturePath;
            if (CurrentView is SettingsViewModel)
            {
                CurrentView = SettingsVM;
            }
        }
        public ICommand PauseArchiveSprinter { get; set; }
        private void _pauseArchiveSprinter(object obj)
        {
            _reader.DataCompleted = true;
            while (!DataMngr.DataCompleted || ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning)
            {
                Thread.Sleep(500);
            }
            var pauseConfig = new Pause();
            pauseConfig.CurrentTimeStamp = DataMngr.CurrentTimeStamp;
            pauseConfig.LastReadFileTime = _reader.DateTimeStart;
            //pauseConfig.NextUnreadFileTime = DateTime.ParseExact(_reader.DateTimeStart, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            pauseConfig.LastWrittenFileName = DataMngr.LastWrittenFile;
            var config = JsonConvert.SerializeObject(pauseConfig, Formatting.Indented);
            var pf = ProjectControlVM.SelectedProject.SelectedRun.TaskPath + "Pause.json";
            // in order for the file existence converter to work to show the resume button as soon as pause is clicked, 
            // I need to generate the pause.json file first before update the PauseFilePath so the converter actually finds the json file.
            using (StreamWriter outputFile = new StreamWriter(pf))
            {
                outputFile.WriteLine(config);
            }
            ProjectControlVM.SelectedProject.SelectedRun.PauseFilePath = pf;
        }
        public ICommand ResumeArchiveSprinter { get; set; }
        private void _resumeArchiveSprinter(object obj)
        {
            if (File.Exists(ProjectControlVM.SelectedProject.SelectedRun.PauseFilePath))
            {
                Pause config;
                using (StreamReader reader = File.OpenText(ProjectControlVM.SelectedProject.SelectedRun.PauseFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    config = (Pause)serializer.Deserialize(reader, typeof(Pause));
                }
                File.Delete(ProjectControlVM.SelectedProject.SelectedRun.PauseFilePath);
                var sigcsv = new CSVFileReader();
                var data = sigcsv.ReadSignatureCSV(config.LastWrittenFileName);
                var lasttime = data.LastOrDefault();
                ProjectControlVM.SelectedProject.SelectedRun.PauseFilePath = null;
                ProjectControlVM.SelectedProject.SelectedRun.CheckTaskDirIntegrity();
                _numberOfFilesRead = 0;
                try
                {
                    _setupFileManager();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                _reader.DateTimeStart = config.LastReadFileTime;
                var numberOfDataWriters = SettingsVM.DataWriters.Count();
                var numberOfSignatures = SettingsVM.SignatureSettings.Count();
                _setupDataManager(numberOfDataWriters, numberOfSignatures);
                DataMngr.FirstFile = false;
                DataMngr.ResumedTask = true;
                DataMngr.CurrentTimeStamp = config.CurrentTimeStamp;
                DataMngr.TimeZero = lasttime.AddSeconds(Convert.ToInt32(SettingsVM.WindowSizeStr) - Convert.ToInt32(SettingsVM.WindowOverlapStr));
                DataMngr.SignatureCanStart = true;
                ProjectControlVM.SelectedProject.SelectedRun.IsTaskRunning = true;
                _currentRunningTask = ProjectControlVM.SelectedProject.SelectedRun;
                ProjectControlVM.CanRun = false;
                NoTaskingIsRunning = false;
                _startAS(numberOfDataWriters, numberOfSignatures);
            }
        }
        public ICommand StopArchiveSprinter { get; set; }
        private void _stopArchiveSprinter(object obj)
        {
            _reader.DataCompleted = true;
        }
        private string _currentFileTime;
        public string CurrentFileTime 
        {
            get { return _currentFileTime; }
            set
            {
                _currentFileTime = value;
                OnPropertyChanged();
            }
        }
        private DateTime _getFileDateTime(string filename)
        {
            string[] namestrings = Path.GetFileNameWithoutExtension(filename).Split('_');
            int digit;
            var dateS = "";
            var timeS = "";
            foreach (var strs in namestrings)
            {
                try
                {
                    digit = int.Parse(strs);
                    if (strs.Length == 8)
                        dateS = strs;
                    else if (strs.Length == 6)
                        timeS = strs;
                }
                catch (Exception ex)
                {
                }
            }
            string s = dateS.Substring(0, 4) + "/" + dateS.Substring(4, 2) + "/" + dateS.Substring(6, 2) + " "
                            + timeS.Substring(0, 2) + ":" + timeS.Substring(2, 2) + ":" + timeS.Substring(4, 2);
            DateTime b = DateTime.Parse(s);
            return b;
        }
        private bool _noTaskingIsRunning;
        public bool NoTaskingIsRunning 
        {
            get { return _noTaskingIsRunning; }
            set
            {
                _noTaskingIsRunning = value;
                OnPropertyChanged();
            }
        }
        private ASTaskViewModel _currentRunningTask;
    }
}
