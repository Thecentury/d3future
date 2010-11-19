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
using vf = Microsoft.Research.DynamicDataDisplay.SampleDataSources;
using Microsoft.Research.DynamicDataDisplay.SampleDataSources;

namespace VectorField2D.App
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
			PotentialField field = new PotentialField();
			field.AddPotentialPoint(100,100,1);
			DataContext = vf.VectorField2D.CreateTangentPotentialField(field, 200, 200);
		}
	}
}
