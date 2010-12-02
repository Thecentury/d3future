using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Filters
{
	/// <summary>
	/// Represents a utility class which wraps each value in a sequence with a wrapper, which
	/// contains an position of wrapped element in a sequence.
	/// </summary>
	public static class IndexWrapper
	{
		public const int Empty = -1;

		/// <summary>
		/// Generates the index-wrapped series for specified series.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		public static IEnumerable<IndexWrapper<T>> Generate<T>(IEnumerable<T> series, int startingWith = 0)
		{
			IndexWrapper<T> indexWrapper = new IndexWrapper<T>();

			int index = startingWith;
			foreach (var item in series)
			{
				indexWrapper.Data = item;
				indexWrapper.Index = index;

				yield return indexWrapper;

				index++;
			}
		}

		/// <summary>
		/// Generates the index-wrapped sequence for a given sequence of items.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="startingIndex">Index of the starting.</param>
		/// <returns></returns>
		public static IEnumerable<IndexWrapper<object>> Generate(IList items, int startingIndex)
		{
			IndexWrapper<object> indexWrapper = new IndexWrapper<object>();

			for (int i = 0; i < items.Count; i++)
			{
				indexWrapper.Data = items[i];
				indexWrapper.Index = startingIndex + i;

				yield return indexWrapper;
			}
		}
	}
}
