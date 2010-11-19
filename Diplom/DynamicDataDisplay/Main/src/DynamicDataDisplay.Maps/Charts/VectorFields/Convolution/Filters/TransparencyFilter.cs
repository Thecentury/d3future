using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class TransparencyFilter : VectorFieldConvolutionFilter
	{
		private readonly int[] whiteNoize;

		public TransparencyFilter(int[] whiteNoize)
		{
			if (whiteNoize == null)
				throw new ArgumentNullException("whiteNoize");
			this.whiteNoize = whiteNoize;
		}

		public override int[] ApplyFilter(int[] pixels, int width, int height, Vector[,] field)
		{
			for (int i = 0; i < pixels.Length; i++)
			{
				int pixel = pixels[i];
				int whiteNoizePixel = whiteNoize[i];

				if (pixel == whiteNoizePixel)
					pixels[i] = Colors.Transparent.ToArgb();
			}

			return pixels;
		}
	}
}
