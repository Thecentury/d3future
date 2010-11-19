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

namespace FluidDynamics
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

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//var field = VectorField2D.CreateTangentPotentialField(256, 256,
			//                new PotentialPoint(new Point(20, 10), 1),
			//                new PotentialPoint(new Point(128, 128), -2),
			//                new PotentialPoint(new Point(65, 85), 3),
			//                new PotentialPoint(new Point(150, 30), 10),
			//                new PotentialPoint(new Point(100, 100), -5));
			var field = VectorField2D.CreateCheckerboard(cellSize: 20);

			DataContext = field;

			int size = 102;
			for (int ix = 0; ix < size; ix++)
			{
				for (int iy = 0; iy < size; iy++)
				{
					//if (Math.Abs(size / 2 - ix) < size / 5 && Math.Abs(size / 2 - iy) < size / 5)
					if (Math.Abs(size / 3 - iy) < 5 && Math.Abs(ix - (ix / 10) * 10) < 2)
						fluidImage.Solver.OccupiedCells[iy * size + ix] = true;
				}
			}
		}
	}
}
