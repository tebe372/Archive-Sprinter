using AS.Core.ViewModels;
using AS.SampleDataManager;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveSprinterGUI.ViewModels
{
    public class SampleDataManagerViewModel : ViewModelBase
    {
        private SampleDataMngr _model;

        public SampleDataManagerViewModel()
        {
            _model = SampleDataMngr.Instance;
            _model.SignalCheckStatusChanged += __signalCheckStatusChanged;
            DataviewGroupMethods = new List<string>() { "View Signal by Type", "View Signal by PMU" };
            SelectedDataViewingGroupMethod = "View Signal by Type";
        }

        private void __signalCheckStatusChanged(SignalTree e)
        {
            OnSignalTreeCheckStatusChanged(e);
        }
        public event SignalTreeCheckStatusChangedEventHandler SignalCheckStatusChanged;
        public delegate void SignalTreeCheckStatusChangedEventHandler(SignalTree e);
        private void OnSignalTreeCheckStatusChanged(SignalTree e)
        {
            SignalCheckStatusChanged?.Invoke(e);
        }
        public List<string> DataviewGroupMethods { get; set; }
        private string _selectedDataViewingGroupMethod;
        public string SelectedDataViewingGroupMethod 
        {
            get { return _selectedDataViewingGroupMethod; }
            set
            {
                _selectedDataViewingGroupMethod = value;
                OnPropertyChanged();
            }
        }
        #region Raw signals
        private ObservableCollection<SignalTree> _groupedRawSignalsByType;
        public ObservableCollection<SignalTree> GroupedRawSignalsByType
        {
            get
            {
                return _model.GroupedSignalsByType;
            }
            set
            {
                _model.GroupedSignalsByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTree> _groupedRawSignalsByPMU;
        public ObservableCollection<SignalTree> GroupedRawSignalsByPMU
        {
            get
            {
                return _model.GroupedSignalsByPMU;
            }
            set
            {
                _model.GroupedSignalsByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
