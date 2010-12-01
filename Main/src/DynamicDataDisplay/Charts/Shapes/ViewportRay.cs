using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Represents a ray in viewport space.
	/// </summary>
	public class ViewportRay : ViewportLine
	{
		private readonly LineGeometry geometry = new LineGeometry();
		private double direction = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewportRay"/> class.
		/// </summary>
		public ViewportRay() { }

		/// <summary>
		/// Gets or sets the direction of ViewportRay.
		/// Positive value means that line will be drawn up to positive infinity,
		/// and negative - up to negative infinity. Default value is negative.
		/// </summary>
		/// <value>The direction.</value>
		public double Direction
		{
			get { return direction; }
			set { direction = value; }
		}

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

			if (direction > 0)
				p1 = Point2.DataToScreen(viewport.Transform);
			else
				p2 = Point1.DataToScreen(viewport.Transform);

			geometry.StartPoint = p1;
			geometry.EndPoint = p2;
		}

		protected override Geometry DefiningGeometry
		{
			get { return geometry; }
		}
	}
}
