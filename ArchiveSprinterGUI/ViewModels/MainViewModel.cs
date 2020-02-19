using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
using ArchiveSprinterGUI.ViewModels.SignalInspectionViewModels;
using AS.Core.Models;
using AS.IO;
using AS.SampleDataManager;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //scan through all the steps of configuration and figure out needed signals.

            //send list of signals to reader control

            //read files first by sending this source parameter
            var source = SettingsVM.DataSourceVM.Model;
            var data = new FileReadingManager(source);
            data.DataReadingDone += Data_DataReadingDone;
            data.Start();
        }

        private void Data_DataReadingDone(object sender, List<Signal> e)
        {
            _processData(e);
        }

        private void _processData(List<Signal> e)
        {
            foreach (var item in SettingsVM.PreProcessSteps)
            {
                item.Model.Process(e);
            }
            foreach (var item in SettingsVM.SignatureSettings)
            {
                item.Model.Process(e);
            }
        }
    }
}
