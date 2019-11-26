using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Core.Models;

namespace AS.Config
{
    public class Configuration
    {
        public List<InputFileInfo> InputFiles = new List<InputFileInfo> { }; // List of input file information
        // TODO: information about processing windows

        public List<PreProcessSetting> PreProcessSteps = new List<PreProcessSetting> { };  // Customization & Data Quality steps
        public List<SignatureSetting> SignatureSettings { get; set; }  // Signature calculation settings


    }

    public class InputFileInfo
    {
        public InputFileInfo()
        {
            FileDirectory = "";
            FileType = DataFileType.pdat;
        }

        public InputFileInfo(string directory)
        {
            FileDirectory = directory;
            FileType = DataFileType.pdat;
        }
        public string FileDirectory { get; set; }
        public DataFileType FileType;

    }
}
