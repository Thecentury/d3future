using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.ViewportConstraints
{
	public sealed class DateTimeVerticalAxisConstraint : ViewportConstraint
	{
		private readonly double minSeconds = new TimeSpan(DateTime.MinValue.Ticks).TotalSeconds;
		private readonly double maxSeconds = new TimeSpan(DateTime.MaxValue.Ticks).TotalSeconds;

		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			DataRect borderRect = DataRect.Create(proposedDataRect.XMin, minSeconds, proposedDataRect.XMax, maxSeconds);
			if (proposedDataRect.IntersectsWith(borderRect))
			{
				DataRect croppedRect = DataRect.Intersect(proposedDataRect, borderRect);
				return croppedRect;
			}

			return previousDataRect;
		}
	}
}
