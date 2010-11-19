using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.ViewportConstraints;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class VerticalDateTimeAxis : DateTimeAxis
	{
		public VerticalDateTimeAxis()
		{
			Placement = AxisPlacement.Left;
			Constraint = new DateTimeVerticalAxisConstraint();
		}

		protected override void ValidatePlacement(AxisPlacement newPlacement)
		{
			if (newPlacement == AxisPlacement.Bottom || newPlacement == AxisPlacement.Top)
				throw new ArgumentException(Strings.Exceptions.VerticalAxisCannotBeHorizontal);
		}
	}
}
