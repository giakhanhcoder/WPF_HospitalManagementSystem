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
using System.Windows.Shapes;

namespace WPF_VisualProgrammingHW
{
    /// <summary>
    /// Interaction logic for ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window
    {
        public ContentWindow()
        {
            InitializeComponent();
        }
        
        private Authors _authorsWindow;
        private void Authors_Click(object sender, RoutedEventArgs e)
        {
            _authorsWindow = new Authors();
            MyFrame.Content = null;
            MyFrame.Content = _authorsWindow;
        }

        private void Books_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Publishers_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Clear_Click(object sender, RoutedEventArgs e) => MyFrame.Content = null;
        private void ContentWindow_OnClosed(object sender, EventArgs e) => Environment.Exit(0);

    }
}
