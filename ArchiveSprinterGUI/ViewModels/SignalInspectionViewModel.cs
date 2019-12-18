using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveSprinterGUI.ViewModels
{
    public class SignalInspectionViewModel : ViewModelBase
    {
        public SignalInspectionViewModel()
        {
            SampleDataMngr = new SampleDataManagerViewModel();
        }
        public SampleDataManagerViewModel SampleDataMngr { get; set; }
    }
}
