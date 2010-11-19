using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class FuncDataSource3D<T> : IDataSource3D<T>
	{
		protected FuncDataSource3D() { }

		public FuncDataSource3D(int width, int height, int depth, Func<int, int, int, Point3D> fieldGetter, Func<int, int, int, T> dataGetter)
		{
			this.width = width;
			this.height = height;
			this.depth = depth;

			this.grid = new FuncGrid3D(fieldGetter);
			this.data = new FuncData3D<T>(dataGetter);
		}

		#region IDataSource3D<T> Members

		private FuncData3D<T> data;
		public IData3D<T> Data
		{
			get { return data; }
		}

		protected void SetData(FuncData3D<T> data)
		{
			this.data = data;
		}

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		#endregion

		#region IGridSource3D Members

		private int width;
		public int Width
		{
			get { return width; }
			protected set { width = value; }
		}

		private int height;
		public int Height
		{
			get { return height; }
			protected set { height = value; }
		}

		private int depth;
		public int Depth
		{
			get { return depth; }
			protected set { depth = value; }
		}

		private FuncGrid3D grid;
		public IGrid3D Grid
		{
			get { return grid; }
		}

		protected void SetGrid(FuncGrid3D grid)
		{
			this.grid = grid;
		}

		#endregion
	}
}
