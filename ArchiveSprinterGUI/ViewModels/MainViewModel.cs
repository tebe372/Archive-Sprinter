using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
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

    }
}
