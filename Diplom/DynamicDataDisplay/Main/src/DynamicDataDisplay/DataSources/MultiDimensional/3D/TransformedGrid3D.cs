using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	internal sealed class TransformedGrid3D : IGrid3D
	{
		private readonly IGrid3D child;
		private readonly Transform3D transform;

		public TransformedGrid3D(IGrid3D child, Transform3D transform)
		{
			this.child = child;
			this.transform = transform;
		}

		#region IGrid3D Members

		public Point3D this[int i, int j, int k]
		{
			get
			{
				var originalPoint = child[i, j, k];
				var transformedPoint = transform.Transform(originalPoint);
				return transformedPoint;
			}
		}

		#endregion
	}
}
