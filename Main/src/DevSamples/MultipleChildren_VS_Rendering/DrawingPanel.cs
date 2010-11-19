using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace MultipleChildren_VS_Rendering
{
	public class DrawingPanel : FrameworkElement
	{
		private Point[] points;
		public Point[] Points
		{
			get { return points; }
			set { points = value; InvalidateVisual(); }
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var dc = drawingContext;

			Rect bounds = new Rect(RenderSize);

			dc.DrawRectangle(Brushes.Orange.MakeTransparent(0.3), null, bounds);

			if (points == null) return;

			for (int i = 0; i < points.Length; i++)
			{
				Point viewportPt = points[i];
				var screenPoint = new Point(viewportPt.X * bounds.Width, viewportPt.Y * bounds.Height);

				dc.DrawRectangle(Brushes.Blue, null, RectExtensions.FromCenterSize(screenPoint, new Size(2, 2)));
			}
		}
	}
}
