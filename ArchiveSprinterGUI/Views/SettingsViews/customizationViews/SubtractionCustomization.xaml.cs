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
using ArchiveSprinterGUI.ViewModels.SettingsViewModels;

namespace ArchiveSprinterGUI.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for SubtractionCustomization.xaml
    /// </summary>
    public partial class SubtractionCustomization : UserControl
    {
        public SubtractionCustomization()
        {
            InitializeComponent();
        }

        private void MinuendOrDividentTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            
            ((PreProcessStepViewModel)((TextBox)sender).DataContext).CurrentCursor = "Minuend";
        }

        private void SubtrahendOrDivisorTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((PreProcessStepViewModel)((TextBox)sender).DataContext).CurrentCursor = "Subtrahend";
        }

        private void MinuendOrDividentTextBoxLostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SubtrahendOrDivisorTextBoxLostFocus(object sender, RoutedEventArgs e)
        {

        }

    }
}
