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
        public int CurrentTabIndex { get; set; } = 0;
        public DataSourceViewModel DataSourceVM { get; set; } = new DataSourceViewModel();
    }
}
