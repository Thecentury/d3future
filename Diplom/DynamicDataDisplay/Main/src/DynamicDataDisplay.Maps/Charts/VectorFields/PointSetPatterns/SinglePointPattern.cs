using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution
{
	public sealed class SinglePointPattern : PointSetPattern
	{
		private Point point = new Point(0.5, 0.5);
		public Point Point
		{
			get { return point; }
			set { point = value; }
		}

		public override IEnumerable<Point> GeneratePoints()
		{
			yield return point;
		}
	}
}
