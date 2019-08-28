using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core.Models
{
    public class SiteCoordinates
    {
        public SiteCoordinates()
        {
            Name = "";
            Latitude = "";
            Longitude = "";
            _internalCounter += 1;
            _internalID = _internalCounter;
        }

      /*  public SiteCoordinates(ConfigSite item) : this(item.Name)
        {
            Latitude = item.Latitude;
            Longitude = item.Longitude;
        }
        */
        public SiteCoordinates(string name) : this()
        {
            Name = name;
        }

        public SiteCoordinates(double lat, double lng) : this()
        {
            Latitude = lat.ToString();
            Longitude = lng.ToString();
        }

        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        private static int _internalCounter = 0;
        private int _internalID;

        public int GetInternalID()
        {
            return _internalID;
        }
    }
}
