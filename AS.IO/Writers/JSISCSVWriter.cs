using AS.Core.Models;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AS.IO
{
    public class JSISCSVWriter
    {
        public void WriteJSISCSV(List<Signal> signals, string filename)
        {
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            DataToBeWritten data;
            try
            {
                data = _convertDataToBeWritten(signals);
            }
            catch (OutOfMemoryException oomEx)
            {
                throw oomEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine(String.Join(",", data.NameRowList));
                outputFile.WriteLine(String.Join(",", data.TypeRowList));
                outputFile.WriteLine(String.Join(",", data.UnitRowList));
                outputFile.WriteLine(String.Join(",", data.PMUList));
                for (int index = 0; index < data.Data.RowCount; index++)
                {
                    string str;
                    try
                    {
                        str = string.Join(",", data.Data.Row(index).ToArray());
                    }
                    catch (OutOfMemoryException oomEx)
                    {
                        throw oomEx;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    try
                    {
                        outputFile.WriteLine(str);
                    }
                    catch (OutOfMemoryException oomEx)
                    {
                        throw oomEx;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        private DataToBeWritten _convertDataToBeWritten(List<Signal> signals)
        {
            DataToBeWritten data = new DataToBeWritten();
            if (signals.Count != 0)
            {
                var firstTimeStamp = DateTime.FromOADate(signals.FirstOrDefault().TimeStampNumber.FirstOrDefault()).ToString(@"yyyyMMdd_HHmmss");
                var firstDateTime = DateTime.ParseExact(firstTimeStamp, "yyyyMMdd_HHmmss", null, System.Globalization.DateTimeStyles.None).ToOADate();// - new DateTime(0001, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                var timeArr = signals.FirstOrDefault().TimeStampNumber.ToArray();
                //var firstTimeStamp2 = DateTime.FromOADate(InputSignals.FirstOrDefault().TimeStampNumber.FirstOrDefault()).ToString(@"yyyyMMdd_HHmmss.ffffff");
                //Data = Matrix<double>.Build.Dense(timeArr.Count(), 1, (i, j) => (double)i / (double)InputSignals.FirstOrDefault().SamplingRate);
                try
                {
                    data.Data = Matrix<double>.Build.Dense(timeArr.Count(), 1, (i, j) => (timeArr[i] - firstDateTime) * 86400);
                }
                catch (OutOfMemoryException oomEx)
                {
                    throw oomEx;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                var orderedSignal = signals.OrderBy(x => x.PMUName);
                foreach (var signal in orderedSignal)
                {
                    if (signal.Data.Count > 0)
                    {
                        try
                        {
                            data.NameRowList.Add(signal.SignalName);
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        try
                        {
                            data.TypeRowList.Add(_typeConverter(signal.TypeAbbreviation));
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        try
                        {
                            data.PMUList.Add(signal.PMUName);
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        Vector<double> dataInVector;
                        string unit;
                        try
                        {
                            _unitConverter(signal.Data, signal.Unit, out dataInVector, out unit);
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        try
                        {
                            data.UnitRowList.Add(unit);
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        try
                        {
                            data.Data = data.Data.InsertColumn(data.Data.ColumnCount, dataInVector);
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {
                        data.NameRowList.Add(signal.SignalName);
                        data.TypeRowList.Add(_typeConverter(signal.TypeAbbreviation));
                        data.PMUList.Add(signal.PMUName);
                        data.UnitRowList.Add("complexReal");
                        _splitComplexNumberReal(signal.ComplexData, out Vector<double> dataInVector);
                        data.Data = data.Data.InsertColumn(data.Data.ColumnCount, dataInVector);
                        data.NameRowList.Add(signal.SignalName);
                        data.TypeRowList.Add(_typeConverter(signal.TypeAbbreviation));
                        data.PMUList.Add(signal.PMUName);
                        data.UnitRowList.Add("complexImag");
                        _splitComplexNumberImag(signal.ComplexData, out Vector<double> dataInVectorImag);
                        data.Data = data.Data.InsertColumn(data.Data.ColumnCount, dataInVectorImag);
                    }
                }
            }
            return data;
        }

        private void _splitComplexNumberImag(List<Complex> complexData, out Vector<double> dataInVector)
        {
            dataInVector = Vector<double>.Build.Dense(complexData.Select(x => x.Imaginary).ToArray());
        }

        private void _splitComplexNumberReal(List<Complex> complexData, out Vector<double> dataInVector)
        {
            dataInVector = Vector<double>.Build.Dense(complexData.Select(x => x.Real).ToArray());
        }

        private void _unitConverter(List<double> data, string sgl, out Vector<double> convertedData, out string unit)
        {
            switch (sgl)
            {
                case "mHz":
                    convertedData = Vector<double>.Build.Dense(data.ToArray());
                    convertedData = convertedData / 1000;
                    unit = "Hz";
                    break;
                case "V":
                    convertedData = Vector<double>.Build.Dense(data.ToArray());
                    convertedData = convertedData / 1000;
                    unit = "kV";
                    break;
                case "RAD":
                    convertedData = Vector<double>.Build.Dense(data.ToArray());
                    convertedData = convertedData * 180 / Math.PI;
                    unit = "DEG";
                    break;
                default:
                    convertedData = Vector<double>.Build.Dense(data.ToArray());
                    unit = sgl;
                    break;
            }
        }
        private string _typeConverter(string type)
        {
            switch (type)
            {
                case "VMP":
                    //case "VMA":
                    //case "VMB":
                    //case "VMC":
                    return "VPM";
                case "VAP":
                    //case "VAA":
                    //case "VAB":
                    //case "VAC":
                    return "VPA";
                case "IMP":
                    //case "IMA":
                    //case "IMB":
                    //case "IMC":
                    return "IPM";
                case "IAP":
                    //case "IAA":
                    //case "IAB":
                    //case "IAC":
                    return "IPA";
                case "F":
                case "P":
                case "Q":
                    return type;
                default:
                    return "OTHER";
            }
        }
    }
    public class DataToBeWritten// : IDisposable
    {
        public DataToBeWritten()
        {
            NameRowList = new List<string>() { "Time" };
            TypeRowList = new List<string>() { "Type" };
            UnitRowList = new List<string>() { "Second" };
            PMUList = new List<string>() { "Time" };
        }
        public Matrix<double> Data;
        /// <summary>
        ///     ''' first row of the csv file, all signal names
        ///     ''' </summary>
        public List<string> NameRowList;
        /// <summary>
        ///     ''' second row of the csv file, all signal types
        ///     ''' </summary>
        public List<string> TypeRowList;
        /// <summary>
        ///     ''' third row of the csv file, all signal units
        ///     ''' </summary>
        public List<string> UnitRowList;
        /// <summary>
        ///     ''' fourth row of the csv file, the full standard name of the signal:
        ///     ''' 16-character PMU name . 16-character signal name . suffix
        ///     ''' </summary>
        public List<string> PMUList;
    }
}
