using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Markers2;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Contains useful extension methods of Plotter2D class.
	/// </summary>
	public static class PlotterExtensions
	{
		/// <summary>
		/// Adds the line chart with specified items source to plotter.
		/// </summary>
		/// <param name="plotter">The plotter.</param>
		/// <param name="data">The data.</param>
		/// <returns>An instance of LineChart.</returns>
		public static LineChart AddLineChart(this Plotter2D plotter, object data)
		{
			if (plotter == null)
				throw new ArgumentNullException("plotter");
			if (data == null)
				throw new ArgumentNullException("data");

			LineChart chart = new LineChart { ItemsSource = data };
			plotter.Children.Add(chart);

			return chart;
		}
	}
}
