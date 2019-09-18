using ArchiveSprinterGUI.ViewModels;
using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
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
    public class RadioButtonViewConverter1 : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SettingsViewModel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
