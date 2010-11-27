using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	/// <summary>
	/// Represents a dictionary with automatic removal of old values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class RingDictionary<T>
	{
		private int startGeneration = 0;
		private int generation;
		private readonly Dictionary<int, T> dict = new Dictionary<int, T>();
		private readonly int maxElements = 5;

		public RingDictionary(int maxElements = 5)
		{
			Contract.Assert(maxElements >= 1);

			this.maxElements = maxElements;
		}

		public int Generation
		{
			get { return generation; }
		}

		public int StartGeneration
		{
			get { return startGeneration; }
		}

		public bool ContainsValue(T value)
		{
			return dict.ContainsValue(value);
		}

		public void AddValue(T value)
		{
			Contract.Assert(value != null);

			dict.Add(generation++, value);
			Cleanup();
		}

		public int Count
		{
			get { return dict.Count; }
		}

		private void Cleanup()
		{
			while ((generation - startGeneration) > maxElements)
			{
				dict.Remove(startGeneration);
				startGeneration++;
			}
		}
	}
}
