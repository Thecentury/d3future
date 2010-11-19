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

namespace DynamicDataDisplay.VirtualEarthDisplay
{
    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    internal partial class MenuButton : UserControl
    {
        public MenuButton()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return textBlok.Text; }
            set { textBlok.Text = value; }
        }

        public ImageSource ImageSource
        {
            get { return image.Source; }
            set
            {
                image.Source = value;
            }
        }
    }
}
