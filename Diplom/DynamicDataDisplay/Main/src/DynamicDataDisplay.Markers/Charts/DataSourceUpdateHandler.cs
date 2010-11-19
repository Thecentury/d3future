using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public abstract class DataSourceUpdateHandler
	{
		public abstract void OnUpdate(NotifyCollectionChangedEventArgs e, PointChartBase chart);

		public virtual void OnPlotterAttached(Plotter2D plotter, PointChartBase chart) { }
		public virtual void OnPlotterDetached(Plotter2D plotter, PointChartBase chart) { }
	}
}
