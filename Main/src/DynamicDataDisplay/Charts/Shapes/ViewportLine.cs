using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Represents a line in viewport space, coming through given points.
	/// </summary>
	public class ViewportLine : ViewportShape
	{
		private LineGeometry lineGeometry = new LineGeometry();

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewportLine"/> class.
		/// </summary>
		public ViewportLine() { }

		#region Properties

		#region Point1 property

		/// <summary>
		/// Gets or sets the first point. It is a DependencyProperty.
		/// </summary>
		/// <value>The point1.</value>
		public Point Point1
		{
			get { return (Point)GetValue(Point1Property); }
			set { SetValue(Point1Property, value); }
		}

		public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
		  "Point1",
		  typeof(Point),
		  typeof(ViewportLine),
		  new FrameworkPropertyMetadata(new Point(0, 0), OnPointReplaced));

		private static void OnPointReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ViewportLine owner = (ViewportLine)d;
			owner.UpdateUIRepresentation();
		}

		#endregion

		#region Point2 property

		/// <summary>
		/// Gets or sets the second point. It is a DependencyProperty.
		/// </summary>
		/// <value>The point2.</value>
		public Point Point2
		{
			get { return (Point)GetValue(Point2Property); }
			set { SetValue(Point2Property, value); }
		}

		public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
		  "Point2",
		  typeof(Point),
		  typeof(ViewportLine),
		  new FrameworkPropertyMetadata(new Point(1, 1), OnPointReplaced));

		#endregion

		#endregion

		protected override void UpdateUIRepresentationCore()
		{
			base.UpdateUIRepresentationCore();

			Viewport2D viewport = Plotter.Viewport;
			double deltaX = Point1.X - Point2.X;
			double deltaY = Point1.Y - Point2.Y;
			double m = deltaY / deltaX;
			double b = Point1.Y - Point1.X * deltaY / deltaX;

			Func<double, double> func = x => m * x + b;

			double xMin = viewport.Visible.XMin;
			double xMax = viewport.Visible.XMax;
			Point p1 = new Point(xMin, func(xMin)).DataToScreen(viewport.Transform);
			Point p2 = new Point(xMax, func(xMax)).DataToScreen(viewport.Transform);

			lineGeometry.StartPoint = p1;
			lineGeometry.EndPoint = p2;
		}

		protected override Geometry DefiningGeometry
		{
			get { return lineGeometry; }
		}
	}
}
