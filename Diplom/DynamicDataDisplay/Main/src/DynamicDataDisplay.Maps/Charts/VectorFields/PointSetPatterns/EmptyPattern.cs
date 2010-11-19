using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class EmptyPattern : PointSetPattern
	{
		public override IEnumerable<Point> GeneratePoints()
		{
			yield break;
		}
	}
}
