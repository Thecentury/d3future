using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class DataSourceDataSourceFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			var dataSource = data as PointDataSourceBase;
			return dataSource;
		}
	}
}
