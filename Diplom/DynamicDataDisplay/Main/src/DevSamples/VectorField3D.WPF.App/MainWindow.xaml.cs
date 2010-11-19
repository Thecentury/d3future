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
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.ThreeDimensional;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;
using Microsoft.Research.DynamicDataDisplay.SampleDataSources;

namespace VectorField3D.WPF.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(MainWindow_Loaded);
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var dataSource = VectorField2D.CreateTangentPotentialField(256, 256,
				new PotentialPoint(new Point(20, 10), 1),
				new PotentialPoint(new Point(128, 128), -2),
				new PotentialPoint(new Point(65, 85), 3),
				new PotentialPoint(new Point(150, 30), 10),
				new PotentialPoint(new Point(100, 100), -5));
		
			meshChart.DataSource = dataSource;
			meshChart.Plotter.DataContext = dataSource;
		}
	}
}
