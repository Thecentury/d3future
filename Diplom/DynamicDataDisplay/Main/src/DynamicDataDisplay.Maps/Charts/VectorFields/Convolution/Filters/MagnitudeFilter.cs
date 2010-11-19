using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class MagnitudeFilter : VectorFieldConvolutionFilter
	{
		public IPalette Palette { get; set; }

		#region IVectorFieldConvolutionFilter Members

		public override int[] ApplyFilter(int[] pixels, int width, int height, Vector[,] field)
		{
			double maxLength = Double.NegativeInfinity;
			double minLength = Double.PositiveInfinity;

			// determine min and max length
			// this works faster than parallel enumerable version.
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					var length = field[ix, iy].Length;
					if (length > maxLength) maxLength = length;
					if (length < minLength) minLength = length;
				}
			}

			int[] resultPixels = new int[width * height];
			pixels.CopyTo(resultPixels, 0);

			// attempt to avoid exception on line
			// 'HsbColor color = HsbColor.FromArgb(pixels[i]);'
			if (pixels.Length == 0)
				return pixels;

			Parallel.For(0, width * height, i =>
			{
				HsbColor color = HsbColor.FromArgb(pixels[i]);

				int ix = i % width;
				int iy = i / width;
				var length = field[ix, height - 1 - iy].Length;

				var ratio = (length - minLength) / (maxLength - minLength);
				if (ratio.IsNaN())
					ratio = 0;

				var paletteColor = Palette.GetColor(ratio).ToHsbColor();

				color.Hue = paletteColor.Hue;
				color.Saturation = paletteColor.Saturation;

				resultPixels[i] = color.ToArgb();
			});

			return resultPixels;
		}

		#endregion
	}
}
