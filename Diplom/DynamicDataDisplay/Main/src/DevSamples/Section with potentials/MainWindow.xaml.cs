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
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;

namespace Section_with_potentials
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private FuncUniformDataSource3D<Vector3D> dataSource3D;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(MainWindow_Loaded);
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			PotentialField3D field = new PotentialField3D();
			field.AddPotentialPoint(new Point3D(0.5, 0.5, 0.5), 2);
			field.AddPotentialPoint(new Point3D(0.2, 0.2, 0.5), -3);
			field.AddPotentialPoint(new Point3D(0.8, 0.2, 0.9), 10);
			field.AddPotentialPoint(new Point3D(0.3, 0.7, 0.1), 5);

			dataSource3D = VectorField3D.CreateTangentPotentialField(field, 200, 200, 200);
			DataContext = dataSource3D;

			sectionChartX.ThirdCoordinate = 0.0001;
			sectionChartY.ThirdCoordinate = 0.0001;
			sectionChartZ.ThirdCoordinate = 0.0001;

			UpdateSelectedTab();

			PotentialFieldChart3D fieldChart = new PotentialFieldChart3D { Field = field };
			viewport.Children.Insert(0, fieldChart);
		}

		private void zSection_SliderValueChanged(object sender, EventArgs e)
		{
			sectionChartZ.ThirdCoordinate = zSection.SliderPercentage;
		}

		private void xSection_SliderValueChanged(object sender, EventArgs e)
		{
			sectionChartX.ThirdCoordinate = xSection.SliderPercentage;
		}

		private void ySection_SliderValueChanged(object sender, EventArgs e)
		{
			sectionChartY.ThirdCoordinate = ySection.SliderPercentage;
		}

		void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateSelectedTab();
		}

		private void UpdateSelectedTab()
		{
			sectionChartX.Border.Color = Section3DChartBase.DefaultColor;
			sectionChartY.Border.Color = Section3DChartBase.DefaultColor;
			sectionChartZ.Border.Color = Section3DChartBase.DefaultColor;

			switch (tabControl.SelectedIndex)
			{
				case 0:
					sectionChartZ.Border.Color = Section3DChartBase.SelectedColor;
					break;
				case 1:
					sectionChartY.Border.Color = Section3DChartBase.SelectedColor;
					break;
				case 2:
					sectionChartX.Border.Color = Section3DChartBase.SelectedColor;
					break;
				default:
					break;
			}
		}

	}
}
