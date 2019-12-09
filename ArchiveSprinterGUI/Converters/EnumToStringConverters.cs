using AS.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArchiveSprinterGUI.Converters
{
    public class EnumToStringConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case DataFileType.csv:
                    return "JSIS CSV";
                case DataFileType.pdat:
                    return "PDAT";
                case DataFileType.powHQ:
                    return "HQ Point on Wave";
                case DataFileType.PI:
                    return "PI Database";
                case DataFileType.OpenHistorian:
                    return "openHistorian";
                case DataFileType.OpenPDC:
                    return "openPDC";
                default:
                    throw new Exception("Input data file type not valid!");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "JSIS CSV":
                    return DataFileType.csv;
                case "PDAT":
                    return DataFileType.pdat;
                case "HQ Point on Wave":
                    return DataFileType.powHQ;
                case "PI Database":
                    return DataFileType.PI;
                case "openHistorian":
                    return DataFileType.OpenHistorian;
                case "openPDC":
                    return DataFileType.OpenPDC;
                default:
                    throw new Exception("Enum type not valid!");
            }
        }
    }
}
