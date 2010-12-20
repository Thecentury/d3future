using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a way how InjectedPlotter's Visible property is coerced when parent plotter's visible changes.
	/// </summary>
	public enum ViewportConjunctionMode
	{
		/// <summary>
		/// InjectedPlotter is absolutely independent on parent Plotter.
		/// </summary>
		None,
		/// <summary>
		/// InjectedPlotter will take X values of Parent Plotter's Visible rect.
		/// </summary>
		X,
		/// <summary>
		/// InjectedPlotter will take Y values of Parent Plotter's Visible rect.
		/// </summary>
		Y,
		/// <summary>
		/// InjectedPlotter's Visible follows Parent Plotter's Viisble.
		/// </summary>
		XY
	}
}
