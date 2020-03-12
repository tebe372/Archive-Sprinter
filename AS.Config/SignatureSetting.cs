using AS.ComputationManager.Calculations;
using AS.Core.Models;
using AS.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AS.Config
{
    public abstract class SignatureSetting
    {
        public abstract void Process(DataStore dataMngr);
        public abstract string SignatureName { get; }
        public int WindowSize { get; set; }
        public int WindowOverlap { get; set; }
        public bool CheckNanResult { get; set; }
        public bool OmitNan { get; set; }
        public List<string> InputSignals { get; set; }
    }
    public class Mean : SignatureSetting
    {
        public override string SignatureName { get { return "Mean"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Variance : SignatureSetting
    {
        public override string SignatureName { get { return "Variance"; } }
        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            List<Signal> signals = new List<Signal>();
            while (true)
            {
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (!dataMngr.GetData(signals, startT, endT, InputSignals))
                {
                    if (dataMngr.DataCompleted)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    var variance = SignatureCalculations.Variance(item.Data);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }            
        }
    }
    public class StandardDeviation : SignatureSetting
    {
        public override string SignatureName { get { return "Standard Deviation"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Kurtosis : SignatureSetting
    {
        public override string SignatureName { get { return "Kurtosis"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Skewness : SignatureSetting
    {
        public override string SignatureName { get { return "Skewness"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class CorrelationCoefficient : SignatureSetting
    {
        public override string SignatureName { get { return "Correlation Coefficient"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Covariance : SignatureSetting
    {
        public override string SignatureName { get { return "Covariance"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Periodogram : SignatureSetting
    {
        public override string SignatureName { get { return "Periodogram"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class GMSCSpectrum : SignatureSetting
    {
        public override string SignatureName { get { return "Generalized Magnitude Squared Coherence (GMSC) Spectrum"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Percentile : SignatureSetting
    {
        public override string SignatureName { get { return "Percentile"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Quartiles : SignatureSetting
    {
        public override string SignatureName { get { return "Quartiles"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Median : SignatureSetting
    {
        public override string SignatureName { get { return "Median"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Maximum : SignatureSetting
    {
        public override string SignatureName { get { return "Maximum"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Minimum : SignatureSetting
    {
        public override string SignatureName { get { return "Minimum"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Range : SignatureSetting
    {
        public override string SignatureName { get { return "Range"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Rise : SignatureSetting
    {
        public override string SignatureName { get { return "Rise"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
    public class Histogram : SignatureSetting
    {
        public override string SignatureName { get { return "Histogram"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }
    }
}
