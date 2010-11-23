using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class PointArrayFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			Point[] array = data as Point[];
			if (array != null)
			{
				var dataSource = new PointArrayDataSource(array);
				return dataSource;
			}

			return null;
		}
	}
}
