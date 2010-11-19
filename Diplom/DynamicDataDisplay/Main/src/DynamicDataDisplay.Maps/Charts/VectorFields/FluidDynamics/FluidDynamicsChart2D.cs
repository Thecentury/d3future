//#define parallel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class FluidDynamicsChart2D : Image
	{
		#region Physics

		private void Diffuse(int N, int b, double[,] x, double[,] x0, double diff, double dt)
		{
			double a = dt * diff * N * N;

			for (int k = 0; k < 20; k++)
				for (int i = 1; i <= N; i++)
					for (int j = 1; j <= N; j++)
						x[i, j] = (x0[i, j] + a * (x[i - 1, j] + x[i + 1, j] + x[i, j - 1] + x[i, j + 1])) / (1 + 4 * a);

			SetBoundary(N, b, x);
		}

		private void Advect(int N, int b, double[,] d, double[,] d0, double[,] u, double[,] v, double dt)
		{
			double dt0 = dt * N;

#if parallel
			Parallel.For(1, N + 1, i =>
#else
			for (int i = 1; i <= N; i++)
#endif
			{
				for (int j = 1; j <= N; j++)
				{
					double x = i - dt0 * u[i, j];
					double y = j - dt0 * v[i, j];

					if (x < 0.5)
						x = 0.5;
					else if (x > N + 0.5)
						x = N + 0.5;

					int i0 = (int)x;
					int i1 = i0 + 1;

					if (y < 0.5)
						y = 0.5;
					else if (y > N + 0.5)
						y = N + 0.5;

					int j0 = (int)y;
					int j1 = j0 + 1;

					double s1 = x - i0;
					double s0 = 1 - s1;
					double t1 = y - j0;
					double t0 = 1 - t1;

					d[i, j] = s0 * (t0 * d0[i0, j0] + t1 * d0[i0, j1]) +
						s1 * (t0 * d0[i1, j0] + t1 * d0[i1, j1]);
				}
			}
#if parallel
);
#endif

			SetBoundary(N, b, d);
		}

		void DensityStep(int N, double[,] x, double[,] x0, double[,] u, double[,] v, double diff, double dt)
		{
			AddSource(N, x, x0, dt);

			Diffuse(N, 0, x0, x, diff, dt);
			Advect(N, 0, x, x0, u, v, dt);
		}

		void VelocityStep(int N, double[,] u, double[,] v, double[,] u0, double[,] v0, double visc, double dt)
		{
			AddSource(N, u, u0, dt);
			AddSource(N, v, v0, dt);
			Diffuse(N, 1, u0, u, visc, dt);
			Diffuse(N, 2, v0, v, visc, dt);
			Project(N, u0, v0, u, v);
			Advect(N, 1, u, u0, u0, v0, dt);
			Advect(N, 2, v, v0, u0, v0, dt);
			Project(N, u, v, u0, v0);
		}

		private void Project(int N, double[,] u, double[,] v, double[,] p, double[,] div)
		{
			double h = 1.0 / N;
#if parallel
			Parallel.For(1, N + 1, i =>
#else
			for (int i = 1; i <= N; i++)
#endif
			{
				for (int j = 1; j <= N; j++)
				{
					div[i, j] = -0.5 * h * (u[i + 1, j] - u[i - 1, j] + v[i, j + 1] - v[i, j - 1]);
					p[i, j] = 0;
				}
			}
#if parallel
);
#endif

			SetBoundary(N, 0, div);
			SetBoundary(N, 0, p);

			for (int k = 0; k < 20; k++)
			{
				for (int i = 1; i <= N; i++)
				{
					for (int j = 1; j <= N; j++)
					{
						p[i, j] = 0.25 * (div[i, j] + p[i - 1, j] + p[i + 1, j] + p[i, j - 1] + p[i, j + 1]);
					}
				}

				SetBoundary(N, 0, p);
			}

#if parallel
			Parallel.For(1, N + 1, i =>
#else
			for (int i = 1; i <= N; i++)
#endif
			{
				for (int j = 1; j <= N; j++)
				{
					u[i, j] -= 0.5 * (p[i + 1, j] - p[i - 1, j]) / h;
					v[i, j] -= 0.5 * (p[i, j + 1] - p[i, j - 1]) / h;
				}
			}
#if parallel
);
#endif

			SetBoundary(N, 1, u);
			SetBoundary(N, 2, v);
		}

		private void AddSource(int N, double[,] x, double[,] x0, double dt)
		{
			for (int ix = 1; ix <= N; ix++)
			{
				x[ix, 1] += dt * x0[ix, 1];
			}
		}

		private void SetBoundary(int N, int b, double[,] x)
		{
			for (int i = 1; i <= N; i++)
			{
				x[0, i] = b == 1 ? -x[1, i] : x[1, i];
				x[N + 1, i] = b == 1 ? -x[N, i] : x[N, i];
				x[i, 0] = b == 2 ? -x[i, 1] : x[i, 1];
				x[i, N + 1] = b == 2 ? -x[i, N] : x[i, N];
			}

			x[0, 0] = 0.5 * (x[1, 0] + x[0, 1]);
			x[0, N + 1] = 0.5 * (x[1, N + 1] + x[0, N]);
			x[N + 1, 0] = 0.5 * (x[N, 0] + x[N + 1, 1]);
			x[N + 1, N + 1] = 0.5 * (x[N, N + 1] + x[N + 1, N]);
		}

		#endregion Physics

		#region Properties



		#endregion Properties

		private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = TimeSpan.FromMilliseconds(40) };
		private readonly Stopwatch watch = Stopwatch.StartNew();
		private long prevElapsedMilliseconds = 0;
		protected const int N = 100;
		private readonly WriteableBitmap bmp = new WriteableBitmap(N + 2, N + 2, 96, 96, PixelFormats.Bgra32, null);
		private readonly Image image = new Image();
		protected readonly double[,] u, v, u_prev, v_prev, dens, dens_prev;

		public FluidDynamicsChart2D()
		{
			Stretch = Stretch.Fill;

			timer.Tick += timer_Tick;
			timer.Start();

			u = new double[N + 2, N + 2];
			v = new double[N + 2, N + 2];
			u_prev = new double[N + 2, N + 2];
			v_prev = new double[N + 2, N + 2];
			dens = new double[N + 2, N + 2];
			dens_prev = new double[N + 2, N + 2];

			Source = bmp;
		}

		void timer_Tick(object sender, EventArgs e)
		{
			using (new DisposableTimer("tick"))
			{
				RebuildUI();
			}
		}

		protected void RebuildUI()
		{
			var time = watch.ElapsedMilliseconds;
			double dt = time - prevElapsedMilliseconds;
			prevElapsedMilliseconds = time;

			dt /= 1000;
			GetFluidData(dens_prev, u_prev, v_prev);
			VelocityStep(N, u, v, u_prev, v_prev, 0.01, dt);
			DensityStep(N, dens, dens_prev, u, v, 2.0, dt);

			double densityMin = Double.PositiveInfinity;
			double densityMax = Double.NegativeInfinity;
			for (int i = 0; i < N + 2; i++)
			{
				for (int j = 0; j < N + 2; j++)
				{
					var value = dens[i, j];
					if (value < densityMin)
						densityMin = value;
					if (value > densityMax)
						densityMax = value;
				}
			}

			int[] pixels = new int[(N + 2) * (N + 2)];

			for (int i = 0; i < N + 2; i++)
			{
				for (int j = 0; j < N + 2; j++)
				{
					double value = dens[i, N + 1 - j];
					HsbColor color = new HsbColor(10, 1, (value - densityMin) / (densityMax - densityMin));

					pixels[(N + 1 - j) * (N + 2) + i] = color.ToArgb();
				}
			}

			bmp.WritePixels(new Int32Rect(0, 0, N + 2, N + 2), pixels, ((N + 2) * PixelFormats.Bgra32.BitsPerPixel + 7) / 8, 0);
		}

		protected virtual void GetFluidData(double[,] prevDensity, double[,] u_prev, double[,] v_prev)
		{
			//prevDensity[N / 2, N / 2] += 100;

			//for (int i = 1; i <= N; i++)
			//{
			//    u_prev[1, i] = -1;
			//}
		}
	}
}
