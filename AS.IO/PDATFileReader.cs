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
        public List<Core.Models.Signal> Read(string filename)
        {
            var reader = new PDATReader(filename);
            var signals = reader.GetAllSignalsFromPDATFile();
            var signalList = new List<Core.Models.Signal>();
            var time1 = signals.FirstOrDefault().PointsList[0].T;
            var time2 = signals.FirstOrDefault().PointsList[1].T;
            int samplingRate = (int)Math.Round((1 / (time2 - time1)) / 10) * 10;
            foreach (var sig in signals)
            {
                var newSignal = new Core.Models.Signal(sig.ShortName, sig.Header);
                //var name = sig.Name;
                _getDataTimeStamp(newSignal, sig.EventDate, sig.PointsList);
                var time = sig.EventDate;
                var data = sig.PointsList;
                newSignal.TypeAbbreviation = _getSignalType(sig.Type);
                newSignal.Unit = sig.Unit;
                newSignal.SamplingRate = samplingRate;
                signalList.Add(newSignal);
            }
            return signalList;
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
