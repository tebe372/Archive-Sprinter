using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.SampleDataManager;
using AS.Core;

namespace AS.Config
{
    public class DataSourceSetting
    {
        public DataSourceSetting()
        {
            FileDirectory = "";
            FileType = DataFileType.csv;
        }

        public DataSourceSetting(string directory)
        {
            FileDirectory = directory;
            FileType = DataFileType.csv;
        }
        public string FileDirectory { get; set; }
        public DataFileType FileType;
        private string _exampleFile;
        public string ExampleFile 
        {
            get { return _exampleFile; }
            set
            {
                if (_exampleFile != value)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (File.Exists(value))
                        {
                            if (Utilities.CheckDataFileMatch(value, FileType))
                            {
                                _exampleFile = value;
                                var filename = "";
                                try
                                {
                                    filename = Path.GetFileNameWithoutExtension(value);
                                }
                                catch (ArgumentException ex)
                                {
                                    throw new Exception("Data file path contains one or more of the invalid characters. Original message: " + ex.Message);
                                }
                                if (FileType == DataFileType.PI || FileType == DataFileType.OpenHistorian || FileType == DataFileType.OpenPDC)
                                {
                                    Mnemonic = "";
                                    try
                                    {
                                        FileDirectory = Path.GetDirectoryName(value);
                                        var type = Path.GetExtension(value);
                                        if (type == ".xml")
                                        {
                                            //PresetList = _model.GetPresets(value);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Error extracting file directory from selected file. Original message: " + ex.Message);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        Mnemonic = filename.Substring(0, filename.Length - 16);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Error extracting Mnemonic from selected data file. Original message: " + ex.Message);
                                    }
                                    try
                                    {
                                        var fullPath = Path.GetDirectoryName(value);
                                        var slashIndex = fullPath.LastIndexOf(@"\");
                                        if (slashIndex > 0)
                                        {
                                            var oneLevelUp = fullPath.Substring(0, slashIndex);
                                            if (Directory.Exists(oneLevelUp))
                                            {
                                                slashIndex = oneLevelUp.LastIndexOf(@"\");
                                                if (slashIndex > 0)
                                                {
                                                    var twoLevelUp = oneLevelUp.Substring(0, slashIndex);
                                                    if (Directory.Exists(twoLevelUp))
                                                    {
                                                        FileDirectory = twoLevelUp;
                                                    }
                                                    else
                                                    {
                                                        FileDirectory = oneLevelUp;
                                                    }
                                                }
                                                else
                                                {
                                                    FileDirectory = oneLevelUp;
                                                }
                                            }
                                            else
                                            {
                                                FileDirectory = fullPath;
                                            }
                                        }
                                        else
                                        {
                                            FileDirectory = fullPath;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Error extracting file directory from selected file. Original message: " + ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("The example file  " + Path.GetFileName(value) + " has a type that does not match the selected file type.");
                            }
                        }
                        else
                        {
                            throw new Exception("The example file  " + Path.GetFileName(value) + "  could not be found in the directory  " + Path.GetDirectoryName(value) + ".\n"
                                            + "Please go to the 'Data Source' tab, update the location of the example file, and click the 'Read File' button.");
                        }
                    }
                    //var reader = DataFileReaderFactory.Create(DataFileType.csv);
                    //List<Signal> signals = reader.Read(value);
                    //if (signals != null && signals.Count() > 0)
                    //{
                    //    SampleDataMngr sdm = SampleDataMngr.Instance;
                    //    sdm.AddSampleSignals(signals);
                    //}
                }
            }
        }
        public string Mnemonic { get; set; }
        public int SamplingRate { get; set; }
        public int NumberOfDataPointInFile { get; set; }
    }
}
