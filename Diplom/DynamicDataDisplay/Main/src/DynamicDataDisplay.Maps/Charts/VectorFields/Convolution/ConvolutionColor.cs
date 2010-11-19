using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	[DebuggerDisplay("R={R}, G={G}, B={B}")]
	internal struct ConvolutionColor
	{
		public ConvolutionColor(double r, double g, double b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
		}

		public double R;
		public double G;
		public double B;

		public static ConvolutionColor operator +(ConvolutionColor color1, ConvolutionColor color2)
		{
			return new ConvolutionColor(color1.R + color2.R, color1.G + color2.G, color1.B + color2.B);
		}

		public static ConvolutionColor operator *(ConvolutionColor color, double d)
		{
			return new ConvolutionColor(color.R * d, color.G * d, color.B * d);
		}

		public static ConvolutionColor operator /(ConvolutionColor color, double d)
		{
			return new ConvolutionColor((int)(color.R / d), (int)(color.G / d), (int)(color.B / d));
		}

		public static ConvolutionColor FromArgb(int argb)
		{
			ConvolutionColor result = new ConvolutionColor();
			argb &= 0x00FFFFFF;
			result.R = argb >> 16;
			result.G = argb >> 8 & 0xFF;
			result.B = argb & 0xFF;
			return result;
		}

		public int ToArgb()
		{
			return 0xFF << 24 | (((int)R) & 0xFF) << 16 | (((int)G) & 0xFF) << 8 | (((int)B) & 0xFF);
		}

		public int ToBgra()
		{
			return (((int)B) & 0xFF) << 24 | (((int)G) & 0xFF) << 16 | (((int)R) & 0xFF) << 8 | 0xFF;
		}

		public static implicit operator ConvolutionColor(Color color)
		{
			return new ConvolutionColor { R = color.R, G = color.G, B = color.B };
		}

		public void MakeGrayScale()
		{
			double gray = 0.3 * R + 0.59 * G + 0.11 * B;
			R = G = B = gray;
		}
	}
}
