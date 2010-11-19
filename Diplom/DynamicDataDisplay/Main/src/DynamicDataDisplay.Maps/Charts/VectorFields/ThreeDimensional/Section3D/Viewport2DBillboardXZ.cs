using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class Viewport2DBillboardXZ : Viewport2DBillboardBase
	{
		public Viewport2DBillboardXZ()
		{
			meshGeometry.TextureCoordinates.Clear();
			meshGeometry.TextureCoordinates.Add(new Point(1, 1));
			meshGeometry.TextureCoordinates.Add(new Point(0, 1));
			meshGeometry.TextureCoordinates.Add(new Point(1, 0));
			meshGeometry.TextureCoordinates.Add(new Point(0, 0));
		}

		public override void UpdateUI()
		{
			double left = Center.X - Width / 2;
			double right = Center.X + Width / 2;
			double front = Center.Y - Height / 2;
			double back = Center.Y + Height / 2;

			double y = ThirdCoordinate;

			meshGeometry.Positions.Clear();
			meshGeometry.Positions.Add(new Point3D(left, y, back));
			meshGeometry.Positions.Add(new Point3D(right, y, back));
			meshGeometry.Positions.Add(new Point3D(left, y, front));
			meshGeometry.Positions.Add(new Point3D(right, y, front));
		}
	}
}
