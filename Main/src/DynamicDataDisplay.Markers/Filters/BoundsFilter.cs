using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Filters;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Filters
{
	public class BoundsFilter : PointsFilter2d
	{
		protected internal override IEnumerable<IndexWrapper<Point>> Filter(IEnumerable<IndexWrapper<Point>> series)
		{
			var visible = this.Environment.Visible;
			var increasedVisible = visible.ZoomOutFromCenter(2.0);

			return series.Where(wrapper => increasedVisible.Contains(wrapper.Data));
		}
	}
}
