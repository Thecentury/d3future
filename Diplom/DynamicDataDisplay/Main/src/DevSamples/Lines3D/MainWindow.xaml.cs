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
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;

namespace Lines3D
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

		private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
		private IUniformDataSource3D<Vector3D> dataSource3D;
		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			tabControl.SelectionChanged += new SelectionChangedEventHandler(tabControl_SelectionChanged);

			PotentialField3D field = new PotentialField3D();
			field.AddPotentialPoint(new Point3D(0.5, 0.5, 0.5), 2);
			field.AddPotentialPoint(new Point3D(0.2, 0.2, 0.5), -3);
			field.AddPotentialPoint(new Point3D(0.8, 0.2, 0.9), 10);
			field.AddPotentialPoint(new Point3D(0.3, 0.7, 0.1), 5);

			PotentialFieldChart3D fieldChart = new PotentialFieldChart3D { Field = field };
			viewport.Children.Insert(0, fieldChart);

			dataSource3D = VectorField3D.CreateTangentPotentialField(field, 200, 200, 200);

			//dataSource3D = VectorField3D.CreatePotentialField(200, 200, 200,
			//    new PotentialPoint3D(new Point3D(0.5, 0.5, 0.5), 2), new PotentialPoint3D(new Point3D(0.2, 0.2, 0.5), -1));

			DataContext = dataSource3D;
			isoSurface.DataSource = dataSource3D.GetMagnitudeDataSource();


			//var spiralDS = VectorField3D.CreateSpiral(
			//    latticeX: 10, latticeY: 20, latticeZ: 3,
			//    width: 2, height: 2, depth: 1).TransformGrid(transform);

			var filteredDataSource = dataSource3D.Filter(20, 20, 20);
			//vectorChart3D.DataSource = filteredDataSource;
			//gridChart.GridSource = filteredDataSource;

			//timer.Start();

			plotterXY.DataContext = dataSource3D.CreateSectionXY(0.0);
			plotterXZ.DataContext = dataSource3D.CreateSectionXZ(0.0);
			plotterYZ.DataContext = dataSource3D.CreateSectionYZ(0.0);

			sectionChartX.ThirdCoordinate = 0.0001;
			sectionChartY.ThirdCoordinate = 0.0001;
			sectionChartZ.ThirdCoordinate = 0.0001;
			sectionChartX.UpdateUI();
			sectionChartY.UpdateUI();
			sectionChartZ.UpdateUI();

			mainTabControl.SelectedIndex = 3;
			convolutionStack.DataSource = dataSource3D;

			UpdateSelectedTab();

			progressBar.SetBinding(ProgressBar.ValueProperty, new Binding("RenderingProgress") { Source = convolutionStack });
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

		private void valueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			convolutionStack.Value = e.NewValue / 100.0;
		}
	}
}
