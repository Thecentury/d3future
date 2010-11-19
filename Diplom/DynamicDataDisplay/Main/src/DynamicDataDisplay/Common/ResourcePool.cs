using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	/// <summary>
	/// Represents a pool of objects, which can be used to decrease GC count.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ResourcePool<T>
	{
		private readonly List<T> pool = new List<T>();

		public T Get()
		{
			T item;

			if (pool.Count == 0)
			{
				item = default(T);
			}
			else
			{
				int index = pool.Count - 1;
				item = pool[index];
				pool.RemoveAt(index);
			}

			return item;
		}

		public void Put(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

#if DEBUG
			if (pool.IndexOf(item) != -1)
				Debugger.Break();
#endif

			pool.Add(item);
		}

		public int Count
		{
			get { return pool.Count; }
		}

		public bool IsEmpty
		{
			get { return pool.Count == 0; }
		}

		public void Clear()
		{
			pool.Clear();
		}
	}
}
