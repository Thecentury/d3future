using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class RangeExtensions
	{
		public static double GetLength(this Range<Point> range)
		{
			Point p1 = range.Min;
			Point p2 = range.Max;

			return (p1 - p2).Length;
		}

		public static double GetLength(this Range<double> range)
		{
			return range.Max - range.Min;
		}

		/// <summary>
		/// Determines whether specified range contains the specified value.
		/// </summary>
		/// <param name="range">The range.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified range]; otherwise, <c>false</c>.
		/// </returns>
		public static bool Contains(this Range<double> range, double value)
		{
			return range.Min < value && value < range.Max;
		}
	}
}
