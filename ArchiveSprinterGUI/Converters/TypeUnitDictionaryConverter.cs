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
    public class TypeUnitDictionaryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var key = (string)values[1];
            var dict = (Dictionary<string, List<string>>)values[0];
            if ((key) is string && !string.IsNullOrEmpty(key) && (dict) is Dictionary<string, List<string>>)
                return dict[key];
            else
                return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
