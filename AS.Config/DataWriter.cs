using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
