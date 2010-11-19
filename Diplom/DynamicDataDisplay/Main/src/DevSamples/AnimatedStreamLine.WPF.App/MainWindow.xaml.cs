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
using Microsoft.Research.DynamicDataDisplay.SampleDataSources;
using System.Diagnostics;

namespace AnimatedStreamLine.WPF.App
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

		private readonly PotentialField field = new PotentialField();
		private readonly DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
		private readonly Stopwatch stopwatch = Stopwatch.StartNew();
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			DataContext = VectorField2D.CreateDynamicTangentPotentialField(field);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			double time = stopwatch.Elapsed.TotalSeconds / 10;
			field.Clear();

			field.AddPotentialPoint(100 + 50 * Math.Cos(time) + 10 * Math.Cos(2.1 * time), 100 + 40 * Math.Sin(time), 1);
			field.AddPotentialPoint(20, 10, -1);
			field.AddPotentialPoint(180, 20, 3);
			field.AddPotentialPoint(20, 180, -0.5);
			field.AddPotentialPoint(180, 180, 2);

			field.RaiseChanged();
		}
	}
}
