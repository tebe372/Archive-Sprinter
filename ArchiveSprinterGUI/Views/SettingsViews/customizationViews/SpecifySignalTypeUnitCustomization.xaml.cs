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
    /// Interaction logic for SpecifySignalTypeUnitCustomization.xaml
    /// </summary>
    public partial class SpecifySignalTypeUnitCustomization : UserControl
    {
        public SpecifySignalTypeUnitCustomization()
        {
            InitializeComponent();
        }

        private void WatermarkTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var s = sender as TextBox;
            //s.Background = Utility.WhiteColor;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "PMUnameBox" || b.Name == "SignalNameBox")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void WatermarkTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var s = sender as TextBox;
            //s.Background = Utility.WhiteColor;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "PMUnameBox" || b.Name == "SignalNameBox")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }
    }
}
