using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class BackPattern3D : PointSetPattern3D
	{
		public override IEnumerable<Point3D> GeneratePoints()
		{
			int sidePointsCount = (int)Math.Sqrt(PointsCount);

			double delta = 1.0 / (sidePointsCount - 1);
			for (int i = 0; i < sidePointsCount; i++)
			{
				for (int j = 0; j < sidePointsCount; j++)
				{
					yield return new Point3D(i * delta, j * delta, 0.05);
				}
			}
		}
	}
}
