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
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace KawoneBugRepro
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private List<Trade> _listOfTrade;
		private List<Trade> _listMarker;

		public Window1()
		{
			InitializeComponent();

			_listOfTrade = new List<Trade>();
			_listMarker = new List<Trade>();
			BeginSimulation();
			EditGraph();
		}
		public void BeginSimulation()
		{
			Random random = new Random();

			for (int counter = 1; counter < 1000; counter++)
			{
				Trade trade = new Trade(counter, random.NextDouble());

				_listOfTrade.Add(trade);

				if (counter % 10 == 0)
				{
					_listMarker.Add(trade);
				}
			}
		}

		public void EditGraph()
		{
			ObservableDataSource<Trade> source = new ObservableDataSource<Trade>(_listOfTrade);
			source.SetXMapping(c => c.Counter);
			source.SetYMapping(d => d.Price);

			ObservableDataSource<Trade> sourceMarker = new ObservableDataSource<Trade>(_listMarker);
			sourceMarker.SetXMapping(c => c.Counter);
			sourceMarker.SetYMapping(d => d.Price);

			plotter.AddLineGraph(source, new Pen(Brushes.Gold, 3), new PenDescription("chart"));

			LineAndMarker<MarkerPointsGraph> chartMarkerBuy = plotter.AddLineGraph(
				sourceMarker,
				new Pen(Brushes.Red, 3),
				new TrianglePointMarker { Size = 5, Fill = Brushes.Blue },
				new PenDescription("marker"));
			//chartMarkerBuy.LineGraph.DataSource = null;

			plotter.Viewport.FitToView();
		}
	}

	public class Trade
	{
		public double Price { get; set; }
		public double Counter { get; set; }

		public Trade(double counter, double price)
		{
			Price = price;
			Counter = counter;
		}
	}
}
