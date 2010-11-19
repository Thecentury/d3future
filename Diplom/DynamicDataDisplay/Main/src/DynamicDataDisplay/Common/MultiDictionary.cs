using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	internal sealed class MultiDictionary<TKey, TValue>
	{
		private Dictionary<TKey, HashSet<TValue>> dict = new Dictionary<TKey, HashSet<TValue>>();
		public void Add(TKey key, TValue value)
		{
			if (!dict.ContainsKey(key))
				dict[key] = new HashSet<TValue>();

			dict[key].Add(value);
		}

		public IEnumerable<TKey> Keys
		{
			get { return dict.Keys; }
		}

		public IEnumerable<TValue> this[TKey key]
		{
			get { return dict[key]; }
		}
	}
}
