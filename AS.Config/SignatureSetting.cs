using AS.ComputationManager.Calculations;
using AS.Core.Models;
using AS.DataManager;
using Newtonsoft.Json;
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
        public abstract void ProcessNANData(Signal sig); //either interpolate or omit
        public abstract string SignatureName { get; }
        public int WindowSize { get; set; }
        public int WindowOverlap { get; set; }
        private string _windowSizeStr;
        [JsonIgnore]
        public string WindowSizeStr 
        {
            get { return _windowSizeStr; }
            set
            {
                _windowSizeStr = value;
                if (!string.IsNullOrEmpty(value))
                {
                    WindowSize = Convert.ToInt32(value);
                    WindowSizeNumberOfSamples = WindowSize * SamplingRate;
                }
            } 
        }
        private string _windowOverlapStr;
        [JsonIgnore]
        public string WindowOverlapStr 
        {
            get { return _windowOverlapStr; }
            set
            {
                _windowOverlapStr = value;
                if (!string.IsNullOrEmpty(value))
                {
                    WindowOverlap = Convert.ToInt32(value);
                    WindowOverlapNumberOfSamples = WindowOverlap * SamplingRate;
                }
            }
        }
        [JsonIgnore]
        public bool CheckNanResult { get; set; }
        public bool OmitNan { get; set; }
        [JsonIgnore]
        public List<string> InputSignals { get; set; }
        private int _samplingRate;
        public int SamplingRate 
        {
            get { return _samplingRate; }
            set
            {
                if (_samplingRate != value)
                {
                    _samplingRate = value;
                    WindowOverlapNumberOfSamples = WindowOverlap * value;
                    WindowSizeNumberOfSamples = WindowSize * value;
                }
            }
        }
        [JsonIgnore]
        public int WindowSizeNumberOfSamples { get; set; }
        [JsonIgnore]
        public int WindowOverlapNumberOfSamples { get; set; }
        public void RemoveNanValue(Signal sig)
        {
            if (sig.Data.Count > 0)
            {
                for (int i = sig.Data.Count - 1; i >= 0; i--)
                {
                    if (double.IsNaN(sig.Data[i]))
                    {
                        sig.Data.RemoveAt(i);
                    }
                }
            }
            else
            {
                for (int i = sig.ComplexData.Count - 1; i >= 0; i--)
                {
                    if (double.IsNaN(sig.ComplexData[i].Real) || double.IsNaN(sig.ComplexData[i].Imaginary))
                    {
                        sig.ComplexData.RemoveAt(i);
                    }
                }
            }
        }
        public void InterpolateNanValue(Signal sig)
        {

        }
    }
    public class Mean : SignatureSetting
    {
        public override string SignatureName { get { return "Mean"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back

            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                //if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                //{
                //    if (dataMngr.HasDataAfter(startT, endT))
                //    {
                //        startT = endT.AddSeconds(-WindowOverlap);
                //        endT = startT.AddSeconds(WindowSize);
                //        continue;
                //    }
                //    else
                //    {
                //        if (dataMngr.DataCompleted)
                //        {
                //            break;
                //        }
                //        else
                //        {
                //            Thread.Sleep(500);
                //            continue;
                //        }
                //    }
                //}
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                //if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                //{
                //    if (dataMngr.DataCompleted)
                //    {
                //        if (endT <= dataMngr.FinalTimeStamp)
                //        {
                //            startT = endT.AddSeconds(-WindowOverlap);
                //            endT = startT.AddSeconds(WindowSize);
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        if (dataMngr.HasDataAfter(startT, endT))
                //        {
                //            startT = endT.AddSeconds(-WindowOverlap);
                //            endT = startT.AddSeconds(WindowSize);
                //        }
                //        else
                //        {
                //            Thread.Sleep(500);
                //        }
                //        continue;
                //    }
                //}
                foreach (var item in signals)
                {
                    double mean = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            mean = SignatureCalculations.Mean(item.Data);
                        }
                    }
                    else
                    {
                        mean = SignatureCalculations.Mean(item.Data);
                    }
                    dataMngr.AddResults(item.TimeStamps.FirstOrDefault(), "Mean", item.PMUName, item.SignalName, mean, item.TimeStamps.LastOrDefault());
#if DEBUG
                    Console.WriteLine("Mean:");
                    Console.WriteLine(mean);
#endif
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Mean");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
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

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double variance = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            variance = SignatureCalculations.Variance(item.Data);
                        }
                    }
                    else
                    {
                        variance = SignatureCalculations.Variance(item.Data);
                    }
                    dataMngr.AddResults(startT, "Variance", item.PMUName, item.SignalName, variance, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Variance:");
                    //Console.WriteLine(variance);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Variance");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class StandardDeviation : SignatureSetting
    {
        public override string SignatureName { get { return "Standard Deviation"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double std = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            std = SignatureCalculations.Stdev(item.Data);
                        }
                    }
                    else
                    {
                        std = SignatureCalculations.Stdev(item.Data);
                    }
                    dataMngr.AddResults(startT, "Standard Deviation", item.PMUName, item.SignalName, std, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Standard Deviation:");
                    //Console.WriteLine(std);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Standard Deviation");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Kurtosis : SignatureSetting
    {
        public override string SignatureName { get { return "Kurtosis"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double kurt = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            kurt = SignatureCalculations.Kurtosis(item.Data);
                        }
                    }
                    else
                    {
                        kurt = SignatureCalculations.Kurtosis(item.Data);
                    }
                    dataMngr.AddResults(startT, "Kurtosis", item.PMUName, item.SignalName, kurt, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Kurtosis:");
                    //Console.WriteLine(kurt);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Kurtosis");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Skewness : SignatureSetting
    {
        public override string SignatureName { get { return "Skewness"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double skew = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            skew = SignatureCalculations.Skewness(item.Data);
                        }
                    }
                    else
                    {
                        skew = SignatureCalculations.Skewness(item.Data);
                    }
                    dataMngr.AddResults(startT, "Skewness", item.PMUName, item.SignalName, skew, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Skewness:");
                    //Console.WriteLine(skew);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Skewness");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class CorrelationCoefficient : SignatureSetting
    {
        public override string SignatureName { get { return "Correlation Coefficient"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            //omit nan in pairs
        }
    }
    public class Covariance : SignatureSetting
    {
        public override string SignatureName { get { return "Covariance"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            //omit nan in pairs
        }
    }
    public class Periodogram : SignatureSetting
    {
        public override string SignatureName { get { return "Periodogram"; } }
        public int MaxPatch { get; set; } // maximum number of continuous NAN that an be interpolated 

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            // interpolate
            Functions.Interpolate(sig.Flags, sig.Data, MaxPatch);
        }
    }
    public class GMSCSpectrum : SignatureSetting
    {
        public override string SignatureName { get { return "Generalized Magnitude Squared Coherence (GMSC) Spectrum"; } }
        public int MaxPatch { get; set; } // maximum number of continuous NAN that an be interpolated 

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            //interpolate
        }
    }
    public class Percentile : SignatureSetting
    {
        public override string SignatureName { get { return "Percentile"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Quartiles : SignatureSetting
    {
        public override string SignatureName { get { return "Quartiles"; } }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Median : SignatureSetting
    {
        public override string SignatureName { get { return "Median"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                //if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                //{
                //    if (dataMngr.HasDataAfter(startT, endT))
                //    {
                //        startT = endT.AddSeconds(-WindowOverlap);
                //        endT = startT.AddSeconds(WindowSize);
                //        continue;
                //    }
                //    else
                //    {
                //        if (dataMngr.DataCompleted)
                //        {
                //            break;
                //        }
                //        else
                //        {
                //            Thread.Sleep(500);
                //            continue;
                //        }
                //    }
                //}
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                //if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                //{
                //    if (dataMngr.DataCompleted)
                //    {
                //        if (endT <= dataMngr.FinalTimeStamp)
                //        {
                //            startT = endT.AddSeconds(-WindowOverlap);
                //            endT = startT.AddSeconds(WindowSize);
                //        }
                //        else
                //        {
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        Thread.Sleep(500);
                //        continue;
                //    }
                //}
                foreach (var item in signals)
                {
                    double med = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            med = SignatureCalculations.Median(item.Data);
                        }
                    }
                    else
                    {
                        med = SignatureCalculations.Median(item.Data);
                    }
                    dataMngr.AddResults(item.TimeStamps.FirstOrDefault(), "Median", item.PMUName, item.SignalName, med, item.TimeStamps.LastOrDefault());
#if DEBUG
                    Console.WriteLine("Median:");
                        Console.WriteLine(med);
#endif
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Median");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Maximum : SignatureSetting
    {
        public override string SignatureName { get { return "Maximum"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
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
                    double max = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            max = SignatureCalculations.Maximum(item.Data);
                        }
                    }
                    else
                    {
                        max = SignatureCalculations.Maximum(item.Data);
                    }
                    dataMngr.AddResults(startT, "Maximum", item.PMUName, item.SignalName, max, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Maximum:");
                    //Console.WriteLine(max);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Maximum");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Minimum : SignatureSetting
    {
        public override string SignatureName { get { return "Minimum"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double min = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            min = SignatureCalculations.Minimum(item.Data);
                        }
                    }
                    else
                    {
                        min = SignatureCalculations.Minimum(item.Data);
                    }
                    dataMngr.AddResults(startT, "Minimum", item.PMUName, item.SignalName, min, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Minimum:");
                    //Console.WriteLine(min);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Minimum");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Range : SignatureSetting
    {
        public override string SignatureName { get { return "Range"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double range = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            range = SignatureCalculations.Range(item.Data);
                        }
                    }
                    else
                    {
                        range = SignatureCalculations.Range(item.Data);
                    }
                    dataMngr.AddResults(startT, "Range", item.PMUName, item.SignalName, range, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Range:");
                    //Console.WriteLine(range);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Range");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Rise : SignatureSetting
    {
        public override string SignatureName { get { return "Rise"; } }

        public override void Process(DataStore dataMngr)
        {
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
                //according to the input channels in variance, take part of the e as input to the following function call.
                if (dataMngr.HasDataAfter(startT, endT))
                {
                    if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                    {
                        startT = endT.AddSeconds(-WindowOverlap);
                        endT = startT.AddSeconds(WindowSize);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted)
                    {
                        if (!dataMngr.GetData(signals, startT, endT, WindowSizeNumberOfSamples, InputSignals))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                }
                foreach (var item in signals)
                {
                    double rise = Double.NaN;
                    if (item.Flags.Contains(false))
                    {
                        if (OmitNan)
                        {
                            ProcessNANData(item);
                            rise = SignatureCalculations.Rise(item.Data);
                        }
                    }
                    else
                    {
                        rise = SignatureCalculations.Rise(item.Data);
                    }
                    dataMngr.AddResults(startT, "Rise", item.PMUName, item.SignalName, rise, item.TimeStamps.LastOrDefault());
                    //Console.WriteLine("Rise:");
                    //Console.WriteLine(rise);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Rise");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class Histogram : SignatureSetting
    {
        public override string SignatureName { get { return "Histogram"; } }
        public int NumberOfBins { get; set; }

        public override void Process(DataStore dataMngr)
        {
            throw new NotImplementedException();
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
}
