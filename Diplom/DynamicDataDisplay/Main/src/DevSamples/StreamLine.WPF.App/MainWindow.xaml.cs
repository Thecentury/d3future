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
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;

namespace StreamLine.WPF.App
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			var vectorField = //VectorField2D.CreateCheckerboard(200, 200);
			VectorField2D.CreateTangentPotentialField(256, 256,
				new PotentialPoint(new Point(20, 10), 1),
			new PotentialPoint(new Point(128, 128), -2),
			new PotentialPoint(new Point(65, 85), 3),
			new PotentialPoint(new Point(150, 30), 10),
			new PotentialPoint(new Point(100, 100), -5));

			DataContext = vectorField;
			streamlineChart.DataSource = vectorField;

			horizontalSection.SetBinding(HorizontalCrossSectionChart.SectionCoordinateProperty, new Binding("Position.Y") { Source = point });
			horizontalSection.Palette = convolutionChart.MagnitudeFilter.Palette;
			horizontalSection.DataSource = vectorField;

			verticalSection.SetBinding(VerticalCrossSectionChart.SectionCoordinateProperty, new Binding("Position.X") { Source = point });
			verticalSection.Palette = convolutionChart.MagnitudeFilter.Palette;
			verticalSection.DataSource = vectorField;
		}
	}
}
