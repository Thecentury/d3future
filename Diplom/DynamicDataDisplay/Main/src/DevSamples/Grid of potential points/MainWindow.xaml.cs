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

namespace Grid_of_potential_points
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

		const double width = 400;
		const double height = 400;
		const int xCount = 10;
		const int yCount = 10;
		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			const double xDelta = width / xCount;
			const double yDelta = height / yCount;
			PotentialField field = new PotentialField();
			for (int ix = 0; ix < xCount; ix++)
			{
				for (int iy = 0; iy < yCount; iy++)
				{
					field.AddPotentialPoint(new Point(ix * xDelta, iy * yDelta), (ix + iy) % 2 == 0 ? 1 : -1);
				}
			}

			DataContext = VectorField2D.CreateTangentPotentialField(field, (int)width, (int)height);
		}
	}
}
