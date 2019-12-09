using AS.Core.Models;
using AS.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.SampleDataManager;

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
                    _exampleFile = value;
                    var reader = DataFileReaderFactory.Create(DataFileType.csv);
                    List<Signal> signals = reader.Read(value);
                    if (signals != null && signals.Count() > 0)
                    {
                        SampleDataMngr sdm = SampleDataMngr.Instance;
                        sdm.AddSampleSignals(signals);
                    }
                }
            }
        }
        public string Mnemonic { get; set; }

        public bool CheckDataFileMatch()
        {
            var tp = "";
            try
            {
                tp = Path.GetExtension(ExampleFile).Substring(1).ToLower();
            }
            catch
            {
            }
            if (FileType.ToString().ToLower() == tp)
                return true;
            else if (FileType == DataFileType.powHQ && tp == "mat")
                return true;
            else if ((FileType == DataFileType.PI || FileType == DataFileType.OpenHistorian || FileType == DataFileType.OpenPDC) && tp == "xml")
                return true;
            else
                return false;
        }
    }
}
