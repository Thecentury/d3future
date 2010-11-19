using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public sealed class FuncData3D<T> : IData3D<T>
	{
		private readonly Func<int, int, int, T> getter;
		public FuncData3D(Func<int, int, int, T> getter)
		{
			this.getter = getter;
		}

		#region IData3D<T> Members

		public T this[int i, int j, int k]
		{
			get
			{
				T value = getter(i, j, k);
				return value;
			}
		}

		#endregion
	}
}
