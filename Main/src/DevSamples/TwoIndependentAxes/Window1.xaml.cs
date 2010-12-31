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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.ViewportConstraints;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using Microsoft.Research.DynamicDataDisplay.Markers2;

namespace TwoIndependentAxes
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			var rpms = Enumerable.Range(0, 9).Select(i => i * 1000.0);
			var hps = new double[] { 0, 24, 52, 74, 98, 112, 124, 122, 116 };

			var horsePowersDS = DataSource.Create(rpms, hps);
			// shown at main ChartPlotter
			var hpsLine = plotter.AddLineChart(horsePowersDS).
				WithStroke(Brushes.Red).
				WithStrokeThickness(2).
				WithDescription("HP per RPM");

			var torque = new double[] { 0, 22, 45, 54, 58, 55, 50, 47, 45 };
			var torqueDS = DataSource.Create(rpms, torque);

			// shown at inner InjectedPlotter
			var line = innerPlotter.AddLineChart(torqueDS).
				WithStroke(Brushes.Blue).
				WithStrokeThickness(2).
				WithDescription("Torque per RPM");

			var values = new double[] { 10, 9, 7, 8, 5, 6, 4, 3, 2, 1 };
			var valuesDS = DataSource.Create(rpms, values);

			// shown at inner DependentPlotter
			dependentPlotter.AddLineChart(valuesDS).
				WithStroke(Brushes.LawnGreen).
				WithStrokeThickness(2).
				WithDescription("Some fake values");
		}

		private void removeAllChartsBtn_Click(object sender, RoutedEventArgs e)
		{
			innerPlotter.Children.RemoveAll<LineChart>();
			plotter.Children.RemoveAll<LineChart>();
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if (None.IsChecked == true)
				innerPlotter.ConjunctionMode = ViewportConjunctionMode.None;
			else if (X.IsChecked == true)
				innerPlotter.ConjunctionMode = ViewportConjunctionMode.X;
			else if (Y.IsChecked == true)
				innerPlotter.ConjunctionMode = ViewportConjunctionMode.Y;
			else if (XY.IsChecked == true)
				innerPlotter.ConjunctionMode = ViewportConjunctionMode.XY;
		}

		private void DependentRadioButton_Checked(object sender, RoutedEventArgs e) {
			if (None2.IsChecked == true)
				dependentPlotter.ConjunctionMode = ViewportConjunctionMode.None;
			else if (X2.IsChecked == true)
				dependentPlotter.ConjunctionMode = ViewportConjunctionMode.X;
			else if (Y2.IsChecked == true)
				dependentPlotter.ConjunctionMode = ViewportConjunctionMode.Y;
			else if (XY2.IsChecked == true)
				dependentPlotter.ConjunctionMode = ViewportConjunctionMode.XY;
		}
	}
}
