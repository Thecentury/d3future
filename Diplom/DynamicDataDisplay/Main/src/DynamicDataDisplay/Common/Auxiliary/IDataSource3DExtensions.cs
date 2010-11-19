using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class IDataSource3DExtensions
	{
		public static TransformedGridDataSource3D<T> TransformGrid<T>(this IDataSource3D<T> dataSource, Transform3D transform)
		{
			return new TransformedGridDataSource3D<T>(dataSource, transform);
		}

		public static FilteringDataSource<T> Filter<T>(this IDataSource3D<T> dataSource, int xStep = 2, int yStep = 2, int zStep = 2)
		{
			return new FilteringDataSource<T>(dataSource, xStep, yStep, zStep);
		}

		public static T[, ,] DataToArray<T>(this IDataSource3D<T> dataSource)
		{
			T[, ,] result = new T[dataSource.Width, dataSource.Height, dataSource.Depth];

			var data = dataSource.Data;

			Parallel.For(0, dataSource.Width, ix =>
			{
				for (int iy = 0; iy < dataSource.Height; iy++)
				{
					for (int iz = 0; iz < dataSource.Depth; iz++)
					{
						result[ix, iy, iz] = data[ix, iy, iz];
					}
				}
			});

			return result;
		}

		public static IDataSource3D<double> GetMagnitudeDataSource(this IDataSource3D<Vector3D> dataSource)
		{
			return new VectorToMagnitudeDataSource3D(dataSource);
		}
	}
}
