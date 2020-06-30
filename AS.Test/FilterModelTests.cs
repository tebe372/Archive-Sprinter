using System;
using System.Collections.Generic;
using System.Linq;
using AS.ComputationManager.Calculations;
using AS.Config;
using AS.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace AS.Test
{
    [TestClass]
    public class FilterModelTests
    {
        private List<Signal> signals;

        [TestFixtureSetUp]
        public void Init()
        {
        }
        [TestCase(false, 80, new int[] { 2, 0, 6, 3, 4, 3, 7, 6, 3, 7 })]
        [TestCase(false, 70, new int[] { 2, 0, 6, 3, 4, 3, 10, 6, 3, 10 })]
        [TestCase(false, 50, new int[] { 2, 0, 10, 3, 4, 3, 10, 10, 3, 10 })]
        [TestCase(false, 40, new int[] { 2, 0, 10, 3, 10, 3, 10, 10, 3, 10 })]
        [TestCase(false, 30, new int[] { 2, 0, 10, 10, 10, 10, 10, 10, 10, 10 })]
        [TestCase(false, 10, new int[] { 10, 0, 10, 10, 10, 10, 10, 10, 10, 10 })]
        [TestCase(false, 0, new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 })]
        [TestCase(true, 70, new int[] { 2, 0, 6, 3, 4, 3, 7, 6, 3, 7 })]
        [TestCase(true, 60, new int[] { 3, 0, 8, 3, 4, 3, 8, 7, 3, 8 })]
        [TestCase(true, 40, new int[] { 3, 0, 10, 3, 6, 5, 10, 7, 5, 10 })]
        [TestCase(true, 30, new int[] { 3, 0, 10, 7, 10, 7, 10, 7, 7, 10 })]
        [TestCase(true, 10, new int[] { 3, 0, 10, 10, 10, 7, 10, 7, 7, 10 })]
        [TestCase(true, 0, new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 })]
        public void DataFrameDQFilterTest(bool keepDiffPMUSeparate, int percentThresh, int[] falseCount)
        {
            signals = new List<Signal>();
            for (int i = 1; i <= 10; i++)
            {
                var s = new Signal();
                s.PMUName = "PMU_" + (i % 3).ToString();
                s.SignalName = "Name_" + (i % 5).ToString();
                s.Data = new List<double>();
                for (int ii = 1; ii <= 10; ii++)
                {
                    s.Data.Add(Math.Cos(i * i + ii * ii * ii + i + ii + i * ii) / (Math.Log(ii * i + i + ii) + ii) * ii);
                }
                signals.Add(s);
            }
            foreach (var s in signals)
            {
                Filters.OutlierFilt(s, 1);
            }
            var testObject = new DataFrameDQFilt();
            testObject.KeepDiffPMUSeparate = keepDiffPMUSeparate;
            testObject.PercentBadThresh = percentThresh;
            testObject.InputSignals = signals.Select(x => x.PMUName + "_" + x.SignalName).ToList();
            var filteredSignals = testObject.Process(signals);
            var signalCount = signals.Count;
            var signalLength = signals.FirstOrDefault().Data.Count;
            for (int ii = 0; ii < signalLength; ii++)
            {
                int countFalse = 0;
                for (int i = 0; i < signalCount; i++)
                {
                    if (!signals[i].Flags[ii])
                    {
                        countFalse++;
                    }
                }
                NUnit.Framework.Assert.AreEqual(countFalse, falseCount[ii]);
            }
        }
        [TestCase(false, 50, 41)]
        [TestCase(false, 42, 41)]
        [TestCase(false, 41, 100)]
        [TestCase(false, 40.5, 100)]
        [TestCase(false, 40, 100)]
        [TestCase(true, 50, 41)]
        [TestCase(true, 42, 64)]
        [TestCase(true, 41, 64)]
        [TestCase(true, 40.5, 64)]
        [TestCase(true, 40, 100)]
        public void PMUallDQFilterTest(bool keepDiffPMUSeparate, double percentThresh, int falseCount)
        {
            signals = new List<Signal>();
            for (int i = 1; i <= 10; i++)
            {
                var s = new Signal();
                s.PMUName = "PMU_" + (i % 3).ToString();
                s.SignalName = "Name_" + (i % 5).ToString();
                s.Data = new List<double>();
                for (int ii = 1; ii <= 10; ii++)
                {
                    s.Data.Add(Math.Cos(i * i + ii * ii * ii + i + ii + i * ii) / (Math.Log(ii * i + i + ii) + ii) * ii);
                }
                signals.Add(s);
            }
            foreach (var s in signals)
            {
                Filters.OutlierFilt(s, 1);
            }
            var testObject = new PMUallDQFilt();
            testObject.KeepDiffPMUSeparate = keepDiffPMUSeparate;
            testObject.PercentBadThresh = percentThresh;
            testObject.InputSignals = signals.Select(x => x.PMUName + "_" + x.SignalName).ToList();
            var filteredSignals = testObject.Process(signals);
            var signalCount = signals.Count;
            var signalLength = signals.FirstOrDefault().Data.Count;
            int countFalse = 0;
            for (int ii = 0; ii < signalLength; ii++)
            {
                for (int i = 0; i < signalCount; i++)
                {
                    if (!signals[i].Flags[ii])
                    {
                        countFalse++;
                    }
                }
            }
            NUnit.Framework.Assert.AreEqual(countFalse, falseCount);
        }
        [TestCase(true, 30, new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 })]
        [TestCase(true, 40, new int[] { 10, 10, 10, 10, 3, 10, 10, 10, 3, 10 })]
        [TestCase(true, 50, new int[] { 10, 10, 4, 4, 3, 10, 4, 4, 3, 4 })]
        [TestCase(true, 60, new int[] { 5, 5, 4, 4, 3, 5, 4, 4, 3, 4 })]
        public void PMUchanDQFilterTest(bool keepDiffPMUSeparate, double percentThresh, int[] falseCount)
        {
            signals = new List<Signal>();
            for (int i = 1; i <= 10; i++)
            {
                var s = new Signal();
                s.PMUName = "PMU_" + (i % 3).ToString();
                s.SignalName = "Name_" + (i % 5).ToString();
                s.Data = new List<double>();
                for (int ii = 1; ii <= 10; ii++)
                {
                    s.Data.Add(Math.Cos(i * i + ii * ii * ii + i + ii + i * ii) / (Math.Log(ii * i + i + ii) + ii) * ii);
                }
                signals.Add(s);
            }
            foreach (var s in signals)
            {
                Filters.OutlierFilt(s, 1);
            }
            var testObject = new PMUchanDQFilt();
            testObject.KeepDiffPMUSeparate = keepDiffPMUSeparate;
            testObject.PercentBadThresh = percentThresh;
            testObject.InputSignals = signals.Select(x => x.PMUName + "_" + x.SignalName).ToList();
            var filteredSignals = testObject.Process(signals);
            var signalCount = signals.Count;
            var signalLength = signals.FirstOrDefault().Data.Count;
            for (int i = 0; i < signalCount; i++)
            {
                int countFalse = 0;
                for (int ii = 0; ii < signalLength; ii++)
            {
                    if (!signals[i].Flags[ii])
                    {
                        countFalse++;
                    }
                }
                NUnit.Framework.Assert.AreEqual(countFalse, falseCount[i]);
            }
        }
    }
}
