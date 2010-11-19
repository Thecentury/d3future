using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Petzold.Media3D;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class SectionChartXY : Section3DChartBase
	{
		public SectionChartXY()
		{
			UpdateUI();
		}

		public override void UpdateUI()
		{
			billboard.LowerLeft = new Point3D(Center.X - Width / 2, Center.Y - Height / 2, ThirdCoordinate);
			billboard.LowerRight = new Point3D(Center.X + Width / 2, Center.Y - Height / 2, ThirdCoordinate);
			billboard.UpperLeft = new Point3D(Center.X - Width / 2, Center.Y + Height / 2, ThirdCoordinate);
			billboard.UpperRight = new Point3D(Center.X + Width / 2, Center.Y + Height / 2, ThirdCoordinate);

			base.UpdateUI();
		}
	}
}
