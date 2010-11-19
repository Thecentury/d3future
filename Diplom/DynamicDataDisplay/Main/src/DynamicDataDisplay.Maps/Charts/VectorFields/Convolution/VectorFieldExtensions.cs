using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	internal static class VectorFieldExtensions
	{
		public static Range<double> GetMinMaxLength(this IDataSource2D<Vector> dataSource)
		{
			double minLength = Double.PositiveInfinity;
			double maxLength = Double.NegativeInfinity;

			var data = dataSource.Data;
			int width = dataSource.Width;
			int height = dataSource.Height;
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					var vector = data[ix, iy];
					var length = vector.Length;

					if (length < minLength)
						minLength = length;
					if (length > maxLength)
						maxLength = length;
				}
			}

			if (minLength > maxLength)
			{
				minLength = maxLength = 0;
			}
			return new Range<double>(minLength, maxLength);
		}

		public static Range<double> GetMinMax(this IDataSource2D<Vector> dataSource, Func<Vector, double> extractor)
		{
			double min = Double.PositiveInfinity;
			double max = Double.NegativeInfinity;

			var data = dataSource.Data;
			int width = dataSource.Width;
			int height = dataSource.Height;
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					var vector = data[ix, iy];
					var value = extractor(vector);

					if (value < min)
						min = value;
					if (value > max)
						max = value;
				}
			}

			if (min > max)
			{
				min = max = 0;
			}
			return new Range<double>(min, max);
		}
	}
}
