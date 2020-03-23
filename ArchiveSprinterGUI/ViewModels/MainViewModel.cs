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
            //scan through all the steps of configuration and figure out needed signals.

            //send list of signals to reader control

            //read files first by sending this source parameter
            var source = SettingsVM.DataSourceVM.Model;
            var data = new FileReadingManager(source);
            data.FileReadingDone += FileReadingDone;
            data.DataReadingDone += DataReadingDone;
            DataMngr.Clean();
            // this need to be put on a thread
            try
            {
                await Task.Run(async () => { await data.Start(); }) ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //data.Start();
            // call another function that start the signature calculation, might be the computation/data manager, on a thread too.
            Thread.Sleep(500);
            try
            {
                await Task.Run(async () => { await _signatureCalculation(); });                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Thread.Sleep(500);
            try
            {
                await Task.Run(async () => { await _writeSignatureResults(); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DataReadingDone(object sender)
        {
            DataMngr.DataCompleted = true;
        }

        public DataStore DataMngr { get; set; }

        private void FileReadingDone(object sender, List<Signal> e)
        {
            // this function call have been put on thread, and put the data that have gone through pre process steps into a container in time order
            _preprocessData(e);
        }

        private void _preprocessData(List<Signal> e)
        {
            bool newStage = true;
            foreach (var item in SettingsVM.PreProcessSteps)
            {
                item.GetSignalNameList();
                if (item.Model is Customization && newStage)
                {
                    newStage = false;
                    // check flags and change values to NAN.
                    foreach (var sig in e)
                    {
                        sig.ChangeFlaggedValueToNAN();
                    }
                }
                else if (!newStage && item.Model is Filter)
                {
                    newStage = true;
                }
                item.Model.Process(e);
            }
            DataMngr.AddData(e);
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
                await Task.Factory.StartNew(() => item.Model.Process(DataMngr));
            }
        }
        private async Task _writeSignatureResults()
        {
            DataMngr.DatawriteOutFrequency = _settingsVM.Model.DatawriteOutFrequency;
            DataMngr.DatawriteOutFrequencyUnit = _settingsVM.Model.DatawriteOutFrequencyUnit;
            DataMngr.WindowSize = _settingsVM.Model.WindowSize;
            DataMngr.WindowOverlap = _settingsVM.Model.WindowOverlap;
            DataMngr.NumberOfSignatures = SettingsVM.SignatureSettings.Count();
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

    }
}
