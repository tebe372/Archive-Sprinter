using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Core
{
    public static class Utilities
    {
        public static bool CheckDataFileMatch(string file, DataFileType type)
        {
            var tp = "";
            try
            {
                tp = Path.GetExtension(file).Substring(1).ToLower();
            }
            catch
            {
            }
            if (type.ToString().ToLower() == tp)
                return true;
            else if (type == DataFileType.powHQ && tp == "mat")
                return true;
            else if ((type == DataFileType.PI || type == DataFileType.OpenHistorian || type == DataFileType.OpenPDC) && tp == "xml")
                return true;
            else
                return false;
        }
    }
}
