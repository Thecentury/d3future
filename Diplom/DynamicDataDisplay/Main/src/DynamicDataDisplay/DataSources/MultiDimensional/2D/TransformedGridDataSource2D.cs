using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public sealed class TransformedGridDataSource2D<T> : DecoratorDataSource2D<T>, INonUniformDataSource2D<T> where T : struct
	{
		private readonly Func<int, int, Point, Point> gridFunc;
		public TransformedGridDataSource2D(IDataSource2D<T> child, Func<int, int, Point, Point> gridFunc)
			: base(child)
		{
			if (gridFunc == null)
				throw new ArgumentNullException("gridFunc");

			this.gridFunc = gridFunc;
		}

		public override T[,] Data
		{
			get { return Child.Data; }
		}

		private Point[,] grid = null;
		public override Point[,] Grid
		{
			get
			{
				if (grid == null)
				{
					int width = Width;
					int height = Height;

					grid = new Point[width, height];

					for (int ix = 0; ix < width; ix++)
						for (int iy = 0; iy < height; iy++)
							grid[ix, iy] = gridFunc(ix, iy, Child.Grid[ix, iy]);
				}
				return grid;
			}
		}

		#region INonUniformDataSource2D<T> Members

		private double[] xCoordinates;
		public double[] XCoordinates
		{
			get
			{
				if (xCoordinates == null)
				{
					INonUniformDataSource2D<T> nonUniformChild = Child as INonUniformDataSource2D<T>;
					if (nonUniformChild == null)
						throw new InvalidOperationException("Child does not implement INonUniformDataSource2D<{0}>.".F(typeof(T).Name));

					xCoordinates = new double[Width];
					for (int ix = 0; ix < Width; ix++)
					{
						xCoordinates[ix] = gridFunc(ix, 0, Child.Grid[ix, 0]).X;
					}
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
					INonUniformDataSource2D<T> nonUbiformChild = Child as INonUniformDataSource2D<T>;
					if (nonUbiformChild == null)
						throw new InvalidOperationException("Child does not implement INonUniformDataSource2D<{0}>.".F(typeof(T).Name));

					yCoordinates = new double[Height];
					for (int iy = 0; iy < Height; iy++)
					{
						yCoordinates[iy] = gridFunc(0, iy, Child.Grid[0, iy]).Y;
					}
				}

				return yCoordinates;
			}
		}

		#endregion
	}
}
