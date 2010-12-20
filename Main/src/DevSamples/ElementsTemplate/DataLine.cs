using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;

namespace ElementsTemplate
{
	public class DataLine : IEnumerable<Point>
	{
		private readonly IEnumerable<Point> data;

		public DataLine(IEnumerable<Point> data)
		{
			this.data = data;
		}

		public Brush Brush {
			get { return ColorHelper.RandomBrush; }
		}

		#region IEnumerable<Point> Members

		public IEnumerator<Point> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
