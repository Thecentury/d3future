using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	/// <summary>
	/// Represents a utility class capable to split sequence of points, using missing values as delimeters.
	/// </summary>
	public sealed class MissingValueSplitter
	{
		public IEnumerable<LinePart> SplitMissingValue(IEnumerable<IndexWrapper<Point>> points, double xMissingValue = Double.NaN, double yMissingValue = Double.NaN)
		{
			List<IndexWrapper<Point>> list = new List<IndexWrapper<Point>>();
			double parameter = 0;
			bool split = false;

			int minIndex = IndexWrapper.Empty;
			int maxIndex = IndexWrapper.Empty;


			foreach (var index in points)
			{
				Point point = index.Data;
				if (point.X.Equals(xMissingValue))
				{
					parameter = point.Y;
					split = true;
				}
				else if (point.Y.Equals(yMissingValue))
				{
					parameter = point.X;
					split = true;
				}
				else
				{
					list.Add(index);
				}

				if (split)
				{
					split = false;

					if (list.Count > 0)
					{
						minIndex = list[0].Index;
						maxIndex = list[list.Count - 1].Index;
					}

					yield return new LinePart
					{
						Data = list,
						Parameter = parameter,
						MinIndex = minIndex,
						MaxIndex = maxIndex
					};
					list = new List<IndexWrapper<Point>>();
				}
			}

			if (list.Count > 0)
			{
				minIndex = list[0].Index;
				maxIndex = list[list.Count - 1].Index;
			}

			if (!split)
				yield return new LinePart
				{
					Data = list,
					Parameter = 0,
					MinIndex = minIndex,
					MaxIndex = maxIndex
				};
		}
	}
}
