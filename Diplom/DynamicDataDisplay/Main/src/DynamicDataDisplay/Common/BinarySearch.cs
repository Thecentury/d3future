using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	internal static class BinarySearch
	{
		public const int NotFound = -1;

		public static int SearchInterval(double[] array, double value)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (array.Length < 2)
				return NotFound;

			if (array[0] > value)
				return NotFound;
			if (array[array.Length - 1] < value)
				return NotFound;

			int minI = 0;
			int maxI = array.Length - 1;
			int i = (maxI + minI) / 2;
			while (minI != maxI)
			{
				var array_i = array[i];
				if (array_i <= value && value <= array[i + 1])
					return i;

				if (value > array_i)
				{
					minI = i;
					i = (maxI + minI) / 2;
				}
				else
				{
					maxI = i;
					i = (maxI + minI) / 2;
				}
			}

			return NotFound;
		}
	}
}
