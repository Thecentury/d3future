using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class SectionChartXZ : Section3DChartBase
	{
		public SectionChartXZ()
		{
			UpdateUI();
		}

		public override void UpdateUI()
		{
			billboard.LowerLeft = new Point3D(Center.X - Width / 2, ThirdCoordinate, Center.Y - Height / 2);
			billboard.LowerRight = new Point3D(Center.X + Width / 2, ThirdCoordinate, Center.Y - Height / 2);
			billboard.UpperLeft = new Point3D(Center.X - Width / 2, ThirdCoordinate, Center.Y + Height / 2);
			billboard.UpperRight = new Point3D(Center.X + Width / 2, ThirdCoordinate, Center.Y + Height / 2);

			base.UpdateUI();
		}
	}
}
