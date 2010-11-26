﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ResourcePool<T>
	{
		private readonly List<T> pool = new List<T>();

		public T Get()
		{
			T item;

			if (pool.Count < 1)
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
			Contract.Assert(item != null);

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

		public void Clear()
		{
			pool.Clear();
		}
	}
}
