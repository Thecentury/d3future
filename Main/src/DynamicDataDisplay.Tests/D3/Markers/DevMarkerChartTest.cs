//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.Research.DynamicDataDisplay.Charts.Markers;
//using System.Windows;
//using System.Collections.ObjectModel;
//using DynamicDataDisplay.Markers;
//using Microsoft.Research.DynamicDataDisplay;

//namespace DynamicDataDisplay.Tests.D3.Markers
//{
//    [TestClass]
//    public class MarkerChartTest
//    {
//        [TestMethod]
//        public void TestAddAndRemoveItems()
//        {
//            Assert.Inconclusive();

//            ChartPlotter plotter = new ChartPlotter();
//            plotter.PerformLoad();

//            MarkerChart chart = new MarkerChart();
//            chart.MarkerGenerator = new EllipseMarker();

//            plotter.Children.Add(chart);

//            ObservableCollection<Point> source = new ObservableCollection<Point>();
//            chart.ItemsSource = source;

//            const int count = 10;

//            // stage 1
//            for (int i = 0; i < count; i++)
//            {
//                source.Add(new Point());
//                Assert.AreEqual(source.Count, chart.Items.Count);
//            }

//            for (int i = count - 1; i >= 0; i--)
//            {
//                source.RemoveAt(i);
//                Assert.AreEqual(source.Count, chart.Items.Count);
//            }


//            // stage 2
//            for (int i = 0; i < count; i++)
//            {
//                source.Add(new Point());
//                Assert.AreEqual(source.Count, chart.Items.Count);
//            }

//            Random rnd = new Random();
//            for (int i = count - 1; i >= 0; i--)
//            {
//                int index = rnd.Next(0, source.Count - 1);
//                source.RemoveAt(index);
//                Assert.AreEqual(source.Count, chart.Items.Count);
//            }

//            // todo check that all items are visible
//        }
//    }
//}
