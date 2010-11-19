using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class ArrayDataSource3D<T> : IDataSource3D<T>
	{
		private T[, ,] array;
		public T[, ,] Array
		{
			get { return array; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				array = value;
				data = new ArrayData3D<T>(array);
				UpdateSize();
				grid = new UniformGrid3D(0, 0, 0, 1.0 / width, 1.0 / height, 1.0 / depth);
				RaiseChanged();
			}
		}

		private int width, height, depth;
		private UniformGrid3D grid;

		protected ArrayDataSource3D()
		{

		}

		public ArrayDataSource3D(T[, ,] array)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			Array = array;
		}

		private void UpdateSize()
		{
			width = array.GetLength(0);
			height = array.GetLength(1);
			depth = array.GetLength(2);
		}

		#region IDataSource3D<T> Members

		private ArrayData3D<T> data;
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

		public IGrid3D Grid
		{
			get { return grid; }
		}

		#endregion
	}
}
