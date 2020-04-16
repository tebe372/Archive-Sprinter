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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            var signalList = new List<Core.Models.Signal>();
            var signals = reader.GetAllSignalsFromPDATFile();
            if (signals.Count() > 0)
            {
                decimal diff;
                var firstSig = signals.FirstOrDefault();
                _numberOfDataPointInFile = firstSig.PointsList.Count();
                for (int i = 0; i < _numberOfDataPointInFile - 1; i++)
                {
                    var time1 = firstSig.PointsList[i].T;
                    var time2 = firstSig.PointsList[i + 1].T;
                    diff = time2 - time1;
                    if (diff != 0)
                    {
                        _samplingRate = (int)Math.Round((1 / diff) / 10) * 10;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("sampling rate is 0 at: " + filename + "\nThe numbers are: " + time1.ToString() + " and " + time2.ToString());
                    }
                }
                foreach (var sig in signals)
                {
                    var newSignal = new Core.Models.Signal(sig.ShortName, sig.Header);
                    //var name = sig.Name;
                    _getDataTimeStamp(newSignal, sig.EventDate, sig.PointsList);
                    var time = sig.EventDate;
                    var data = sig.PointsList;
                    newSignal.TypeAbbreviation = _getSignalType(sig.Type);
                    newSignal.Unit = sig.Unit;
                    newSignal.SamplingRate = _samplingRate;
                    signalList.Add(newSignal);
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
                newSignal.Data.Add(d.Value);
                newSignal.TimeStampNumber.Add(timeStamp.ToOADate());
                newSignal.TimeStamps.Add(timeStamp);
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
