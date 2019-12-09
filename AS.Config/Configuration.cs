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
        public List<DataSourceSetting> InputFiles = new List<DataSourceSetting> { }; // List of input file information
        // TODO: information about processing windows

        public List<PreProcessSetting> PreProcessSteps = new List<PreProcessSetting> { };  // Customization & Data Quality steps
        public List<SignatureSetting> SignatureSettings { get; set; }  // Signature calculation settings


    }

}
