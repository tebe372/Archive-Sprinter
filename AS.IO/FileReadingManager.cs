//using AS.Config;
using AS.Core;
using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.IO
{
    /// <summary>
    /// this class manages file reading from the sources,
    /// it should read all eligible files in the directory
    /// it should be able to combine/split read data according to user requirement
    /// it might also need to be able to calculate how many works we need 
    /// </summary>
    public class FileReadingManager
    {
        public FileReadingManager()
        {

        }
        //public FileReadingManager(DataSourceSetting source)
        //{
        //    SourceDirectory = source.FileDirectory;
        //    FileType = source.FileType;
        //    SamplingRate = source.SamplingRate;
        //    NumberOfDataPointInFile = source.NumberOfDataPointInFile;
        //    Mnemonic = source.Mnemonic;
        //}

        public async Task Start()
        {
            var reader = DataFileReaderFactory.Create(FileType);
            DataCompleted = false;
            if (Directory.Exists(SourceDirectory))
            {
                var fileLength = NumberOfDataPointInFile / (double)SamplingRate;
                DateTime time, endtime;
                try
                {
                    time = DateTime.ParseExact(DateTimeStart, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    endtime = DateTime.ParseExact(DateTimeEnd, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                DateTime lastTimeStamp = new DateTime();
                while (time <= endtime && !DataCompleted)
                {
                    var fileTime = time.ToString("_yyyyMMdd_HHmmss");
                    var yearDir = fileTime.Substring(1, 4);
                    var dateDir = fileTime.Substring(3, 6);
                    CurrentFile = SourceDirectory + "\\" + yearDir + "\\" + dateDir + "\\" + Mnemonic + fileTime + "." + FileType;
#if DEBUG
                    Console.WriteLine(CurrentFile);
#endif
                    if (File.Exists(CurrentFile))
                    {
#if DEBUG
                        Console.WriteLine("File exists: " + CurrentFile);
#endif
                        List<Signal> signals = null;
                        try
                        {
                            signals = reader.Read(CurrentFile);
                        }
                        catch (Exception ex)
                        {
                            time = time.AddSeconds(fileLength);
                            continue;
                        }
                        var keepSig = new List<Signal>();
                        //var a = _getFileDateTime(CurrentFile);
                        var aa = 1.0 / SamplingRate * (NumberOfDataPointInFile - 1);
                        var lastTimePoint = time.AddSeconds(aa);
                        foreach (var item in signals)
                        {
                            var sig = item.PMUName + "_" + item.SignalName;
                            if (NeededSignalList.Contains(sig))
                            {
                                var b = item.TimeStamps.LastOrDefault() - lastTimePoint;
                                var c = b.TotalSeconds;
                                var lastTimeStampValidity = Math.Abs(c);
                                if (item.SamplingRate != SamplingRate || item.TimeStamps.Count() != NumberOfDataPointInFile || lastTimeStampValidity > 1 / 600)
                                {
                                    //Console.WriteLine(file + " has bad sampling rate or time stamps.");
                                }
                                else
                                {
                                    keepSig.Add(item);
                                }
                            }
                        }
                        if (keepSig.Count() == NeededSignalList.Count())
                        {
                            lastTimeStamp = keepSig.FirstOrDefault().TimeStamps.LastOrDefault();
                            OnFileReadingDone(keepSig);
                        }
                        else
                        {
                        }
                    }
                    time = time.AddSeconds(fileLength);
                }
                //string[] allFiles;
                //try
                //{
                //    allFiles = Directory.GetFiles(SourceDirectory, "*.*", SearchOption.AllDirectories);
                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}
                //Array.Sort(allFiles);
                //DateTime lastTimeStamp = new DateTime();
                //foreach (var file in allFiles)
                //{
                //    if (Utilities.CheckDataFileMatch(file, FileType))
                //    {
                //        //Console.WriteLine("Reading " + file);
                //        List<Signal> signals = null;
                //        try
                //        {
                //            signals = reader.Read(file);
                //        }
                //        catch (Exception ex)
                //        {
                //            continue;
                //        }
                //        var keepSig = new List<Signal>();
                //        var a = _getFileDateTime(file);
                //        var aa = 1.0 / SamplingRate * (NumberOfDataPointInFile - 1);
                //        var lastTimePoint = a.AddSeconds(aa);
                //        foreach (var item in signals)
                //        {
                //            var sig = item.PMUName + "_" + item.SignalName;
                //            if (NeededSignalList.Contains(sig))
                //            {
                //                var b = item.TimeStamps.LastOrDefault() - lastTimePoint;
                //                var c = b.TotalSeconds;
                //                var lastTimeStampValidity = Math.Abs(c);
                //                if (item.SamplingRate != SamplingRate || item.TimeStamps.Count() != NumberOfDataPointInFile || lastTimeStampValidity > 1 / 600)
                //                {
                //                    //Console.WriteLine(file + " has bad sampling rate or time stamps.");
                //                }
                //                else
                //                {
                //                    keepSig.Add(item);
                //                }
                //            }
                //        }
                //        if (keepSig.Count() == NeededSignalList.Count())
                //        {
                //            lastTimeStamp = keepSig.FirstOrDefault().TimeStamps.LastOrDefault();
                //            OnFileReadingDone(keepSig);
                //        }
                //        else
                //        {
                //        }
                //    }
                //}
                if (fileLength >= WindowSize)
                {
                    time = time.AddSeconds(-fileLength);
                }
                else
                {
                    var n = Math.Ceiling(WindowSize / fileLength);
                    time = time.AddSeconds(-fileLength * n);
                }
                DateTimeStart = time.ToString("M/d/yyyy h:mm:ss tt");
                OnDataReadingDone(lastTimeStamp);
            }
        }

        public string SourceDirectory { get; set; }
        public DataFileType FileType { get; set; }
        public List<string> NeededSignalList { get; set; }

        //used when done reading one file
        public delegate void FileReadingDoneEventhandler(object sender, List<Signal> e);
        public event FileReadingDoneEventhandler FileReadingDone;
        protected virtual void OnFileReadingDone(List<Signal> e)
        {
            FileReadingDone?.Invoke(this, e);
        }
        //used when done reading all files in the directory
        public delegate void DataReadingDoneEventhandler(object sender, DateTime e);
        public event DataReadingDoneEventhandler DataReadingDone;
        protected virtual void OnDataReadingDone(DateTime e)
        {
            DataReadingDone?.Invoke(this, e);
        }
        public int SamplingRate { get; set; }
        public int NumberOfDataPointInFile { get; set; }
        public string Mnemonic { get; set; }
        public string DateTimeEnd { get; set; }
        public string DateTimeStart { get; set; }

        //private string _exampleFileName;
        //public string ExampleFileName 
        //{
        //    get { return _exampleFileName; }
        //    set 
        //    { 
        //        _exampleFileName = value; 
        //    } 
        //}

        public bool DataCompleted { get; set; }
        public string CurrentFile { get; set; }
        public int WindowSize { get; set; }
    }
}
