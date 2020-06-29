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

        public static void OutlierFilt(Signal s, double stdDevMult)
        {
            var median = SignatureCalculations.Median(s.Data);
            var std = SignatureCalculations.Stdev(s.Data);
            for (int i = 0; i < s.Data.Count; i++)
            {
                if (Math.Abs(s.Data[i] - median) > stdDevMult * std)
                {
                    s.Flags[i] = false;
                }
            } 
        }

        public static void StaleDQFilt(Signal s, int staleThresh)
        {
            var staleCount = 1;
            int staleStartIdx = 0;
            if (s.Data.Count > 0)
            {
                double currentNumber = s.Data[0];
                for (int i = 1; i < s.Data.Count; i++)
                {
                    if (currentNumber == s.Data[i])
                    {
                        staleCount++;
                    }
                    else
                    {
                        if (staleCount >= staleThresh)
                        {
                            for (int ii = staleStartIdx; ii < i; ii++)
                            {
                                s.Flags[ii] = false;
                            }
                        }
                        currentNumber = s.Data[i];
                        staleCount = 1;
                        staleStartIdx = i;
                    }
                }
                if (staleCount >= staleThresh)
                {
                    for (int ii = staleStartIdx; ii < s.Data.Count; ii++)
                    {
                        s.Flags[ii] = false;
                    }
                }
            }
            else
            {
                double currentRNumber = s.ComplexData[0].Real;
                double currentINumber = s.ComplexData[0].Imaginary;
                for (int i = 1; i < s.ComplexData.Count; i++)
                {
                    if (currentRNumber == s.ComplexData[i].Real && currentINumber == s.ComplexData[i].Imaginary)
                    {
                        staleCount++;
                    }
                    else
                    {
                        if (staleCount >= staleThresh)
                        {
                            for (int ii = staleStartIdx; ii < i; ii++)
                            {
                                s.Flags[ii] = false;
                            }
                        }
                        currentRNumber = s.ComplexData[i].Real;
                        currentINumber = s.ComplexData[i].Imaginary;
                        staleCount = 1;
                        staleStartIdx = i;
                    }
                }
                if (staleCount >= staleThresh)
                {
                    for (int ii = staleStartIdx; ii < s.ComplexData.Count; ii++)
                    {
                        s.Flags[ii] = false;
                    }
                }
            }

        }
        public static void DataFrameDQFilt(List<Signal> ss, int percentBadThresh)
        {
            int signalLength = ss.FirstOrDefault().Flags.Count;
            //if (ss.FirstOrDefault().ComplexData.Count > 0)
            //{
            //    signalLength = ss.FirstOrDefault().ComplexData.Count;
            //}
            var signalCount = ss.Count();
            for (int i = 0; i < signalLength; i++)
            {
                var nanN = 0;
                for (int ii = 0; ii < signalCount; ii++)
                {
                    if (!ss[ii].Flags[i])
                    {
                        nanN++;
                    }
                }
                if ((double)nanN / (double)signalCount * 100 >= percentBadThresh)
                {
                    for (int ii = 0; ii < signalCount; ii++)
                    {
                        ss[ii].Flags[i] = false;
                    }
                }
            }
        }

        public static void PMUchanDQFilt(Signal s, int percentBadThresh)
        {
            //if (s.Data.Count > 0)
            //{
            var nanN = 0;
            var signalCount = s.Flags.Count;
            for (int i = 0; i < signalCount; i++)
             {
                if (!s.Flags[i])
                {
                    nanN++;
                }
            }
            if ((double)nanN / (double)signalCount * 100 >= percentBadThresh)
            {
                for (int i = 0; i < signalCount; i++)
                {
                    s.Flags[i] = false;
                }
            }
            //}
            //else
            //{
            //    for (int i = 0; i < s.ComplexData.Count; i++)
            //    {

            //    }
            //}
        }

        public static void PMUallDQFilt(List<Signal> ss, int percentBadThresh)
        {
            int signalLength = ss.FirstOrDefault().Flags.Count;
            var signalCount = ss.Count();
            var nanN = 0;
            for (int i = 0; i < signalLength; i++)
            {
                for (int ii = 0; ii < signalCount; ii++)
                {
                    if (!ss[ii].Flags[i])
                    {
                        nanN++;
                    }
                }
            }
            if ((double)nanN / (double)signalCount / (double) signalLength * 100 >= percentBadThresh)
            {
                for (int i = 0; i < signalLength; i++)
                {
                    for (int ii = 0; ii < signalCount; ii++)
                    {
                        ss[ii].Flags[i] = false;
                    }
                }
            }
        }

        //public static void WrappingFailureDQFilt(Signal s, string angleThresh)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
