﻿using System;
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
using AS.Utilities;
using Xceed.Wpf.Toolkit;

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
            var s = sender as WatermarkTextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is WatermarkTextBox)
                {
                    WatermarkTextBox b = item as WatermarkTextBox;
                    b.Background = Utility.HighlightColor;
                } 
            }
        }

        private void SubtrahendOrDivisorTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((PreProcessStepViewModel)((TextBox)sender).DataContext).CurrentCursor = "Subtrahend";
            var s = sender as WatermarkTextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is WatermarkTextBox)
                {
                    WatermarkTextBox b = item as WatermarkTextBox;
                    b.Background = Utility.HighlightColor;
                }
            }
        }

        private void MinuendOrDividentTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as WatermarkTextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is WatermarkTextBox)
                {
                    WatermarkTextBox b = item as WatermarkTextBox;
                    b.Background = Utility.WhiteColor;
                }
            }

        }

        private void SubtrahendOrDivisorTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as WatermarkTextBox;
            StackPanel p = s.Parent as StackPanel;
            foreach (var item in p.Children)
            {
                if (item is WatermarkTextBox)
                {
                    WatermarkTextBox b = item as WatermarkTextBox;
                    b.Background = Utility.WhiteColor;
                }
            }
        }

    }
}
