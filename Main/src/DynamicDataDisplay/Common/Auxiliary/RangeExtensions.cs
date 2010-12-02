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

		public static int GetLength(this Range<int> range)
		{
			return range.Max - range.Min;
		}

		public static bool FromTheLeft(this Range<int> range, Range<int> other)
		{
			return other.Min < range.Min &&
				other.Max < range.Min;
		}

		public static bool FromTheRight(this Range<int> range, Range<int> other)
		{
			return other.Min > range.Max &&
				other.Max > range.Max;
		}

		public static bool IntersectsWith(this Range<int> range, Range<int> other)
		{
			return range.IsBetween(other.Min) ||
				range.IsBetween(other.Max);
		}

		public static bool IsBetween(this Range<int> range, int value)
		{
			return range.Min <= value && value <= range.Max;
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
