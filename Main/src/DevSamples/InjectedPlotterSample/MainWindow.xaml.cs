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
using Microsoft.Research.DynamicDataDisplay;

namespace InjectedPlotterSample
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{

			//InjectedPlotter injectedPlotter = new InjectedPlotter();

			//ChartPlotter plotter = new ChartPlotter();
			//plotter.Children.Add(injectedPlotter);

			//injectedPlotter.Children.Add(new HorizontalAxisTitle("123"));
			
			InitializeComponent();

			//host.Children.Add(plotter);

		}
	}
}
