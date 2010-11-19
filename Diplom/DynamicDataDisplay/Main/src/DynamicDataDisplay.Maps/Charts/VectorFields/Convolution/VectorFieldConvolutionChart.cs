#define p // parallel

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Maps.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.INonUniformDataSource2D<System.Windows.Vector>;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public class VectorFieldConvolutionChart : VectorFieldChartBase
	{
		public VectorFieldConvolutionChart()
		{
		}

		WriteableBitmap bmp;

		private void UpdateBitmap(int[] pixels)
		{
			if (bmp == null)
				return;

			bmp.Dispatcher.BeginInvoke(() =>
			{
				int width = (int)bmp.Width;
				int height = (int)bmp.Height;
				bmp.WritePixels(new Int32Rect(0, 0, width, height), pixels, (width * bmp.Format.BitsPerPixel + 7) / 8, 0);
			}, DispatcherPriority.Loaded);
		}

		private readonly NormalizeFilter normalizeFilter = new NormalizeFilter();
		public NormalizeFilter NormalizeFilter
		{
			get { return normalizeFilter; }
		}

		private readonly MagnitudeFilter magnitudeFilter = new MagnitudeFilter { Palette = new PowerPalette(new UniformLinearPalette(Colors.Green, Colors.GreenYellow, Colors.Red), 0.075) };
		public MagnitudeFilter MagnitudeFilter
		{
			get { return magnitudeFilter; }
		}

		private TransparencyFilter transparencyFilter;

		int[] unmodifiedWhiteNoize = null;
		internal int[] WhiteNoize
		{
			get { return unmodifiedWhiteNoize; }
			set { unmodifiedWhiteNoize = value; }
		}

		private Vector missingvalue = new Vector(-999, -999);
		public Vector MissingValue
		{
			get { return missingvalue; }
			set { missingvalue = value; }
		}

		private void CreateConvolutionBmp(int width, int height, ParallelOptions parallelOptions)
		{
			var dataSource = this.GetValueSync<IDataSource2D<Vector>>(DataSourceProperty);
			if (dataSource == null)
				return;

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			int[] pixels = null;
			if (unmodifiedWhiteNoize == null)
			{
				pixels = ImageHelper.CreateWhiteNoizeImage(width, height);
				unmodifiedWhiteNoize = new int[pixels.Length];
				pixels.CopyTo(unmodifiedWhiteNoize, 0);
			}
			else
			{
				pixels = new int[unmodifiedWhiteNoize.Length];
				unmodifiedWhiteNoize.CopyTo(pixels, 0);
			}

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			UpdateBitmap(pixels);

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			bool cancelled = false;
			int[] effectivePixels = CreateConvolutionArray(width, height, pixels, parallelOptions, out cancelled);

			if (cancelled)
				return;

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			UpdateBitmap(effectivePixels);

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			if (transparencyFilter == null)
				transparencyFilter = new TransparencyFilter(unmodifiedWhiteNoize);

			//effectivePixels = transparencyFilter.ApplyFilter(effectivePixels, width, height, dataSource.Data);

			effectivePixels = normalizeFilter.ApplyFilter(effectivePixels, width, height, dataSource.Data);

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			effectivePixels = magnitudeFilter.ApplyFilter(effectivePixels, width, height, dataSource.Data);

			if (parallelOptions.CancellationToken.IsCancellationRequested)
				return;

			UpdateBitmap(effectivePixels);

			RaiseRenderingFinished();
		}

		private void RaiseRenderingFinished()
		{
			Dispatcher.BeginInvoke(() =>
			{
				RaiseEvent(new RoutedEventArgs(BackgroundRenderer.RenderingFinished));
			});
		}

		private int[] CreateConvolutionArray(int width, int height, int[] pixels, ParallelOptions parallelOptions, out bool cancelled)
		{
			cancelled = false;

			var dataSource = this.GetValueSync<DataSource>(DataSourceProperty);
			if (dataSource == null)
			{
				return new int[0];
			}

			const int Length = 20;
			const int maxIterations = 20;

			int pixelsCount = width * height;
			int[] effectivePixels = new int[pixelsCount];

			if (parallelOptions.CancellationToken.IsCancellationRequested)
			{
				cancelled = true;
				return null;
			}

			pixels.CopyTo(effectivePixels, 0);

			var bounds = dataSource.GetGridBounds();

			try
			{
#if p
				Parallel.For(0, pixelsCount, parallelOptions, (i, loopState) =>
				{
					if (i % 1000 == 0 && parallelOptions.CancellationToken.IsCancellationRequested)
					{
						loopState.Break();
					}
#else
				for (int i = 0; i < pixelsCount; i++)
				{
#endif

					if (i % 2000 == 0)
						UpdateBitmap(effectivePixels);

					int ix = i % width;
					int iy = i / width;

					double sumDistance = 1;
					double positiveDistance = 0;
					Point position = dataSource.Grid[ix, iy];
					int intColor = Colors.Transparent.ToArgb();
					Vector v = dataSource.Data[ix, iy];
					if (v != missingvalue)
					{
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

							vector = vector.ChangeLength(bounds.Width, bounds.Height, width, height);

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
						intColor = color.ToArgb();
					}

					effectivePixels[(height - 1 - iy) * width + ix] = intColor;
				}
#if p
				);
#endif
			}
			catch (OperationCanceledException)
			{
				cancelled = true;
			}

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
				return 0.5;
		}

		private static bool GetCoordinate(DataSource dataSource, Point point, out IntPoint coordinate)
		{
			int ix = BinarySearch.SearchInterval(dataSource.XCoordinates, point.X);
			int iy = BinarySearch.SearchInterval(dataSource.YCoordinates, point.Y);

			coordinate = new IntPoint(ix, iy);

			return ix > BinarySearch.NotFound && iy > BinarySearch.NotFound;
		}

		CancellationTokenSource tokenSource = new CancellationTokenSource();
		DataRect contentBounds = DataRect.Empty;
		protected override void RebuildUI()
		{
			//if (Plotter == null)
			//    return;
			if (DataSource == null)
				return;

			tokenSource.Cancel();
			tokenSource = new CancellationTokenSource();

			var dataSource = this.GetValueSync<IDataSource2D<Vector>>(DataSourceProperty);
			contentBounds = dataSource.Grid.GetGridBounds();
			Viewport2D.SetContentBounds(this, contentBounds);
			ViewportPanel.SetViewportBounds(image, contentBounds);

			int width = dataSource.Width;
			int height = dataSource.Height;
			// do not create new bitmap if size of previous one equals size of new data source
			if (bmp == null || width != bmp.PixelWidth || height != bmp.PixelHeight)
			{
				bmp = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, palette: null);
				image.Source = bmp;
			}

			ParallelOptions parallelOptions = new ParallelOptions { CancellationToken = tokenSource.Token };
			Task.Factory.StartNew(() => CreateConvolutionBmp(width, height, parallelOptions), tokenSource.Token);
		}

		private readonly Image image = new Image { Stretch = Stretch.Fill };
		public Image Image
		{
			get { return image; }
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			panel.Children.Add(image);
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			panel.Children.Remove(image);

			base.OnPlotterDetaching(plotter);
		}
	}
}
