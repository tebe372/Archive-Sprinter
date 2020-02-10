using AS.ComputationManager.Functions;
using AS.ComputationManager.Models;
using AS.Core.Models;
using AS.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Test
{
    [TestFixture]
    public class SignalInspectionTest
    {
        List<List<double>> data;
        List<Signal> signals;

        [TestFixtureSetUp]
        public void Init()
        {
            var reader = DataFileReaderFactory.Create(DataFileType.csv);
            //Console.WriteLine(Environment.CurrentDirectory);
            signals = reader.Read("Data\\SpectralInpectionData.csv");
            data = new List<List<double>>();
            foreach (var sig in signals)
            {
                data.Add(sig.Data);
            }
        }

        [TestCase(50d, 10d, DetectorWindowType.hamming, 5d, 1500, 30, true, 0.1, 1d, "PSD (dB)")]
        [TestCase(50d, 10d, DetectorWindowType.hamming, 5d, 1500, 30, false, 0.1, 1d, "PSD")]
        public void TestInspectionSpectral(double analysisLength, double windowLength, DetectorWindowType windowType, double windowOverlap, int zeroPadding, int fs, bool logScale, double freqMin, double freqMax, string expectedResults)
        {
            var parameters = new InspectionAnalysisParameters();
            parameters.AnalysisLength = analysisLength;
            parameters.WindowLength = windowLength;
            parameters.WindowType = windowType;
            parameters.WindowOverlap = windowOverlap;
            parameters.ZeroPadding = zeroPadding;
            parameters.Fs = fs;
            parameters.LogScale = logScale;
            parameters.FreqMax = freqMax;
            parameters.FreqMin = freqMin;

            var result = Functions.InspectionSpectral(data, parameters);

            NUnit.Framework.Assert.AreEqual(result.X.Count, 46);
            NUnit.Framework.Assert.AreEqual(result.Y.Count, 2);
            NUnit.Framework.Assert.AreEqual(result.Y[0].Count, 46);
            NUnit.Framework.Assert.AreEqual(result.Ylabel, expectedResults);
            NUnit.Framework.Assert.AreEqual(result.Xlabel, "F (Hz)");
        }

        [TestCase(30, 1500, 300, 150, 1500, DetectorWindowType.bartlett, 92.91136328314748, 93.5613256488336, 89.170754510762691, 89.605225419546358, 3.0781774090733134E-06, 1.4641431497970023E-06)]
        [TestCase(30, 1500, 300, 150, 1500, DetectorWindowType.blackman, 79.654708691803663, 78.782979097541642, 77.364979556415733, 76.485541363684916, 2.9899437190726274E-06, 1.2460333193309568E-06)]
        [TestCase(30, 1500, 300, 150, 1500, DetectorWindowType.hamming, 91.947160307760839, 92.233833565501, 88.316932076741679, 88.444148359081083, 2.570007369228773E-05, 1.7677167742430081E-05)]
        [TestCase(30, 1500, 300, 150, 1500, DetectorWindowType.hann, 86.9525247817376, 86.663143961863412, 83.93697072277223, 83.576992855047578, 3.0496039999617379E-06, 1.3517198775756861E-06)]
        [TestCase(30, 1500, 300, 150, 1500, DetectorWindowType.rectwin, 101.37427672758135, 105.57784318369679, 95.489610719180547, 98.557488123545838, 0.0011724188067890509, 0.00096269717777784517)]
        public void TestComputePSD(int fs, int N, double Nw, double Nover, int Nzp, DetectorWindowType windowtype, double expValue00, double expValue10, double expValue0e, double expValue1e, double expValue0m, double expValue1m)
        {
            List<List<double>> dataInFreqDomain;
            List<double> freqAll;
            Functions.ComputePSD(data, fs, N, Nw, Nover, Nzp, windowtype, out dataInFreqDomain, out freqAll);

            var slength = dataInFreqDomain[0].Count;
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain.Count, 2);
            NUnit.Framework.Assert.AreEqual(slength, 1500);
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain[0][0], expValue00);
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain[1][0], expValue10);
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain[0][slength - 1], expValue0e);
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain[1][slength - 1], expValue1e);
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain[0][slength / 2], expValue0m);
            NUnit.Framework.Assert.AreEqual(dataInFreqDomain[1][slength / 2], expValue1m);
            NUnit.Framework.Assert.AreEqual(freqAll.Count, 1500);
            NUnit.Framework.Assert.AreEqual(freqAll[0], 0);
            NUnit.Framework.Assert.AreEqual(freqAll[freqAll.Count - 1], 29.98);
            NUnit.Framework.Assert.AreEqual(freqAll[freqAll.Count / 2], 15);

        }

        [TestCase(true, 0.0001, 0.0002, 2.1253916067406697, 15.924010664293252, 2.8988536659188369, 15.947758422173292, 1.8494682305189385, 15.947636969458509)]
        [TestCase(false, 0.0001, 0.0002, -1.63132, -39.1202, -1.94933, -39.3347, -1.5309, -39.3336)]
        public void TestCutPSDbyFreq(bool LS, double freqMin, double freqMax, double expValue00, double expValue10, double expValue0e, double expValue1e, double expValue0m, double expValue1m)
        {
            InspectionAnalysisResults re = null;
            var freq = signals.First().TimeStampNumber;
            if (LS)
            {
                var data2 = new List<List<double>>();
                foreach (var item in data)
                {
                    data2.Add(item.Select(x => Math.Abs(x)).ToList());
                }
                re = Functions.CutPSDbyFreq(LS, freqMin, freqMax, data2, freq);
            }
            else
            {
                re = Functions.CutPSDbyFreq(LS, freqMin, freqMax, data, freq);
            }

            var slength = re.Y[0].Count;
            NUnit.Framework.Assert.AreEqual(re.X.Count, 259);
            NUnit.Framework.Assert.AreEqual(re.Y.Count, 2);
            NUnit.Framework.Assert.AreEqual(slength, 259);
            NUnit.Framework.Assert.AreEqual(re.Y[0][0], expValue00);
            NUnit.Framework.Assert.AreEqual(re.Y[1][0], expValue10);
            NUnit.Framework.Assert.AreEqual(re.Y[0][slength - 1], expValue0e);
            NUnit.Framework.Assert.AreEqual(re.Y[1][slength - 1], expValue1e);
            NUnit.Framework.Assert.AreEqual(re.Y[0][slength / 2], expValue0m);
            NUnit.Framework.Assert.AreEqual(re.Y[1][slength / 2], expValue1m);
            NUnit.Framework.Assert.AreEqual(re.X[0], 0.0001003125);
            NUnit.Framework.Assert.AreEqual(re.X[slength - 1], 0.00019984953703703704);
            NUnit.Framework.Assert.AreEqual(re.X[slength / 2], 0.00015008101851851852);
        }
    }
}
