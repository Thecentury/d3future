using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;
using Petzold.Media3D;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class BoundsMesh : ModelVisual3D
	{
		private Binding colorBinding;

		public BoundsMesh()
		{
			colorBinding = new Binding("LineColor") { Source = this };
		}

		#region Properties

		#region Bounds property

		public Rect3D Bounds
		{
			get { return (Rect3D)GetValue(BoundsProperty); }
			set { SetValue(BoundsProperty, value); }
		}

		public static readonly DependencyProperty BoundsProperty = DependencyProperty.Register(
		  "Bounds",
		  typeof(Rect3D),
		  typeof(BoundsMesh),
		  new FrameworkPropertyMetadata(Rect3D.Empty, OnBoundsReplaced));

		private static void OnBoundsReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BoundsMesh owner = (BoundsMesh)d;
			owner.OnBoundsChanged((Rect3D)e.NewValue);
		}

		private void OnBoundsChanged(Rect3D bounds)
		{
			Children.Clear();

			double x0 = bounds.X;
			double x1 = x0 + bounds.SizeX;
			double y0 = bounds.Y;
			double y1 = y0 + bounds.SizeY;
			double z0 = bounds.Z;
			double z1 = z0 + bounds.SizeZ;

			// bottom lines
			AddLine(x0, y0, z0, x1, y0, z0);
			AddLine(x1, y0, z0, x1, y1, z0);
			AddLine(x1, y1, z0, x0, y1, z0);
			AddLine(x0, y1, z0, x0, y0, z0);

			// top lines
			AddLine(x0, y0, z1, x1, y0, z1);
			AddLine(x1, y0, z1, x1, y1, z1);
			AddLine(x1, y1, z1, x0, y1, z1);
			AddLine(x0, y1, z1, x0, y0, z1);

			// vertical lines
			AddLine(x0, y0, z0, x0, y0, z1);
			AddLine(x1, y0, z0, x1, y0, z1);
			AddLine(x0, y1, z0, x0, y1, z1);
			AddLine(x1, y1, z0, x1, y1, z1);
		}

		private void AddLine(double x0, double y0, double z0, double x1, double y1, double z1)
		{
			WireLine line = new WireLine { Point1 = new Point3D(x0, y0, z0), Point2 = new Point3D(x1, y1, z1) };
			BindingOperations.SetBinding(line, WireLine.ColorProperty, colorBinding);
			Children.Add(line);
		}

		#endregion Bounds property

		#region LineColor property

		public Color LineColor
		{
			get { return (Color)GetValue(LineColorProperty); }
			set { SetValue(LineColorProperty, value); }
		}

		public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(
		  "LineColor",
		  typeof(Color),
		  typeof(BoundsMesh),
		  new FrameworkPropertyMetadata(Colors.Black));


		#endregion LineColor property

		#endregion Properties
	}
}
