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

namespace VoronoiSample
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

		const int count = 3;
		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			polygon.Points.Changed += new EventHandler(Points_Changed);
			//Random rnd = new Random();

			//PointCollection points = new PointCollection();
			//for (int i = 0; i < count; i++)
			//{
			//    Point p = new Point(rnd.NextDouble(), rnd.NextDouble());
			//    points.Add(p);
			//}
			//polygon.Points = points;
			//voronoiChart.Points = points;
		}

		void Points_Changed(object sender, EventArgs e)
		{
			voronoiChart.Points = polygon.Points;
		}
	}
}
