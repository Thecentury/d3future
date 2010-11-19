using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class Viewport2DBillboardXY : Viewport2DBillboardBase
	{
		public override void UpdateUI()
		{
			double left = Center.X - Width / 2;
			double right = Center.X + Width / 2;
			double top = Center.Y + Height / 2;
			double bottom = Center.Y - Height / 2;

			double z = ThirdCoordinate;
			double z1 = ThirdCoordinate - zDelta;

			meshGeometry.Positions.Clear();
			meshGeometry.Positions.Add(new Point3D(left, bottom, z));
			meshGeometry.Positions.Add(new Point3D(right, bottom, z));
			meshGeometry.Positions.Add(new Point3D(left, top, z));
			meshGeometry.Positions.Add(new Point3D(right, top, z));
		}
	}
}
