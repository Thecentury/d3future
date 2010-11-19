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
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace NormalizedAxesSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		VerticalAxis yAxisLeft;
		VerticalAxis yAxisRight;
		HorizontalDateTimeAxis xAxis;
		double tick1, tick2;

		public Window1()
		{
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			ChartPlotter plotter = CreatePlotter();
			ConfigureYAxisLeft(plotter);
			ConfigureYAxisRight(plotter);
			ConfigureXAxis(plotter);
			AddChartGraphs(plotter);
			GetXAxisExtents(plotter);
			double buf = (tick2 - tick1) / 20;
			plotter.Viewport.Visible = new DataRect(new Point(tick1 - buf, -.05), new Point(tick2 + buf, 1.05));
			this.Content = plotter;
		}

		private void GetXAxisExtents(ChartPlotter plotter)
		{
			tick1 = double.MaxValue;
			tick2 = double.MinValue;
			foreach (LineGraph lg in plotter.Children.OfType<LineGraph>())
			{
				tick1 = smaller(tick1, lg.DataSource.GetPoints().Min(s => s.X));
				tick2 = bigger(tick2, lg.DataSource.GetPoints().Max(s => s.X));
			}
		}

		private double smaller(double d1, double d2)
		{
			if (d1 <= d2) return d1;
			else return d2;
		}

		private double bigger(double d1, double d2)
		{
			if (d1 >= d2) return d1;
			else return d2;
		}

		private void AddChartGraphs(ChartPlotter plotter)
		{
			CreatePowGraphOnLeftYAxis(plotter);
			CreateLinearGraphOnRightYAxis(plotter);
		}

		private void CreatePowGraphOnLeftYAxis(ChartPlotter plotter)
		{
			EnumerableDataSource<TPoint> edsPow = new EnumerableDataSource<TPoint>(
				 Enumerable.Range(1, 2000).Select(s =>
					 new TPoint
					 {
						 X = DateTime.Now.AddYears(-20).AddDays(s),
						 Y = Math.Pow(10, s / 700.0)
					 }
					 ).ToList());
			edsPow.SetXMapping(s => xAxis.ConvertToDouble(s.X));
			edsPow.SetYMapping(s => s.Y);

			double minData, maxData, M, B;
			GetMinAndMaxForEDS(edsPow, out minData, out maxData);
			Get_M_and_B(Math.Floor(Math.Log10(minData)), Math.Ceiling(Math.Log10(maxData)), out M, out B);

			Func<double, double> ConvertToDouble = s => M * Math.Log10(s) + B;
			Func<double, double> ConvertFromDouble = t => Math.Pow(10.0, (t - B) / M);
			Func<Point, Point> DataToViewport = s => new Point(s.X, ConvertToDouble(s.Y));
			Func<Point, Point> ViewportToData = t => new Point(t.X, ConvertFromDouble(t.Y));

			Brush brushPow = new SolidColorBrush(Colors.Green);
			Pen linePenPow = new Pen(brushPow, 2.0);
			PenDescription descPow = new PenDescription("f(x) = 10 ^ x");
			LineGraph lg = plotter.AddLineGraph(edsPow, linePenPow, descPow);

			lg.DataTransform = new LambdaDataTransform(DataToViewport, ViewportToData);
			yAxisLeft.ConvertFromDouble = ConvertFromDouble;
			yAxisLeft.ConvertToDouble = ConvertToDouble;
		}

		private void GetMinAndMaxForEDS(EnumerableDataSource<TPoint> eds, out double minData, out double maxData)
		{
			minData = eds.GetPoints().Min(s => s.Y);
			maxData = eds.GetPoints().Max(s => s.Y);
		}

		private void GetMinAndMaxForLineGraph(LineGraph lg, out double minData, out double maxData)
		{
			minData = lg.DataSource.GetPoints().Min(s => s.Y);
			maxData = lg.DataSource.GetPoints().Max(s => s.Y);
		}

		private void Get_M_and_B(double min, double max, out double M, out double B)
		{
			// @min, 0
			// @max, 1

			// y = m x + b

			// 0 = m * min + b
			// 1 = m * max + b

			// b = -m  min
			// b = 1 = m * max

			// m * min + m * max = 1
			// m = 1 / (min + max)

			// b = -m * min
			// b = -min / (min + max)



			M = 1 / (min + max);
			B = -min / (min + max);

		}

		private void CreateLinearGraphOnRightYAxis(ChartPlotter plotter)
		{
			EnumerableDataSource<TPoint> edsLinear = new EnumerableDataSource<TPoint>(
				Enumerable.Range(1, 2000).Select(s =>
					new TPoint
					{
						X = DateTime.Now.AddYears(-20).AddDays(s),
						Y = s
					}
					).ToList());
			edsLinear.SetXMapping(s => xAxis.ConvertToDouble(s.X));
			edsLinear.SetYMapping(s => s.Y);

			double minData, maxData, M, B;
			GetMinAndMaxForEDS(edsLinear, out minData, out maxData);
			Get_M_and_B(minData, maxData, out M, out B);

			Func<double, double> ConvertToDouble = s => M * s + B;
			Func<double, double> ConvertFromDouble = t => (t - B) / M;
			Func<Point, Point> DataToViewport = s => new Point(s.X, ConvertToDouble(s.Y));
			Func<Point, Point> ViewportToData = t => new Point(t.X, ConvertFromDouble(t.Y));

			Brush brushLinear = new SolidColorBrush(Colors.Blue);
			Pen linePenLinear = new Pen(brushLinear, 2.0);
			PenDescription descLinear = new PenDescription("f(x) = x");
			LineGraph lg = plotter.AddLineGraph(edsLinear, linePenLinear, descLinear);

			lg.DataTransform = new LambdaDataTransform(DataToViewport, ViewportToData);
			yAxisRight.ConvertFromDouble = ConvertFromDouble;
			yAxisRight.ConvertToDouble = ConvertToDouble;
		}

		private void ConfigureXAxis(ChartPlotter plotter)
		{
			xAxis = new HorizontalDateTimeAxis();
			plotter.MainHorizontalAxis = xAxis;
		}

		private void ConfigureYAxisLeft(ChartPlotter plotter)
		{
			yAxisLeft = new VerticalAxis
			{
				TicksProvider = new LogarithmNumericTicksProvider(10),
				LabelProvider = new UnroundingLabelProvider(),
				ConvertFromDouble = t => Math.Pow(10.0, t),
				ConvertToDouble = s => Math.Log10(s)
			};
			plotter.MainVerticalAxis = yAxisLeft;
			plotter.AxisGrid.DrawVerticalMinorTicks = true;
		}

		private void ConfigureYAxisRight(ChartPlotter plotter)
		{
			yAxisRight = new VerticalAxis()
			{
				Placement = AxisPlacement.Right
			};
			plotter.Children.Add(yAxisRight);
			yAxisRight.ConvertFromDouble = s => (s + .5) * 500.0;
			yAxisRight.ConvertToDouble = s => s / 500.0 - .5;
		}

		private static ChartPlotter CreatePlotter()
		{
			ChartPlotter plotter = new ChartPlotter();
			return plotter;
		}
	}

	public class TPoint
	{
		public TPoint() { }
		public TPoint(DateTime x, double y)
		{
			_X = x;
			_Y = y;
		}

		private DateTime _X;
		public DateTime X
		{
			get { return _X; }
			set { _X = value; }
		}

		private double _Y;
		public double Y
		{
			get { return _Y; }
			set { _Y = value; }
		}
	}
}