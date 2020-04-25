using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ArchiveSprinterGUI.Converters
{
    public class DataWriterExpanderHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
            {
                var name = values[0].ToString();
                var count = values[1].ToString();
                return name + " " + count;
            }
            else
            {
                return "No Data Writer Selected Yet!";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
