using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class UniformField3DWrapper
	{
		private readonly IData3D<Vector3D> field;
		private readonly int width;
		private readonly int height;
		private readonly int depth;

		public UniformField3DWrapper(IData3D<Vector3D> field, int width, int height, int depth)
		{
			if (field == null)
				throw new ArgumentNullException("field");
			if (width <= 1)
				throw new ArgumentOutOfRangeException("width");
			if (height <= 1)
				throw new ArgumentOutOfRangeException("height");
			if (depth <= 1)
				throw new ArgumentOutOfRangeException("depth");

			this.field = field;
			this.width = width;
			this.height = height;
			this.depth = depth;
		}


		/// <summary>
		/// Gets the vector.
		/// </summary>
		/// <param name="x">The x, [0..1].</param>
		/// <param name="y">The y, [0..1].</param>
		/// <param name="z">The z, [0..1].</param>
		/// <returns></returns>
		public Vector3D GetVector(double x, double y, double z)
		{
			//x /= width;
			//y /= height;
			//z /= depth;

			if (x < 0 || x > 1 || y < 0 || y > 1 || z < 0 || z > 1 || x.IsNaN() || y.IsNaN() || z.IsNaN())
				return new Vector3D(Double.NaN, Double.NaN, Double.NaN);

			int realWidth = width - 1;
			int i0 = (int)(x * realWidth);
			int i1 = Math.Min(i0 + 1, realWidth);

			int realHeight = height - 1;
			int j0 = (int)(y * realHeight);
			int j1 = Math.Min(j0 + 1, realHeight);

			int realDepth = depth - 1;
			int k0 = (int)(z * realDepth);
			int k1 = Math.Min(k0 + 1, realDepth);

			double xRatio = x - i0 / (double)realWidth;
			double yRatio = y - j0 / (double)realHeight;
			double zRatio = z - k0 / (double)realDepth;

			Vector3D result =
				((1 - xRatio) * field[i0, j0, k0] + xRatio * field[i1, j0, k0] +
				(1 - xRatio) * field[i0, j1, k0] + xRatio * field[i1, j1, k0] +
				(1 - xRatio) * field[i0, j0, k1] + xRatio * field[i1, j0, k1] +
				(1 - xRatio) * field[i0, j1, k1] + xRatio * field[i1, j1, k1] +

				(1 - yRatio) * field[i0, j0, k0] + yRatio * field[i0, j1, k0] +
				(1 - yRatio) * field[i1, j0, k0] + yRatio * field[i1, j1, k0] +
				(1 - yRatio) * field[i0, j0, k1] + yRatio * field[i0, j1, k1] +
				(1 - yRatio) * field[i1, j0, k1] + yRatio * field[i1, j1, k1] +

				(1 - zRatio) * field[i0, j0, k0] + zRatio * field[i0, j0, k1] +
				(1 - zRatio) * field[i1, j0, k0] + zRatio * field[i1, j0, k1] +
				(1 - zRatio) * field[i0, j1, k0] + zRatio * field[i0, j1, k1] +
				(1 - zRatio) * field[i1, j1, k0] + zRatio * field[i1, j1, k1]
				) / 12;

			return result;
		}

		/// <summary>
		/// Gets the vector.
		/// </summary>
		/// <param name="point">The point, x=[0..1], y=[0..1], z=[0..1].</param>
		/// <returns></returns>
		public Vector3D GetVector(Point3D point)
		{
			return GetVector(point.X, point.Y, point.Z);
		}
	}
}
