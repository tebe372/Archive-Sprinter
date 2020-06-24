using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AS.ComputationManager.Calculations
{
    public static class Filters
    {
        public static void DropOutMissingFilt(Signal s)
        {
            if (s.Data.Count() > 0)
            {
                for (int idx = 0; idx < s.Data.Count; idx++)
                {
                    if (double.IsNaN(s.Data[idx]))
                    {
                        s.Flags[idx] = false;
                    }
                }
            }
            else
            {
                for (int idx = 0; idx < s.ComplexData.Count; idx++)
                {
                    if (s.ComplexData[idx] == null)
                    {
                        s.Flags[idx] = false;
                    }
                }
            }
        }

        public static void DropOutZeroFilt(Signal s)
        {
            if (s.Data.Count() > 0)
            {
                for (int idx = 0; idx < s.Data.Count; idx++)
                {
                    if (s.Data[idx] < 1e-15)
                    {
                        s.Flags[idx] = false;
                    }
                }
            }
            else
            {
                for (int idx = 0; idx < s.ComplexData.Count; idx++)
                {
                    if (Complex.Abs(s.ComplexData[idx]) == 0)
                    {
                        s.Flags[idx] = false;
                    }
                }
            }
        }

        public static void PMUflagFilt(Signal s)
        {
            for (int idx = 0; idx < s.Data.Count; idx++)
            {
                if (s.Stat[idx] >= 4096)
                {
                    s.Flags[idx] = false;
                }
            }
        }

        public static void VoltPhasorFilt(Signal s, string type, double voltMax, double voltMin, double nomVoltage)
        {
            if (s.Unit == "V")
            {
                nomVoltage = nomVoltage * 1000;
            }
            if (type == "VP")
            {
                for (int i = 0; i < s.ComplexData.Count; i++)
                {
                    // need to write a ABS function that would work for both complex number and double
                    var v = Complex.Abs(s.ComplexData[i]);
                    if (v > voltMax * nomVoltage || v < voltMin * nomVoltage)
                    {
                        s.Flags[i] = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < s.Data.Count; i++)
                {
                    // need to write a ABS function that would work for both complex number and double
                    //var v = _abs(s.Data[i]);
                    var v = s.Data[i];
                    if (v > voltMax * nomVoltage || v < voltMin * nomVoltage)
                    {
                        s.Flags[i] = false;
                    }
                }
            }
        }

        //private static double _abs(double v)
        //{
        //    if (v >= 0)
        //    {
        //        return v;
        //    }
        //    else
        //    {
        //        return -v;
        //    }
        //}

        //private static double _abs(Complex v)
        //{
        //    return Complex.Abs(v);
        //}

        public static void FreqFilt(Signal s, double freqMaxChan, double freqMinChan, double freqPctChan, double freqMinSamp, double freqMaxSamp)
        {
            int outCount = 0;
            for (int i = 0; i < s.Data.Count; i++)
            {
                var v = s.Data[i];
                if (v > freqMaxChan || v < freqMinChan)
                {
                    outCount++;
                }
                if (v > freqMaxSamp || v < freqMinSamp)
                {
                    s.Flags[i] = false;
                }
            }
            //Console.WriteLine(outCount / s.Data.Count * 100);
            if (outCount / (double)s.Data.Count * 100 > freqPctChan)
            {
                s.Flags = s.Flags.Select(x => x = false).ToList();
            }
        }

        public static void OutlierFilt(Signal signal, double stdDevMult)
        {
            throw new NotImplementedException();
        }

        public static void StaleDQFilt(Signal signal, string staleThresh, bool flagAllByFreq)
        {
            throw new NotImplementedException();
        }

        public static void DataFrameDQFilt(Signal signal, string percentBadThresh)
        {
            throw new NotImplementedException();
        }

        public static void PMUchanDQFilt(Signal signal, string percentBadThresh)
        {
            throw new NotImplementedException();
        }

        public static void PMUallDQFilt(Signal signal, string percentBadThresh)
        {
            throw new NotImplementedException();
        }

        public static void WrappingFailureDQFilt(Signal signal, string angleThresh)
        {
            throw new NotImplementedException();
        }
    }
}
