using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ViewportConstraints
{
	/// <summary>
	/// Represents a ViewportConstraint, which the minimal size of <see cref="Viewport"/>'s Visible.
	/// </summary>
	public class MinimalSizeConstraint : ViewportConstraint
	{
		private double minSize = 1E-11;

		/// <summary>
		/// Gets or sets the minimal size of Viewport's Visible.
		/// </summary>
		/// <value>The minimal size of Viewport's Visible.</value>
		public double MinSize
		{
			get { return minSize; }
			set
			{
				if (minSize != value)
				{
					minSize = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Applies the restriction.
		/// </summary>
		/// <param name="previousDataRect">Previous data rectangle.</param>
		/// <param name="proposedDataRect">Proposed data rectangle.</param>
		/// <param name="viewport">The viewport, to which current restriction is being applied.</param>
		/// <returns>New changed visible rectangle.</returns>
		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			if (proposedDataRect.Width < minSize || proposedDataRect.Height < minSize)
				return previousDataRect;

			return proposedDataRect;
		}
	}
}
