using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class Viewport2DBillboardYZ : Viewport2DBillboardBase
	{
		public Viewport2DBillboardYZ()
		{
			meshGeometry.TextureCoordinates.Clear();
			meshGeometry.TextureCoordinates.Add(new Point(1, 0));
			meshGeometry.TextureCoordinates.Add(new Point(0, 0));
			meshGeometry.TextureCoordinates.Add(new Point(1, 1));
			meshGeometry.TextureCoordinates.Add(new Point(0, 1));
		}

		public override void UpdateUI()
		{
			double top = Center.X + Height / 2;
			double bottom = Center.Y - Height / 2;
			double front = Center.Y - Height / 2;
			double back = Center.Y + Height / 2;

			double x = ThirdCoordinate;

			meshGeometry.Positions.Clear();
			meshGeometry.Positions.Add(new Point3D(x, top, back));
			meshGeometry.Positions.Add(new Point3D(x, bottom, back));
			meshGeometry.Positions.Add(new Point3D(x, top, front));
			meshGeometry.Positions.Add(new Point3D(x, bottom, front));
		}
	}
}
