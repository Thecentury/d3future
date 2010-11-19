using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public sealed class FuncNonUniformDataSource3D<T> : INonUniformDataSource3D<T>
	{
		public FuncNonUniformDataSource3D(Func<int, int, int, T> dataGetter, double[] x, double[] y, double[] z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.data = new FuncData3D<T>(dataGetter);
		}

		#region INonUniformDataSource3D<T> Members

		private readonly double[] x;
		public double[] XCoordinates
		{
			get { return x; }
		}

		private readonly double[] y;
		public double[] YCoordinates
		{
			get { return y; }
		}

		private readonly double[] z;
		public double[] ZCoordinates
		{
			get { return z; }
		}

		#endregion

		#region IDataSource3D<T> Members

		private readonly FuncData3D<T> data;
		public IData3D<T> Data
		{
			get { return data; }
		}

		public event EventHandler Changed;

		#endregion

		#region IGridSource3D Members

		public int Width
		{
			get { return x.Length; }
		}

		public int Height
		{
			get { return y.Length; }
		}

		public int Depth
		{
			get { return z.Length; }
		}

		private sealed class NonUniformGrid3D : IGrid3D
		{
			private readonly double[] x;
			private readonly double[] y;
			private readonly double[] z;

			#region IGrid3D Members

			public Point3D this[int i, int j, int k]
			{
				get { return new Point3D(x[i], y[j], z[k]); }
			}

			#endregion
		}

		private readonly NonUniformGrid3D grid;
		public IGrid3D Grid
		{
			get { return grid; }
		}

		#endregion
	}
}
