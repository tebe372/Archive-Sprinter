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
    public class PMUWithSamplingRate
    {
        public PMUWithSamplingRate(string pmu, int rate, int count)
        {
            _pmu = pmu;
            _samplingRate = rate;
            _signalLength = count;
        }
        public PMUWithSamplingRate()
        {
        }
        private string _pmu;
        public string PMU
        {
            get
            {
                return _pmu;
            }
            set
            {
                _pmu = value;
            }
        }
        private int _samplingRate;
        public int SamplingRate
        {
            get
            {
                return _samplingRate;
            }
            set
            {
                _samplingRate = value;
            }
        }
        private int _signalLength;
        public int SignalLength 
        {
            get { return _signalLength; }
            set { _signalLength = value; } 
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            PMUWithSamplingRate p = (PMUWithSamplingRate)obj;
            return this.PMU == p.PMU && this.SamplingRate == p.SamplingRate && this.SignalLength == p.SignalLength; // AndAlso Me.SamplingRate = p.SamplingRate
        }
        public override int GetHashCode()
        {
            // If item Is Nothing Then Return 0
            // Dim hashItemName = If(item.PMU Is Nothing, 0, item.PMU.GetHashCode())
            // Dim hashRate = item.SamplingRate.GetHashCode()
            return PMU.GetHashCode(); // Xor SamplingRate.GetHashCode()
        }
        public static bool operator ==(PMUWithSamplingRate x, PMUWithSamplingRate y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            else if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
            {
                return false;
            }
            else
            {
                return x.PMU == y.PMU && x.SamplingRate == y.SamplingRate && x.SignalLength == y.SignalLength; // AndAlso x.SamplingRate = y.SamplingRate
            }
        }
        public static bool operator !=(PMUWithSamplingRate x, PMUWithSamplingRate y)
        {
            return !(x == y);
        }
    }
    //public class PMUWithSamplingRateComparer : IEqualityComparer<PMUWithSamplingRate>
    //{
    //    public bool Equals(PMUWithSamplingRate x, PMUWithSamplingRate y)
    //    {
    //        if (x == y)
    //            return true;
    //        if (x == null || y == null)
    //            return false;
    //        return (x.PMU == y.PMU); // AndAlso (x.SamplingRate = y.SamplingRate)
    //    }
    //    public int GetHashCode(PMUWithSamplingRate item)
    //    {
    //        if (item == null)
    //            return 0;
    //        var hashItemName = item.PMU == null ? 0 : item.PMU.GetHashCode();
    //        // Dim hashRate = item.SamplingRate.GetHashCode()
    //        return hashItemName; // Xor hashRate
    //    }
    //}

}
