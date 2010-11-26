using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Markers2;
using Microsoft.Research.DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Controls;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicDataDisplay.Tests.D3.Markers
{
	[TestClass]
	public class LineChartTest
	{
		private Panel canvas;
		private LineChart chart;
		private IEnumerable<Path> paths;

		public LineChartTest()
		{
			chart = new LineChart();
			chart.DataSource = new TestDataSource();

			ChartPlotter plotter = new ChartPlotter();
			plotter.PerformLoad();
			plotter.Children.Add(chart);

			PrivateObject privateObject = new PrivateObject(chart);
			LineChart_Accessor target = new LineChart_Accessor(privateObject);

			canvas = target.canvas;

			paths = canvas.Children.Cast<Path>();
		}

		[TestMethod]
		public void SettingLineStroke()
		{
			chart.Stroke = Brushes.Purple;
			chart.Wait(DispatcherPriority.Background);

			foreach (var path in paths)
			{
				Assert.AreEqual(chart.Stroke, path.Stroke);
			}
		}

		[TestMethod]
		public void SettingLineThickness()
		{
			chart.StrokeThickness = 2.1;
			chart.Wait(DispatcherPriority.Background);

			foreach (var path in paths)
			{
				Assert.AreEqual(chart.StrokeThickness, path.StrokeThickness);
			}
		}

		[TestMethod]
		public void SettingStrokeDashArray()
		{
			chart.StrokeDashArray = new DoubleCollection(new double[] { 1, 1 });
			chart.Wait(DispatcherPriority.Background);

			foreach (var path in paths)
			{
				Assert.AreEqual(chart.StrokeDashArray, path.StrokeDashArray);
			}
		}
	}
}
