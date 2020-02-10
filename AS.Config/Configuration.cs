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

        public readonly List<string> DQFilterList = new List<string> {
            //"Status Flags",
                                                                                "Zeros",
                                                                                "Missing",
            //     "Nominal Voltage",
            //    "Nominal Frequency",
            //   "Outliers",
            //     "Stale Data",
            //     "Data Frame",
            //     "Channel",
            //     "Entire PMU",
            //     "Angle Wrapping"
        };
        public readonly List<string> CustomizationList = new List<string> { "Scalar Repetition",
                                                                            "Addition", 
                                                                            "Subtraction", 
                                                                            "Multiplication", 
                                                                            "Division",
                                                                            "Exponential",
                                                                            "Sign Reversal", 
                                                                            "Absolute Value", 
                                                                            "Real Component", 
                                                                            "Imaginary Component", 
                                                                            "Angle Calculation",
                                                                            "Complex Conjugate",
                                                                            "Phasor Creation", 
                                                                            "Power Calculation", 
                                                                            "Signal Type/Unit", 
                                                                            "Metric Prefix", 
                                                                            "Angle Conversion", 
                                                                            "Duplicate Signals"};


        public void AddConfigStep(string stepName)
        {
            // Create new filter specification
            // Add to list of steps
        }

    }

}
