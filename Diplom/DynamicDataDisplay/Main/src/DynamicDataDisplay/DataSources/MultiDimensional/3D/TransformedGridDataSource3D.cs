using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public sealed class TransformedGridDataSource3D<T> : IDataSource3D<T>
	{
		private readonly IDataSource3D<T> child;
		private readonly TransformedGrid3D transformedGrid;
		public TransformedGridDataSource3D(IDataSource3D<T> child, Transform3D transform)
		{
			if (child == null)
				throw new ArgumentNullException("child");
			if (transform == null)
				throw new ArgumentNullException("transform");

			this.child = child;
			this.transformedGrid = new TransformedGrid3D(child.Grid, transform);
		}

		#region IDataSource3D<T> Members

		public IData3D<T> Data
		{
			get { return child.Data; }
		}

		public event EventHandler Changed
		{
			add { child.Changed += value; }
			remove { child.Changed -= value; }
		}

		#endregion

		#region IGridSource3D Members

		public int Width
		{
			get { return child.Width; }
		}

		public int Height
		{
			get { return child.Height; }
		}

		public int Depth
		{
			get { return child.Depth; }
		}

		public IGrid3D Grid
		{
			get { return transformedGrid; }
		}

		#endregion
	}
}
