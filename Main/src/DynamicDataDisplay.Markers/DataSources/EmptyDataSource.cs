using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources
{
	public sealed class EmptyDataSource : PointDataSourceBase
	{
		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			return Enumerable.Empty<Point>();
		}
	}
}
