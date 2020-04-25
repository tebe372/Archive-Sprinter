using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ArchiveSprinterGUI.Converters
{
    public class SignalSelectionDropDownConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value1 = values[0];
            var index = values[1];
            if (index is int)
            {
                switch (index)
                {
                    case 1:
                        value1 = new List<string>() { "View Signal by Type", "View Signal by PMU", "Input Channels by Step", "Output Channels by Step" };
                        break;
                    case 2:
                        value1 = new List<string>() { "View Signal by Type", "View Signal by PMU", "Output from Data Quality and Customization by Signal Type", "Output from Data Quality and Customization by PMU", "Input Channels by Step" };
                        break;
                    case 3:
                        value1 = new List<string>() { "View Signal by Type", "View Signal by PMU", "Output from Data Quality and Customization by Signal Type", "Output from Data Quality and Customization by PMU", "Input Channels by Step" };
                        break;
                    default:
                        break;
                }
            }
            return value1;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
