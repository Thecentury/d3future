using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class FuncUniformDataSource3D<T> : IUniformDataSource3D<T>
	{
		protected double x;
		protected double y;
		protected double z;

		protected double xSize;
		protected double ySize;
		protected double zSize;

		protected FuncData3D<T> data;

		protected int width;
		protected int height;
		protected int depth;

		protected FuncUniformDataSource3D() { }

		public FuncUniformDataSource3D(Func<int, int, int, T> dataGetter,
			double x = 0, double xSize = 1,
			double y = 0, double ySize = 1,
			double z = 0, double zSize = 1,
			int width = 10, int height = 10, int depth = 10)
		{
			this.data = new FuncData3D<T>(dataGetter);

			this.x = x;
			this.xSize = xSize;
			this.y = y;
			this.ySize = ySize;
			this.z = z;
			this.zSize = zSize;

			this.width = width;
			this.height = height;
			this.depth = depth;

			this.grid = new UniformGrid3D(x, y, z, xSize / width, ySize / height, zSize / depth);
		}

		protected void SetData(FuncData3D<T> data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			this.data = data;
		}

		#region IUniformDataSource3D<T> Members

		public double X { get { return x; } }

		public double Y { get { return y; } }

		public double Z { get { return z; } }

		public double XSize { get { return xSize; } }

		public double YSize { get { return ySize; } }

		public double ZSize { get { return zSize; } }

		#endregion

		#region INonUniformDataSource3D<T> Members

		private double[] xCoordinates = null;
		public double[] XCoordinates
		{
			get
			{
				if (xCoordinates == null)
				{
					double delta = xSize / width;
					xCoordinates = Enumerable.Range(0, Width).Select(i => x + i * delta).ToArray();
				}
				return xCoordinates;
			}
		}

		private double[] yCoordinates;
		public double[] YCoordinates
		{
			get
			{
				if (yCoordinates == null)
				{
					double delta = ySize / height;
					yCoordinates = Enumerable.Range(0, height).Select(i => y + i * delta).ToArray();
				}
				return yCoordinates;
			}
		}

		private double[] zCoordinates = null;
		public double[] ZCoordinates
		{
			get
			{
				if (zCoordinates == null)
				{
					double delta = zSize / depth;
					zCoordinates = Enumerable.Range(0, depth).Select(i => z + i * delta).ToArray();
				}
				return zCoordinates;
			}
		}

		#endregion

		#region IDataSource3D<T> Members

		public IData3D<T> Data
		{
			get { return data; }
		}

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		#endregion

		#region IGridSource3D Members

		public int Width
		{
			get { return width; }
		}

		public int Height
		{
			get { return height; }
		}

		public int Depth
		{
			get { return depth; }
		}

		private readonly UniformGrid3D grid;
		public IGrid3D Grid
		{
			get { return grid; }
		}

		#endregion
	}
}
