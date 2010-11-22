using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	/// <summary>
	/// Provides interpolated values for uniform fields in a given field position.
	/// </summary>
	internal sealed class UniformFieldWrapper
	{
		private readonly Vector[,] field;
		private readonly int width;
		private readonly int height;

		public UniformFieldWrapper(Vector[,] field, int width, int height)
		{
			this.field = field;
			this.width = width;
			this.height = height;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x">x in [0..1]</param>
		/// <param name="y">y in [0..1]</param>
		/// <returns></returns>
		public Vector GetVector(double x, double y)
		{
			x /= width;
			y /= height;

			if (x >= 1 || x <= 0 || y <= 0 || y >= 1 || x.IsNaN() || y.IsNaN())
				return new Vector(Double.NaN, Double.NaN);

			int realWidth = width - 1;
			int i0 = (int)(x * realWidth);
			int i1 = Math.Min(i0 + 1, realWidth);

			int realHeight = height - 1;
			int j0 = (int)(y * realHeight);
			int j1 = Math.Min(j0 + 1, realHeight);

			double xRatio = x - i0 / (double)realWidth;
			double yRatio = y - j0 / (double)realHeight;

			Vector result = 0.25 * ((1 - xRatio) * field[i0, j0] + xRatio * field[i1, j0] +
				(1 - xRatio) * field[i0, j1] + xRatio * field[i1, j1] +
				(1 - yRatio) * field[i0, j0] + yRatio * field[i0, j1] +
				(1 - yRatio) * field[i1, j0] + yRatio * field[i1, j1]
				);

			return result;
		}

		public Vector GetVector(Point point)
		{
			return GetVector(point.X, point.Y);
		}
	}
}
