using AS.ComputationManager.Models;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AS.ComputationManager.Functions
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
            double[] win;

            switch (parameters.WindowType)
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
                    throw new Exception(parameters.WindowType.ToString() + " is not an acceptable window type.");
            }
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
            var removedMeanData = new List<List<double>>();
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
                removedMeanData.Add(newSig);
            }


            return new InspectionAnalysisResults();
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
