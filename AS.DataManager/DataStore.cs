using AS.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AS.DataManager
{
    public class DataStore
    {
        //should be able to find the signal we need quickly
        //signal should be found by name and time
        public DataStore()
        {
            DataCompleted = false;
            Signals = new Dictionary<string, Dictionary<DateTime, Signal>>();
            StartTimeStamps = new List<DateTime>();
            EndTimeStamps = new List<DateTime>();
            TimePairs = new Dictionary<DateTime, DateTime>();
            //FirstFileRead = false;
        }
        // when all data in the data source directory are read and preprocessed and put in the store
        public bool DataCompleted { get; set; }
        public DateTime TimeZero { get; set; }
        public Dictionary<string, Dictionary<DateTime, Signal>> Signals { get; set; }
        public List<DateTime> StartTimeStamps { get; set; }
        public List<DateTime> EndTimeStamps { get; set; }
        public Dictionary<DateTime, DateTime> TimePairs { get; set; }
        public bool FirstFile { get; set; }
        public int WriteOutIntervalInSeconds { get; set; }
        private int _datawriteOutFrequency;
        public int DatawriteOutFrequency {
            set {
                if (_datawriteOutFrequency != value)
                {
                    _datawriteOutFrequency = value;
                    //if (!string.IsNullOrEmpty(_datawriteOutFrequencyUnit))
                    //{
                    //    WriteOutIntervalInSeconds = _convertWriteOutIntervalToSeconds(_datawriteOutFrequency, _datawriteOutFrequencyUnit);
                    //}
                }
            }
            get { return _datawriteOutFrequency; }
        }
        private string _datawriteOutFrequencyUnit;
        public string DatawriteOutFrequencyUnit 
        {
            get { return _datawriteOutFrequencyUnit; }
            set
            {
                if (_datawriteOutFrequencyUnit != value)
                {
                    _datawriteOutFrequencyUnit = value;
                    //if (_datawriteOutFrequency != 0)
                    //{
                    //    WriteOutIntervalInSeconds = _convertWriteOutIntervalToSeconds(_datawriteOutFrequency, _datawriteOutFrequencyUnit);
                    //}
                }
            }
        }
        public int NumberOfColumns { get; set; }
        public DateTime NextTimeStamp { get; set; }
        public DateTime CurrentTimeStamp { get; set; }
        public int WindowOverlap { get; set; }
        public int WindowSize { get; set; }
        public int NumberOfSignatures { get; set; }
        public List<string> FinishedSignatures { get; set; } = new List<string>();
        public string SignatureOutputDir { get; set; }
        public DateTime FinalTimeStamp { get; set; }
        //public bool FirstFileRead { get; set; }
        private object _theEndTimeStampsLock = new object();
        private object _theResultsLock = new object();
        private object _theInputSignalsLock = new object();
        private DateTime _getNextWriteOutTime(DateTime current, int interval, string unit)
        {
            DateTime result;
            DateTime t;
            switch (unit)
            {
                case "Hours":
                    t = current.AddHours(interval);
                    result = new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0, 0);
                    return result;
                case "Days":
                    t = current.AddDays(interval);
                    result = new DateTime(t.Year, t.Month, t.Day);
                    return result;
                case "Weeks":
                    t = current.AddDays(interval * 7);
                    result = new DateTime(t.Year, t.Month, t.Day);
                    return result;
                case "Months":
                    t = current.AddMonths(interval);
                    result = new DateTime(t.Year, t.Month, 0);
                    return result;
                default:
                    return current;
            }
        }
        public void AddData(List<Signal> e)
        {
            if (e != null && e.Count > 0)
            {
                var startT = e.FirstOrDefault().TimeStamps.FirstOrDefault();
                if (FirstFile)
                {
                    TimeZero = startT;
                    //FirstFileRead = true;
                    FirstFile = false;
                }
                else if (TimeZero >= startT)
                {
                    throw new Exception("Possible data source files time order problem.");
                }
                StartTimeStamps.Add(startT);
                var endT = e.FirstOrDefault().TimeStamps.LastOrDefault();
                //Console.WriteLine("Added end timestamp: " + endT.ToString("yyyyMMdd_HHmmss.ffffff") + " in " + e.FirstOrDefault().TimeStamps.Count() + " timestamps.");
                TimePairs[endT] = startT;
                foreach (var sig in e)
                {
                    var name = sig.PMUName + "_" + sig.SignalName;

                    lock (_theInputSignalsLock)
                    {
                        if (!Signals.ContainsKey(name))
                        {
                            Signals[name] = new Dictionary<DateTime, Signal>();
                        }
                        Signals[name][endT] = sig;
                    }
                }
                lock (_theEndTimeStampsLock)
                {
                    EndTimeStamps.Add(endT);
                }
            }
        }

        public bool HasDataAfter(DateTime startT, DateTime endT)
        {
            bool result;
            lock (_theEndTimeStampsLock)
            {
                EndTimeStamps.Sort();
                result = endT <= EndTimeStamps.LastOrDefault();
            }
            return result;
        }

        public bool GetData(List<Signal> signals, DateTime startT, DateTime endT, int windowSize, List<string> signalNames)
        {
            bool foundStart = false;
            List<DateTime> possibleTimeStamps = new List<DateTime>();
            lock (_theEndTimeStampsLock)
            {
                EndTimeStamps.Sort();

                int i1;
                int i2 = 0;
                for (i1 = 0; i1 < EndTimeStamps.Count; i1++)
                {
                    var firstEnd = EndTimeStamps[i1];
                    //find the first fragment that the start time stamp lies in.
                    if (startT <= firstEnd)
                    {
                        foundStart = true;
                        //find the last fragment that the end time stamp lines in
                        var endIdx = _findEndTimeFrame(endT, i1, EndTimeStamps);
                        if (endIdx == null)
                        {
                            // can not find end, the end might have not been read yet.
                            return false;
                        }
                        else
                        {
                            i2 = (int)endIdx;
                        }
                        break;
                    }
                }
                if (!foundStart)
                {
                    return false;
                }
                //else
                //{
                //    //delete all time stamp before i1
                //    EndTimeStamps.RemoveRange(0, i1);
                //    //for (int i = i1 - 1; i >= 0; i--)
                //    //{
                //    //    EndTimeStamps.RemoveAt(i);
                //    //}
                //}
                //find the continuous time stamps that will be concatenated
                possibleTimeStamps = EndTimeStamps.GetRange(i1, i2 - i1 + 1);
            }
            foreach (var name in signalNames)
            {
                Dictionary<DateTime, Signal> sig = null;
                Signal firstSig, lastSig;
                lock (_theInputSignalsLock)
                {
                    sig = Signals[name];
                    firstSig = sig[possibleTimeStamps[0]];
                    lastSig = sig[possibleTimeStamps.LastOrDefault()];
                }
                var thisSig = new Signal(firstSig.PMUName, firstSig.SignalName);
                var firstDataPoint = firstSig.TimeStamps.IndexOf(startT);
                var lastDataPoint = lastSig.TimeStamps.IndexOf(endT);
                if (firstDataPoint == -1 && lastDataPoint == -1)
                {
                    return false;
                }
                if (possibleTimeStamps.Count == 1)
                {
                    // if here's only 1 fragment, find the start and end point and get data between that range
                    //var lastDataPoint = firstSig.TimeStamps.IndexOf(endT);
                    if (lastDataPoint == -1)
                    {
                        //thisSig.Data = firstSig.Data;
                        thisSig.Data = firstSig.Data.GetRange(firstDataPoint, firstSig.Data.Count - firstDataPoint);
                        thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, firstSig.Flags.Count - firstDataPoint);
                        thisSig.TimeStamps = firstSig.TimeStamps.GetRange(firstDataPoint, firstSig.TimeStamps.Count - firstDataPoint);
                    }
                    else if (firstDataPoint == -1)
                    {
                        thisSig.Data = firstSig.Data.GetRange(0, lastDataPoint + 1);
                        thisSig.Flags = firstSig.Flags.GetRange(0, lastDataPoint + 1);
                        thisSig.TimeStamps = firstSig.TimeStamps.GetRange(0, lastDataPoint + 1);
                    }
                    else
                    {
                        thisSig.Data = firstSig.Data.GetRange(firstDataPoint, lastDataPoint - firstDataPoint + 1);
                        thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, lastDataPoint - firstDataPoint + 1);
                        thisSig.TimeStamps = firstSig.TimeStamps.GetRange(firstDataPoint, lastDataPoint - firstDataPoint + 1);
                    }
                }
                else
                {

                    if (lastDataPoint == -1)
                    {
                        thisSig.Data = firstSig.Data.GetRange(firstDataPoint, firstSig.Data.Count - firstDataPoint);
                        thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, firstSig.Flags.Count - firstDataPoint);
                        thisSig.TimeStamps = firstSig.TimeStamps.GetRange(firstDataPoint, firstSig.TimeStamps.Count - firstDataPoint);
                        lock (_theInputSignalsLock)
                        {
                            for (int ii = 1; ii < possibleTimeStamps.Count; ii++)
                        {
                                if (sig[possibleTimeStamps[ii]].TimeStamps.LastOrDefault() <= endT)
                                {
                                    thisSig.Data.AddRange(sig[possibleTimeStamps[ii]].Data);
                                    thisSig.Flags.AddRange(sig[possibleTimeStamps[ii]].Flags);
                                    thisSig.TimeStamps.AddRange(sig[possibleTimeStamps[ii]].TimeStamps);
                                }
                            }
                        }
                    }
                    else if (firstDataPoint == -1)
                    {
                        lock (_theInputSignalsLock)
                        {
                            for (int ii = 0; ii < possibleTimeStamps.Count; ii++)
                        {
                                if (sig[possibleTimeStamps[ii]].TimeStamps.LastOrDefault() <= endT)
                                {
                                    thisSig.Data.AddRange(sig[possibleTimeStamps[ii]].Data);
                                    thisSig.Flags.AddRange(sig[possibleTimeStamps[ii]].Flags);
                                    thisSig.TimeStamps.AddRange(sig[possibleTimeStamps[ii]].TimeStamps);
                                }
                                else
                                {
                                    thisSig.Data.AddRange(sig[possibleTimeStamps[ii]].Data.GetRange(0, lastDataPoint + 1));
                                    thisSig.Flags.AddRange(sig[possibleTimeStamps[ii]].Flags.GetRange(0, lastDataPoint + 1));
                                    thisSig.TimeStamps.AddRange(sig[possibleTimeStamps[ii]].TimeStamps.GetRange(0, lastDataPoint + 1));
                                }
                            }
                        }
                    }
                    else
                    {
                        // if need to put several fragment together, find the first partial piece first
                        thisSig.Data = firstSig.Data.GetRange(firstDataPoint, firstSig.Data.Count - firstDataPoint);
                        thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, firstSig.Flags.Count - firstDataPoint);
                        thisSig.TimeStamps = firstSig.TimeStamps.GetRange(firstDataPoint, firstSig.TimeStamps.Count - firstDataPoint);
                        // if there are middle pieces, add the middle pieces which should be whole pieces
                        for (int ii = 1; ii < possibleTimeStamps.Count; ii++)
                        {
                            thisSig.Data.AddRange(sig[possibleTimeStamps[ii]].Data);
                            thisSig.Flags.AddRange(sig[possibleTimeStamps[ii]].Flags);
                            thisSig.TimeStamps.AddRange(sig[possibleTimeStamps[ii]].TimeStamps);
                        }
                    }
                    // then add the last piece, which could be a partial piece too
                    //var lastSig = sig[possibleTimeStamps[possibleTimeStamps.Count - 1]];
                    //var lastDatPoint = lastSig.TimeStamps.IndexOf(endT);
                    //if (lastDatPoint == -1)
                    //{
                    //    thisSig.Data.AddRange(lastSig.Data);
                    //}
                    //else
                    //{
                    //    thisSig.Data.AddRange(lastSig.Data.GetRange(0, lastDatPoint + 1));
                    //}
                }
                //if (thisSig.Data.Count < windowSize)
                //{
                //    return false;
                //}
                //else
                if (thisSig.Data.Count > windowSize)
                {
                    thisSig.Data.RemoveRange(windowSize, thisSig.Data.Count - windowSize);
                    thisSig.Flags.RemoveRange(windowSize, thisSig.Flags.Count - windowSize);
                    thisSig.TimeStamps.RemoveRange(windowSize, thisSig.TimeStamps.Count - windowSize);
                }
                signals.Add(thisSig);
            }
            //EndTimeStamps.RemoveRange(0, i1);
            return true;
        }
        private int? _findEndTimeFrame(DateTime endT, int beginIdx, List<DateTime> endTimeStamps)
        {
            for ( int idx = beginIdx;  idx < endTimeStamps.Count;  idx++)
            {
                if (endT <= endTimeStamps[idx])
                {
                    return idx;
                }
            }
            // didn't find the timestamp that is later than endT
            return endTimeStamps.Count - 1;
            //return null;
        }
        public void Clean()
        {
            Signals.Clear();
            StartTimeStamps.Clear();
            EndTimeStamps.Clear();
            TimePairs.Clear();
            FirstFile = true;
            DataCompleted = false;
            FinishedSignatures.Clear();
            _results.Clear();
            //FirstFileRead = false;
        }
        public void WriteResults()
        {
            if (!FirstFile)
            {
                CurrentTimeStamp = TimeZero;
            }
            else
            {
                Thread.Sleep(1000);
                return;
            }
            NextTimeStamp = _getNextWriteOutTime(CurrentTimeStamp, DatawriteOutFrequency, DatawriteOutFrequencyUnit);
            while (true)
            {
                var timeSpan = NextTimeStamp - CurrentTimeStamp;
                var expectedRows = (timeSpan.TotalSeconds - WindowSize) / (WindowSize - WindowOverlap) + 1;
                List<DateTime> timeStampsInRange, timeStampsAfter;
                lock (_theResultsLock)
                {
                    timeStampsInRange = _results.Keys.Where(x => x >= CurrentTimeStamp && x < NextTimeStamp).ToList();
                    timeStampsAfter = _results.Keys.Where(x => x >= NextTimeStamp).ToList();
                }
                var rows = timeStampsInRange.Count();

                if (rows == expectedRows)
                {
                    _writeASignatureOutput(timeStampsInRange);
                    _removeDataWrittenOut(timeStampsInRange);
                    CurrentTimeStamp = NextTimeStamp;
                    NextTimeStamp = _getNextWriteOutTime(CurrentTimeStamp, DatawriteOutFrequency, DatawriteOutFrequencyUnit);
                }
                else if (NumberOfSignatures == FinishedSignatures.Count)
                {
                    if (timeStampsAfter.Count() > 0)
                    {
                        if (timeStampsInRange.Count() > 0)
                        {
                            _writeASignatureOutput(timeStampsInRange);
                            _removeDataWrittenOut(timeStampsInRange);
                        }
                        CurrentTimeStamp = NextTimeStamp;
                        NextTimeStamp = _getNextWriteOutTime(CurrentTimeStamp, DatawriteOutFrequency, DatawriteOutFrequencyUnit);
                    }
                    else
                    {
                        if (timeStampsInRange.Count() > 0)
                        {
                            _writeASignatureOutput(timeStampsInRange);
                            _removeDataWrittenOut(timeStampsInRange);
                        }
                        break;
                    }
                }
                else
                {
                    if (timeStampsAfter.Count() > 0)
                    {
                        if (timeStampsInRange.Count() > 0)
                        {
                            _writeASignatureOutput(timeStampsInRange);
                            _removeDataWrittenOut(timeStampsInRange);
                        }
                        CurrentTimeStamp = NextTimeStamp;
                        NextTimeStamp = _getNextWriteOutTime(CurrentTimeStamp, DatawriteOutFrequency, DatawriteOutFrequencyUnit);
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
            }
        }

        private void _writeASignatureOutput(List<DateTime> timeStampsInRange)
        {
            var filename = SignatureOutputDir + "//Signature_" + CurrentTimeStamp.ToString("yyyyMMdd_HHmmss") + ".csv";
            List<string> titleRow = new List<string> { "Time", "Time" };
            List<string> PMURow = new List<string> { "Time", "Time" };
            List<string> SignalRow = new List<string> { "Start Time", "End Time" };
            bool firstTimeStamp = true;
            timeStampsInRange.Sort();
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                foreach (var time in timeStampsInRange)
                {
                    ConcurrentBag<SignatureResult> signatures;
                    lock (_theResultsLock)
                    {
                        signatures = _results[time];
                    }
                    while (signatures.Count < NumberOfColumns)
                    {
                        Thread.Sleep(500);
                        lock (_theResultsLock)
                        {
                            signatures = _results[time];
                        }
                    }
                    var endTime = signatures.FirstOrDefault().EndTimestamp;
                    List<string> thisRow = new List<string> { time.ToString("yyyyMMdd_HHmmss.ffffff"), endTime.ToString("yyyyMMdd_HHmmss.ffffff") };

                    //if (signatures.Count > NumberOfColumns)
                    //{
                    //    var keep = signatures.GroupBy(x => x.EndTimestamp - x.TimeStamp).OrderByDescending(g => g.Key).First();
                    //    signatures;
                    //    foreach (var item in keep)
                    //    {
                    //        signatures.Add(item);
                    //    }
                    //}
                    var groupedTitle = signatures.GroupBy(x => x.Title).OrderBy(x => x.Key);
                    foreach (var group in groupedTitle)
                    {
                        var title = group.Key;
                        var groupedByPMU = group.GroupBy(x => x.PMUName).OrderBy(x => x.Key);
                        foreach (var group2 in groupedByPMU)
                        {
                            var pmu = group2.Key;
                            var orderedByName = group2.OrderBy(x => x.SignalName);
                            foreach (var sig in orderedByName)
                            {
                                if (firstTimeStamp)
                                {
                                    titleRow.Add(title);
                                    PMURow.Add(pmu);
                                    SignalRow.Add(sig.SignalName);
                                }
                                thisRow.Add(sig.Value.ToString());
                            }
                        }
                    }
                    if (firstTimeStamp)
                    {
                        outputFile.WriteLine(String.Join(",", titleRow));
                        outputFile.WriteLine(String.Join(",", PMURow));
                        outputFile.WriteLine(String.Join(",", SignalRow));
                    }
                    outputFile.WriteLine(String.Join(",", thisRow));
                    firstTimeStamp = false;
                }
            }
        }

        private void _removeDataWrittenOut(List<DateTime> timeStampsInRange)
        {
            foreach (var st in timeStampsInRange)
            {
                if (_results.ContainsKey(st))
                {
                    ConcurrentBag<SignatureResult> value = null;
                    lock (_theResultsLock)
                    {
                        _results.TryRemove(st, out value);
                    }
                }
            }
            var lastTimePoint = timeStampsInRange.LastOrDefault();
            foreach (var sig in Signals)
            {
                var timeDict = sig.Value;
                var tss = timeDict.Keys.Where(x => x < lastTimePoint).ToList();
                for (int i = tss.Count() - 1; i >= 0; i--)
                {
                    var ts = tss[i];
                    if (timeDict.ContainsKey(ts))
                    {
                        lock (_theInputSignalsLock)
                        {
                            timeDict.Remove(ts);
                        }
                        lock (_theEndTimeStampsLock)
                        {
                            EndTimeStamps.Remove(ts);
                        }
                    }
                }
            }
        }

        private ConcurrentDictionary<DateTime, ConcurrentBag<SignatureResult>> _results = new ConcurrentDictionary<DateTime, ConcurrentBag<SignatureResult>>();
        public void AddResults(DateTime timeStamp, string title, string pMUName, string signalName, double value, DateTime lastTimeStamp)
        {
            lock (_theResultsLock)
            {
                if (!_results.ContainsKey(timeStamp))
                {
                    _results[timeStamp] = new ConcurrentBag<SignatureResult>();
                }
                var item = _results[timeStamp].Where(x => x.Title == title && x.PMUName == pMUName && x.SignalName == signalName);
                if (item.Any())
                {
                    var tsp = item.First().EndTimestamp - item.First().TimeStamp;
                    if (tsp < lastTimeStamp - timeStamp)
                    {
                        item.First().TimeStamp = timeStamp;
                        item.First().EndTimestamp = lastTimeStamp;
                        item.First().Value = value;
                    }
                }
                else
                {
                    _results[timeStamp].Add(new SignatureResult(timeStamp, lastTimeStamp, title, pMUName, signalName, value));
                }
            }
        }

    }
    public class SignatureResult
    {
        public SignatureResult(DateTime timeStamp, DateTime endTtimeStamp, string title, string pMUName, string signalName, double value)
        {
            TimeStamp = timeStamp;
            Title = title;
            PMUName = pMUName;
            SignalName = signalName;
            Value = value;
            EndTimestamp = endTtimeStamp;
        }

        public string Title { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public double Value { get; set; }
        public DateTime EndTimestamp { get; set; }
    }
}
