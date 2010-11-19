using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class Point3DExtensions
	{
		public static Point3D TransformToBounds(this Point3D point, Rect3D bounds)
		{
			point.X = point.X * bounds.SizeX + bounds.X;
			point.Y = point.Y * bounds.SizeY + bounds.Y;
			point.Z = point.Z * bounds.SizeZ + bounds.Z;

			return point;
		}

		public static Point3D TransformTo01(this Point3D point, Rect3D bounds)
		{
			point.X = (point.X - bounds.X) / bounds.SizeX;
			point.Y = (point.Y - bounds.Y) / bounds.SizeY;
			point.Z = (point.Z - bounds.Z) / bounds.SizeZ;

			return point;
		}
	}
}
