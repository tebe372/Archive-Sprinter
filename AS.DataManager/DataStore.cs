using AS.Core.Models;
using System;
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

        private DateTime _getNextWriteOutTime(DateTime current, int interval, string unit)
        {
            switch (unit)
            {
                case "Hours":
                    return current.AddHours(interval);
                case "Days":
                    return current.AddDays(interval);
                case "Weeks":
                    return current.AddDays(interval * 7);
                case "Months":
                    return current.AddMonths(interval);
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
                    FirstFile = false;
                }
                else if (TimeZero >= startT)
                {
                    throw new Exception("Possible data source files time order problem.");
                }
                StartTimeStamps.Add(startT);
                var endT = e.FirstOrDefault().TimeStamps.LastOrDefault();
                EndTimeStamps.Add(endT);
                TimePairs[endT] = startT;
                foreach (var sig in e)
                {
                    var name = sig.PMUName + "_" + sig.SignalName;
                    if (!Signals.ContainsKey(name))
                    {
                        Signals[name] = new Dictionary<DateTime, Signal>();
                    }
                    Signals[name][endT] = sig;
                }
            }
        }

        public bool GetData(List<Signal> signals, DateTime startT, DateTime endT, int windowSize, List<string> signalNames)
        {
            bool foundStart = false;
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
            var possibleTimeStamps = EndTimeStamps.GetRange(i1, i2 - i1 + 1);
            foreach (var name in signalNames)
            {

                var sig = Signals[name];
                var firstSig = sig[possibleTimeStamps[0]];
                var thisSig = new Signal(firstSig.PMUName, firstSig.SignalName);
                var firstDataPoint = firstSig.TimeStamps.IndexOf(startT);
                if (firstDataPoint == -1)
                {
                    return false;
                }
                if (possibleTimeStamps.Count == 1)
                {
                    // if here's only 1 fragment, find the start and end point and get data between that range
                    var lastDataPoint = firstSig.TimeStamps.IndexOf(endT);
                    if (lastDataPoint == -1)
                    {
                        //thisSig.Data = firstSig.Data;
                        thisSig.Data = firstSig.Data.GetRange(firstDataPoint, firstSig.Data.Count - firstDataPoint);
                        thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, firstSig.Flags.Count - firstDataPoint);
                        thisSig.TimeStamps = firstSig.TimeStamps.GetRange(firstDataPoint, firstSig.TimeStamps.Count - firstDataPoint);
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
                if (thisSig.Data.Count < windowSize)
                {
                    return false;
                }
                else
                {
                    thisSig.Data.RemoveRange(windowSize, thisSig.Data.Count - windowSize);
                    thisSig.Flags.RemoveRange(windowSize, thisSig.Flags.Count - windowSize);
                    thisSig.TimeStamps.RemoveRange(windowSize, thisSig.TimeStamps.Count - windowSize);
                    signals.Add(thisSig);
                }
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
        }
        public void WriteResults()
        {
            CurrentTimeStamp = TimeZero;
            NextTimeStamp = _getNextWriteOutTime(CurrentTimeStamp, DatawriteOutFrequency, DatawriteOutFrequencyUnit);
            while (true)
            {
                var timeSpan = NextTimeStamp - CurrentTimeStamp;
                var expectedRows = (timeSpan.TotalSeconds - WindowSize) / (WindowSize - WindowOverlap) + 1;
                var timeStampsInRange = _results.Keys.Where(x => x >= CurrentTimeStamp && x < NextTimeStamp);
                var rows = timeStampsInRange.Count();
                if (NumberOfSignatures == FinishedSignatures.Count)
                {
                    var filename = "Signature_" + CurrentTimeStamp.ToString("yyyyMMdd_HHmmss") + ".csv";
                    List<string> titleRow = new List<string> { "Time", "Time" };
                    List<string> PMURow = new List<string> { "Time", "Time" };
                    List<string> SignalRow = new List<string> { "Start Time", "End Time" };
                    bool firstTimeStamp = true;
                    using (StreamWriter outputFile = new StreamWriter(filename))
                    {
                        foreach (var time in timeStampsInRange)
                        {
                            var signatures = _results[time];
                            var endTime = signatures.FirstOrDefault().EndTimestamp;
                            List<string> thisRow = new List<string> { time.ToString("yyyyMMdd_HHmmss.ffffff"), endTime.ToString("yyyyMMdd_HHmmss.ffffff") };
                            if (signatures.Count == NumberOfColumns)
                            {
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
                    break;
                }
                else if (rows == expectedRows)
                {

                    CurrentTimeStamp = NextTimeStamp;
                    NextTimeStamp = _getNextWriteOutTime(CurrentTimeStamp, DatawriteOutFrequency, DatawriteOutFrequencyUnit);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }
        private Dictionary<DateTime, List<SignatureResult>> _results = new Dictionary<DateTime, List<SignatureResult>>();
        public void AddResults(DateTime timeStamp, string title, string pMUName, string signalName, double value, DateTime lastTimeStamp)
        {
            if (!_results.ContainsKey(timeStamp))
            {
                _results[timeStamp] = new List<SignatureResult>();
            }
            _results[timeStamp].Add(new SignatureResult(timeStamp, lastTimeStamp, title, pMUName, signalName, value));
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
