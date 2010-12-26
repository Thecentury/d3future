using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class AxisPlacementExtensions
	{
		public static bool IsHorizontal(this AxisPlacement placement)
		{
			return placement == AxisPlacement.Bottom || placement == AxisPlacement.Top;
		}

		public static bool IsVertical(this AxisPlacement placement)
		{
			return placement == AxisPlacement.Left || placement == AxisPlacement.Right;
		}

		public static ChangeType GetPanChangeType(this AxisPlacement placement)
		{
			if (placement.IsHorizontal())
				return ChangeType.PanX;
			else
				return ChangeType.PanY;
		}

		public static ChangeType GetZoomChangeType(this AxisPlacement placement)
		{
			if (placement.IsHorizontal())
				return ChangeType.ZoomX;
			else
				return ChangeType.ZoomY;
		}
	}
}
