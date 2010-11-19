using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public abstract class DecoratorDataSource2D<T> : IDataSource2D<T> where T : struct
	{
		private readonly IDataSource2D<T> child;
		protected DecoratorDataSource2D(IDataSource2D<T> child)
		{
			if (child == null)
				throw new ArgumentNullException("child");
			this.child = child;

			child.Changed += OnChildChanged;
		}

		protected IDataSource2D<T> Child
		{
			get { return child; }
		}

		protected virtual void OnChildChanged(object sender, EventArgs e)
		{
			Changed.Raise(this);
		}

		#region IDataSource2D<T> Members

		public abstract T[,] Data { get; }

		public IDataSource2D<T> GetSubset(int x0, int y0, int countX, int countY, int stepX, int stepY)
		{
			return child.GetSubset(x0, y0, countX, countY, stepX, stepY);
		}

		public Range<T>? Range
		{
			get { return child.Range; }
		}

		public T? MissingValue
		{
			get { return child.MissingValue; }
		}

		#endregion

		#region IGridSource2D Members

		public abstract Point[,] Grid { get; }

		public int Width
		{
			get { return child.Width; }
		}

		public int Height
		{
			get { return child.Height; }
		}

		public event EventHandler Changed;

		#endregion
	}
}
