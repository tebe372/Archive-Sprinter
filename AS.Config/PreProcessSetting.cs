using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AS.Core.Models;


namespace AS.Config
{
    public class PreProcessSetting
    {
    }
    // Data Quality Filter Class
    public class DQFilter : PreProcessSetting
    {
        public DQFilter()
        {

        }
        
  //      public string SetToNaN { get; set; } = "True";
        public string Name { get; set; }
        public IList<SignalSignature> PMUs { get; set; }
    }

    public class StatusFlagsDQFilterModel : DQFilter
    {
        public StatusFlagsDQFilterModel()
        {
        }
       
        public new string Name { get => "Status Flags"; }
    }
    public class ZerosDQFilterModel : DQFilter
    {
        public ZerosDQFilterModel() { }
       
        public new string Name { get => "Zeros"; }
    }
    public class MissingDQFilterModel : DQFilter
    {
        public MissingDQFilterModel() { }
       
        public new string Name { get => "Missing"; }
    }
    public class WrappingFailureDQFilter : DQFilter
    {
        public WrappingFailureDQFilter() { }
    /*    public WrappingFailureDQFilterModel(XElement item) : base(item)
        {
            var par = item.Element("Parameters").Element("AngleThresh").Value;
            if (par != null)
            {
                AngleThresh = par;
            }
        }
        */
        public new string Name { get => "Angle Wrapping"; }
        //      public string AngleThresh { get; set; }
        public Dictionary<string, string> Parameters;
    }

    public class DataFrameDQFilterModel : DQFilter
    {
        public DataFrameDQFilterModel() { }
     /*   public DataFrameDQFilterModel(XElement item) : base(item)
        {
            var par = item.Element("Parameters").Element("PercentBadThresh").Value;
            if (par != null)
            {
                PercentBadThresh = par;
            }
        }
        */
        public new string Name { get => "Data Frame"; }
        public string PercentBadThresh { get; set; }
    }
    public class PMUchanDQFilterModel : DQFilter
    {
        public PMUchanDQFilterModel() { }
       
        public new string Name { get => "Channel"; }
    }
    public class PMUallDQFilterModel : DQFilter
    {
        public PMUallDQFilterModel() { }
        
        public new string Name { get => "Entire PMU"; }
    }

    public class StaleDQFilterModel : DQFilter
    {
        public StaleDQFilterModel() { }
      /*  public StaleDQFilterModel(XElement item) : base(item)
        {
            var par = item.Element("Parameters").Element("StaleThresh").Value;
            if (par != null)
            {
                StaleThresh = par;
            }
            par = item.Element("Parameters").Element("FlagAllByFreq").Value;
            if (par != null)
            {
                if (par.ToLower() == "true")
                {
                    FlagAllByFreq = true;
                }
                else
                {
                    FlagAllByFreq = false;
                }
            }
        }
        */
        public new string Name { get => "Stale Data"; }
        public string StaleThresh { get; set; }
        public bool FlagAllByFreq { get; set; }
    }

    public class OutlierDQFilterModel : DQFilter
    {
        public OutlierDQFilterModel() { }
    /*    public OutlierDQFilterModel(XElement item) : base(item)
        {
            var par = item.Element("Parameters").Element("StdDevMult").Value;
            if (par != null)
            {
                StdDevMult = par;
            }
        }
        */
        public new string Name { get => "Outliers"; }
        public string StdDevMult { get; set; }
    }

    public class FreqDQFilterModel : DQFilter
    {
        public FreqDQFilterModel() { }
    /*    public FreqDQFilterModel(XElement item) : base(item)
        {
            var par = item.Element("Parameters").Element("FreqMaxChan").Value;
            if (par != null)
            {
                FreqMaxChan = par;
            }
            par = item.Element("Parameters").Element("FreqMinChan").Value;
            if (par != null)
            {
                FreqMinChan = par;
            }
            par = item.Element("Parameters").Element("FreqPctChan").Value;
            if (par != null)
            {
                FreqPctChan = par;
            }
            par = item.Element("Parameters").Element("FreqMinSamp").Value;
            if (par != null)
            {
                FreqMinSamp = par;
            }
            par = item.Element("Parameters").Element("FreqMaxSamp").Value;
            if (par != null)
            {
                FreqMaxSamp = par;
            }
        }
        */
        public new string Name { get => "Nominal Frequency"; }
        public string FreqMaxChan { get; set; }
        public string FreqMinChan { get; set; }
        public string FreqPctChan { get; set; }
        public string FreqMinSamp { get; set; }
        public string FreqMaxSamp { get; set; }
    }

    public class VoltPhasorDQFilterModel : DQFilter
    {
        public VoltPhasorDQFilterModel() { }
     /*   public VoltPhasorDQFilterModel(XElement item) : base(item)
        {
            var par = item.Element("Parameters").Element("NomVoltage").Value;
            if (par != null)
            {
                NomVoltage = par;
            }
            par = item.Element("Parameters").Element("VoltMin").Value;
            if (par != null)
            {
                VoltMin = par;
            }
            par = item.Element("Parameters").Element("VoltMax").Value;
            if (par != null)
            {
                VoltMax = par;
            }
        }
        */
        public new string Name { get => "Nominal Voltage"; }
        public string VoltMin { get; set; }
        public string VoltMax { get; set; }
        public string NomVoltage { get; set; }
    }
}
