using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class FluidVectorChart2D : FluidImage, IPlotterElement
	{
		private IDataSource2D<Vector> dataSource;
		private UniformField2DWrapper wrapper;
		private double[] uOld, vOld, densityOld;

		public FluidVectorChart2D()
		{
			// убираем силу, которая поднимает пар вверх
			Solver.buoyancyCoeff = 0;
		}

		#region Properties

		#region DataSource property

		public IDataSource2D<Vector> DataSource
		{
			get { return (IDataSource2D<Vector>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource2D<Vector>),
		  typeof(FluidVectorChart2D),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FluidVectorChart2D owner = (FluidVectorChart2D)d;
			owner.dataSource = (IDataSource2D<Vector>)e.NewValue;
			owner.wrapper = new UniformField2DWrapper(owner.dataSource.Data);
			owner.UpdateField();
		}

		#endregion

		#region Pattern property

		private PointSetPattern pattern = new BorderPattern();
		public PointSetPattern Pattern
		{
			get { return pattern; }
			set { pattern = value; }
		}

		#endregion Pattern property

		private double velocityFactor = 500;
		public double VelocityFactor
		{
			get { return velocityFactor; }
			set { velocityFactor = value; }
		}

		private int skipFrames = 4;
		public int SkipFrames
		{
			get { return skipFrames; }
			set { skipFrames = value; }
		}

		#endregion Properties

		private void UpdateField()
		{
			var dataSource = DataSource;
			var bounds = dataSource.GetGridBounds();

			Viewport2D.SetContentBounds(this, bounds);
			ViewportPanel.SetViewportBounds(this, bounds);

			int length = (n + 2) * (n + 2);
			uOld = new double[length];
			vOld = new double[length];
			densityOld = new double[length];

			for (int i = 0; i < length; i++)
			{
				int ix = i % n;
				int iy = i / n;

				Vector vector = wrapper.GetVector(ix / (double)n, iy / (double)n);
				if (vector.X.IsNaN() || vector.Y.IsNaN())
					vector = new Vector();
				vOld[i] = 50000 * vector.X;
				uOld[i] = 50000 * vector.Y;
			}
		}

		protected override void UpdateLocation(MouseEventArgs e)
		{
			Point[] intermediatePoints = new Point[64];
			var pointsCount = Mouse.GetIntermediatePoints(this, intermediatePoints);

			Array.Clear(vOld, 0, vOld.Length);
			Array.Clear(uOld, 0, vOld.Length);

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				for (int i = 0; i < pointsCount; i++)
				{
					var pt = ScreenToField(intermediatePoints[i]);
					pt = NormalizePoint(pt);
					UpdateDensity(pt);
				}

				vOld.CopyTo(Solver.vOld, 0);
				uOld.CopyTo(Solver.uOld, 0);
			}
		}

		int counter = 0;
		protected override void UpdateDynamicDensity()
		{
			if (densityOld == null) 
				return;

			counter++;
			if (counter % 4 != 0)
				return;

			Array.Clear(densityOld, 0, densityOld.Length);

			foreach (Point point in pattern.GeneratePoints())
			{
				int imageX = (int)(point.X * (n + 2));
				int imageY = (int)(point.Y * (n + 2));

				Solver.densityOld[I(imageX, imageY)] = 10;
			}

			int length = (n + 2) * (n + 2);

			for (int i = 0; i < length; i++)
			{
				int ix = i % n;
				int iy = i / n;

				Vector vector = wrapper.GetVector(ix / (double)n, iy / (double)n);
				if (vector.X.IsNaN() || vector.Y.IsNaN())
					vector = new Vector();
				Solver.vOld[i] = velocityFactor * vector.X;
				Solver.uOld[i] = velocityFactor * vector.Y;
			}
		}

		#region IPlotterElement Members

		private readonly ViewportHostPanel panel = new ViewportHostPanel();
		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.Children.BeginAdd(panel);
			panel.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			panel.Children.Remove(this);
			plotter.Children.BeginRemove(panel);
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion // IPlotterElement Members
	}
}
