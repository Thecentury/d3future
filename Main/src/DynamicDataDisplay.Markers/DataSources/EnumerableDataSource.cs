using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class EnumerableDataSource : PointDataSourceBase
	{
		public EnumerableDataSource(IEnumerable collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = collection;
			TrySubscribeOnCollectionChanged(collection);
		}

		private readonly IEnumerable collection;

		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			return collection;
		}

		public override object GetDataType()
		{
			throw new NotImplementedException();
		}
	}
}
