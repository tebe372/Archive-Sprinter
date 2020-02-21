using AS.ComputationManager.Models;
using AS.Core.Models;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AS.ComputationManager.Calculations
{
    public static class Functions
    {
        public static InspectionAnalysisResults InspectionSpectral(List<List<double>> data, InspectionAnalysisParameters parameters)
        {

            var fs = parameters.Fs;
            int N = (int)Math.Round(parameters.AnalysisLength * fs);
            var Nw = Math.Round(parameters.WindowLength * fs);
            var Nover = Math.Round(parameters.WindowOverlap * fs);
            var LS = parameters.LogScale;
            var windowtype = parameters.WindowType;

            var freqMin = 0d;
            var freqMax = 0d;
            if (parameters.FreqMin != null)
            {
                freqMin = (double)parameters.FreqMin;
            }
            if (parameters.FreqMax != null)
            {
                freqMax = (double)parameters.FreqMax;
            }
            var Nzp = 0;
            if (parameters.ZeroPadding != null)
            {
                Nzp = (int)parameters.ZeroPadding;
            }

            List<List<double>> dataInFreqDomain;
            List<double> freqAll;

            ComputePSD(data, fs, N, Nw, Nover, Nzp, windowtype, out dataInFreqDomain, out freqAll);

            InspectionAnalysisResults re = CutPSDbyFreq(LS, freqMin, freqMax, dataInFreqDomain, freqAll);

            re.Xlabel = "F (Hz)";
            if (LS)
            {
                re.Ylabel = "PSD (dB)";
            }
            else
            {
                re.Ylabel = "PSD";
            }
            //need to do some argument validation 
            //such as the window size need to be smaller than the actual signal length
            //overlap size need to be smaller than window size

            return re;
        }

        public static InspectionAnalysisResults CutPSDbyFreq(bool LS, double freqMin, double freqMax, List<List<double>> dataInFreqDomain, List<double> freqAll)
        {
            var re = new InspectionAnalysisResults();
            var keepFreq = new List<double>();
            var keepPxx = new List<List<double>>();
            for (int i = 0; i < dataInFreqDomain.Count; i++)
            {
                keepPxx.Add(new List<double>());
            }
            for (int i = 0; i < freqAll.Count; i++)
            {
                var freq = freqAll[i];
                if (freq >= freqMin && freq <= freqMax)
                {
                    keepFreq.Add(freq);
                    for (int ii = 0; ii < dataInFreqDomain.Count; ii++)
                    {
                        if (LS)
                        {
                            keepPxx[ii].Add(10 * Math.Log10(dataInFreqDomain[ii][i]));
                        }
                        else
                        {
                            keepPxx[ii].Add(dataInFreqDomain[ii][i]);
                        }
                    }
                }
            }

            re.Y = keepPxx;
            re.X = keepFreq;
            return re;
        }

        public static void ComputePSD(List<List<double>> data, int fs, int N, double Nw, double Nover, int Nzp, DetectorWindowType windowtype, out List<List<double>> dataInFreqDomain, out List<double> freqAll)
        {

            double[] win;

            switch (windowtype)
            {
                case Core.Models.DetectorWindowType.hann:
                    win = Window.Hann((int)Nw);
                    break;
                case Core.Models.DetectorWindowType.rectwin:
                    win = _rectwin((int)Nw);
                    break;
                case Core.Models.DetectorWindowType.bartlett:
                    win = Window.Bartlett((int)Nw);
                    break;
                case Core.Models.DetectorWindowType.hamming:
                    win = Window.Hamming((int)Nw);
                    break;
                case Core.Models.DetectorWindowType.blackman:
                    win = Window.Blackman((int)Nw);
                    break;
                default:
                    throw new Exception(windowtype.ToString() + " is not an acceptable window type.");
            }
            var U = win.Sum(x => Math.Pow(x, 2));

            dataInFreqDomain = new List<List<double>>();

            //find the frequency vector which should be the x axis of the spectrum
            freqAll = new List<double>();
            for (int i = 0; i < Nzp; i++)
            {
                freqAll.Add((double)fs * i / Nzp);
            }

            //comput the number of segments: (signalLength - overlap)/(windowLength - overlap)
            //round towards zero
            var M = Math.Floor((N - Nover) / (Nw - Nover));

            foreach (var sig in data)
            {
                var newSig = new List<double>();
                var dat = sig.GetRange(0, N);
                var mean = dat.Average();
                foreach (var p in dat)
                {
                    var pp = p - mean;
                    newSig.Add(pp);
                }
                double[] Pxx = null;
                for (int i = 0; i < M; i++)
                {
                    var start = (int)(i * (Nw - Nover));
                    var end = (int)(i * (Nw - Nover) + Nw);
                    Console.WriteLine("start from " + start.ToString() + ", end at " + end.ToString());
                    //to match InspectionSpectral where mean is removed before calling pwelch
                    var frag = newSig.GetRange(start, (int)Nw);
                    //to match CalcPSD_OmegaB.m where mean is not removed from the data before calling fft
                    //var frag = dat.GetRange(start, (int)Nw);
                    Console.WriteLine("number of point " + frag.Count());

                    //double[] real = null;
                    var fragcount = frag.Count;

                    //if (fragcount % 2 == 0)
                    //{
                    //    real = new double[fragcount + 2];
                    //    real[fragcount] = 0d;
                    //    real[fragcount + 1] = 0d;
                    //}
                    //else
                    //{
                    //    real = new double[fragcount + 1];
                    //    real[fragcount] = 0d;
                    //}
                    var complex = new Complex[Nzp];
                    //multiply with filter win first
                    for (int ii = 0; ii < frag.Count; ii++)
                    {
                        //real[ii] = frag[ii] * win[ii];
                        complex[ii] = new Complex(frag[ii] * win[ii], 0);
                    }
                    //might need an empty vector to hold results, or pass by ref?
                    //Console.WriteLine(frag[0]);
                    //var fragarray = frag.ToArray();
                    //Console.WriteLine("[{0}]", string.Join(", ", real));
                    //Fourier.ForwardReal(real, fragcount, FourierOptions.Matlab);
                    //Console.WriteLine("[{0}]", string.Join(", ", real));
                    //Console.WriteLine("[{0}]", string.Join(", ", complex));
                    Fourier.Forward(complex, FourierOptions.Matlab);
                    //Console.WriteLine("[{0}]", string.Join(", ", complex));
                    //Console.WriteLine("real: {0}, imaginary: {1}, magnitude: {2}", complex[0].Real, complex[0].Imaginary, complex[0].Magnitude);

                    if (Pxx == null)
                    {
                        Pxx = new double[complex.Length];
                    }

                    for (int ii = 0; ii < complex.Length; ii++)
                    {
                        Pxx[ii] = Pxx[ii] + (Math.Pow(complex[ii].Magnitude, 2) / (U * M));
                    }
                }
                //Pxx[0] = Pxx[0] / 2;
                dataInFreqDomain.Add(Pxx.ToList());
            }
        }

        private static double[] _rectwin(int nw)
        {
            double[] re = new double[nw];
            for (int i = 0; i < nw; i++)
            {
                re[i] = 1.0;
            }
            return re;
        }
    }
}
