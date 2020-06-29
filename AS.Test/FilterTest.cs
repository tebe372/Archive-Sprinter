using System;
using System.Collections.Generic;
using System.Linq;
using AS.ComputationManager.Calculations;
using AS.Core.Models;
using NUnit.Framework;

namespace AS.Test
{
    public class FilterTest
    {
        private Signal signal;
        private List<Signal> signals;

        [TestFixtureSetUp]
        public void Init()
        {
            signal = new Signal();
            signal.Data = new List<double> { 0, 0, 0, 3, 4, 5, 6, 7, 8, 8, 8, 8, 8, 13, 14, 15, 16, 17, 17, 17, 17, 21, 22, 23, 23, 23, 26, 26, 28, 28, 30, 30 };
            signals = new List<Signal>();
            for (int i = 1; i <= 10; i++)
            {
                var s = new Signal();
                s.Data = new List<double>();
                for (int ii = 1; ii <= 10; ii++)
                {
                    s.Data.Add(Math.Cos(i * i + ii * ii * ii + i + ii + i * ii) / (Math.Log(ii * i + i + ii) + ii) * ii);
                }
                signals.Add(s);
            }
        }

        [TestCase(1, 32, true)]
        [TestCase(2, 21, true)]
        [TestCase(3, 15, true)]
        [TestCase(4, 9, true)]
        [TestCase(5, 5, true)]
        [TestCase(6, 0, false)]
        public void TestStaleDQFilter(int threshold, int falseCount, bool containsFalse)
        {
            signal.Data = new List<double> { 0, 0, 0, 3, 4, 5, 6, 7, 8, 8, 8, 8, 8, 13, 14, 15, 16, 17, 17, 17, 17, 21, 22, 23, 23, 23, 26, 26, 28, 28, 30, 30 };
            for (int i = 0; i < signal.Flags.Count; i++)
            {
                signal.Flags[i] = true;
            }
            Filters.StaleDQFilt(signal, threshold);
            int countFalse = 0;
            for (int i = 0; i < signal.Flags.Count; i++)
            {
                if (!signal.Flags[i])
                {
                    countFalse++;
                }
            }
            NUnit.Framework.Assert.AreEqual(signal.Flags.Contains(false), containsFalse);
            NUnit.Framework.Assert.AreEqual(countFalse, falseCount);
        }
        [TestCase(3, 0, false)]
        [TestCase(2, 0, false)]
        [TestCase(1, 14, true)]
        [TestCase(0, 32, true)]
        public void TestOutlierDQFilter(int threshold, int falseCount, bool containsFalse)
        {
            for (int i = 0; i < signal.Flags.Count; i++)
            {
                signal.Flags[i] = true;
                signal.Data[i] = i;
            }
            Filters.OutlierFilt(signal, threshold);
            int countFalse = 0;
            for (int i = 0; i < signal.Flags.Count; i++)
            {
                if (!signal.Flags[i])
                {
                    countFalse++;
                }
            }
            NUnit.Framework.Assert.AreEqual(signal.Flags.Contains(false), containsFalse);
            NUnit.Framework.Assert.AreEqual(countFalse, falseCount);
        }
        [TestCase(60, 10, 2)]
        [TestCase(70, 10, 6)]
        [TestCase(60, 10, 7)]
        [TestCase(70, 10, 9)]
        [TestCase(40, 10, 4)]
        [TestCase(30, 10, 3)]
        [TestCase(30, 10, 5)]
        [TestCase(30, 10, 8)]
        [TestCase(20, 10, 0)]
        [TestCase(0, 10, 1)]
        public void TestDataFrameDQFilter(int threshold, int falseCount, int row)
        {
            foreach (var s in signals)
            {
                Filters.OutlierFilt(s, 1);
            }
            Filters.DataFrameDQFilt(signals, threshold);
            var signalCount = signals.Count;

            int countFalse = 0;
            for (int ii = 0; ii < signalCount; ii++)
            {
                if (!signals[ii].Flags[row])
                {
                    countFalse++;
                }
            }
            NUnit.Framework.Assert.AreEqual(countFalse, falseCount);
        }
        [TestCase(60, new bool[] { true, true, true, true, true, true, true, true, true, true })]
        [TestCase(50, new bool[] { false, false, true, true, true, false, true, true, true, true })]
        [TestCase(40, new bool[] { false, false, false, false, true, false, false, false, true, false })]
        [TestCase(30, new bool[] { false, false, false, false, false, false, false, false, false, false })]
        public void TestchanDQFilter(int threshold, bool[] containsTrue)
        {
            NUnit.Framework.Assert.AreEqual(signals.Count, containsTrue.Length);
            foreach (var s in signals)
            {
                Filters.OutlierFilt(s, 1);
                Filters.PMUchanDQFilt(s, threshold);
            }
            for (int i = 0; i < signals.Count; i++)
            {
                NUnit.Framework.Assert.AreEqual(signals[i].Flags.Contains(true), containsTrue[i]);
            }
        }
        [TestCase(50, true)]
        [TestCase(40, false)]
        public void TestPMUallDQFilter(int threshold, bool containsTrue)
        {
            foreach (var s in signals)
            {
                Filters.OutlierFilt(s, 1);
            }
            Filters.PMUallDQFilt(signals, threshold);
            var hasTrue = false;
            for (int i = 0; i < signals.Count; i++)
            {
                if (signals[i].Flags.Contains(true))
                {
                    hasTrue = true;
                    break;
                }
            }
            NUnit.Framework.Assert.AreEqual(hasTrue, containsTrue);
        }
    }
}
