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
        }
        #region Raw signals
        private ObservableCollection<SignalTypeHierachy> _groupedRawSignalsByType;
        public ObservableCollection<SignalTypeHierachy> GroupedRawSignalsByType
        {
            get
            {
                return _groupedRawSignalsByType;
            }
            set
            {
                _groupedRawSignalsByType = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SignalTypeHierachy> _groupedRawSignalsByPMU;
        public ObservableCollection<SignalTypeHierachy> GroupedRawSignalsByPMU
        {
            get
            {
                return _groupedRawSignalsByPMU;
            }
            set
            {
                _groupedRawSignalsByPMU = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
