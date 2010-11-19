using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class BorderPattern : PointSetPattern
	{
		private double borderDistanceRatio = 0.01;
		public double BorderDistanceRatio
		{
			get { return borderDistanceRatio; }
			set { borderDistanceRatio = value; }
		}

		public override IEnumerable<Point> GeneratePoints()
		{
			int smallSideCount = PointsCount / 4;
			int largeSideCount = smallSideCount + 2;

			for (int i = 0; i < largeSideCount; i++)
			{
				double x = i / (largeSideCount + 1.0) + 0.5 / (largeSideCount + 1);
				yield return new Point(x, borderDistanceRatio);
				yield return new Point(x, 1 - borderDistanceRatio);
			}

			for (int i = 0; i < smallSideCount; i++)
			{
				double y = i / (smallSideCount + 3.0) + 1.5 / (largeSideCount + 1);
				yield return new Point(borderDistanceRatio, y);
				yield return new Point(1 - borderDistanceRatio, y);
			}
		}
	}
}
