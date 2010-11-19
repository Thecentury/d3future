using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class RandomPattern3D: PointSetPattern3D
	{
		private readonly Random rnd = new Random();
		public override IEnumerable<Point3D> GeneratePoints()
		{
			for (int i = 0; i < PointsCount; i++)
			{
				yield return new Point3D(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble());
			}
		}
	}
}
