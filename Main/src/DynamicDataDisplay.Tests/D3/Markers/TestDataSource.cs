using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using System.Windows;

namespace DynamicDataDisplay.Tests.D3.Markers
{
	public class TestDataSource : PointDataSourceBase
	{
		public TestDataSource()
		{
			DataToPoint = o => (Point)o;
			PointToData = p => p;
		}

		protected override System.Collections.IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			yield return new Point();
			yield return new Point();
		}
	}
}
