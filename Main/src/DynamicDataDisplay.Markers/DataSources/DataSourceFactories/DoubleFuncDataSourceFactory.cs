using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;
using DynamicDataDisplay.Markers.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class DoubleFuncDataSourceFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			Func<double, double> func = data as Func<double, double>;
			if (func != null)
			{
				DoubleFuncDataSource ds = new DoubleFuncDataSource(func);
				return ds;
			}

			return null;
		}
	}
}
