using AS.Core.Models;
using System;
using MathNet.Numerics.Statistics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace AS.ComputationManager.Calculations
{
    public static class SignatureCalculations
    {
        public static double Mean(List<double> data)
        {
            /// mean of data
            /// return a single double value
            /// Returns NaN if data is empty or if any entry is NaN
            return Statistics.Mean(data);
        }

        public static double Median(List<double> data)
        {
            /// median of data, the second quartile, the 50th percentile
            /// return a single double value
            /// Returns NaN if data is empty or if any entry is NaN
            return Statistics.Median(data);
        }

        public static double Minimum(List<double> data)
        {
            /// smallest of data
            /// return a single double value
            /// Returns NaN if data is empty or if any entry is NaN
            return Statistics.Minimum(data);
        }

        public static double Maximum(List<double> data)
        {
            /// largest of data
            /// return a single double value
            /// Returns NaN if data is empty or if any entry is NaN
            return Statistics.Maximum(data);
        }

        public static double Range(List<double> data)
        {
            /// difference between largest and smallest values in data
            /// return a single double value
            /// Returns NaN if data is empty or if any entry is NaN
            return Statistics.Maximum(data) - Statistics.Minimum(data);
        }

        public static double Rise(List<double> data)
        {
            /// difference between first and last sample in data
            /// return a single double value
            /// Returns NaN if data is empty or if any entry is NaN
            return data[data.Count - 1] - data[0];
        }

        public static double Variance(List<double> data)
        {
            /// variance of data
            /// return a single double value
            /// Returns NaN if data has less than two entries or if any entry is NaN
            return Statistics.Variance(data);
        }

        public static double Stdev(List<double> data)
        {
            /// standard deviation of data
            /// return a single double value
            /// Returns NaN if data has less than two entries or if any entry is NaN
            return Statistics.StandardDeviation(data);
        }

        public static double Kurtosis(List<double> data)
        {
            /// kurtosis of data (a measure of the tailedness - the sharpness of the peak - of the pdf)
            /// return a single double value
            /// Returns NaN if data has less than four entries or if any entry is NaN
            return Statistics.Kurtosis(data);
        }

        public static double Skewness(List<double> data)
        {
            /// skewness of data (a measure of the asymmetry of the pdf about its mean)
            /// return a single double value
            /// Returns NaN if data has less than three entries or if any entry is NaN
            return Statistics.Skewness(data);
        }
        public static double CorrelationCoeff(List<double> dataA, List<double> dataB)
        {
            /// Computes the Pearson Product-Moment Correlation coefficient between data A and B
            /// return a single double value
            return Correlation.Pearson(dataA, dataB);
        }

        public static double Covariance(List<double> dataA, List<double> dataB)
        {
            /// Computes covairnce between data A and B
            /// return a single double value
            /// Returns NaN if data has less than two entries or if any entry is NaN
            return Statistics.Covariance(dataA, dataB);
        }

        public static double Percentile(List<double> data, int p)
        {
            /// Sample percentile (The xth percentile is the value that is greater than x% of the measurements)
            /// Return: a single double value
            /// int p: input percentile [0 100]
            /// Note: Using p = 25,50,75 for quartiles Q1, Q2, Q3
            return Statistics.Percentile(data, p);
        }

        public static List<List<double>> Hist(List<double> fulldata, int B, double minValue, double maxValue)
        {
            /// Counts the number of values over ranges
            /// int B: number of bins
            /// double minValue: lower bound
            /// double maxValue: upper bound
            var data = from i in fulldata
                       where i >= minValue & i <= maxValue
                       select i;
            var H = new Histogram(data, B, minValue, maxValue);
            var output = new List<List<double>>();
            output.Add(new List<double>());
            output.Add(new List<double>());
            for (int i = 0; i < H.BucketCount; i++)
            {
                output[0].Add((H[i].UpperBound + H[i].LowerBound)/2); /// Center of Bins
                output[1].Add(H[i].Count);                            /// Value(count) of Bins

            }
            return output;
        }

        public static double RootMeanSquare(List<double> data, bool removeMean)
        {
            if (removeMean)
            {
                return RMSNormalized(data);
            }
            else
            {
                return RMS(data);
            }
        }

        public static double RMS(List<double> data)
        {
            var n = data.Count;
            if (n == 0)
            {
                return double.NaN;
            }

            double total = 0;
            for (int i = 0; i < n; i++)
            {
                total += data[i] * data[i];
            }

            return Math.Sqrt(total / n);
        }

        public static double RMSNormalized(List<double> data)
        {
            var n = data.Count;
            if (n == 0)
            {
                return double.NaN;
            }

            double mean = data.Mean();
            double total = 0;
            for (int i = 0; i < n; i++)
            {
                var r = data[i] - mean;
                total += r * r;
            }

            return Math.Sqrt(total / n);
        }

        public static Dictionary<string, double> FrequencyBandRMS(List<double> data, int samplingRate, bool calculateFull, bool calculateBand2, bool calculateBand3, bool calculateBand4)
        {
            var result = new Dictionary<string, double>(); // might need to be a dictionary

            var signalLen = data.Count;
            var mean = data.Mean();
            var complex = new Complex[signalLen];
            for (int i = 0; i < signalLen; i++)
            {
                complex[i] = new Complex(data[i] - mean, 0);
            }
            Fourier.Forward(complex, FourierOptions.Matlab);
            var Pk = new List<double>();
            var fk = new List<double>();
            var band2 = new List<double>();
            var band3 = new List<double>();
            var band4 = new List<double>();
            for (int i = 0; i < signalLen; i++)
            {
                Pk.Add(Math.Pow(complex[i].Magnitude, 2) / signalLen);
                var fki = i / signalLen * samplingRate;
                fk.Add(fki);
                if (fki >= 0.1 && fki <= 1)
                {
                    band2.Add(fki);
                }
                else if (fki >= 1 && fki <= 5)
                {
                    band3.Add(fki);
                }
                else if (fki >= 5 && fki <= samplingRate / 2)
                {
                    band4.Add(fki);
                }
            }
            if (calculateFull)
            {
                result["Full"] = Math.Sqrt(Pk.Sum() / signalLen);
            }
            if (calculateBand2)
            {
                result["Band2"] = Math.Sqrt(band2.Sum() / signalLen * 2);
            }
            if (calculateBand3)
            {
                result["Band3"] = Math.Sqrt(band3.Sum() / signalLen * 2);
            }
            if (calculateBand4)
            {
                result["Band4"] = Math.Sqrt(band4.Sum() / signalLen * 2);
            }
            return result;
        }
    }
}
