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
using Microsoft.Research.DynamicDataDisplay.SampleDataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace StreamLine.WPF.App
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			convolutionChart.MagnitudeFilter.Palette = new PowerPalette(new UniformLinearPalette(Colors.Green, Colors.GreenYellow, Colors.Red));

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			var vectorField = VectorField.CreatePotentialField(100, 100,
				new PotentialPoint(new Point(0, 0), 1),
				new PotentialPoint(new Point(50, 50), -2),
				new PotentialPoint(new Point(75, 25), 3));
			DataContext = vectorField;

			streamlineChart.DataSource = vectorField;
		}
	}
}
