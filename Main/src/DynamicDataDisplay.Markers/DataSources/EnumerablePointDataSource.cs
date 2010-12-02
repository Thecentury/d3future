using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Filters;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class EnumerablePointDataSource : PointDataSourceBase
	{
		public EnumerablePointDataSource(IEnumerable<Point> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = collection;
			TrySubscribeOnCollectionChanged(collection);

			this.DataToPoint = o => (Point)o;
			this.PointToData = p => p;
		}

		private readonly IEnumerable<Point> collection;
		public IEnumerable<Point> Collection
		{
			get { return collection; }
		}

		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			// todo possibly implement better logic here

			return collection;
		}

		public override IEnumerable<Point> GetPointData(Range<int> range)
		{
			return collection.Skip(range.Min).Take(range.GetLength());
		}

		public override object GetDataType()
		{
			return typeof(Point);
		}
	}
}
