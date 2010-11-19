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
using Microsoft.Research.DynamicDataDisplay.Navigation;
using System.Threading;
using System.Globalization;

namespace TypifiedPlotterSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		GenericChartPlotterOld<TimeSpan, double> plotter;
		public Window1()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

			InitializeComponent();
			plotter = new GenericChartPlotterOld<TimeSpan, double>();

			Loaded += new RoutedEventHandler(Window1_Loaded);

			//plotter.Children.RemoveAll<IPlotterElement, DefaultContextMenu>();
			grid.Children.Add(plotter);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			const int count = 60;
			var x = Enumerable.Range(0, count).Select(X => 0.1 * X);
			//var xDS = x.Select(X => new DateTime(1900 + (int)(X * 10), 1, 1)).AsXDataSource();
			var xDS = x.Select(X => new TimeSpan(0, (int)(X * 10), 0)).AsXDataSource();
			var y = x.Select(X => Math.Sin(X));
			var yDS = y.AsYDataSource();

			plotter.AddLineGraph(new CompositeDataSource(xDS, yDS), 2);

			//plotter.VisibleRect = new DataRect<DateTime, double>(
			//    new DateTime(1940, 1, 1), -1.2, new DateTime(1950, 1, 1), 1.2);
		}
	}
}
