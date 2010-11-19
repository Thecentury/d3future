using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DynamicDataDisplay.VirtualEarthDisplay
{
    /// <summary>
    /// Interaction logic for Requirements.xaml
    /// </summary>
    internal partial class Requirements : UserControl
    {
        public Requirements()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://go.microsoft.com/fwlink/?LinkId=106129");
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("http://go.microsoft.com/fwlink/?LinkId=106130");
        }
    }
}
