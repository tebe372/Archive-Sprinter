using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
using ArchiveSprinterGUI.ViewModels.SignalInspectionViewModels;
using AS.Config;
using AS.Core.Models;
using AS.DataManager;
using AS.IO;
using AS.SampleDataManager;
using AS.Utilities;
using System;
using System.Collections.Generic;
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
            _currentView = _settingsVM;
            MainViewSelected = new RelayCommand(_switchView);
            StartArchiveSprinter = new RelayCommand(_startArchiveSprinter);
            DataMngr = new DataStore();
            SaveConfigFile = new RelayCommand(_saveConfigFile);
            OpenConfigFile = new RelayCommand(_openConfigFile);
            _numberOfFilesRead = 0;
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
        private async void _startArchiveSprinter(object obj)
        {
            _numberOfFilesRead = 0;
            //scan through all the steps of configuration and figure out needed signals.
            var neededSignalList = _getAllNeededSignals();

            //send list of signals to reader control

            //read files first by sending this source parameter
            var source = SettingsVM.DataSourceVM.Model;
            var data = new FileReadingManager();
            data.SourceDirectory = source.FileDirectory;
            data.FileType = source.FileType;
            data.SamplingRate = source.SamplingRate;
            data.NumberOfDataPointInFile = source.NumberOfDataPointInFile;
            data.Mnemonic = source.Mnemonic;
            data.NeededSignalList = neededSignalList;
            data.FileReadingDone += FileReadingDone;
            data.DataReadingDone += DataReadingDone;
            data.DateTimeStart = SettingsVM.DateTimeStart;
            data.DateTimeEnd = SettingsVM.DateTimeEnd;

            DataMngr.Clean();
            var numberOfDataWriters = SettingsVM.DataWriters.Count();
            DataMngr.NumberOfDataWriters = numberOfDataWriters;
            // this need to be put on a thread
            try
            {
                Task.Run(async () => { await _startAS(data); }) ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //data.Start();
            // call another function that start the signature calculation, might be the computation/data manager, on a thread too.
            while (_numberOfFilesRead <= 1)
            {
                Thread.Sleep(500);
            }
            if (numberOfDataWriters > 0)
            {
                try
                {
                    Task.Run(async () => { await _startDataWriters(); });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            try
            {
                Task.Run(async () => { await _signatureCalculation(); });                
            }
            catch (Exception ex)
            {
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
        private async Task _startDataWriters()
        {
            DataMngr.DatawriteOutFrequency = _settingsVM.Model.DatawriteOutFrequency;
            DataMngr.DatawriteOutFrequencyUnit = _settingsVM.Model.DatawriteOutFrequencyUnit;
            DataMngr.WindowSize = _settingsVM.Model.WindowSize;
            DataMngr.WindowOverlap = _settingsVM.Model.WindowOverlap;
            DataMngr.NumberOfSignatures = SettingsVM.SignatureSettings.Count();
            DataMngr.SignatureOutputDir = SettingsVM.SignatureOutputDir;
            DataMngr.NumberOfDataPointInFile = SettingsVM.DataSourceVM.Model.NumberOfDataPointInFile;
            DataMngr.SamplingRate = SettingsVM.DataSourceVM.Model.SamplingRate;
            foreach (var item in SettingsVM.DataWriters)
            {
                item.GetSignalNameList();
                Task.Factory.StartNew(() => item.Model.Process(DataMngr));
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
        }
        public DataStore DataMngr { get; set; }
        private int _numberOfFilesRead;
        private void FileReadingDone(object sender, List<Signal> e)
        {
            // this function call have been put on thread, and put the data that have gone through pre process steps into a container in time order
            _preprocessData(e);
            _numberOfFilesRead++;
        }
        private async Task _startAS(FileReadingManager mgr)
        {
            try
            {
                mgr.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void _preprocessData(List<Signal> e)
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
                    try
                    {
                        item.Model.Process(e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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
                DataMngr.AddData(e);
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
                item.GetSignalNameList();
                item.GetSamplingRAte();
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
                    case "Covariance":
                        var c = item.InputChannels.Count;
                        columnCount += c * (c + 1) / 2;
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
        public ICommand SaveConfigFile { get; set; }
        private void _saveConfigFile(object obj)
        {
            SettingsVM.SaveConfigFile();
        }
        public ICommand OpenConfigFile { get; set; }
        private void _openConfigFile(object obj)
        {
            SettingsVM.OpenConfigFile();
        }
    }
}
