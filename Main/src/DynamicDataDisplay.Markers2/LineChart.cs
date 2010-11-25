using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay.Markers.Extensions;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	public class LineChart : LineChartBase
	{
		private readonly List<Path> shapes = new List<Path>();
		private readonly ViewportHostPanel panel = new ViewportHostPanel();
		private readonly Canvas canvas = new Canvas();

		#region Overrides

		protected override void OnDataSourceReplaced(PointDataSourceBase oldDataSource, PointDataSourceBase newDataSource)
		{
			base.OnDataSourceReplaced(oldDataSource, newDataSource);

			CreateUIRepresentation();
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			CreateUIRepresentation();
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			DestroyUIRepresentation();

			base.OnPlotterDetaching(plotter);
		}

		protected override void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			base.OnViewportPropertyChanged(e);

			UpdateUIRepresentation();
		}

		#endregion

		private void CreateUIRepresentation()
		{
			if (Plotter == null)
				return;

			if (DataSource == null)
				return;

			DataSourceEnvironment environment = CreateEnvironment();
			var points = DataSource.GetPoints(environment);

			// do nothing if there is nothing to draw
			if (!points.Any())
				return;

			var screenPoints = points.DataToScreen(Plotter.Viewport.Transform).ToList();

			StreamGeometry geometry = new StreamGeometry();
			using (var context = geometry.Open())
			{
				context.BeginFigure(screenPoints[0], isFilled: false, isClosed: false);
				context.PolyLineTo(screenPoints, isStroked: true, isSmoothJoin: false);
			}

			Path path = new Path { Stroke = ColorHelper.RandomBrush, StrokeThickness = 3 };
			shapes.Add(path);
			path.Data = geometry;

			canvas.Children.Add(path);

			canvas.Background = Brushes.Aqua;
			panel.Background = Brushes.CornflowerBlue.MakeTransparent(0.3);

			Rectangle r = new Rectangle { Stroke = Brushes.Chartreuse, StrokeThickness = 2 };
			ViewportPanel.SetViewportBounds(r, new DataRect(0.3, 0.3, 0.6, 0.6));
			panel.Children.Add(r);

			ViewportPanel.SetViewportBounds(canvas, environment.Visible);

			if (canvas.Parent == null)
				panel.Children.Add(canvas);

			Plotter.Dispatcher.BeginInvoke(() =>
			{
				if (panel.Plotter == null)
					Plotter.Children.Add(panel);
			});
		}

		private void UpdateUIRepresentation()
		{
			Viewport2D viewport = Plotter.Viewport;

			bool visibleChangedSignificantly = !DataRect.EqualsEpsSizes(viewport.Visible, VisibleWhileCreation, rectanglesEps);
			bool outputChangedSignificantly = !DataRect.EqualsEpsSizes(new DataRect(viewport.Output), new DataRect(OutputWhileCreation), rectanglesEps);

			if (visibleChangedSignificantly || outputChangedSignificantly)
			{
				DestroyUIRepresentation();
				CreateUIRepresentation();
				return;
			}

			Vector shift = viewport.Output.Location - OutputWhileCreation.Location;
			TranslateTransform translate = new TranslateTransform(shift.X, shift.Y);

			TransformGroup group = new TransformGroup();
			group.Children.Add(translate);
		}

		private void DestroyUIRepresentation()
		{
			panel.Children.Clear();
			canvas.Children.Clear();

			if (Plotter != null)
				Plotter.CentralGrid.Children.Remove(panel);
		}
	}
}
