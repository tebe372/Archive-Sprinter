using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Utilities;
using AS.Config;

namespace ArchiveSprinterGUI.ViewModels
{
    public class DataFilterViewModel : ViewModelBase
    {
        public DataFilterViewModel()
        {
            _model = new Filter();
        }
        private PreProcessSetting _model;
    }

}
