using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.SampleDataManager
{
    public class SampleDataMngr
    {

        private static SampleDataMngr _instance = null;
        public static SampleDataMngr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SampleDataMngr();
                }
                return _instance;
            }
        }
        private SampleDataMngr()
        {
        }

        public void AddSampleSignals(List<Signal> signals)
        {
            throw new NotImplementedException();
        }
    }
}
