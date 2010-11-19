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
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.SampleDataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Section_LIC
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

		private IUniformDataSource3D<Vector3D> dataSource3D;

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			PotentialField3D field = new PotentialField3D();
			field.AddPotentialPoint(new Point3D(0.5, 0.5, 0.5), 2);
			field.AddPotentialPoint(new Point3D(0.2, 0.2, 0.5), -3);
			field.AddPotentialPoint(new Point3D(0.8, 0.2, 0.9), 10);
			field.AddPotentialPoint(new Point3D(0.3, 0.7, 0.1), 5);

			dataSource3D = VectorField3D.CreateTangentPotentialField(field, 200, 200, 200);

			plotterXY.DataContext = dataSource3D.CreateSectionXY(0.0);
			plotterXZ.DataContext = dataSource3D.CreateSectionXZ(0.0);
			plotterYZ.DataContext = dataSource3D.CreateSectionYZ(0.0);
		}

		private void xSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (dataSource3D == null) return;

			var value = e.NewValue / 100;
			plotterXY.DataContext = dataSource3D.CreateSectionXY(value);
			vpXY.ThirdCoordinate = value;
		}

		private void ySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (dataSource3D == null) return;

			var value = e.NewValue / 100;
			plotterXZ.DataContext = dataSource3D.CreateSectionXZ(value);
			vpXZ.ThirdCoordinate = value;
		}

		private void zSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (dataSource3D == null) return;

			var value = e.NewValue / 100;
			plotterYZ.DataContext = dataSource3D.CreateSectionYZ(value);
			vpYZ.ThirdCoordinate = value;
		}
	}
}
