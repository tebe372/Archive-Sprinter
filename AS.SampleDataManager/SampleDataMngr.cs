using AS.Core.Models;
using AS.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            _signals = new List<Signal>();
        }

        public event EventHandler SignalsUpdated;

        private List<Signal> _signals;
        public List<Signal> Signals
        {
            get { return _signals; }
            set
            {
                _signals = value;
                SignalsUpdated?.Invoke(this, null);
            }
        }

        public void AddSampleSignals(List<Signal> signals)
        {
            _signals = signals;

            SignalsUpdated?.Invoke(this, null);
        }
    }
}
