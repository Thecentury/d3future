using System;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace BattulaRepro
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private ChartPlotter plot = new ChartPlotter();
		private HorizontalDateTimeAxis axis = new HorizontalDateTimeAxis();
		public Window1()
		{
			InitializeComponent();

			Content = plot;

			plot.MainHorizontalAxis = axis;

			const int N = 10;

			double[] x = new double[N];
			double[] y = new double[N];
			DateTime[] date = new DateTime[N];

			for (int i = 0; i < N; i++)
			{
				x[i] = i * 0.1;
				y[i] = Math.Sin(x[i]);
				date[i] = DateTime.Now.AddMinutes(-N + i);
			}

			EnumerableDataSource<double> xs = new EnumerableDataSource<double>(x);
			xs.SetYMapping(_x => _x);
			EnumerableDataSource<DateTime> ys = new EnumerableDataSource<DateTime>(date);
			ys.SetXMapping(axis.ConvertToDouble);

			CompositeDataSource ds = new CompositeDataSource(xs, ys);

			plot.AddLineGraph(ds);
		}
	}
}
