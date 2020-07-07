using AS.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArchiveSprinterGUI.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for PowerCalculationsCustomization.xaml
    /// </summary>
    public partial class PowerCalculationsCustomization : UserControl
    {
        public PowerCalculationsCustomization()
        {
            InitializeComponent();
        }
        private void VmagPhasorTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "VmagPMU" || b.Name == "VmagChannel" || b.Name == "VphasorPMU" || b.Name == "VphasorChannel")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void VmagPhasorTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "VmagPMU" || b.Name == "VmagChannel" || b.Name == "VphasorPMU" || b.Name == "VphasorChannel")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }

        private void ImagPhasorTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "ImagPMU" || b.Name == "ImagChannel" || b.Name == "IphasorPMU" || b.Name == "IphasorChannel")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void ImagPhasorTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "ImagPMU" || b.Name == "ImagChannel" || b.Name == "IphasorPMU" || b.Name == "IphasorChannel")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }
        private void VangTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "VangPMU" || b.Name == "VangChannel" )
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void VangTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "VangPMU" || b.Name == "VangChannel")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }

        private void IangTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "IangPMU" || b.Name == "IangChannel")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void IangTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "IangPMU" || b.Name == "IangChannel")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }
    }
}
