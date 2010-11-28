using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	/// <summary>
	/// Represents a utility class capable to split sequence of points, using missing values as delimeters.
	/// </summary>
	public sealed class LineSplitter
	{
		public IEnumerable<LinePart> Split(IEnumerable<Point> points, double xMissingValue = Double.NaN, double yMissingValue = Double.NaN)
		{
			List<Point> list = new List<Point>();
			double parameter = 0;
			bool split = false;

			foreach (var point in points)
			{
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
					list.Add(point);
				}

				if (split)
				{
					split = false;
					yield return new LinePart { Points = list, Parameter = parameter };
					list = new List<Point>();
				}
			}

			if (!split)
				yield return new LinePart { Points = list, Parameter = 0 };
		}
	}
}
