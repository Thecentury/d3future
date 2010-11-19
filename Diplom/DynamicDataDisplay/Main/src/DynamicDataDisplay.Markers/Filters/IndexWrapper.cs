using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Filters
{
	public static class IndexWrapper
	{
		public static IEnumerable<IndexWrapper<T>> Generate<T>(IEnumerable<T> series)
		{
			IndexWrapper<T> indexWrapper = new IndexWrapper<T>();

			int index = 0;
			foreach (var item in series)
			{
				indexWrapper.Data = item;
				indexWrapper.Index = index;

				yield return indexWrapper;

				index++;
			}
		}

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
