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
            DataviewGroupMethods = new List<string>() { "View Signal by Type", "View Signal by PMU" };
            SelectedDataViewingGroupMethod = "View Signal by Type";
        }
        public List<String> DataviewGroupMethods { get; set; }
        public string SelectedDataViewingGroupMethod 
        { 
            get; set; 
        }
        #region Raw signals
        private ObservableCollection<SignalTree> _groupedRawSignalsByType;
        public ObservableCollection<SignalTree> GroupedRawSignalsByType
        {
            get
            {
                return _model.GroupedSignalsByType;
            }
            //set
            //{
            //    _groupedRawSignalsByType = value;
            //    OnPropertyChanged();
            //}
        }
        private ObservableCollection<SignalTree> _groupedRawSignalsByPMU;
        public ObservableCollection<SignalTree> GroupedRawSignalsByPMU
        {
            get
            {
                return _model.GroupedSignalsByPMU;
            }
            //set
            //{
            //    _groupedRawSignalsByPMU = value;
            //    OnPropertyChanged();
            //}
        }
        #endregion
    }
}
