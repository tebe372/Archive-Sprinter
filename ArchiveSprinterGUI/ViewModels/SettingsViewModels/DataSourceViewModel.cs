using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveSprinterGUI.ViewModels.SettingsViewModels
{
    public class DataSourceViewModel
    {
        public DataFileType FileType { get; set; } = DataFileType.csv;
    }
}
