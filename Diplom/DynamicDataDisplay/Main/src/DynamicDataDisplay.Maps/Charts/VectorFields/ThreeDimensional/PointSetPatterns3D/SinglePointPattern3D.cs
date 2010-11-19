using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class SinglePointPattern3D : PointSetPattern3D
	{
		private Point3D point = new Point3D(0.5, 0.5, 0.5);
		public Point3D Point
		{
			get { return point; }
			set
			{
				if (point == value)
					return;

				point = value;
				RaiseChanged();
			}
		}

		public override IEnumerable<Point3D> GeneratePoints()
		{
			yield return point;
		}
	}
}
