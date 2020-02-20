using AS.Config;
using AS.Core;
using AS.Core.Models;
using System;
using System.Collections.Generic;
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
        public FileReadingManager(DataSourceSetting source)
        {
            SourceDirectory = source.FileDirectory;
            FileType = source.FileType;
        }

        public void Start()
        {
            var reader = DataFileReaderFactory.Create(FileType);
            if (Directory.Exists(SourceDirectory))
            {
                var allFiles = Directory.GetFiles(SourceDirectory, "*.*", SearchOption.AllDirectories);
                foreach (var file in allFiles)
                {
                    if (Utilities.CheckDataFileMatch(file, FileType))
                    {
                        var signals = reader.Read(file);
                        OnDataReadingDone(signals);
                    }
                }
            }
        }

        public string SourceDirectory { get; set; }
        public DataFileType FileType { get; set; }
        public delegate void DataReadingDoneEventhandler(object sender, List<Signal> e);
        public event DataReadingDoneEventhandler DataReadingDone;
        protected virtual void OnDataReadingDone(List<Signal> e)
        {
            DataReadingDone?.Invoke(this, e);
        }
    }
}
