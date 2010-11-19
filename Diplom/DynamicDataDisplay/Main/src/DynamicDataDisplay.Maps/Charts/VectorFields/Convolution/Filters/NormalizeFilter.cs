using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class NormalizeFilter : VectorFieldConvolutionFilter
	{
		public override int[] ApplyFilter(int[] pixels, int width, int height, Vector[,] field)
		{
			double minBrightness = Double.PositiveInfinity;
			double maxBrightness = Double.NegativeInfinity;

			for (int i = 0; i < pixels.Length; i++)
			{
				int argb = pixels[i];
				var color = HsbColor.FromArgb(argb);
				var brightness = color.Brightness;

				if (brightness < minBrightness)
					minBrightness = brightness;
				if (brightness > maxBrightness)
					maxBrightness = brightness;
			}

			if (minBrightness == 0 || maxBrightness == 1)
				return pixels;

			Parallel.For(0, pixels.Length, i =>
			{
				int argb = pixels[i];
				var color = HsbColor.FromArgb(argb);
				var brightness = color.Brightness;

				double ratio = (brightness - minBrightness) / (maxBrightness - minBrightness);
				color.Brightness = ratio;
				pixels[i] = color.ToArgb();
			});

			return pixels;
		}
	}
}
