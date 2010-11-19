using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	/// <summary>
	/// Represents a color step with its offset in limits [0..1].
	/// </summary>
	[DebuggerDisplay("Color={Color}, Offset={Offset}")]
	public class LinearPaletteColorStep
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPaletteColorStep"/> class.
		/// </summary>
		public LinearPaletteColorStep() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPaletteColorStep"/> class.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="offset">The offset.</param>
		public LinearPaletteColorStep(Color color, double offset)
		{
			this.Color = color;
			this.Offset = offset;
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public Color Color { get; set; }
		/// <summary>
		/// Gets or sets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public double Offset { get; set; }
	}
}
