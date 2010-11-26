using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class ResourcePoolExtensions
	{
		/// <summary>
		/// Gets item from the pool, or creates new item if pool doesn't have more items.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pool">The pool.</param>
		/// <returns></returns>
		public static T GetOrCreate<T>(this ResourcePool<T> pool) where T : new()
		{
			T instance = pool.Get();
			if (instance == null)
			{
				instance = new T();
			}

			return instance;
		}

		/// <summary>
		/// Releases all items of given sequence into the pool.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pool">Pool, which will contain all released elements from the sequence.</param>
		/// <param name="sequence"></param>
		public static void ReleaseAll<T>(this ResourcePool<T> pool, IEnumerable<T> sequence)
		{
			foreach (var item in sequence)
			{
				pool.Put(item);
			}
		}
	}
}
