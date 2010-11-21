using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.INonUniformDataSource2D<System.Windows.Vector>;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using System.Threading;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public class VectorFieldConvolutionChart : Image
	{
		public VectorFieldConvolutionChart()
		{
			Stretch = Stretch.Fill;
		}

		#region Properties

		public DataSource DataSource
		{
			get { return (DataSource)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(DataSource),
		  typeof(VectorFieldConvolutionChart),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VectorFieldConvolutionChart owner = (VectorFieldConvolutionChart)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(IDataSource2D<Vector> prevDataSource, IDataSource2D<Vector> currDataSource)
		{
			var dataSource = this.GetValueSync<IDataSource2D<Vector>>(DataSourceProperty);
			var contentBounds = dataSource.Grid.GetGridBounds();
			if (Parent != null)
				Viewport2D.SetContentBounds(Parent, contentBounds);
			ViewportPanel.SetViewportBounds(this, contentBounds);

			int width = currDataSource.Width;
			int height = currDataSource.Height;
			bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
			Source = bmp;

			Task task = new Task(() => CreateConvolutionBmp(width, height));
			task.Start();
		}

		#endregion // end of Properties

		WriteableBitmap bmp;

		private void UpdateBitmap(int[] pixels)
		{
			bmp.Dispatcher.BeginInvoke(() =>
			{
				int width = (int)bmp.Width;
				int height = (int)bmp.Height;
				bmp.WritePixels(new Int32Rect(0, 0, width, height), pixels, (width * bmp.Format.BitsPerPixel + 7) / 8, 0);
			});
		}


		private readonly NormalizeFilter normalizeFilter = new NormalizeFilter();
		public NormalizeFilter NormalizeFilter
		{
			get { return normalizeFilter; }
		}

		private readonly MagnitudeFilter magnitudeFilter = new MagnitudeFilter { Palette = new UniformLinearPalette(Colors.Green, Colors.GreenYellow, Colors.Red) };
		public MagnitudeFilter MagnitudeFilter
		{
			get { return magnitudeFilter; }
		}

		private void CreateConvolutionBmp(int width, int height)
		{
			int[] pixels = new int[width * height];

			var dataSource = this.GetValueSync<IDataSource2D<Vector>>(DataSourceProperty); ;
			GenerateWhiteNoizeImage(width, height, pixels);
			//GenerateWhiteCirclesImage(width, height, pixels);

			UpdateBitmap(pixels);

			int[] effectivePixels = CreateConvolutionArray(width, height, pixels);

			UpdateBitmap(effectivePixels);

			normalizeFilter.ApplyFilter(effectivePixels, width, height, dataSource.Data);

			magnitudeFilter.ApplyFilter(effectivePixels, width, height, dataSource.Data);

			UpdateBitmap(effectivePixels);
		}

		private static void GenerateWhiteNoizeImage(int width, int height, int[] pixels)
		{
			Random rnd = new Random();
			for (int i = 0; i < width * height; i++)
			{
				HsbColor color = new HsbColor(0, 0, Math.Round(5 * rnd.NextDouble()) / 4);
				int argb = color.ToArgb();
				pixels[i] = argb;
			}
		}

		private static void GenerateWhiteCirclesImage(int width, int height, int[] pixels)
		{
			DrawingImage image = new DrawingImage();

			Random rnd = new Random();
			const double radius = 4;
			const int circlesNum = 100;

			DrawingGroup group = new DrawingGroup();
			var dc = group.Open();
			for (int i = 0; i < circlesNum; i++)
			{
				dc.DrawEllipse(Brushes.White, null, new Point(rnd.NextDouble() * width, rnd.NextDouble() * height), radius, radius);
			}
			dc.Close();

			image.Drawing = group;

			RenderTargetBitmap renderBmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
			renderBmp.Render(new Image { Source = image });

			renderBmp.CopyPixels(pixels, (width * renderBmp.Format.BitsPerPixel + 7) / 8, 0);
		}

		private int[] CreateConvolutionArray(int width, int height, int[] pixels)
		{
			var dataSource = this.GetValueSync<DataSource>(DataSourceProperty); ;

			const int Length = 20;
			const int maxIterations = 20;

			int pixelsCount = width * height;
			int[] effectivePixels = new int[pixelsCount];

			pixels.CopyTo(effectivePixels, 0);

			System.Threading.Tasks.Parallel.For(0, pixelsCount, i =>
			{
				if (i % 1000 == 0)
					UpdateBitmap(effectivePixels);

				int ix = i % width;
				int iy = i / width;

				double sumDistance = 1;
				double positiveDistance = 0;
				Point position = dataSource.Grid[ix, iy];
				ConvolutionColor color = ConvolutionColor.FromArgb(pixels[ix + width * iy]);

				int iterationsCounter = 0;
				do
				{
					int i_x = ix;
					int i_y = iy;
					iterationsCounter++;

					Vector vector =
						//GetVector(dataSource, new IntPoint(i_x, i_y), position); 
						dataSource.Data[i_x, i_y];

					if (vector.Length > 0)
						vector.Normalize();

					position += vector;
					positiveDistance += vector.Length;

					IntPoint coordinate;
					bool found = GetCoordinate(dataSource, position, out coordinate);
					if (found)
					{
						i_x = coordinate.X;
						i_y = coordinate.Y;
						var currentColor = ConvolutionColor.FromArgb(pixels[i_x + i_y * width]);
						color += currentColor;

						sumDistance += 1;
					}
					else
						break;
				}
				while (positiveDistance < Length && iterationsCounter < maxIterations);

				var negativeDistance = 0.0;
				iterationsCounter = 0;
				do
				{
					int i_x = ix;
					int i_y = iy;
					iterationsCounter++;

					Vector vector =
						//GetVector(dataSource, new IntPoint(i_x, i_y), position);
						 dataSource.Data[i_x, i_y];
					if (vector.Length > 0)
						vector.Normalize();

					position -= vector;
					negativeDistance += vector.Length;

					IntPoint coordinate;
					bool found = GetCoordinate(dataSource, position, out coordinate);
					if (found)
					{
						i_x = coordinate.X;
						i_y = coordinate.Y;
						var currentColor = ConvolutionColor.FromArgb(pixels[i_x + i_y * width]);
						color += currentColor;

						sumDistance += 1;
					}
					else
						break;
				}
				while (negativeDistance < Length && iterationsCounter < maxIterations);

				color /= sumDistance;
				effectivePixels[i] = color.ToArgb();
			});

			return effectivePixels;
		}

		private static ConvolutionColor GetColor(DataSource dataSource, int[] pixels, IntPoint coordinate, Point point)
		{
			int x = coordinate.X;
			int y = coordinate.Y;

			var x0 = dataSource.XCoordinates[x];
			var x1 = dataSource.XCoordinates[x + 1];

			var y0 = dataSource.YCoordinates[y];
			var y1 = dataSource.YCoordinates[y + 1];

			double xRatio = GetRatio(x0, x1, point.X);
			double yRatio = GetRatio(y0, y1, point.Y);

			int width = dataSource.Width;

			var v00 = pixels[x + y * width];
			var v01 = pixels[x + 1 + y * width];
			var v10 = pixels[x + (y + 1) * width];
			var v11 = pixels[x + 1 + (y + 1) * width];

			var result = (int)(((1 - xRatio) * v00 + xRatio * v10 +
							(1 - xRatio) * v01 + xRatio * v11 +
							(1 - yRatio) * v00 + yRatio * v01 +
							(1 - yRatio) * v10 + yRatio * v11) * 0.25);
			//var result = v00;

			return ConvolutionColor.FromArgb(result);
		}

		private static Vector GetVector(DataSource dataSource, IntPoint index, Point position)
		{
			int ix0 = index.X;
			int iy0 = index.Y;

			int ix1 = ix0 + 1 < dataSource.Width ? ix0 + 1 : ix0;
			int iy1 = iy0 + 1 < dataSource.Height ? iy0 + 1 : iy0;

			var x0 = dataSource.XCoordinates[ix0];
			var x1 = dataSource.XCoordinates[ix1];

			var y0 = dataSource.YCoordinates[iy0];
			var y1 = dataSource.YCoordinates[iy1];

			double xRatio = GetRatio(x0, x1, position.X);
			double yRatio = GetRatio(y0, y1, position.Y);

			Vector v00 = dataSource.Data[ix0, iy0];
			Vector v01 = dataSource.Data[ix0, iy1];
			Vector v10 = dataSource.Data[ix1, iy0];
			Vector v11 = dataSource.Data[ix1, iy1];

			Vector result = (1 - xRatio) * v00 + xRatio * v10 +
							(1 - xRatio) * v01 + xRatio * v11 +
							(1 - yRatio) * v00 + yRatio * v01 +
							(1 - yRatio) * v10 + yRatio * v11;

			result *= 0.25;

			return result;
		}

		private static double GetRatio(double a, double b, double x)
		{
			if (a != b)
				return (x - a) / (b - a);
			else
				return x;
		}

		private static bool GetCoordinate(DataSource dataSource, Point point, out IntPoint coordinate)
		{
			var ix = IndexOf(dataSource.XCoordinates, point.X);
			int iy = IndexOf(dataSource.YCoordinates, point.Y);

			coordinate = new IntPoint(ix, iy);

			return ix > -1 && iy > -1;
		}

		private static int IndexOf(double[] coordinates, double x)
		{
			int ix = -1;
			for (int i = 0; i < coordinates.Length - 1; i++)
			{
				if (coordinates[i + 1] >= x)
				{
					ix = i;
					break;
				}
			}

			return ix;
		}

		//#region IPlotterElement Members

		//Plotter2D plotter;
		//public void OnPlotterAttached(Plotter plotter)
		//{
		//    this.plotter = (Plotter2D)plotter;
		//    plotter.CentralGrid.Children.Add(this);
		//}

		//public void OnPlotterDetaching(Plotter plotter)
		//{
		//    plotter.CentralGrid.Children.Remove(this);
		//    this.plotter = null;
		//}

		//public Plotter Plotter
		//{
		//    get { return plotter; }
		//}

		//#endregion
	}
}
