using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class VectorField3DExtensions
	{
		public static IDataSource2D<Vector> CreateSectionXY(this IUniformDataSource3D<Vector3D> dataSource, double ratio)
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");
			if (ratio < 0 || ratio > 1)
				throw new IndexOutOfRangeException("ratio should be in [0..1].");

			int z0 = (int)Math.Floor(dataSource.Depth * ratio);
			int z1 = Math.Min(z0 + 1, dataSource.Depth - 1);

			double zRatio = dataSource.Depth * ratio - z0;
			if (zRatio < 0 || zRatio > 1)
				throw new ArgumentOutOfRangeException();

			int width = dataSource.Width;
			int height = dataSource.Height;
			var data = dataSource.Data;
			Vector[,] dataArray = new Vector[width, height];

			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					Vector3D vec3D = data[ix, iy, z0] * (1 - zRatio) + data[ix, iy, z1] * zRatio;
					dataArray[ix, iy] = new Vector(vec3D.X, vec3D.Y);
				}
			}

			NonUniformDataSource2D<Vector> result = new NonUniformDataSource2D<Vector>(dataSource.XCoordinates, dataSource.YCoordinates, dataArray);

			return result;
		}

		public static IDataSource2D<Vector> CreateSectionXZ(this IUniformDataSource3D<Vector3D> dataSource, double ratio)
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");
			if (ratio < 0 || ratio > 1)
				throw new IndexOutOfRangeException("ratio should be in [0..1].");

			int y0 = (int)Math.Floor(dataSource.Height * ratio);
			int y1 = Math.Min(y0 + 1, dataSource.Height - 1);

			double yRatio = dataSource.Height * ratio - y0;
			if (yRatio < 0 || yRatio > 1)
				throw new ArgumentOutOfRangeException();

			int width = dataSource.Width;
			int depth = dataSource.Depth;
			var data = dataSource.Data;
			Vector[,] dataArray = new Vector[width, depth];

			for (int ix = 0; ix < width; ix++)
			{
				for (int iz = 0; iz < depth; iz++)
				{
					Vector3D vec3D = data[ix, y0, iz] * (1 - yRatio) + data[ix, y1, iz] * yRatio;
					dataArray[ix, iz] = new Vector(vec3D.X, vec3D.Z);
				}
			}

			NonUniformDataSource2D<Vector> result = new NonUniformDataSource2D<Vector>(dataSource.XCoordinates, dataSource.ZCoordinates, dataArray);

			return result;
		}

		public static IDataSource2D<Vector> CreateSectionYZ(this IUniformDataSource3D<Vector3D> dataSource, double ratio)
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");
			if (ratio < 0 || ratio > 1)
				throw new IndexOutOfRangeException("ratio should be in [0..1].");

			int x0 = (int)Math.Floor(dataSource.Width * ratio);
			int x1 = Math.Min(x0 + 1, dataSource.Width - 1);

			double xRatio = dataSource.Width * ratio - x0;
			if (xRatio < 0 || xRatio > 1)
				throw new ArgumentOutOfRangeException();

			int height = dataSource.Height;
			int depth = dataSource.Depth;
			var data = dataSource.Data;
			Vector[,] dataArray = new Vector[height, depth];

			for (int iy = 0; iy < height; iy++)
			{
				for (int iz = 0; iz < depth; iz++)
				{
					Vector3D vec3D = data[x0, iy, iz] * (1 - xRatio) + data[x1, iy, iz] * xRatio;
					dataArray[iy, iz] = new Vector(vec3D.Y, vec3D.Z);
				}
			}

			NonUniformDataSource2D<Vector> result = new NonUniformDataSource2D<Vector>(dataSource.YCoordinates, dataSource.ZCoordinates, dataArray);

			return result;
		}
	}
}
