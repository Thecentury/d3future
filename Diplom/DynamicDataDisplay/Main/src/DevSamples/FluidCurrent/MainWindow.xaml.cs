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
using FluidCurrentModelling2;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Factory;
using Microsoft.Research.Science.Data.Memory;
using Microsoft.Research.Science.Data.Proxy;
using FluidCurrentModelling2.DataStructures;
using FluidCurrentModelling2.ModellingMath;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace FluidCurrent
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

		NumericalParameters parameters = new NumericalParameters(0.01, 0.02, 0.02, 0.01, 40, 40, 50, 40, 150, 0.78, 1.4);
		DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = TimeSpan.FromMilliseconds(50) };
		FluidCurrentSolver solver;
		int iterationIndex = 0;
		DataSetSource3D dataSource;


		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DataSetFactory.RegisterAssembly(typeof(MemoryDataSet).Assembly);

			solver = new FluidCurrentSolver(parameters);
			solver.Init("msds:memory");

			dataSource = new DataSetSource3D(solver.DataSet);
			dynamicStreamLine.DataSource = dataSource;

			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			convolutionChart1.AddHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler(OnRenderingFinished));
			convolutionChart2.AddHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler(OnRenderingFinished));
			convolutionChart3.AddHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler(OnRenderingFinished));

			//Iteration();
		}

		private int n = 0;
		private void OnRenderingFinished(object sender, RoutedEventArgs e)
		{
			n++;
			if (n == 3)
			{
				n = 0;
				//Dispatcher.BeginInvoke(() => { Iteration(); }, DispatcherPriority.Background);
				timer.Start();
			}
		}

		private void Iteration()
		{
			n = 0;
			if (iterationIndex >= parameters.Nt)
				return;

			iterationTextBlock.Text = iterationIndex.ToString();

			solver.PerformIteration(iterationIndex++);
			convolutionChart1.DataSource = dataSource.CreateSectionXZ(0.5);
			convolutionChart2.DataSource = dataSource.CreateSectionYZ(0.5);
			convolutionChart3.DataSource = dataSource.CreateSectionXY(0.5);
			convolutionChart4.DataSource = dataSource.CreateSectionXY(0.14);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			if (iterationIndex >= parameters.Nt)
			{
				timer.Stop();
				return;
			}

			Iteration();
			timer.Stop();
		}
	}
}
