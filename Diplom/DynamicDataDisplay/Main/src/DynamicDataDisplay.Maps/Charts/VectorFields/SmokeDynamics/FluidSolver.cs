#define p // parallel
#define po // parallel obstacles

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class FluidSolver
	{
		int n, size;
		double dt;

		double visc = 0.0;
		double diff = 0.0;
		/// <summary>
		/// Force of friction between fluid and obstacle.
		/// </summary>
		double friction = 0.0;

		double[] tmp;

		internal double[] density, densityOld;
		internal double[] u, uOld;
		internal double[] v, vOld;
		internal double[] curl;
		internal double buoyancyCoeff = 0.025;

		/// <summary>
		/// Set the grid size and timestep.
		/// </summary>
		/// <param name="n"></param>
		/// <param name="dt"></param>
		public void Setup(int n, double dt)
		{
			this.n = n;
			this.dt = dt;
			size = (n + 2) * (n + 2);

			Reset();
		}

		/// <summary>
		/// Reset the datastructures.
		/// We use 1d arrays for speed.
		/// </summary>
		public void Reset()
		{
			density = new double[size];
			densityOld = new double[size];
			u = new double[size];
			uOld = new double[size];
			v = new double[size];
			vOld = new double[size];
			curl = new double[size];
			occupiedCells = new bool[size];
		}


		/// <summary>
		/// Calculate the buoyancy force as part of the velocity solver.
		/// Fbuoy = -a*d*Y + b*(T-Tamb)*Y where Y = (0,1). The constants
		/// a and b are positive with appropriate (physically meaningful)
		/// units. T is the temperature at the current cell, Tamb is the
		/// average temperature of the fluid grid. The density d provides
		/// a mass that counteracts the buoyancy force.
		/// 
		/// In this simplified implementation, we say that the tempterature
		/// is synonymous with density (since smoke is *hot*) and because
		/// there are no other heat sources we can just use the density
		/// field instead of a new, seperate temperature field.
		/// (Плавучесть, способность держаться на поверхности воды.)
		/// </summary>
		/// <param name="buoyancy">Array to store buoyancy force for each cell.</param>
		public void Buoyancy(double[] buoyancy)
		{
			double temperatureAmbient = 0;
			// todo возможно, вернуть старые значения
			double a = 0.000625; // отвечает за диффузию газа
			double b = buoyancyCoeff;// 0.025;// коэффициент отвечает за всплытие нагретого газа вверх

			using (new DisposableTimer("buoyancy", isActive: false))
			{
#if !p
			// sum all temperatures
			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					temperatureAmbient += density[I(i, j)];
				}
			}
#else
				temperatureAmbient = density.AsParallel().Sum();
#endif
			}

			// get average temperature
			temperatureAmbient /= (n * n);

			// for each cell compute buoyancy force
			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					buoyancy[I(i, j)] = a * density[I(i, j)] - b * (density[I(i, j)] - temperatureAmbient);
				}
			}
		}

		/// <summary>
		/// Calculate the curl at position (i, j) in the fluid grid
		/// Physically this represents the vortex strength at the
		/// cell. Computed as follows: w = (del x U) where U is the
		/// velocity vector at (i, j).
		/// Vortex - водоворот, вихрь, воронка.
		/// Curl - водоворот.
		/// </summary>
		/// <param name="i">The x index of the cell.</param>
		/// <param name="j">The y index of the cell.</param>
		/// <returns></returns>
		public double CurlFunction(int i, int j)
		{
			double du_dy = (u[I(i, j + 1)] - u[I(i, j - 1)]) * 0.5;
			double dv_dx = (v[I(i + 1, j)] - v[I(i - 1, j)]) * 0.5;

			return du_dy - dv_dx;
		}

		/// <summary>
		/// Calculate the vorticity confinement force for each cell
		/// in the fluid grid. At a point (i,j), Fvc = N x w where
		/// w is the curl at (i,j) and N = del |w| / |del |w||.
		/// N is the vector pointing to the vortex center, hence we
		/// add force perpendicular to N.
		/// Ограничение вихреобразования.
		/// /// </summary>
		/// <param name="vorticity_x">
		/// The array to store the x component of the
		/// vorticity confinement force for each cell.
		/// </param>
		/// <param name="vorticity_y">
		/// The array to store the y component of the
		/// vorticity confinement force for each cell.
		/// </param>
		public void VorticityConfinement(double[] vorticity_x, double[] vorticity_y)
		{
			double dw_dx, dw_dy;
			double length;
			double v;

			// Calculate magnitude of curl(u,v) for each cell. (|w|)
			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					curl[I(i, j)] = Math.Abs(CurlFunction(i, j));
				}
			}

			for (int i = 2; i < n; i++)
			{
				for (int j = 2; j < n; j++)
				{

					// Find derivative of the magnitude (n = del |w|)
					dw_dx = (curl[I(i + 1, j)] - curl[I(i - 1, j)]) * 0.5;
					dw_dy = (curl[I(i, j + 1)] - curl[I(i, j - 1)]) * 0.5;

					// Calculate vector length. (|n|)
					// Add small factor to prevent divide by zeros.
					length = Math.Sqrt(dw_dx * dw_dx + dw_dy * dw_dy)
							 + 0.000001;

					// N = ( n/|n| )
					dw_dx /= length;
					dw_dy /= length;

					v = CurlFunction(i, j);

					// N x w
					vorticity_x[I(i, j)] = dw_dy * -v;
					vorticity_y[I(i, j)] = dw_dx * v;
				}
			}
		}

		/// <summary>
		/// The basic velocity solving routine as described by Stam.
		/// </summary>
		public void SolveVelocity()
		{
			// add velocity that was input by mouse
			addSource(u, uOld);
			addSource(v, vOld);

			// add in vorticity confinement force
			VorticityConfinement(uOld, vOld);
			addSource(u, uOld);
			addSource(v, vOld);

			// add in buoyancy force
			Buoyancy(vOld);
			addSource(v, vOld);

			// was added by me
			if (buoyancyCoeff == 0)
			{
				Buoyancy(uOld);
				addSource(u, uOld);
			}

			// swapping arrays for economical mem use
			// and calculating diffusion in velocity.
			swapU();
			Diffuse(0, u, uOld, visc);

			swapV();
			Diffuse(0, v, vOld, visc);

			// we create an incompressible field
			// for more effective advection.
			Project(u, v, uOld, vOld);

			swapU(); swapV();

			// self advect velocities
			Advect(1, u, uOld, uOld, vOld);
			Advect(2, v, vOld, uOld, vOld);

			// make an incompressible field
			Project(u, v, uOld, vOld);

			// clear all input velocities for next frame
			Array.Clear(uOld, 0, size);
			Array.Clear(vOld, 0, size);
		}


		/**
		 * The basic density solving routine.
		 **/

		public void SolveDensity()
		{
			// add density inputted by mouse
			addSource(density, densityOld);
			swapD();

			Diffuse(0, density, densityOld, diff);
			swapD();

			Advect(0, density, densityOld, u, v);

			// clear input density array for next frame
			Array.Clear(densityOld, 0, densityOld.Length);
		}

		private void addSource(double[] x, double[] x0)
		{
			for (int i = 0; i < size; i++)
			{
				x[i] += dt * x0[i];
			}
		}

		/**
		 * Calculate the input array after advection. We start with an
		 * input array from the previous timestep and an output array.
		 * For all grid cells we need to calculate for the next timestep,
		 * we trace the cell's center position backwards through the
		 * velocity field. Then we interpolate from the grid of the previous
		 * timestep and assign this value to the current grid cell.
		 *
		 * @param b Flag specifying how to handle boundries.
		 * @param d Array to store the advected field.
		 * @param d0 The array to advect.
		 * @param du The x component of the velocity field.
		 * @param dv The y component of the velocity field.
		 **/

		private void Advect(int b, double[] d, double[] d0, double[] du, double[] dv)
		{
			int i0, j0, i1, j1;
			double x, y, s0, t0, s1, t1, dt0;

			dt0 = dt * n;

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					// go backwards through velocity field
					x = i - dt0 * du[I(i, j)];
					y = j - dt0 * dv[I(i, j)];

					// interpolate results
					if (x > n + 0.5) x = n + 0.5;
					if (x < 0.5)
						x = 0.5;

					i0 = (int)x;
					i1 = i0 + 1;

					if (y > n + 0.5) y = n + 0.5;
					if (y < 0.5)
						y = 0.5;

					j0 = (int)y;
					j1 = j0 + 1;

					s1 = x - i0;
					s0 = 1 - s1;
					t1 = y - j0;
					t0 = 1 - t1;

					d[I(i, j)] = s0 * (t0 * d0[I(i0, j0)] + t1 * d0[I(i0, j1)])
							   + s1 * (t0 * d0[I(i1, j0)] + t1 * d0[I(i1, j1)]);

				}
			}

			SetBoundry(b, d);
			if (b == 1)
			{
				SetObstacles(d, ArrayType.X);
			}
			else if (b == 2)
			{
				SetObstacles(d, ArrayType.Y);
			}
		}

		/**
		 * Recalculate the input array with diffusion effects.
		 * Here we consider a stable method of diffusion by
		 * finding the densities, which when diffused backward
		 * in time yield the same densities we started with.
		 * This is achieved through use of a linear solver to
		 * solve the sparse matrix built from this linear system.
		 *
		 * @param b Flag to specify how boundries should be handled.
		 * @param c The array to store the results of the diffusion
		 * computation.
		 * @param c0 The input array on which we should compute
		 * diffusion.
		 * @param diff The factor of diffusion.
		 **/

		private void Diffuse(int b, double[] c, double[] c0, double diff)
		{
			double a = dt * diff * n * n;
			LinearSolver(b, c, c0, a, 1 + 4 * a);
		}


		/**
		 * Use project() to make the velocity a mass conserving,
		 * incompressible field. Achieved through a Hodge
		 * decomposition. First we calculate the divergence field
		 * of our velocity using the mean finite difference approach,
		 * and apply the linear solver to compute the Poisson
		 * equation and obtain a "height" field. Now we subtract
		 * the gradient of this field to obtain our mass conserving
		 * velocity field.
		 *
		 * @param x The array in which the x component of our final
		 * velocity field is stored.
		 * @param y The array in which the y component of our final
		 * velocity field is stored.
		 * @param p A temporary array we can use in the computation.
		 * @param div Another temporary array we use to hold the
		 * velocity divergence field.
		 *
		 **/

		void Project(double[] x, double[] y, double[] p, double[] div)
		{
			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					div[I(i, j)] = (x[I(i + 1, j)] - x[I(i - 1, j)]
								  + y[I(i, j + 1)] - y[I(i, j - 1)])
								  * -0.5 / n;
					p[I(i, j)] = 0;
				}
			}

			SetBoundry(0, div);
			SetBoundry(0, p);

			LinearSolver(0, p, div, 1, 4);

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= n; j++)
				{
					x[I(i, j)] -= 0.5 * n * (p[I(i + 1, j)] - p[I(i - 1, j)]);
					y[I(i, j)] -= 0.5 * n * (p[I(i, j + 1)] - p[I(i, j - 1)]);
				}
			}

			SetBoundry(1, x);
			SetObstacles(x, ArrayType.X);
			SetBoundry(2, y);
			SetObstacles(y, ArrayType.Y);
		}


		/**
		 * Iterative linear system solver using the Gauss-sidel
		 * relaxation technique. Room for much improvement here...
		 *
		 **/

		void LinearSolver(int b, double[] x, double[] x0, double a, double c)
		{


#if p
			Parallel.For(0, 20, k =>
			{
				for (int i = 1; i <= n; i++)
				{
					for (int j = 1; j <= n; j++)
					{
						x[I(i, j)] = (a * (x[I(i - 1, j)] + x[I(i + 1, j)]
										+ x[I(i, j - 1)] + x[I(i, j + 1)])
										+ x0[I(i, j)]) / c;
					}
				}
				SetBoundry(b, x);

				if (x == u)
					SetObstacles(x, ArrayType.X);
				else if (x == v)
					SetObstacles(x, ArrayType.Y);
			});
#else
			for (int k = 0; k < 20; k++)
			{
				for (int i = 1; i <= n; i++)
				{
					for (int j = 1; j <= n; j++)
					{
						x[I(i, j)] = (a * (x[I(i - 1, j)] + x[I(i + 1, j)]
										+ x[I(i, j - 1)] + x[I(i, j + 1)])
										+ x0[I(i, j)]) / c;
					}
				}
				SetBoundry(b, x);
			}
#endif
		}

		private bool[] occupiedCells;
		public bool[] OccupiedCells
		{
			get { return occupiedCells; }
		}

		// specifies simple boundry conditions.
		private void SetBoundry(int b, double[] x)
		{
			for (int i = 1; i <= n; i++)
			{
				x[I(0, i)] =
					b == 1 ? -x[I(1, i)] : x[I(1, i)];
				x[I(n + 1, i)] =
					b == 1 ? -x[I(n, i)] : x[I(n, i)];
				x[I(i, 0)] =
					b == 2 ? -x[I(i, 1)] : x[I(i, 1)];
				x[I(i, n + 1)] =
					b == 2 ? -x[I(i, n)] : x[I(i, n)];
			}

			x[I(0, 0)] = 0.5 * (x[I(1, 0)] + x[I(0, 1)]);
			x[I(0, n + 1)] = 0.5 * (x[I(1, n + 1)] + x[I(0, n)]);
			x[I(n + 1, 0)] = 0.5 * (x[I(n, 0)] + x[I(n + 1, 1)]);
			x[I(n + 1, n + 1)] = 0.5 * (x[I(n, n + 1)] + x[I(n + 1, n)]);
		}

		private enum ArrayType
		{
			X,
			Y,
			None
		}

		private void SetObstacles(double[] arr, ArrayType type)
		{
			var o = occupiedCells;

			if (type == ArrayType.X)
			{
#if po
				Parallel.For(1, n + 1, ix =>
#else
				for (int ix = 1; ix <= n; ix++)
#endif
				{
					for (int iy = 1; iy <= n; iy++)
					{
						Vector vector = GetNormal(ix, iy);
						int index = I(ix, iy);
						if (vector.Length > 0)
							arr[index] *= vector.X * (1 - friction);
						else if (o[index])
							arr[index] = 0;
					}
				}
#if po
);
#endif
			}
			else if (type == ArrayType.Y)
			{
#if po
				Parallel.For(1, n + 1, ix =>
#else
				for (int ix = 1; ix <= n; ix++)
#endif
				{
					for (int iy = 1; iy <= n; iy++)
					{
						Vector vector = GetNormal(ix, iy);
						int index = I(ix, iy);
						if (vector.Length > 0)
							arr[index] *= vector.Y * (1 - friction);
						else if (o[index])
							arr[index] = 0;
					}
				}
#if po
);
#endif
			}
		}

		private Vector GetNormal(int x, int y)
		{
			int front = I(x, y + 1);
			int back = I(x, y - 1);
			int left = I(x - 1, y);
			int right = I(x + 1, y);
			var o = occupiedCells;

			Vector result = new Vector();

			if (o[front])
				result += new Vector(0, 1);
			if (o[left])
				result += new Vector(1, 0);
			if (o[right])
				result += new Vector(-1, 0);
			if (o[back])
				result += new Vector(0, -1);

			if (result.Length > 0)
				result.Normalize();

			return result;
		}

		// util array swapping methods
		public void swapU() { tmp = u; u = uOld; uOld = tmp; }
		public void swapV() { tmp = v; v = vOld; vOld = tmp; }
		public void swapD() { tmp = density; density = densityOld; densityOld = tmp; }

		// util method for indexing 1d arrays
		private int I(int i, int j) { return i + (n + 2) * j; }
	}
}
