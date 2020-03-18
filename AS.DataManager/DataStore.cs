using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    }
                    else
                    {
                        thisSig.Data = firstSig.Data.GetRange(firstDataPoint, lastDataPoint - firstDataPoint + 1);
                        thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, lastDataPoint - firstDataPoint + 1);
                    }
                }
                else
                {
                    // if need to put several fragment together, find the first partial piece first
                    thisSig.Data = firstSig.Data.GetRange(firstDataPoint, firstSig.Data.Count - firstDataPoint);
                    thisSig.Flags = firstSig.Flags.GetRange(firstDataPoint, firstSig.Flags.Count - firstDataPoint);
                    // if there are middle pieces, add the middle pieces which should be whole pieces
                    for (int ii = 1; ii < possibleTimeStamps.Count; ii++)
                    {
                        thisSig.Data.AddRange(sig[possibleTimeStamps[ii]].Data);
                        thisSig.Flags.AddRange(sig[possibleTimeStamps[ii]].Flags);
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
        }
    }
}
