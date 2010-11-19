using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class UniformField2DWrapper
	{
		private readonly Vector[,] field;
		private readonly int width;
		private readonly int height;

		public UniformField2DWrapper(Vector[,] field)
		{
			this.field = field;
			this.width = field.GetLength(0);
			this.height = field.GetLength(1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x">x in [0..1]</param>
		/// <param name="y">y in [0..1]</param>
		/// <returns></returns>
		public Vector GetVector(double x, double y)
		{
			if (x < 0 || x > 1 || x.IsNaN())
				return new Vector(Double.NaN, Double.NaN);
			if (y < 0 || y > 1 || y.IsNaN())
				return new Vector(Double.NaN, Double.NaN);

			int realWidth = width - 1;
			int i0 = (int)(x * realWidth);
			int i1 = Math.Min(i0 + 1, realWidth);

			int realHeight = height - 1;
			int j0 = (int)(y * realHeight);
			int j1 = Math.Min(j0 + 1, realHeight);

			double xRatio = x - i0 / (double)realWidth;
			double yRatio = y - j0 / (double)realHeight;

			Vector result = 0.25 * (field[i0, j0] * (1 - xRatio - yRatio) + field[i1, j0] * (1 + xRatio - yRatio) +
				field[i0, j1] * (1 - xRatio + yRatio) + field[i1, j1] * (xRatio + yRatio));

			//Vector result = 0.25 * ((1 - xRatio) * field[i0, j0] + xRatio * field[i1, j0] +
			//    (1 - xRatio) * field[i0, j1] + xRatio * field[i1, j1] +
			//    (1 - yRatio) * field[i0, j0] + yRatio * field[i0, j1] +
			//    (1 - yRatio) * field[i1, j0] + yRatio * field[i1, j1]
			//    );

			return result;
		}


		/// <summary>
		/// Gets the vector.
		/// </summary>
		/// <param name="point">The point, x and y should be in [0..1].</param>
		/// <returns></returns>
		public Vector GetVector(Point point)
		{
			return GetVector(point.X, point.Y);
		}
	}
}
