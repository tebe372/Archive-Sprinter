using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ArchiveSprinterGUI.Converters
{
    public class CanStartRunConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool? canTaskRun = values[0] as bool?;
            bool? noTaskIsRunning = values[1] as bool?;
            if ((bool)canTaskRun && (bool)noTaskIsRunning)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
