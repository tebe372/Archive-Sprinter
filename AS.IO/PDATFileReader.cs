using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDAT_Reader;
using DataReaderBase;

namespace AS.IO
{
    public class PDATFileReader : IDataFileReader
    {
        private int _samplingRate = -1;
        private int _numberOfDataPointInFile = 0;
        public List<Core.Models.Signal> Read(string filename)
        {
            PDATReader reader = null;
            try
            {
                reader = new PDATReader(filename);
            }
            catch (OutOfMemoryException oomEx)
            {
                throw oomEx;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            List<Core.Models.Signal> signalList = null;
            try
            {
                signalList = new List<Core.Models.Signal>();
            }
            catch (OutOfMemoryException oomEx)
            {
                throw oomEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            List<DataReaderBase.Signal> signals = null;
            try
            {
                signals = reader.GetAllSignalsFromPDATFile();
            }
            catch (OutOfMemoryException oomEx)
            {
                throw oomEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (signals.Count() > 0)
            {
                decimal diff;
                var firstSig = signals.FirstOrDefault();
                _numberOfDataPointInFile = firstSig.PointsList.Count();
                if (_numberOfDataPointInFile >= 2)
                {
                    //for (int i = 0; i < _numberOfDataPointInFile - 1; i++)
                    //{
                    var time1 = firstSig.PointsList[0].T;
                    var time2 = firstSig.PointsList[1].T;
                    diff = time2 - time1;
                    if (diff != 0)
                    {
                        _samplingRate = (int)Math.Round((1 / diff) / 10) * 10;
                        //break;
                    }
                    else
                    {
                        //Console.WriteLine("sampling rate is 0 at: " + filename + "\nThe numbers are: " + time1.ToString() + " and " + time2.ToString());
                    }
                    //}
                }
                Dictionary<string, List<DataReaderBase.Signal>> groupbyPMU = null;
                try
                {
                    groupbyPMU = signals.GroupBy(x => x.ShortName).ToDictionary(y => y.Key, y => y.ToList());
                }
                catch (OutOfMemoryException oomEx)
                {
                    throw oomEx;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                foreach (var gr in groupbyPMU)
                {
                    var stat = gr.Value.FirstOrDefault().stat.ToList();
                    foreach (var sig in gr.Value)
                    {
                        var newSignal = new Core.Models.Signal(sig.ShortName, sig.Header);
                        //var name = sig.Name;
                        try
                        {
                            _getDataTimeStamp(newSignal, sig.EventDate, sig.PointsList);
                        }
                        catch (OutOfMemoryException oomEx)
                        {
                            throw oomEx;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        //var time = sig.EventDate;
                        //var data = sig.PointsList;
                        newSignal.TypeAbbreviation = _getSignalType(sig.Type);
                        newSignal.Unit = sig.Unit;
                        newSignal.SamplingRate = _samplingRate;
                        newSignal.Stat = stat;
                        signalList.Add(newSignal);
                    }
                }
            }
            return signalList;
        }

        public int GetSamplingRate()
        {
            return _samplingRate;
        }

        public int GetNumberOfDataPointInFile()
        {
            return _numberOfDataPointInFile;
        }

        private void _getDataTimeStamp(Core.Models.Signal newSignal, DateTimeOffset eventDate, List<Point> pointsList)
        {
            for (int i = 0; i < pointsList.Count; i++)
            {
                var d = pointsList[i];
                var timeStamp = eventDate.DateTime.AddSeconds((double)d.T);
                try
                {
                    newSignal.Data.Add(d.Value);
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
                    newSignal.TimeStampNumber.Add(timeStamp.ToOADate());
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
                    newSignal.TimeStamps.Add(timeStamp);
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

        private string _getSignalType(string type)
        {
            switch (type)
            {
                case "VPM":
                    {
                        return "VMP";
                    }

                case "VAM":
                    {
                        return "VMA";
                    }

                case "VBM":
                    {
                        return "VMB";
                    }

                case "VCM":
                    {
                        return "VMC";
                    }

                case "VPA":
                    {
                        return "VAP";
                    }

                case "VAA":
                    {
                        return "VAA";
                    }

                case "VBA":
                    {
                        return "VAB";
                    }

                case "VCA":
                    {
                        return "VAC";
                    }

                case "IPM":
                    {
                        return "IMP";
                    }

                case "IPA":
                    {
                        return "IAP";
                    }

                case "F":
                    {
                        return "F";
                    }

                case "R":
                    {
                        return "R";
                    }

                case "P":
                    {
                        return "P";
                    }

                case "Q":
                    {
                        return "Q";
                    }

                case "D":
                    {
                        return "D";
                    }

                default:
                    {
                        return "Other";
                    }
            }
        }
    }
}
