using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media;
using System.Diagnostics.Contracts;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// This is a class for displaying some debug visuals on the plotter;
	/// </summary>
	public class VisualDebug : IPlotterElement
	{
		private Plotter2D plotter;
		private static readonly ViewportHostPanel panel = new ViewportHostPanel();
		private static readonly VisualDebug current = new VisualDebug();
		private static readonly Dictionary<string, object> objects = new Dictionary<string, object>();

		public static void DrawRectangle(string name, DataRect bounds, Brush stroke = null, Brush fill = null, double strokeThickness = 1.0)
		{
			Contract.Assert(name != null);

			if (stroke == null)
				stroke = Brushes.Blue;

			Rectangle rect;
			if (objects.ContainsKey(name))
				rect = (Rectangle)objects[name];
			else
			{
				rect = new Rectangle();
				objects.Add(name, rect);
			}

			rect.Stroke = stroke;
			rect.StrokeThickness = strokeThickness;
			rect.Fill = fill;

			ViewportPanel.SetViewportBounds(rect, bounds);

			if (rect.Parent == null)
				panel.Children.Add(rect);

			EnsurePanelAdded();
		}

		private static void EnsurePanelAdded()
		{
			if (Plotter.Current == null)
				return;
			if (panel.Parent != null)
				return;

			Plotter.Current.Children.Add(panel);
		}

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.Dispatcher.BeginInvoke(() =>
			{
				plotter.Children.Add(panel);
			});
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.Dispatcher.BeginInvoke(() =>
			{
				plotter.Children.Remove(panel);
			});

			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
