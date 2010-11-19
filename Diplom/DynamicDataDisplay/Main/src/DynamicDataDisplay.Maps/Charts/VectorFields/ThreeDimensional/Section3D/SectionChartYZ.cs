using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class SectionChartYZ : Section3DChartBase
	{
		public SectionChartYZ()
		{
			UpdateUI();
		}

		public override void UpdateUI()
		{
			billboard.LowerLeft = new Point3D(ThirdCoordinate, Center.X - Width / 2, Center.Y - Height / 2);
			billboard.LowerRight = new Point3D(ThirdCoordinate,  Center.X + Width / 2, Center.Y - Height / 2);
			billboard.UpperLeft = new Point3D(ThirdCoordinate, Center.X - Width / 2, Center.Y + Height / 2);
			billboard.UpperRight = new Point3D(ThirdCoordinate, Center.X + Width / 2, Center.Y + Height / 2);

			base.UpdateUI();
		}	
	}
}
