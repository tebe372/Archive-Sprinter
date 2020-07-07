using ArchiveSprinterGUI.ViewModels.SettingsViewModels;
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
    /// Interaction logic for CreatePhasorCustomization.xaml
    /// </summary>
    public partial class CreatePhasorCustomization : UserControl
    {
        public CreatePhasorCustomization()
        {
            InitializeComponent();
        }
        private void CreatePhasorTextBoxGotFocusMag(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "Mag" || b.Name == "MagPMU" || b.Name == "Magwm" || b.Name == "MagPMUwm")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void CreatePhasorTextBoxLostFocusMag(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "Mag" || b.Name == "MagPMU" || b.Name == "Magwm" || b.Name == "MagPMUwm")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }

        private void CreatePhasorTextBoxGotFocusAng(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "Ang" || b.Name == "AngPMU" || b.Name == "Angwm" || b.Name == "AngPMUwm")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void CreatePhasorTextBoxLostFocusAng(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "Ang" || b.Name == "AngPMU" || b.Name == "Angwm" || b.Name == "AngPMUwm")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }
    }
}
