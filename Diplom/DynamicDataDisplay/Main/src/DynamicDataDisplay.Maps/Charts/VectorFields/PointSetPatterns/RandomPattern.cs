using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution
{
	public sealed class RandomPattern : PointSetPattern
	{
		private static readonly Random rnd = new Random();
		public override IEnumerable<Point> GeneratePoints()
		{
			for (int i = 0; i < PointsCount; i++)
			{
				yield return new Point(rnd.NextDouble(), rnd.NextDouble());
			}
		}
	}
}
