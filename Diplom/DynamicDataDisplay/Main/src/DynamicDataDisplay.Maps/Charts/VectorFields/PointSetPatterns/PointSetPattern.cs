using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution
{
	public abstract class PointSetPattern
	{
		public abstract IEnumerable<Point> GeneratePoints();

		private int pointsCount = 100;
		public int PointsCount
		{
			get { return pointsCount; }
			set { pointsCount = value; }
		}
	}
}
