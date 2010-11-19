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
using System.Windows.Media.Media3D;

namespace ConvolutionStack
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
			PotentialField3D field = new PotentialField3D();
			field.AddPotentialPoint(new Point3D(0.5, 0.5, 0.5), 2);
			field.AddPotentialPoint(new Point3D(0.2, 0.2, 0.5), -3);
			field.AddPotentialPoint(new Point3D(0.8, 0.2, 0.9), 10);
			field.AddPotentialPoint(new Point3D(0.3, 0.7, 0.1), 5);

			var dataSource3D = VectorField3D.CreateTangentPotentialField(field, 200, 200, 200);

			convolutionStack.DataSource = dataSource3D;

			progressBar.SetBinding(ProgressBar.ValueProperty, new Binding("RenderingProgress") { Source = convolutionStack });
		}

		private void valueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			convolutionStack.Value = e.NewValue / 100.0;
		}
	}
}
