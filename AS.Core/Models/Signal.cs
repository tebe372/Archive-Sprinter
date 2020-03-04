using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Signal //do we implement a flag matrix for each signal?
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
        public bool IsChecked { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsCustomSignal { get; set; }
        public bool? IsNameTypeUnitChanged { get; set; }
        public int SamplingRate { get; set; }
        public int PassedThroughDQFilter { get; set; }
        public int PassedThroughProcessor { get; set; }
        private List<double> _data;
        public List<double> Data 
        {
            get { return _data; }
            set 
            {
                _data = value;
                _flags = null;
            }
        }
        public List<double> TimeStampNumber { get; set; } // .net number of days
        public List<DateTime> TimeStamps { get; set; }
        private List<bool> _flags;
        public List<bool> Flags 
        {
            get
            {
                if (_flags == null)
                {
                    bool[] f = new bool[_data.Count];
                    Utilities.Populate<bool>(f, true);
                    _flags = f.ToList();
                }
                return _flags; 
                
            }
            set{ _flags = value; } 
        }

        public void ChangeFlaggedValueToNAN()
        {
            throw new NotImplementedException();
        }
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
