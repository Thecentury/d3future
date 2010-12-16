using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Markers.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	public static class DataSource
	{
		public static TwoEnumerableOfDoubleDataSource Create(IEnumerable<double> xs, IEnumerable<double> ys)
		{
			return new TwoEnumerableOfDoubleDataSource(xs, ys);
		}

		//public static EnumerableDataSource<T> AsDataSource<T>(this IEnumerable<T> collection)
		//{
		//    // todo add F#-specific handling of <T>

		//    return new EnumerableDataSource<T>(collection);
		//}

		//public static RawPointDataSource AsDataSource(this IEnumerable<Point> pointCollection)
		//{
		//    return new RawPointDataSource(pointCollection);
		//}
	}
}
