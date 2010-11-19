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
using Microsoft.Research.DynamicDataDisplay.SampleDataSources;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Diagnostics;

namespace Convolution.App
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

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			const int size = 200;
			var dataSource = VectorField2D.CreateTangentPotentialField(size, size,
				new PotentialPoint(10, 170, 1),
				new PotentialPoint(100, 30, -1),
				new PotentialPoint(150, 130, 2)
				);

			Stopwatch timer = Stopwatch.StartNew();
			convolutionChart.AddHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler((s, args) =>
			{
				timer.Stop();
				Debug.WriteLine(timer.Elapsed);
			}));
			DataContext = dataSource;
		}
	}
}
