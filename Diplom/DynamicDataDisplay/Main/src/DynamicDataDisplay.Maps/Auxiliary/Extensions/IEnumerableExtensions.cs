using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.StreamLine2D.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Auxiliary.Extensions
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<Point> Filter(this IEnumerable<Point> points, FilterBase filter)
		{
			return filter.Filter(points);
		}
	}
}
