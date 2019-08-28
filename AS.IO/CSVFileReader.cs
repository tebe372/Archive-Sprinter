using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Core.Models;
using Microsoft.VisualBasic.FileIO;

namespace AS.IO
{
    public class CSVFileReader : IDataFileReader
    {
        List<Signal> IDataFileReader.Read(string filename)
        {
            var reader = new TextFieldParser(filename);
            reader.TextFieldType = FieldType.Delimited;
            reader.SetDelimiters(",");
            reader.HasFieldsEnclosedInQuotes = true;

            var signalList = new List<Signal>();

            string pmuName = filename.Split('\\').Last().Split('_')[0];

            List<string> signalNames = reader.ReadFields().Skip(1).ToList();

            List<string> signalTypes = reader.ReadFields().Skip(1).ToList();

            List<string> signalUnits = reader.ReadFields().Skip(1).ToList();

            reader.ReadLine();
            reader.ReadLine();

            var time1 = reader.ReadFields()[0];
            var time2 = reader.ReadFields()[0];

            try
            {
                double t1 = Convert.ToDouble(time1);

                double t2 = Convert.ToDouble(time2);

                double SamplingRate = Math.Round((1 / (t2 - t1)) / 10) * 10;
            }

            catch (Exception)
            {

                DateTime t1 = DateTime.Parse(time1);

                DateTime t2 = DateTime.Parse(time2);

                double dif = t2.Subtract(t1).TotalSeconds;

                double SamplingRate = Math.Round((1 / dif) / 10) * 10;

            }

            //for (var index = 0; index <= signalNames.Count - 1; index++)
            //    {
            //        var newSignal = new SignalSignatureViewModel();
            //        newSignal.PMUName = pmuName;
            //        newSignal.Unit = signalUnits[index];
            //        newSignal.SignalName = signalNames[index];
            //        newSignal.SamplingRate = (int)SamplingRate;
            //        signalList.Add(signalNames[index]);
            //        switch (signalTypes[index])
            //        {
            //            case "VPM":
            //                {
            //                    // signalName = signalNames(index).Split(".")(0) & ".VMP"
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "VMP";
            //                    break;
            //                }

            //            case "VPA":
            //                {
            //                    // signalName = signalNames(index).Split(".")(0) & ".VAP"
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "VAP";
            //                    break;
            //                }

            //            case "IPM":
            //                {
            //                    // signalName = signalNames(index).Split(".")(0) & ".IMP"
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "IMP";
            //                    break;
            //                }

            //            case "IPA":
            //                {
            //                    // signalName = signalNames(index).Split(".")(0) & ".IAP"
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "IAP";
            //                    break;
            //                }

            //            case "F":
            //                {
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "F";
            //                    break;
            //                }

            //            case "P":
            //                {
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "P";
            //                    break;
            //                }

            //            case "Q":
            //                {
            //                    // signalName = signalNames(index)
            //                    newSignal.TypeAbbreviation = "Q";
            //                    break;
            //                }

            //            default:
            //                {
            //                    throw new Exception("Error! Invalid signal type " + signalTypes[index] + " found in file: " + aFileInfo.ExampleFile + " !");
            //                }
            //        }
            //        newSignal.OldSignalName = newSignal.SignalName;
            //        newSignal.OldTypeAbbreviation = newSignal.TypeAbbreviation;
            //        newSignal.OldUnit = newSignal.Unit;
            //        signalSignatureList.Add(newSignal);
            //    }
            //    aFileInfo.SignalList = signalList;
            //    aFileInfo.TaggedSignals = signalSignatureList;
            //    aFileInfo.SamplingRate = (int)SamplingRate;
            //    var newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
            //    newSig.SamplingRate = (int)SamplingRate;
            //    var a = new SignalTypeHierachy(newSig);
            //    a.SignalList = SortSignalByPMU(signalSignatureList);
            //    GroupedRawSignalsByPMU.Add(a);
            //    //newSig = new SignalSignatureViewModel(aFileInfo.FileDirectory + ", Sampling Rate: " + aFileInfo.SamplingRate + "/Second");
            //    //newSig.SamplingRate = (int)SamplingRate;
            //    var b = new SignalTypeHierachy(newSig);
            //    b.SignalList = SortSignalByType(signalSignatureList);
            //    GroupedRawSignalsByType.Add(b);
            //    ReGroupedRawSignalsByType = GroupedRawSignalsByType;
            //}

            return signalList;
        }
    }
}
