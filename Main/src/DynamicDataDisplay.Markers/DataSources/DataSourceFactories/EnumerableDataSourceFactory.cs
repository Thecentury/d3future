using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class EnumerableDataSourceFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			IEnumerable sequence = data as IEnumerable;
			if (sequence != null)
			{
				EnumerableDataSource ds = new EnumerableDataSource(sequence);
				return ds;
			}

			return null;
		}
	}
}
