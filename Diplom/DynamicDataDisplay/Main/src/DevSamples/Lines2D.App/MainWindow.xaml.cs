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
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;

namespace Lines2D.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
		private readonly Stopwatch watch = Stopwatch.StartNew();
		private const int count = 50000;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(MainWindow_Loaded);
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			double delta = watch.Elapsed.TotalSeconds;
			Point3DCollection collection = new Point3DCollection(count);
			for (int i = 0; i < count; i++)
			{
				double x = i / 12500.0;
				collection.Add(new Point3D(x, Math.Sin(x + delta), 0));
			}

			polyline.Points = collection;
		}
	}
}
