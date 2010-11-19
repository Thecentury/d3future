using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	internal sealed class FilteringData<T> : IData3D<T>
	{
		private readonly int xStep;
		private readonly int yStep;
		private readonly int zStep;

		private readonly IData3D<T> child;

		public FilteringData(IData3D<T> child, int xStep, int yStep, int zStep)
		{
			if (child == null)
				throw new ArgumentNullException("child");
			if (xStep <= 1)
				throw new ArgumentOutOfRangeException("xStep", "Should be greater than 1.");
			if (yStep <= 1)
				throw new ArgumentOutOfRangeException("yStep", "Should be greater than 1.");
			if (zStep <= 1)
				throw new ArgumentOutOfRangeException("zStep", "Should be greater than 1.");

			this.child = child;

			this.xStep = xStep;
			this.yStep = yStep;
			this.zStep = zStep;
		}

		#region IData3D<T> Members

		public T this[int i, int j, int k]
		{
			get
			{
				T value = child[i * xStep, j * yStep, k * zStep];
				return value;
			}
		}

		#endregion
	}
}
