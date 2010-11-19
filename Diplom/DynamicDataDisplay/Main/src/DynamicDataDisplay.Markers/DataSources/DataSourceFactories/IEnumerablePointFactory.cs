using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public class IEnumerablePointFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			IEnumerable<Point> collection = data as IEnumerable<Point>;
			if (collection != null)
			{
				var dataSource = new EnumerablePointDataSource(collection);
				return dataSource;
			}

			return null;
		}
	}
}
