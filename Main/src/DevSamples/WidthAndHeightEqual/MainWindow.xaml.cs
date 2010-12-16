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

namespace WidthAndHeightEqual
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//plotter.SetBinding(HeightProperty, new Binding("ActualWidth") { Source = plotter });
		}
	}

	public class UniformPanel : Panel
	{
		protected override Size MeasureOverride(Size availableSize)
		{
			double min = Math.Min(availableSize.Width, availableSize.Height);
			Size minSize = new Size(min, min);

			foreach (UIElement item in InternalChildren)
			{
				item.Measure(minSize);
			}

			return minSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			double min = Math.Min(finalSize.Width, finalSize.Height);
			Size minSize = new Size(min, min);

			foreach (UIElement item in InternalChildren)
			{
				item.Arrange(new Rect(minSize));
			}

			return minSize;
		}
	}
}
