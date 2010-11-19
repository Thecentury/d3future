using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Auxiliary
{
	internal static class ImageHelper
	{
		public static int[] CreateWhiteNoizeImage(int width, int height)
		{
			int[] pixels = new int[width * height];

			Random rnd = new Random();
			for (int i = 0; i < width * height; i++)
			{
				HsbColor color = new HsbColor(0, 0, Math.Round(5 * rnd.NextDouble()) / 4);
				int argb = color.ToArgb();
				pixels[i] = argb;
			}

			return pixels;
		}

		public static int[] CreateSomeColorBitmap(int width, int height)
		{
			int[] pixels = new int[width * height];

			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = unchecked((int)0xA0A0A0FF);
			}

			return pixels;
		}

		public static Brush CreatePaletteBrush(IPalette palette, int width = 256)
		{
			const double dpi = 96;

			WriteableBitmap bmp = new WriteableBitmap(width, 1,
				dpi, dpi,
				PixelFormats.Bgra32, null);

			int[] pixels = new int[width];
			for (int i = 0; i < width; i++)
			{
				double ratio = i / ((double)width);
				Color color = palette.GetColor(ratio);
				int argb = color.ToArgb();
				pixels[i] = argb;
			}

			bmp.WritePixels(
				new Int32Rect(0, 0, width, 1),
				pixels,
				bmp.BackBufferStride,
				0);

			return new ImageBrush(bmp);
		}

		public static int[] CreateDottedImage(int width, int height)
		{
			Random rnd = new Random();
			const int pointsNum = 100000;
			const double radius = 1.5;

			var randomPoints = Enumerable.Range(0, pointsNum).Select(_ => new Point(rnd.NextDouble() * width, rnd.NextDouble() * height));
			randomPoints = Filter(randomPoints, radius);

			DrawingGroup drawing = new DrawingGroup();
			var dc = drawing.Append();
			foreach (var point in randomPoints)
			{
				HsbColor color = new HsbColor(0, 0, Math.Round(5 * rnd.NextDouble()) / 4);
				SolidColorBrush brush = new SolidColorBrush(color.ToArgbColor());

				//drawing.Children.Add(new GeometryDrawing(brush, null, new EllipseGeometry(point, radius, radius)));
				dc.DrawEllipse(brush, null, point, radius, radius);
			}
			dc.Close();

			DrawingImage drawingImage = new DrawingImage();
			drawingImage.Drawing = drawing;
			drawingImage.Freeze();

			if ((imageCreatingThread.ThreadState | ThreadState.Running) != imageCreatingThread.ThreadState)
				imageCreatingThread.Start();
			imageQueue.Add(new RequestInfo { Width = width, Heigth = height, DrawingImage = drawingImage });
			var pixels = resultQueue.Take();

			return pixels;
		}

		static ImageHelper()
		{
			imageCreatingThread.SetApartmentState(ApartmentState.STA);
		}

		private sealed class RequestInfo
		{
			public int Width { get; set; }
			public int Heigth { get; set; }
			public DrawingImage DrawingImage { get; set; }
		}

		private static readonly BlockingCollection<RequestInfo> imageQueue = new BlockingCollection<RequestInfo>();
		private static readonly BlockingCollection<int[]> resultQueue = new BlockingCollection<int[]>();
		private readonly static Thread imageCreatingThread = new Thread(ThreadFunc) { IsBackground = true, Priority = ThreadPriority.BelowNormal };
		private static void ThreadFunc(object state)
		{
			while (true)
			{
				var request = imageQueue.Take();

				int width = request.Width;
				int height = request.Heigth;
				Image image = new Image { Width = width, Height = height, Source = request.DrawingImage, Stretch = Stretch.Fill };
				image.Measure(new Size(width, height));
				image.Arrange(new Rect(0, 0, width, height));
				image.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Background);

#if false
				Window window = new Window();
				window.Content = image;
				window.Show();
#endif

				RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
				renderBitmap.Render(image);
				image.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.Background);

				int[] pixels = new int[width * height];
				renderBitmap.CopyPixels(pixels, (width * PixelFormats.Pbgra32.BitsPerPixel + 7) / 8, 0);

				resultQueue.Add(pixels);
			}
		}

		private static IEnumerable<Point> Filter(IEnumerable<Point> points, double pointSize)
		{
			var roots = new List<Point>();

			foreach (var point in points)
			{
				bool rootNotFound = true;
				foreach (var root in roots)
				{
					if ((root - point).Length < pointSize)
					{
						rootNotFound = false;
						break;
					}
				}

				if (rootNotFound)
				{
					roots.Add(point);
				}
			}

			return roots;
		}
	}
}
