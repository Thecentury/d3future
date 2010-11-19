using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.StreamLine2D.Filters
{
	/// <summary>
	/// Filters points in screen coordinates.
	/// </summary>
	public sealed class FrequencyFilter : FilterBase
	{
		public override IEnumerable<Point> Filter(IEnumerable<Point> points)
		{
			var enumerator = points.GetEnumerator();
			enumerator.MoveNext();

			Point pt = enumerator.Current;
			yield return pt;

			bool yieldedLast = false;
			Point nextPt = enumerator.Current;
			double distance = 0;
			while (enumerator.MoveNext())
			{
				yieldedLast = false;
				nextPt = enumerator.Current;

				distance = (pt - nextPt).Length;

				if (distance > 1)
				{
					distance = 0;
					yieldedLast = true;
					pt = nextPt;
					yield return nextPt;
				}
			}

			if (!yieldedLast)
				yield return nextPt;
		}
	}
}
