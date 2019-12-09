using AS.Config;
using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private Configuration _model;
        public int CurrentTabIndex { get; set; } = 0;
        public DataSourceSettingViewModel DataSourceVM { get; set; } = new DataSourceSettingViewModel();
        private List<DataSourceSettingViewModel> _dataSourceVMList = new List<DataSourceSettingViewModel>();
        public List<DataSourceSettingViewModel> DataSourceVMList
        {  // List of input file information
            get{return _dataSourceVMList; }
            set{ _dataSourceVMList = value; }
        }
    }
}
