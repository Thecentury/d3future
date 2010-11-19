using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class VectorExtensions
	{
		public static Point ToPoint(this Vector vector)
		{
			return new Point(vector.X, vector.Y);
		}

		public static Vector ChangeLength(this Vector vector, double width, double height)
		{
			vector.Normalize();
			vector.X *= width;
			vector.Y *= height;

			return vector;
		}

		public static Vector ChangeLength(this Vector vector, double width, double heigth, int gridWidth, int gridHeight)
		{
			double lengthFactor = Math.Sqrt(width * width / (gridWidth * gridWidth) + heigth * heigth / (gridHeight * gridHeight)) / Math.Sqrt(2);
			vector.Normalize();
			vector *= lengthFactor;

			return vector;
		}

		public static Vector Perpendicular(this Vector v)
		{
			var result = Vector3D.CrossProduct(new Vector3D(v.X, v.Y, 0), new Vector3D(0, 0, 1));
			return new Vector(result.X, result.Y);
		}
	}
}
