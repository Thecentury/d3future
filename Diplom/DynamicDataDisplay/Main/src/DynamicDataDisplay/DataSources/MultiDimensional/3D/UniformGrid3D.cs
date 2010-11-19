using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class UniformGrid3D : IGrid3D
	{
		private readonly double x;
		private readonly double y;
		private readonly double z;

		private readonly double xDelta;
		private readonly double yDelta;
		private readonly double zDelta;

		public UniformGrid3D(double x, double y, double z, double xDelta, double yDelta, double zDelta)
		{
			this.x = x;
			this.y = y;
			this.z = z;

			this.xDelta = xDelta;
			this.yDelta = yDelta;
			this.zDelta = zDelta;
		}

		#region IGrid3D Members

		public Point3D this[int i, int j, int k]
		{
			get { return new Point3D(x + i * xDelta, y + j * yDelta, z + k * zDelta); }
		}

		#endregion
	}
}
