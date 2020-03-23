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

        public List<PreProcessStep> PreProcessSteps = new List<PreProcessStep> { };  // Customization & Data Quality steps
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
        public readonly List<string> CustomizationList = new List<string> { //"Scalar Repetition",
                                                                           // "Addition", 
                                                                            "Subtraction",
            // "Multiplication", 
            //   "Division",
            //   "Exponential",
            //   "Sign Reversal", 
            //   "Absolute Value", 
            //   "Real Component", 
            //    "Imaginary Component", 
            //   "Angle Calculation",
            //   "Complex Conjugate",
            //   "Phasor Creation", 
            //   "Power Calculation", 
            //   "Signal Type/Unit", 
            //   "Metric Prefix", 
            //    "Angle Conversion", 
            //    "Duplicate Signals"
        };
        public List<SignatureCalMenu> SignatureList { get; set; } = new List<SignatureCalMenu> { 
            new SignatureCalMenu ("Sample Statistics", new List<SignatureCalMenu>{
                new SignatureCalMenu("Mean"),
                new SignatureCalMenu("Variance"),
                new SignatureCalMenu("Standard Deviation"),
                new SignatureCalMenu("Kurtosis"),
                new SignatureCalMenu("Skewness"),
            }),
            new SignatureCalMenu("Correlation Coefficient"),
            new SignatureCalMenu("Covariance"),
            //new SignatureCalMenu("Frequency-Domain Methods", new List<SignatureCalMenu>{
            //    new SignatureCalMenu("Periodogram"),
            //    new SignatureCalMenu("Generalized Magnitude Squared Coherence (GMSC) Spectrum"),
            //}),
            new SignatureCalMenu("Order Statistics", new List<SignatureCalMenu>{
                new SignatureCalMenu("Percentile"),
                new SignatureCalMenu("Quartiles"),
                new SignatureCalMenu("Median"),
                new SignatureCalMenu("Maximum"),
                new SignatureCalMenu("Minimum"),
                new SignatureCalMenu("Range")
            }),
            new SignatureCalMenu("Rise"),
            new SignatureCalMenu("Histogram")
        };
        public int WindowSize { get; set; }
        public int WindowOverlap { get; set; }
        public int DatawriteOutFrequency { get; set; }
        private string _windowSizeStr;
        public string WindowSizeStr
        {
            get { return _windowSizeStr; }
            set
            {
                _windowSizeStr = value;
                if (!string.IsNullOrEmpty(value))
                {
                    WindowSize = Convert.ToInt32(value);
                }
            }
        }
        private string _windowOverlapStr;
        public string WindowOverlapStr
        {
            get { return _windowOverlapStr; }
            set
            {
                _windowOverlapStr = value;
                if (!string.IsNullOrEmpty(value))
                {
                    WindowOverlap = Convert.ToInt32(value);
                }
            }
        }
        private string _datawriteOutFrequencyStr;
        public string DatawriteOutFrequencyStr
        {
            get { return _datawriteOutFrequencyStr; }
            set
            {
                _datawriteOutFrequencyStr = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DatawriteOutFrequency = Convert.ToInt32(value);
                }
            }
        }
        public string DatawriteOutFrequencyUnit { get; set; }
        public List<string> DatawriteOutFrequencyUnits { get; set; } = new List<string> { "Hours", "Days", "Weeks", "Month" };
        public void AddConfigStep(string stepName)
        {
            // Create new filter specification
            // Add to list of steps
        }
    }
    public class SignatureCalMenu {
        public SignatureCalMenu(string sig)
        {
            Signature = sig;
        }

        public SignatureCalMenu(string sig, List<SignatureCalMenu> subSigs) : this(sig)
        {
            SubSignature = subSigs;
        }

        public string Signature { get; set; }
        public List<SignatureCalMenu> SubSignature { get; set; }
    }
}
