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
using Microsoft.Research.DynamicDataDisplay.Charts.Filters;
using System.Windows.Threading;
using DynamicDataDisplay.Markers;

namespace NewMarkersSample2
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			timer.Tick += new EventHandler(timer_Tick);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			for (int i = 0; i < count; i++)
			{
				pts[i] = new Point(i, Math.Sin(Environment.TickCount / 1000.0 + i * 0.01));
			}
			chart.DataSource.RaiseCollectionReset();
			chart.MarkerBuilder = new DiamondMarker();
		}

		DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };

		const int count = 1000;
		Point[] pts = new Point[count];
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			chart.Filters.Add(new ParallelClusteringFilter());

			for (int i = 0; i < count; i++)
			{
				pts[i] = new Point(i, Math.Sin(i * 0.01));
			}

			DataContext = pts;
			timer.Start();
		}
	}
}
