using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class GenericIDataSource2DFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			var types = IEnumerableHelper.GetGenericInterfaceArgumentTypes(data, typeof(IDataSource2D<>));

			if (types != null && types.Length > 0)
			{
				Type genericDataSource2DType = typeof(GenericDataSource2D<>).MakeGenericType(types);
				var result = Activator.CreateInstance(genericDataSource2DType, data);
				var dataSource = (PointDataSourceBase)result;
				return dataSource;
			}

			return null;
		}
	}
}
