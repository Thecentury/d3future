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

namespace MultipleChildren_VS_Rendering
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		Point[] pts;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			pts = CreatePts();

			DataContext = pts;
			rightChart.Points = pts;
		}

		Random rnd = new Random();
		const int num = 30000;
		private Point[] CreatePts()
		{
			Point[] res = new Point[num];

			for (int i = 0; i < num; i++)
			{
				res[i] = new Point(rnd.NextDouble(), rnd.NextDouble());
			}

			return res;
		}
	}
}
