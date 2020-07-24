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
            string key = null;
            if (values[1] != DependencyProperty.UnsetValue)
            {
                key = (string)values[1];
            }
            var dict = (Dictionary<string, List<string>>)values[0];
            if (key is string && !string.IsNullOrEmpty(key) && (dict) is Dictionary<string, List<string>>)
                return dict[key];
            else
                return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class UnitMetricConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.ToString();
            switch (type)
            {
                case "VMP":
                case "VMA":
                case "VMB":
                case "VMC":
                case "VPP":
                case "VPA":
                case "VPB":
                case "VPC":
                    {
                        return new List<string>() { "kV", "V" }.ToList();
                    }

                case "IMP":
                case "IMA":
                case "IMB":
                case "IMC":
                case "IPP":
                case "IPA":
                case "IPB":
                case "IPC":
                    {
                        return new List<string>() { "A", "kA" }.ToList();
                    }

                case "R":
                    {
                        return new List<string>() { "Hz/sec", "mHz/sec" }.ToList();
                    }

                case "F":
                    {
                        return new List<string>() { "Hz", "mHz" }.ToList();
                    }

                case "P":
                    {
                        return new List<string>() { "W", "kW", "MW" }.ToList();
                    }

                case "Q":
                    {
                        return new List<string>() { "VAR", "kVAR", "MVAR" }.ToList();
                    }

                case "CP":
                case "S":
                    {
                        return new List<string>() { "VA", "kVA", "MVA" }.ToList();
                    }

                default:
                    {
                        return DependencyProperty.UnsetValue;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
