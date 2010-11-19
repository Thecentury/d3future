using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class FluidImage : Image
	{
		// frame dimensions (dxd pixels)
		protected int size = 102;

		// solver variables
		protected int n = 100;
		protected double dt = 0.2;
		protected readonly FluidSolver solver = new FluidSolver();
		public FluidSolver Solver
		{
			get { return solver; }
		}

		// mouse position
		protected int x, xOld;
		protected int y, yOld;

		DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(50) };
		WriteableBitmap bmp;

		public FluidImage()
		{
			bmp = new WriteableBitmap(n, n, 96, 96, PixelFormats.Bgra32, null);
			Source = bmp;
			Stretch = Stretch.Fill;

			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			Focusable = true;

			solver.Setup(n, dt);
		}

		protected virtual void UpdateDynamicDensity() { }

		private readonly IPalette palette = new UniformLinearPalette(Colors.OrangeRed, Colors.Yellow, Colors.PowderBlue, Colors.LightGray, Colors.White);
		private void timer_Tick(object sender, EventArgs e)
		{
			int length = (n + 2) * (n + 2);
			int[] pixels = new int[length];
			for (int i = 0; i < length; i++)
			{
				pixels[i] = unchecked((int)0xFFFFFFFF);
			}

			UpdateDynamicDensity();

			using (new DisposableTimer("physics", isActive: true, printStart: false))
			{
				solver.SolveVelocity();
				solver.SolveDensity();
			}

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					int index = I(i, j);
					int pixelAddress = j * n + n - 1 - i;
					// draw density
					if (solver.OccupiedCells[index])
						pixels[pixelAddress] = Colors.LimeGreen.ToArgb();
					else if (solver.density[index] > 0)
					{

						double ratio = 1 - solver.density[I(i, j)];

						if (ratio > 1) ratio = 1;
						else if (ratio < 0) ratio = 0;

						Color color = palette.GetColor(ratio);

						pixels[pixelAddress] = color.ToArgb();
					}
				}
			}

			bmp.WritePixels(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight), pixels, (bmp.PixelWidth * PixelFormats.Bgra32.BitsPerPixel + 7) / 8, 0);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.R)
			{
				solver.Reset();
				e.Handled = true;
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			// update mouse position
			xOld = x;
			yOld = y;

			IntPoint pt = ScreenToField(e.GetPosition(this));
			x = pt.X;
			y = pt.Y;

			UpdateLocation(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released &&
				Keyboard.IsKeyUp(Key.Space) && Keyboard.IsKeyUp(Key.Delete))
				return;

			// update mouse position
			xOld = x;
			yOld = y;
			IntPoint pt = ScreenToField(e.GetPosition(this));
			x = pt.X;
			y = pt.Y;

			UpdateLocation(e);
		}


		/// <summary>
		/// get index for fluid cell under mouse position.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		protected IntPoint ScreenToField(Point p)
		{
			IntPoint result = new IntPoint();
			result.X = (int)(n - p.X / ActualWidth * n + 1);
			result.Y = (int)(p.Y / ActualHeight * n + 1);
			return result;
		}

		protected void UpdateDensity(IntPoint pt)
		{
			var i = pt.X;
			var j = pt.Y;

			solver.densityOld[I(i, j)] = 100;
		}

		protected void CreateObstacle(IntPoint pt)
		{
			solver.OccupiedCells[I(pt.X, pt.Y)] = true;
		}

		protected void DeleteObstacle(IntPoint pt)
		{
			solver.OccupiedCells[I(pt.X, pt.Y)] = false;
		}

		protected void UpdateVelocity(IntPoint pt)
		{
			var i = pt.X;
			var j = pt.Y;

			solver.uOld[I(i, j)] = (x - xOld) * 5;
			solver.vOld[I(i, j)] = (y - yOld) * 5;
		}

		protected IntPoint NormalizePoint(IntPoint pt)
		{
			if (pt.X > n)
				pt.X = n;
			else if (pt.X < 1)
				pt.X = 1;
			if (pt.Y > n)
				pt.Y = n;
			else if (pt.Y < 1)
				pt.Y = 1;

			return pt;
		}

		protected virtual void UpdateLocation(MouseEventArgs e)
		{
			Point[] intermediatePoints = new Point[64];
			var pointsCount = Mouse.GetIntermediatePoints(this, intermediatePoints);

			for (int i = 0; i < pointsCount; i++)
			{
				var pt = ScreenToField(intermediatePoints[i]);
				pt = NormalizePoint(pt);
				if (Keyboard.IsKeyDown(Key.Space))
				{
					CreateObstacle(pt);
				}
				else if (Keyboard.IsKeyDown(Key.Delete))
				{
					DeleteObstacle(pt);
				}
				else if (e.LeftButton == MouseButtonState.Pressed)
				{
					UpdateDensity(pt);
				}
				else if (e.RightButton == MouseButtonState.Pressed)
				{
					UpdateVelocity(pt);
				}
			}
		}

		// util function for indexing
		protected int I(int i, int j) { return i + (n + 2) * j; }
	}
}
