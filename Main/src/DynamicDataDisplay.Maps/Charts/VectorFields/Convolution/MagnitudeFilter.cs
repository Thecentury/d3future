using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class MagnitudeFilter : VectorFieldConvolutionFilter
	{
		public IPalette Palette { get; set; }

		#region IVectorFieldConvolutionFilter Members

		public override void ApplyFilter(int[] pixels, int width, int height, Vector[,] field)
		{
			double maxLength = Double.NegativeInfinity;
			double minLength = Double.PositiveInfinity;

			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					var length = field[ix, iy].Length;
					if (length > maxLength) maxLength = length;
					if (length < minLength) minLength = length;
				}
			}

			for (int i = 0; i < width * height; i++)
			{
				HsbColor color = HsbColor.FromArgb(pixels[i]);

				int ix = i % width;
				int iy = i / width;
				var length = field[ix, iy].Length;

				var ratio = (length - minLength) / (maxLength - minLength);
				if (ratio.IsNaN())
					ratio = 0;

				var paletteColor = Palette.GetColor(ratio).ToHsbColor();

				color.Hue = paletteColor.Hue;
				color.Saturation = paletteColor.Saturation;

				pixels[i] = color.ToArgb();
			}
		}

		#endregion
	}
}
