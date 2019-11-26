using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Core.Models;
using Newtonsoft.Json;


namespace AS.Config
{
    public class PreProcessSetting
    {
        public PreProcessSetting(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        [JsonExtensionData]
        private Dictionary<string, object> Parameters = new Dictionary<string, object> { };

        public void AddParameter(string paramName, string paramValue)
        {
            Parameters[paramName] = paramValue;
        }
    }
    // Data Quality Filter Class
    public class Filter: PreProcessSetting
    {
        public Filter(string filterName) : base(filterName)
        {
            
        }
       
        public IList<SignalSignature> PMUs { get; set; }

    }

    // Customization Class
    public class Customization : PreProcessSetting
    {
        public Customization(string customName) : base (customName)
        {
 
        }
    }

}
