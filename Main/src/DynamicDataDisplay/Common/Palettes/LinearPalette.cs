using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	/// <summary>
	/// Represents a palette with start and stop colors and intermediate colors with their custom offsets.
	/// </summary>
	[ContentProperty("Steps")]
	public class LinearPalette : PaletteBase, ISupportInitialize
	{
		private readonly ObservableCollection<LinearPaletteColorStep> steps = new ObservableCollection<LinearPaletteColorStep>();
		public ObservableCollection<LinearPaletteColorStep> Steps
		{
			get { return steps; }
		}

		private Color startColor = Colors.White;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Color StartColor
		{
			get { return startColor; }
			set { startColor = value; }
		}

		private Color endColor = Colors.Black;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Color EndColor
		{
			get { return endColor; }
			set { endColor = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPalette"/> class.
		/// </summary>
		public LinearPalette() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="LinearPalette"/> class.
		/// </summary>
		/// <param name="startColor">The start color.</param>
		/// <param name="endColor">The end color.</param>
		/// <param name="steps">The steps.</param>
		public LinearPalette(Color startColor, Color endColor, params LinearPaletteColorStep[] steps)
		{
			this.steps.Add(new LinearPaletteColorStep(startColor, 0));
			if (steps != null)
				this.steps.AddMany(steps);
			this.steps.Add(new LinearPaletteColorStep(endColor, 1));
		}

		#region IPalette Members

		/// <summary>
		/// Gets the color by interpolation coefficient.
		/// </summary>
		/// <param name="t">Interpolation coefficient, should belong to [0..1].</param>
		/// <returns>Color.</returns>
		public override Color GetColor(double t)
		{
			if (t < 0) return steps[0].Color;
			if (t > 1) return steps[steps.Count - 1].Color;

			int i = 0;
			double x = 0;
			while (x <= t)
			{
				x = steps[i + 1].Offset;
				i++;
			}

			double ratio = (t - steps[i - 1].Offset) / (steps[i].Offset - steps[i - 1].Offset);

			Color c0 = steps[i - 1].Color;
			Color c1 = steps[i].Color;

			Color result = Color.FromRgb(
				(byte)((1 - ratio) * c0.R + ratio * c1.R),
				(byte)((1 - ratio) * c0.G + ratio * c1.G),
				(byte)((1 - ratio) * c0.B + ratio * c1.B));
			return result;
		}

		#endregion

		#region ISupportInitialize Members

		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
			if (steps.Count == 0 || steps[0].Offset > 0)
				this.steps.Insert(0, new LinearPaletteColorStep(startColor, 0));
			if (steps.Count == 0 || steps[steps.Count - 1].Offset < 1)
				this.steps.Add(new LinearPaletteColorStep(endColor, 1));
		}

		#endregion
	}
}
