﻿using AS.Utilities;
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
    /// Interaction logic for AngleConversionCustomization.xaml
    /// </summary>
    public partial class AngleConversionCustomization : UserControl
    {
        public AngleConversionCustomization()
        {
            InitializeComponent();
        }
        private void ExpTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "PMU" || b.Name == "Channel")
                    {
                        b.Background = Utility.HighlightColor;
                    }
                }
            }
        }

        private void ExpTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as TextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is TextBox)
                {
                    TextBox b = item as TextBox;
                    if (b.Name == "PMU" || b.Name == "Channel")
                    {
                        b.Background = Utility.WhiteColor;
                    }
                }
            }
        }
    }
}
