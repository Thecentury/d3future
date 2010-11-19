using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class Vector3DExtensions
	{
		public static Vector3D DecreaseLength(this Vector3D vector, double width, double height, double depth)
		{
			vector.Normalize();
			vector.X *= width;
			vector.Y *= height;
			vector.Z *= depth;

			return vector;
		}

		public static Vector3D DecreaseLength(this Vector3D vector, Size3D size)
		{
			return vector.DecreaseLength(size.X, size.Y, size.Z);
		}

		public static Point3D ToPoint3D(this Vector3D vector)
		{
			return new Point3D(vector.X, vector.Y, vector.Z);
		}

		public static Vector3D TransformToBounds(this Vector3D vector, Rect3D bounds)
		{
			vector.X = vector.X * bounds.SizeX + bounds.X;
			vector.Y = vector.Y * bounds.SizeY + bounds.Y;
			vector.Z = vector.Z * bounds.SizeZ + bounds.Z;

			return vector;
		}
	}
}
