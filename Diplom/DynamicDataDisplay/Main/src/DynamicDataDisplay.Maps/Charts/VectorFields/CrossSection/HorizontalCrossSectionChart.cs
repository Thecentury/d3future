using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class HorizontalCrossSectionChart : CrossSectionChartBase
	{
		private readonly Polygon polygon = new Polygon { Height = 100, Stretch = Stretch.Fill };

		public override void OnPlotterAttached(Plotter plotter)
		{
			Plotter = (Plotter2D)plotter;
			plotter.BottomPanel.Children.Add(polygon);

			RebuildUI();
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			Plotter = (Plotter2D)plotter;
			plotter.BottomPanel.Children.Remove(polygon);
		}

		UniformField2DWrapper fieldWrapper;
		protected override void RebuildUI()
		{
			if (DataSource == null) return;
			if (Plotter == null) return;
			if (Palette == null) return;

			int width = DataSource.Width;
			int height = DataSource.Height;
			fieldWrapper = new UniformField2DWrapper(DataSource.Data);
			var coordinate = SectionCoordinate;

			var minMaxLength = DataSource.GetMinMaxLength();

			PointCollection points = new PointCollection(width + 2);

			var palette = Palette;
			int[] pixels = new int[width];
			for (int ix = 0; ix < width; ix++)
			{
				double x = ix;
				var value = fieldWrapper.GetVector(x / (double)width, coordinate / (double)width);
				double length = value.Length;
				if (length.IsNaN())
					length = minMaxLength.Min;

				double ratio = (length - minMaxLength.Min) / minMaxLength.GetLength();
				if (ratio < 0)
					ratio = 0;
				if (ratio > 1)
					ratio = 1;
				points.Add(new Point(x, 1 - ratio));

				var color = palette.GetColor(ratio);
				pixels[ix] = color.ToArgb();
			}

			points.Add(new Point(width, 1));
			points.Add(new Point(0, 1));

			polygon.Points = points;
			var paletteBmp = BitmapFrame.Create(width, 1, 96, 96, PixelFormats.Bgra32, null, pixels, (width * PixelFormats.Bgra32.BitsPerPixel + 7) / 8);
			var brush = new ImageBrush(paletteBmp);
			polygon.Fill = brush;
		}
	}
}
