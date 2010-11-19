using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.StreamLine2D.Filters
{
	public abstract class FilterBase
	{
		public abstract IEnumerable<Point> Filter(IEnumerable<Point> points);
	}
}
