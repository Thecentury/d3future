using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class IDataSource2DExtensions
	{
		public static Range<double> GetMinMax(this double[,] data)
		{
			data.VerifyNotNull("data");

			int width = data.GetLength(0);
			int height = data.GetLength(1);
			Verify.IsTrueWithMessage(width > 0, Strings.Exceptions.ArrayWidthShouldBePositive);
			Verify.IsTrueWithMessage(height > 0, Strings.Exceptions.ArrayHeightShouldBePositive);

			double min = data[0, 0];
			double max = data[0, 0];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (data[x, y] < min)
						min = data[x, y];
					if (data[x, y] > max)
						max = data[x, y];
				}
			}

			Range<double> res = new Range<double>(min, max);
			return res;
		}

		public static Range<double> GetMinMax(this double[,] data, double missingValue)
		{
			data.VerifyNotNull("data");

			int width = data.GetLength(0);
			int height = data.GetLength(1);
			Verify.IsTrueWithMessage(width > 0, Strings.Exceptions.ArrayWidthShouldBePositive);
			Verify.IsTrueWithMessage(height > 0, Strings.Exceptions.ArrayHeightShouldBePositive);

			double min = Double.MaxValue;
			double max = Double.MinValue;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (data[x, y] != missingValue && data[x, y] < min)
						min = data[x, y];
					if (data[x, y] != missingValue && data[x, y] > max)
						max = data[x, y];
				}
			}

			Range<double> res = new Range<double>(min, max);
			return res;
		}

		public static Range<double> GetMinMax(this IDataSource2D<double> dataSource)
		{
			dataSource.VerifyNotNull("dataSource");

			return GetMinMax(dataSource.Data);
		}

		public static Range<double> GetMinMax(this IDataSource2D<double> dataSource, double missingValue)
		{
			dataSource.VerifyNotNull("dataSource");

			return GetMinMax(dataSource.Data, missingValue);
		}

		public static Range<double> GetMinMax(this IDataSource2D<double> dataSource, DataRect area)
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");

			double min = Double.PositiveInfinity;
			double max = Double.NegativeInfinity;
			int width = dataSource.Width;
			int height = dataSource.Height;
			var grid = dataSource.Grid;
			var data = dataSource.Data;
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					if (area.Contains(grid[ix, iy]))
					{
						var value = data[ix, iy];
						if (value < min)
							min = value;
						if (value > max)
							max = value;
					}
				}
			}

			if (min < max)
				return new Range<double>(min, max);
			else
				return new Range<double>();
		}


		public static DataRect GetGridBounds(this Point[,] grid)
		{
			double minX = grid[0, 0].X;
			double maxX = minX;
			double minY = grid[0, 0].Y;
			double maxY = minY;

			int width = grid.GetLength(0);
			int height = grid.GetLength(1);
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					Point pt = grid[ix, iy];

					double x = pt.X;
					double y = pt.Y;
					if (x < minX) minX = x;
					if (x > maxX) maxX = x;

					if (y < minY) minY = y;
					if (y > maxY) maxY = y;
				}
			}
			return new DataRect(new Point(minX, minY), new Point(maxX, maxY));
		}

		public static DataRect GetGridBounds<T>(this IDataSource2D<T> dataSource) where T : struct
		{
			return dataSource.Grid.GetGridBounds();
		}

		public static DataRect GetGridBounds<T>(this INonUniformDataSource2D<T> dataSource) where T : struct
		{
			var xCoordinates = dataSource.XCoordinates;
			var yCoordinates = dataSource.YCoordinates;

			var xMin = xCoordinates[0];
			var xMax = xCoordinates[xCoordinates.Length - 1];

			var yMin = yCoordinates[0];
			var yMax = yCoordinates[yCoordinates.Length - 1];

			var contentBounds = DataRect.FromPoints(xMin, yMin, xMax, yMax);
			return contentBounds;
		}

		public static IDataSource2D<double> GetMagnitudeDataSource(this IDataSource2D<Vector> dataSource)
		{
			return new VectorToMagnitudeDataSource(dataSource);
		}

		public static IDataSource2D<T> ChangeGrid<T>(this IDataSource2D<T> dataSource, Func<int, int, Point> gridFunc) where T : struct
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");
			if (gridFunc == null)
				throw new ArgumentNullException("gridFunc");

			Func<int, int, Point, Point> func = (ix, iy, oldPoint) => gridFunc(ix, iy);

			return new TransformedGridDataSource2D<T>(dataSource, func);
		}
	}
}
