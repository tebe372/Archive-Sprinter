using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Config
{
    public class Pause
    {
        public string LastReadFileTime { get; set; }
        //public DateTime NextUnreadFileTime { get; set; }
        public DateTime CurrentTimeStamp { get; set; }
        //public DateTime NextTimeStamp { get; set; }
        public string LastWrittenFileName { get; set; }
    }
}
