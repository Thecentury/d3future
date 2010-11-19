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
using System.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace KawoneBugRepro_2
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private ObservableDataSource<Trade> _source;
		private Random _rand;

		public Window1()
		{
			InitializeComponent();
			_rand = new Random();
			EditPlotter();
		}

		void EditPlotter()
		{
			_source = new ObservableDataSource<Trade>();
			_source.SetXMapping(x => x.Counter);
			_source.SetYMapping(y => y.Price);

			var line = plotter.AddLineGraph(_source,
					new Pen(Brushes.Gold, 3),
					new PenDescription("Sin(x + phase)"));
		}

		void AddNewTrade(Trade NewTrade)
		{
			_source.AppendAsync(Dispatcher, NewTrade);
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			Thread thread = new Thread(BeginSimulation);
			thread.IsBackground = true;
			thread.Start();
		}

		void BeginSimulation()
		{
			int counter = 0;
			for (int i = 0; i < 1000000; i++)
			{
				counter++;

				Trade trade = new Trade();
				trade.Counter = counter;
				trade.Price = _rand.NextDouble();

				AddNewTrade(trade);

				Thread.Sleep(1);
			}
		}

		public class Trade
		{
			public double Counter { get; set; }
			public double Price { get; set; }
		}
	}
}