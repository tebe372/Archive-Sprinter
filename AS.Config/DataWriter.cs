using AS.ComputationManager.Calculations;
using AS.Core.Models;
using AS.DataManager;
using AS.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AS.Config
{
    public class DataWriter
    {

        public string Name
        {
            get
            {
                return "Data Writer";
            }
        }
        public string SavePath { get; set; }
        public bool SeparatePMUs { get; set; }
        public string Mnemonic { get; set; }
        public List<string> InputSignals { get; set; }
        //public int NumberOfDataPointInFile { get; set; }
        //public int SamplingRate { get; set; }

        public void Process(DataStore dataMngr)
        {
            string filename;
            var writer = new JSISCSVWriter();
            var lasttime = DateTime.MinValue;
            var fileLength = dataMngr.NumberOfDataPointInFile / (double)dataMngr.SamplingRate;
            while (true)
            {
#if DEBUG
                Console.WriteLine("before getting data, SeparatePMUs: " + SeparatePMUs.ToString());
#endif
                if (dataMngr.StartTimeStamps.Count() > 0)
                {
                    if (dataMngr.GetDataWriterData(InputSignals, lasttime, out List<Signal> signals, out DateTime timeStamp))
                    {
                        lasttime = timeStamp;
                        var datetime = timeStamp.ToString("_yyyyMMdd_HHmmss");
                        var year = datetime.Substring(1, 4);
                        var date = datetime.Substring(3, 6);
#if DEBUG
                        Console.WriteLine("after getting data successfully, SeparatePMUs: " + SeparatePMUs.ToString());
#endif
                        if (SeparatePMUs)
                        {
                            var signalGroupedByPMU = signals.GroupBy(x => x.PMUName);
                            foreach (var item in signalGroupedByPMU)
                            {
                                var pmuName = item.Key;
                                filename = SavePath + "\\" + pmuName + "\\" + year + "\\" + date + "\\" + pmuName + datetime + ".csv";
                                writer.WriteJSISCSV(item.ToList(), filename);
#if DEBUG
                                Console.WriteLine("Wrote separate PMU: " + filename);
#endif
                            }
                        }
                        else
                        {
                            filename = SavePath + "\\" + year + "\\" + date + "\\" + Mnemonic + datetime + ".csv";
                            writer.WriteJSISCSV(signals, filename);
#if DEBUG
                            Console.WriteLine("Wrote all PMU in one file: " + filename);
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine("failed wait for 500 ms.");
#endif
                        Thread.Sleep(500);
                        continue;
                    }
                }
                else
                {
                    if (dataMngr.DataCompleted && dataMngr.StartTimeStamps.Count() == 0)
                    {
                        break;
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine("no data, wait for 500 ms.");
#endif
                        Thread.Sleep(500);
                        continue;
                    }
                }
            }
        }
    }
}
