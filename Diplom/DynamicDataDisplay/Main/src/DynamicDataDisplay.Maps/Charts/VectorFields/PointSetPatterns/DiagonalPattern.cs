using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution
{
	public sealed class DiagonalPattern : PointSetPattern
	{
		public override IEnumerable<Point> GeneratePoints()
		{
			double xDelta = 1.0 / (PointsCount + 1);
			double yDelta = 1.0 / (PointsCount + 1);

			for (int i = 0; i < PointsCount; i++)
			{
				yield return new Point((i + 0.5) * xDelta, (i + 0.5) * yDelta);
			}
		}
	}
}
