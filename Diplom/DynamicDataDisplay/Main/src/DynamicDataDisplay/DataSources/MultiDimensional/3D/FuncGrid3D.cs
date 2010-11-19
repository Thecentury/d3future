using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public sealed class FuncGrid3D : IGrid3D
	{
		private readonly Func<int, int, int, Point3D> getter;

		public FuncGrid3D(Func<int, int, int, Point3D> getter)
		{
			this.getter = getter;
		}

		#region IGrid3D Members

		public Point3D this[int i, int j, int k]
		{
			get { return getter(i, j, k); }
		}

		#endregion
	}
}
