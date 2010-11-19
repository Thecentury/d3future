using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class InteractiveFluidDynamicsChart2D : FluidDynamicsChart2D
	{
		int xOld, yOld, x, y;

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
				return;

			xOld = x;
			yOld = y;

			Point pt = e.GetPosition(this);
			x = (int)(pt.X / ActualWidth * N);
			y = (int)((ActualHeight - pt.Y) / ActualHeight * N);

			UpdateLocation(e);
			e.Handled = true;
		}

		/// <summary>
		/// get index for fluid cell under mouse position.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private IntPoint ScreenToField(Point p)
		{
			IntPoint result = new IntPoint();
			result.X = (int)(p.X / ActualWidth * N);
			result.Y = (int)(p.Y / ActualHeight * N);
			return result;
		}

		private void UpdateDensity(IntPoint pt)
		{
			var i = pt.X;
			var j = pt.Y;

			dens_prev[i, j] = 10;
		}

		private void UpdateVelocity(IntPoint pt)
		{
			var i = pt.X;
			var j = pt.Y;

			u_prev[i, j] = (x - xOld) * 5;
			v_prev[i, j] = (y - yOld) * 5;
		}

		private IntPoint NormalizePoint(IntPoint pt)
		{
			if (pt.X > N) pt.X = N;
			else if (pt.X < 1) pt.X = 1;
			if (pt.Y > N) pt.Y = N;
			else if (pt.Y < 1) pt.Y = 1;

			return pt;
		}

		private void UpdateLocation(MouseEventArgs e)
		{
			Point[] intermediatePoints = new Point[64];
			var pointsCount = Mouse.GetIntermediatePoints(this, intermediatePoints);

			Debug.WriteLine("Intermediate points: " + pointsCount);

			for (int i = 0; i < pointsCount; i++)
			{
				var pt = ScreenToField(intermediatePoints[i]);
				pt = NormalizePoint(pt);
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					UpdateDensity(pt);
				}
				else if (e.RightButton == MouseButtonState.Pressed)
				{
					UpdateVelocity(pt);
				}
			}
		}

		protected override void GetFluidData(double[,] dens_prev, double[,] u_prev, double[,] v_prev)
		{

		}

		protected IntPoint PointToField(Point p)
		{
			int x = (int)(p.X / ActualWidth * N);
			int y = (int)((ActualHeight - p.Y) / ActualHeight * N);

			return new IntPoint { X = x, Y = y };
		}

		protected Vector VectorToField(Vector v)
		{
			v.X = v.X / ActualWidth * N;
			v.Y = -v.Y / ActualHeight * N;

			return v;
		}

		protected struct IntPoint
		{
			public int X;
			public int Y;
		}
	}
}
