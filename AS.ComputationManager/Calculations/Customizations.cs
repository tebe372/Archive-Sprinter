using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AS.ComputationManager.Calculations
{
    public static class Customizations
    {
        public static Signal ScalarRepCustomization(Signal signal, double scalar, int signalLength, DateTime t)
        {
            double sp = signal.SamplingRate;
            double[] rep = new double[signalLength];
            DateTime[] timeStamp = new DateTime[signalLength];
            double[] timeStampNumber = new double[signalLength];
            for (int i = 0; i < signalLength; i++)
            {
                rep[i] = scalar;
                var ss = i * 1.0 / sp;
                var ts = t + TimeSpan.FromSeconds(ss);
                timeStamp[i] = ts;
                timeStampNumber[i] = ts.ToOADate();
            }
            signal.Data = rep.ToList();
            signal.TimeStampNumber = timeStampNumber.ToList();
            signal.TimeStamps = timeStamp.ToList();
            return signal;
        }

        public static List<double> AdditionCustomization(List<double> data1, List<double> data2)
        {
            var result = new List<double>();
            for (int i = 0; i < data1.Count; i++)
            {
                if (double.IsNaN(data1[i]) || double.IsNaN(data2[i]))
                {
                    result.Add(double.NaN);
                }
                else
                {
                    result.Add(data1[i] + data2[i]);
                }
            }
            return result;
        }

        public static List<double> SubtractionCustomization(List<double> subtrahend, List<double> minuend)
        {
            //process the subtraction and return the new signal.
            var result = new List<double>();
            for (int i = 0; i < subtrahend.Count; i++)
            {
                if (double.IsNaN(minuend[i]) || double.IsNaN(subtrahend[i]))
                {
                    result.Add(double.NaN);
                }
                else
                {
                    result.Add(minuend[i] - subtrahend[i]);
                }
            }
            return result;
        }

        public static List<double> MultiplicationCustomization(List<double> data1, List<double> data2)
        {
            var result = new List<double>();
            for (int i = 0; i < data1.Count; i++)
            {
                if (double.IsNaN(data1[i]) || double.IsNaN(data2[i]))
                {
                    result.Add(double.NaN);
                }
                else
                {
                    result.Add(data1[i] * data2[i]);
                }
            }
            return result;
        }

        public static List<double> DivisionCustomization(List<double> dividend, List<double> divisor)
        {
            var result = new List<double>();
            for (int i = 0; i < dividend.Count; i++)
            {
                if (double.IsNaN(dividend[i]) || double.IsNaN(divisor[i]) || divisor[i] == 0)
                {
                    result.Add(double.NaN);
                }
                else
                {
                    result.Add(dividend[i] / divisor[i]);
                }
            }
            return result;
        }

        public static List<double> ExpCustomization(List<double> data, double exp)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(Math.Pow(data[i], exp));
            }
            return result;
        }

        public static List<double> SignReversalCustomization(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(-data[i]);
            }
            return result;
        }

        public static List<Complex> SignReversalCustomization(List<Complex> data)
        {
            var result = new List<Complex>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(-data[i]);
            }
            return result;
        }

        public static List<double> AbsValCustomization(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(Math.Abs(data[i]));
            }
            return result;
        }

        public static List<double> AbsValCustomization(List<Complex> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(Complex.Abs(data[i]));
            }
            return result;
        }

        public static List<double> RealComponentCustomization(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i]);
            }
            return result;
        }

        public static List<double> RealComponentCustomization(List<Complex> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i].Real);
            }
            return result;
        }

        public static List<double> ImagComponentCustomization(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(0);
            }
            return result;
        }

        public static List<double> ImagComponentCustomization(List<Complex> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i].Imaginary);
            }
            return result;
        }

        public static List<double> AngleCalCustomization(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(0);
            }
            return result;
        }

        public static List<double> AngleCalCustomization(List<Complex> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(Math.Atan2(data[i].Imaginary, data[i].Real));
            }
            return result;
        }

        public static List<double> ComplexConjugateCustomization(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i]);
            }
            return result;
        }
        public static List<Complex> ComplexConjugateCustomization(List<Complex> data)
        {
            var result = new List<Complex>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(Complex.Conjugate(data[i]));
            }
            return result;
        }

        public static List<Complex> CreatePhasorCustomization(List<double> mag, List<double> ang)
        {
            List<Complex> result = new List<Complex>();
            for (int i = 0; i < mag.Count; i++)
            {
                result.Add(mag[i] * Complex.Exp(Complex.ImaginaryOne * ang[i]));
            }
            return result;
        }

        public static List<Complex> PowerFromPhasorCustomization(List<Complex> vp, List<Complex> ip)
        {
            var result = new List<Complex>();
            for (int i = 0; i < vp.Count; i++)
            {
                result.Add(3 * vp[i] * Complex.Conjugate(ip[i]));
            }
            return result;
        }

        public static List<double> MetricPrefixCustomization(List<double> data, string inputUnit, string outputUnit)
        {
            //var tt = 1 * double.NaN;
            //Console.WriteLine("1 * double.NaN = {0}", tt);
            double coeff = _getMetricPrefixCoefficient(inputUnit, outputUnit);
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i] * coeff);
            }
            return result;
        }

        public static List<Complex> MetricPrefixCustomization(List<Complex> data, string inputUnit, string outputUnit)
        {
            //var tt = (1 + Complex.ImaginaryOne) * double.NaN;
            //Console.WriteLine("1 + i * double.NaN = {0}", tt);
            double coeff = _getMetricPrefixCoefficient(inputUnit, outputUnit);
            var result = new List<Complex>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i] * coeff);
            }
            return result;
        }

        private static double _getMetricPrefixCoefficient(string inputUnit, string outputUnit)
        {
            switch (inputUnit)
            {
                case "V":
                    switch (outputUnit)
                    {
                        case "V":
                            return 1;
                        case "kV":
                            return 0.001;
                        default:
                            return double.NaN;
                    }
                case "kV":
                    switch (outputUnit)
                    {
                        case "V":
                            return 1000;
                        case "kV":
                            return 1;
                        default:
                            return double.NaN;
                    }
                case "A":
                    switch (outputUnit)
                    {
                        case "A":
                            return 1;
                        case "kA":
                            return 0.001;
                        default:
                            return double.NaN;
                    }
                case "kA":
                    switch (outputUnit)
                    {
                        case "A":
                            return 1000;
                        case "kA":
                            return 1;
                        default:
                            return double.NaN;
                    }
                case "W":
                    switch (outputUnit)
                    {
                        case "W":
                            return 1;
                        case "kW":
                            return 0.001;
                        case "MW":
                            return 0.000001;
                        default:
                            return double.NaN;
                    }
                case "kW":
                    switch (outputUnit)
                    {
                        case "W":
                            return 1000;
                        case "kW":
                            return 1;
                        case "MW":
                            return 0.001;
                        default:
                            return double.NaN;
                    }
                case "MW":
                    switch (outputUnit)
                    {
                        case "W":
                            return 1000000;
                        case "kW":
                            return 1000;
                        case "MW":
                            return 1;
                        default:
                            return double.NaN;
                    }
                case "VAR":
                    switch (outputUnit)
                    {
                        case "VAR":
                            return 1;
                        case "kVAR":
                            return 0.001;
                        case "MVAR":
                            return 0.000001;
                        default:
                            return double.NaN;
                    }
                case "kVAR":
                    switch (outputUnit)
                    {
                        case "VAR":
                            return 1000;
                        case "kVAR":
                            return 1;
                        case "MVAR":
                            return 0.001;
                        default:
                            return double.NaN;
                    }
                case "MVAR":
                    switch (outputUnit)
                    {
                        case "VAR":
                            return 1000000;
                        case "kVAR":
                            return 1000;
                        case "MVAR":
                            return 1;
                        default:
                            return double.NaN;
                    }
                case "VA":
                    switch (outputUnit)
                    {
                        case "VA":
                            return 1;
                        case "kVA":
                            return 0.001;
                        case "MVA":
                            return 0.000001;
                        default:
                            return double.NaN;
                    }
                case "kVA":
                    switch (outputUnit)
                    {
                        case "VA":
                            return 1000;
                        case "kVA":
                            return 1;
                        case "MVA":
                            return 0.001;
                        default:
                            return double.NaN;
                    }
                case "MVA":
                    switch (outputUnit)
                    {
                        case "VA":
                            return 1000000;
                        case "kVA":
                            return 1000;
                        case "MVA":
                            return 1;
                        default:
                            return double.NaN;
                    }
                case "Hz":
                    switch (outputUnit)
                    {
                        case "Hz":
                            return 1;
                        case "mHz":
                            return 1000;
                        default:
                            return double.NaN;
                    }
                case "mHz":
                    switch (outputUnit)
                    {
                        case "Hz":
                            return 0.001;
                        case "mHz":
                            return 1;
                        default:
                            return double.NaN;
                    }
                case "Hz/sec":
                    switch (outputUnit)
                    {
                        case "Hz/sec":
                            return 1;
                        case "mHz/sec":
                            return 1000;
                        default:
                            return double.NaN;
                    }
                case "mHz/sec":
                    switch (outputUnit)
                    {
                        case "Hz/sec":
                            return 0.001;
                        case "mHz/sec":
                            return 1;
                        default:
                            return double.NaN;
                    }
                default:
                    return double.NaN;
            }
        }

        public static List<double> AngleUnitConversionCustomizationForRad(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i] * 180 / Math.PI);
            }
            return result;
        }

        public static List<double> AngleUnitConversionCustomizationForDeg(List<double> data)
        {
            var result = new List<double>();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(data[i] * Math.PI / 180);
            }
            return result;
        }
    }
}
