using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class NormalizeFilter : VectorFieldConvolutionFilter
	{
		public override void ApplyFilter(int[] pixels, int width, int height, Vector[,] field)
		{
			double minBrightness = Double.PositiveInfinity;
			double maxBrightness = Double.NegativeInfinity;

			for (int i = 0; i < pixels.Length; i++)
			{
				int x = i % width;
				int y = i / width;

				int argb = pixels[i];
				var color = HsbColor.FromArgb(argb);
				var brightness = color.Brightness;

				if (brightness < minBrightness)
					minBrightness = brightness;
				if (brightness > maxBrightness)
					maxBrightness = brightness;
			}

			for (int i = 0; i < pixels.Length; i++)
			{
				int argb = pixels[i];
				var color = HsbColor.FromArgb(argb);
				var brightness = color.Brightness;

				double ratio = (brightness - minBrightness) / (maxBrightness - minBrightness);
				color.Brightness = ratio;
				pixels[i] = color.ToArgb();
			}
		}
	}
}
