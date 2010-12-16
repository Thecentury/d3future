using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewLine
{
	/// <summary>
	/// Represents a line segment with coordinates and missing value parameter.
	/// Missing value parameter is a second coordinate of Point which has one 
	/// coordinate equal to missing value.
	/// </summary>
	public sealed class LinePart
	{
		private int minIndex = -1;
		private int maxIndex = -1;

		public LinePart()
		{
			Splitted = false;
		}

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>The data.</value>
		public IEnumerable<IndexWrapper<Point>> Data { get; set; }

		/// <summary>
		/// Gets or sets the missing value parameter - a second coordinate of Point which has one 
		/// coordinate equal to missing value.
		/// </summary>
		/// <value>The parameter.</value>
		public double Parameter { get; set; }

		/// <summary>
		/// Returns a sequence of clean points without index wrapper.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Point> GetPoints()
		{
			foreach (var item in Data)
			{
				yield return item.Data;
			}
		}

		/// <summary>
		/// Gets or sets the index of line start.
		/// </summary>
		/// <value>The index of the min.</value>
		public int MinIndex
		{
			get { return minIndex; }
			set { minIndex = value; }
		}

		/// <summary>
		/// Gets or sets the index of line end.
		/// </summary>
		/// <value>The index of the max.</value>
		public int MaxIndex
		{
			get { return maxIndex; }
			set { maxIndex = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LinePart"/> was splitted.
		/// </summary>
		/// <value><c>true</c> if splitted; otherwise, <c>false</c>.</value>
		public bool Splitted { get; set; }
	}
}
