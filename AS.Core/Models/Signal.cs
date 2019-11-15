using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.Models
{
    public class Signal
    {
        public Signal()
        {
            IsChecked = false;
            IsValid = true;
            IsEnabled = true;
            IsCustomSignal = false;
            IsNameTypeUnitChanged = false;
            Data = new List<double>();
            //MATLABTimeStampNumber = new List<double>();
            TimeStampNumber = new List<double>();
            TimeStamps = new List<DateTime>();
          //  From = CoreUtilities.DummySiteCoordinatesModel;
          //  To = CoreUtilities.DummySiteCoordinatesModel;
          // Locations = new ObservableCollection<SiteCoordinatesModel>();
          //  Locations.Add(CoreUtilities.DummySiteCoordinatesModel);
          //  MapPlotType = SignalMapPlotType.Dot;
        }
        public Signal(string pmu, string signal) : this()
        {
            PMUName = pmu;
            SignalName = signal;
        }
        public string PMUName { get; set; }
        public string SignalName { get; set; }
        public string TypeAbbreviation { get; set; }
        public string Unit { get; set; }
     //   public string OldSignalName { get; set; }
 //       public string OldTypeAbbreviation { get; set; }
 //       public string OldUnit { get; set; }
        public bool? IsValid { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsCustomSignal { get; set; }
        public bool? IsNameTypeUnitChanged { get; set; }
        public int SamplingRate { get; set; }
        public int PassedThroughDQFilter { get; set; }
        public int PassedThroughProcessor { get; set; }
        public List<double> Data { get; set; }
        public List<double> TimeStampNumber { get; set; } // .net number of days
        public List<DateTime> TimeStamps { get; set; }
                                           //public List<double> MATLABTimeStampNumber { get; internal set; }
                                           //    public SiteCoordinatesModel From { get; set; }
                                           //    public SiteCoordinatesModel To { get; set; }
                                           //  public ObservableCollection<SiteCoordinatesModel> Locations { get; set; }
                                           //public SignalMapPlotType MapPlotType { get; set; }
    }

    public class SignalSignature
    {
        public SignalSignature()
        {
            
        }

        
         public string Name { get; set; }
        public IList<string> Channels { get; set; }
    
    }

}
