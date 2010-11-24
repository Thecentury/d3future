using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Collections;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Filters;
using System.Diagnostics.Contracts;

namespace DynamicDataDisplay.Markers.DataSources
{
	public sealed class PointArrayDataSource : PointDataSourceBase
	{
		public PointArrayDataSource(Point[] collection)
		{
			Contract.Assert(collection != null);

			this.collection = collection;
		}

		private readonly Point[] collection;
		public Point[] Collection
		{
			get { return collection; }
		} 

		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			throw new NotImplementedException();
		}

		public override object GetDataType()
		{
			return typeof(Point);
		}
	}
}
