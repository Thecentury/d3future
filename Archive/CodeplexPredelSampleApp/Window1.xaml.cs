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
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.DateTime.Strategies;

namespace CodeplexPredelSampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			ChartPlotter plotter = new ChartPlotter();

			// setting properties of horizontal axis
			HorizontalTimeSpanAxis axis = new HorizontalTimeSpanAxis();
			// calculating minimal amd maximal x values
			double minX = axis.ConvertToDouble(new TimeSpan(-24, 0, 0));
			double maxX = axis.ConvertToDouble(new TimeSpan());

			TimeSpanTicksProvider ticksProvider = (TimeSpanTicksProvider)axis.TicksProvider;
			// changing ticks calculating strategy to prefer separation on hours
			ticksProvider.Strategy = new DelegateDateTimeStrategy((span) =>
			{
				if (span.TotalDays < 2 && span.TotalHours > 2) return DifferenceIn.Hour;

				// null makes to use base class return value
				return null;
			});

			plotter.MainHorizontalAxis = axis;

			DataRect visible = plotter.Viewport.Visible;
			visible.XMin = minX;
			visible.Width = maxX - minX;

			plotter.Viewport.Visible = visible;

			// grid is simply a visual root of window
			grid.Children.Add(plotter);
		}
	}
}
