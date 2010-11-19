using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	internal sealed class FilteringGrid : IGrid3D
	{
		private readonly IGrid3D child;

		private readonly int xStep;
		private readonly int yStep;
		private readonly int zStep;

		public FilteringGrid(IGrid3D grid, int xStep, int yStep, int zStep)
		{
			if (grid == null)
				throw new ArgumentNullException("grid");
			if (xStep <= 1)
				throw new ArgumentOutOfRangeException("xStep", "Should be greater than 1.");
			if (yStep <= 1)
				throw new ArgumentOutOfRangeException("yStep", "Should be greater than 1.");
			if (yStep <= 1)
				throw new ArgumentOutOfRangeException("zStep", "Should be greater than 1.");

			this.child = grid;
			this.xStep = xStep;
			this.yStep = yStep;
			this.zStep = zStep;
		}

		#region IGrid3D Members

		public Point3D this[int i, int j, int k]
		{
			get
			{
				Point3D position = child[i * xStep, j * xStep, k * zStep];
				return position;
			}
		}

		#endregion
	}
}
