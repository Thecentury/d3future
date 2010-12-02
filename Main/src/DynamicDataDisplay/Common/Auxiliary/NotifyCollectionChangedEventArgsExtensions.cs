using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class NotifyCollectionChangedEventArgsExtensions
	{
		public static int GetLastAddedIndex(this NotifyCollectionChangedEventArgs args)
		{
			if (args.NewItems == null)
				throw new InvalidOperationException("Cannot get last added index when NewItems are null.");

			int lastIndex = args.NewStartingIndex + args.NewItems.Count;

			return lastIndex;
		}

		public static Range<int> GetAddedRange(this NotifyCollectionChangedEventArgs args)
		{
			int lastIndex = GetLastAddedIndex(args);

			return new Range<int>(args.NewStartingIndex, lastIndex);
		}
	}
}
