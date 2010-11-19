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
using Microsoft.Research.DynamicDataDisplay;

namespace ZoomSliderSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private Slider zoomSlider = new Slider
		{
			Margin = new Thickness(20),
			Orientation = Orientation.Vertical,
			Height = 200,
			Minimum = 1,
			Maximum = 3
		};

		public Window1()
		{
			InitializeComponent();

			zoomSlider.ValueChanged += zoomSlider_ValueChanged;
			plotter.MainCanvas.Children.Add(zoomSlider);
		}

		void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			DataRect visible = new DataRect(0, 0, 1, 1).ZoomInToCenter(e.NewValue);
			plotter.Visible = visible;
		}
	}
}
