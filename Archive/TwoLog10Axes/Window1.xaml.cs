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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;

namespace TwoLog10Axes
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

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.DataTransform = new Log10Transform();

			double[] xArray = new double[] { 15, 14, 16, 48, 50, 51 };
			double[] yArray = new double[] { 60, 63, 64, 124, 131, 144 };

			var xds = xArray.AsXDataSource();
			var yds = yArray.AsYDataSource();
			var ds = new CompositeDataSource(xds, yds);

			plotter.AddLineGraph(ds);


			// You can try to uncomment the following code
			/*
			HorizontalAxis xAxis = new HorizontalAxis
			{
				TicksProvider = new LogarithmNumericTicksProvider(10),
				LabelProvider = new UnroundingLabelProvider()
			};
			plotter.MainHorizontalAxis = xAxis;

			VerticalAxis yAxis = new VerticalAxis
			{
				TicksProvider = new LogarithmNumericTicksProvider(10),
				LabelProvider = new UnroundingLabelProvider()
			};
			plotter.MainVerticalAxis = yAxis;
			*/
		}
	}
}
