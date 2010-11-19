using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution
{
	public sealed class CirclePattern : PointSetPattern
	{
		private double radiusRatio = 0.5;
		public double RadiusRatio
		{
			get { return radiusRatio; }
			set { radiusRatio = value; }
		}

		public override IEnumerable<Point> GeneratePoints()
		{
			double angleDelta = 2 * Math.PI / PointsCount;
			double xRadius = 0.5 * radiusRatio;
			double yRadius = 0.5 * radiusRatio;

			for (int i = 0; i < PointsCount; i++)
			{
				yield return new Point(0.5 + xRadius * Math.Cos(i * angleDelta), 0.5 + yRadius * Math.Sin(i * angleDelta));
			}
		}
	}
}
