using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class ArrayData3D<T> : IData3D<T>
	{
		public ArrayData3D(T[, ,] array)
		{
			Array = array;
		}

		private T[, ,] array;
		public T[, ,] Array
		{
			get { return array; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				array = value;
			}
		}

		#region IData3D<T> Members

		public T this[int i, int j, int k]
		{
			get { return array[i, j, k]; }
		}

		#endregion
	}
}
