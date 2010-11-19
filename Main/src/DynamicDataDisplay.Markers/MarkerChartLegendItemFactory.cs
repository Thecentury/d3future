//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Research.DynamicDataDisplay.Charts;
//using Microsoft.Research.DynamicDataDisplay.Charts.Markers;

//namespace DynamicDataDisplay.Markers
//{
//    internal sealed class MarkerChartLegendItemFactory 
//    {
//        public IDisposable TryBuildItem(object chart, NewLegend legend)
//        {
//            MarkerChart markerChart = chart as MarkerChart;
//            if (markerChart == null)
//                return null;

//            MarkerChartLegendItem legendItem = new MarkerChartLegendItem(markerChart, legend);
//            return legendItem;
//        }
//    }
//}
