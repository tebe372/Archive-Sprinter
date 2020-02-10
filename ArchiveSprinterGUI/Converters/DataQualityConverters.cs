using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using ArchiveSprinterGUI.ViewModels.SettingsViewModels;

namespace ArchiveSprinterGUI.Converters
{
    public class AddCustomizationParameters : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToList();
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return (object[])DependencyProperty.UnsetValue;
        }
    }

    public class SelectionStatusBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "White";
            else
                return "WhiteSmoke";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }


    public class ExpanderHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is PreProcessStepViewModel))
            {
                PreProcessStepViewModel setting = (PreProcessStepViewModel)value;
                return "Step " + setting.StepCounter + " - " + setting.Name;
            }
            else
            {
                return "No Step Selected Yet!";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
