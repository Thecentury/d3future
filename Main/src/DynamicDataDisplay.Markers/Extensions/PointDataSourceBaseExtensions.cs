using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicDataDisplay.Markers.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Markers.Extensions
{
	public static class PointDataSourceBaseExtensions
	{
		public static IEnumerable<Point> GetPoints(this PointDataSourceBase dataSource, DataSourceEnvironment environment)
		{
			return dataSource.GetData(environment).Cast<object>().Select(o => dataSource.DataToPoint(o));
		}
	}
}
