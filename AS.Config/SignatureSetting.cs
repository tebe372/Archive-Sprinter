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
        public bool InterpolateNanValue(Signal sig, double threshold)
        {
            if (sig.Data.Count > 0)
            {
                bool nanAtBeginning = true;
                int nNan = 0;
                bool nanFrag = false;
                int beginIndex = 0;                             // 1 before the beginning of a NAN sequence.
                int endIndex = 0;                               // 1 after the end of a NAN sequence.
                for (int i = sig.Data.Count - 1; i >= 0; i--)   // going backwards as we need to remove points at the beginning and end if there's NAN.
                {
                    if (double.IsNaN(sig.Data[i]) && nanAtBeginning)
                    {
                        sig.Data.RemoveAt(i);
                    }
                    else
                    {
                        if (nanAtBeginning)
                        {
                            nanAtBeginning = false;
                        }
                        if (double.IsNaN(sig.Data[i]))
                        {
                            if (!nanFrag)
                            {
                                nanFrag = true;
                                beginIndex = i + 1;
                            }
                            nNan++;
                        }
                        else
                        {
                            if (nanFrag)
                            {
                                nanFrag = false;
                                endIndex = i;
                                if (nNan >= threshold)
                                {
                                    return false;
                                }
                                else
                                {
                                    nNan = 0;
                                    var point1 = sig.Data[endIndex];
                                    var point2 = sig.Data[beginIndex];
                                    var slope = (point2 - point1) / (beginIndex - endIndex);
                                    for (int ii = 1 ; ii < beginIndex - endIndex ; ii++)
                                    {
                                        sig.Data[ii + endIndex] = point1 + slope * ii;
                                    }
                                }
                            }
                        }
                    }
                }
                if (nanFrag)
                {
                    for (int i = beginIndex - 1; i >= 0; i--)
                    {
                        sig.Data.RemoveAt(i);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ProcessNANDataInPair(Signal sig1, Signal sig2)
        {
            if (sig1.Data.Count > 0 && sig2.Data.Count > 0)
            {
                for (int i = sig1.Data.Count - 1; i >= 0; i--)
                {
                    if (double.IsNaN(sig1.Data[i]) || double.IsNaN(sig2.Data[i]))
                    {
                        sig1.Data.RemoveAt(i);
                        sig2.Data.RemoveAt(i);
                    }
                }
            }
            else if (sig1.ComplexData.Count > 0 && sig2.ComplexData.Count > 0)
            {
                for (int i = sig1.ComplexData.Count - 1; i >= 0; i--)
                {
                    if (double.IsNaN(sig1.ComplexData[i].Real) || double.IsNaN(sig1.ComplexData[i].Imaginary) || double.IsNaN(sig2.ComplexData[i].Real) || double.IsNaN(sig2.ComplexData[i].Imaginary))
                    {
                        sig1.ComplexData.RemoveAt(i);
                        sig2.ComplexData.RemoveAt(i);
                    }
                }
            }
            else
            {

            }
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        mean = SignatureCalculations.Mean(item.Data);
                    }
                    else
                    {
                        mean = SignatureCalculations.Mean(item.Data);
                    }
                    dataMngr.AddResults(startT, "Mean", item.PMUName, item.SignalName, mean, endT);
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        variance = SignatureCalculations.Variance(item.Data);
                    }
                    else
                    {
                        variance = SignatureCalculations.Variance(item.Data);
                    }
                    dataMngr.AddResults(startT, "Variance", item.PMUName, item.SignalName, variance, endT);
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

                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        std = SignatureCalculations.Stdev(item.Data);
                    }                    
                    else
                    {
                        std = SignatureCalculations.Stdev(item.Data);
                    }
                    dataMngr.AddResults(startT, "Standard Deviation", item.PMUName, item.SignalName, std, endT);
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        kurt = SignatureCalculations.Kurtosis(item.Data);
                    }                    
                    else
                    {
                        kurt = SignatureCalculations.Kurtosis(item.Data);
                    }
                    dataMngr.AddResults(startT, "Kurtosis", item.PMUName, item.SignalName, kurt, endT);
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        skew = SignatureCalculations.Skewness(item.Data);
                    }
                    else
                    {
                        skew = SignatureCalculations.Skewness(item.Data);
                    }
                    dataMngr.AddResults(startT, "Skewness", item.PMUName, item.SignalName, skew, endT);
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
                if (signals.Count != 2)
                {
                    startT = endT.AddSeconds(-WindowOverlap);
                    endT = startT.AddSeconds(WindowSize);
                    continue;
                }
                else
                {
                    double cc = Double.NaN;
                    if (OmitNan)
                    {
                        ProcessNANDataInPair(signals[0], signals[1]);
                        cc = SignatureCalculations.CorrelationCoeff(signals[0].Data, signals[1].Data);
                    }
                    else
                    {
                        cc = SignatureCalculations.CorrelationCoeff(signals[0].Data, signals[1].Data);
                    }
                    dataMngr.AddResults(startT, "CorrelationCo", signals[0].PMUName + "&" + signals[1].PMUName, signals[0].SignalName + "&" + signals[1].SignalName, cc, endT);
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Correlation Coefficient");
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
                for (int i = 0; i < signals.Count - 1; i++)
                {
                    for (int ii = i + 1; ii < signals.Count; ii++)
                    {
                        double co = Double.NaN;
                        if (OmitNan)
                        {
                            ProcessNANDataInPair(signals[i], signals[ii]);
                            co = SignatureCalculations.CorrelationCoeff(signals[i].Data, signals[ii].Data);
                        }
                        else
                        {
                            co = SignatureCalculations.CorrelationCoeff(signals[i].Data, signals[ii].Data);
                        }
                        dataMngr.AddResults(startT, "Covariance", signals[i].PMUName + "&" + signals[ii].PMUName, signals[i].SignalName + "&" + signals[ii].SignalName, co, endT);

                    }
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Covariance");
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
        public int PercentileValue { get; set; }
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
                    double per = Double.NaN;
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        per = SignatureCalculations.Percentile(item.Data, PercentileValue);
                    }
                    else
                    {
                        per = SignatureCalculations.Percentile(item.Data, PercentileValue);
                    }
                    dataMngr.AddResults(startT, "Percentile", item.PMUName, item.SignalName, per, endT);
#if DEBUG
                    Console.WriteLine("Percentile:");
                    Console.WriteLine(per);
#endif
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Percentile");
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
            //if there are still data to be processed
            // two situation: 1, un-processed data available; 2, has to wait for data being read.
            // while waiting, keep checking back
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
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
                    double qua1 = Double.NaN;
                    double qua2 = Double.NaN;
                    double qua3 = Double.NaN;
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        qua1 = SignatureCalculations.Percentile(item.Data, 25);
                        qua2 = SignatureCalculations.Percentile(item.Data, 50);
                        qua3 = SignatureCalculations.Percentile(item.Data, 75);
                    }
                    else
                    {
                        qua1 = SignatureCalculations.Percentile(item.Data, 25);
                        qua2 = SignatureCalculations.Percentile(item.Data, 50);
                        qua3 = SignatureCalculations.Percentile(item.Data, 75);
                    }
                    dataMngr.AddResults(startT, "Quartiles_25", item.PMUName, item.SignalName, qua1, endT);
                    dataMngr.AddResults(startT, "Quartiles_50", item.PMUName, item.SignalName, qua2, endT);
                    dataMngr.AddResults(startT, "Quartiles_75", item.PMUName, item.SignalName, qua3, endT);
#if DEBUG
                    Console.WriteLine("Quartiles:");
                    Console.WriteLine(qua1);
                    Console.WriteLine(qua2);
                    Console.WriteLine(qua3);
#endif
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Quartiles");
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        med = SignatureCalculations.Median(item.Data);
                    }
                    else
                    {
                        med = SignatureCalculations.Median(item.Data);
                    }
                    dataMngr.AddResults(startT, "Median", item.PMUName, item.SignalName, med, endT);
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
                foreach (var item in signals)
                {
                    double max = Double.NaN;
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        max = SignatureCalculations.Maximum(item.Data);
                    }
                    else
                    {
                        max = SignatureCalculations.Maximum(item.Data);
                    }
                    dataMngr.AddResults(startT, "Maximum", item.PMUName, item.SignalName, max, endT);
#if DEBUG
                    Console.WriteLine("Maximum:");
                    Console.WriteLine(max);
#endif
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        min = SignatureCalculations.Minimum(item.Data);
                    }
                    else
                    {
                        min = SignatureCalculations.Minimum(item.Data);
                    }
                    dataMngr.AddResults(startT, "Minimum", item.PMUName, item.SignalName, min, endT);
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        range = SignatureCalculations.Range(item.Data);
                    }
                    else
                    {
                        range = SignatureCalculations.Range(item.Data);
                    }
                    dataMngr.AddResults(startT, "Range", item.PMUName, item.SignalName, range, endT);
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
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        rise = SignatureCalculations.Rise(item.Data);
                    }
                    else
                    {
                        rise = SignatureCalculations.Rise(item.Data);
                    }
                    dataMngr.AddResults(startT, "Rise", item.PMUName, item.SignalName, rise, endT);
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
        public double Minimum { get; set; }
        public double Maximum { get; set; }

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
                //bool firstSignal = true;
                foreach (var item in signals)
                {
                    List<List<double>> re = null;
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        re = SignatureCalculations.Hist(item.Data, NumberOfBins, Minimum, Maximum);
                    }
                    else
                    {
                        re = SignatureCalculations.Hist(item.Data, NumberOfBins, Minimum, Maximum);
                    }
                    for (int i = 0; i < NumberOfBins; i++)
                    {
                        //if (firstSignal)
                        //{
                        //    dataMngr.AddResults(startT, "Histogram_" + "bc_" + i.ToString(), item.PMUName, item.SignalName, re[0][i], endT);
                        //}
                        dataMngr.AddResults(startT, "Histogram_" + re[0][i].ToString(), item.PMUName, item.SignalName, re[1][i], endT);
                    }
                    //if (firstSignal)
                    //{
                    //    firstSignal = false;
                    //}
#if DEBUG
                    Console.WriteLine("Histogram:");
                    Console.WriteLine(re);
#endif
                }
                //bool firstSignal = true;
                //List<List<double>> re = null;
                //List<List<double>> all = new List<List<double>>();
                //foreach (var item in signals)
                //{
                //    if (OmitNan)
                //    {
                //        ProcessNANData(item);
                //        re = SignatureCalculations.Hist(item.Data, NumberOfBins, Minimum, Maximum);
                //    }
                //    else
                //    {
                //        re = SignatureCalculations.Hist(item.Data, NumberOfBins, Minimum, Maximum);
                //    }
                //    if (firstSignal)
                //    {
                //        all.AddRange(re);
                //        firstSignal = false;
                //    }
                //    else
                //    {
                //        all.Add(re[1]);
                //    }
                //}
                //for (int i = 0; i < all.Count; i++)
                //{
                //    for (int ii = 0; ii < NumberOfBins; ii++)
                //    {
                //        if (i == 0)
                //        {
                //            dataMngr.AddResults(signals[i].TimeStamps.FirstOrDefault(), "Histogram_" + i.ToString() + "_bc", signals[i].PMUName, signals[i].SignalName, re[i][ii], signals[i].TimeStamps.LastOrDefault());
                //        }
                //        else
                //        {
                //            dataMngr.AddResults(signals[i].TimeStamps.FirstOrDefault(), "Histogram_" + i.ToString(), signals[i].PMUName, signals[i].SignalName, re[i][ii], signals[i].TimeStamps.LastOrDefault());
                //        }
                //    }
                //}
                ////if (firstSignal)
                ////{
                ////    firstSignal = false;
                ////}
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Histogram");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class RootMeanSquare : SignatureSetting
    {
        public override string SignatureName { get { return "Root Mean Squared Value"; } }
        public bool RemoveMean { get; set; }

        public override void Process(DataStore dataMngr)
        {
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
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
                    double rms = Double.NaN;
                    if (OmitNan)
                    {
                        ProcessNANData(item);
                        rms = SignatureCalculations.RootMeanSquare(item.Data, RemoveMean);
                    }
                    else
                    {
                        rms = SignatureCalculations.RootMeanSquare(item.Data, RemoveMean);
                    }
                    dataMngr.AddResults(startT, "RMS", item.PMUName, item.SignalName, rms, endT);
#if DEBUG
                    Console.WriteLine("Root Mean Squared Value:");
                    Console.WriteLine(rms);
#endif
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Root Mean Squared Value");
        }

        public override void ProcessNANData(Signal sig)
        {
            RemoveNanValue(sig);
        }
    }
    public class FrequencyBandRMS : SignatureSetting
    {
        public override string SignatureName { get { return "Frequency Band Root Mean Squared Value"; } }
        public bool CalculateFull { get; set; }
        public bool CalculateBand2 { get; set; }
        public bool CalculateBand3 { get; set; }
        public bool CalculateBand4 { get; set; }
        public double Threshold { get; set; }
        public override void Process(DataStore dataMngr)
        {
            var startT = dataMngr.TimeZero;
            var endT = startT.AddSeconds(WindowSize);

            while (true)
            {
                List<Signal> signals = new List<Signal>();
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
                    Dictionary<string, double> fbrms = null; //should be a dictionary
                    if (OmitNan)
                    {
                        if (InterpolateNanValue(item, Threshold))
                        {
                            fbrms = SignatureCalculations.FrequencyBandRMS(item.Data, item.SamplingRate, CalculateFull, CalculateBand2, CalculateBand3, CalculateBand4);
                        }
                        else
                        {
                            fbrms = new Dictionary<string, double>();
                            if (CalculateFull)
                            {
                                fbrms["Full"] = double.NaN;
                            }
                            if (CalculateBand2)
                            {
                                fbrms["Band2"] = double.NaN;
                            }
                            if (CalculateBand3)
                            {
                                fbrms["Band3"] = double.NaN;
                            }
                            if (CalculateBand4)
                            {
                                fbrms["Band4"] = double.NaN;
                            }
                        }
                    }
                    else
                    {
                        fbrms = SignatureCalculations.FrequencyBandRMS(item.Data, item.SamplingRate, CalculateFull, CalculateBand2, CalculateBand3, CalculateBand4);
                    }
                    foreach (var re in fbrms)
                    {
                        dataMngr.AddResults(startT, "Frequency Band RMS_" + re.Key, item.PMUName, item.SignalName, re.Value, endT);
#if DEBUG
                        Console.WriteLine("Frequency Band Root Mean Squared Value: " + re.Key);
                        Console.WriteLine(re.Value);
#endif
                    }
                }
                startT = endT.AddSeconds(-WindowOverlap);
                endT = startT.AddSeconds(WindowSize);
            }
            dataMngr.FinishedSignatures.Add("Frequency Band Root Mean Squared Value");
        }

        public override void ProcessNANData(Signal sig)
        {
            //return InterpolateNanValue(sig, Threshold);
        }
    }
}
