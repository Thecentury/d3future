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
using Microsoft.Research.DynamicDataDisplay.DirectX11;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DynamicDataDisplay.DirectX.SampleApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        ChartPlotter plotter = new ChartPlotter();
        DXLineGraph lg = new DXLineGraph();

        public Window1()
        {
            InitializeComponent();

            
        }

        private IPointDataSource CreateSineDataSource(double phase)
        {
            const int N = 100000;

            Point[] pts = new Point[N];
            Random r = new Random();
            for (int i = 0; i < N; i++)
            {
                double x = 10.0* i / N + phase;
                pts[i] = new Point(x, Math.Sin(x - phase) + (r.NextDouble()-0.5)/1000);
            }

            var ds = new EnumerableDataSource<Point>(pts);
            ds.SetXYMapping(pt => pt);

            return ds;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Content = plotter;
            plotter.Children.Add(lg);

            lg.DataSource = CreateSineDataSource(0);
        }
    }
}
