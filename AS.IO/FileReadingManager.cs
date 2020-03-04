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
                Array.Sort(allFiles);
                foreach (var file in allFiles)
                {
                    if (Utilities.CheckDataFileMatch(file, FileType))
                    {
                        var signals = reader.Read(file);
                        OnFileReadingDone(signals);
                    }
                }
                OnDataReadingDone();
            }
        }

        public string SourceDirectory { get; set; }
        public DataFileType FileType { get; set; }
        //used when done reading one file
        public delegate void FileReadingDoneEventhandler(object sender, List<Signal> e);
        public event FileReadingDoneEventhandler FileReadingDone;
        protected virtual void OnFileReadingDone(List<Signal> e)
        {
            FileReadingDone?.Invoke(this, e);
        }
        //used when done reading all files in the directory
        public delegate void DataReadingDoneEventhandler(object sender);
        public event DataReadingDoneEventhandler DataReadingDone;
        protected virtual void OnDataReadingDone()
        {
            DataReadingDone?.Invoke(this);
        }


    }
}
