using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class PointSetPattern3D
	{
		public abstract IEnumerable<Point3D> GeneratePoints();

		private int pointsCount = 50;
		public int PointsCount
		{
			get { return pointsCount; }
			set { pointsCount = value; }
		}

		public event EventHandler Changed;

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
	}
}
