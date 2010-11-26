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
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	public class LineChart : LineChartBase
	{
		private readonly ViewportHostPanel panel = new ViewportHostPanel();
		private readonly Canvas canvas = new Canvas();
		private readonly ResourcePool<Path> pathsPool = new ResourcePool<Path>();

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
			var points = DataSource.GetPoints(environment).ToList();

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
			path.Data = geometry;

			canvas.Children.Add(path);

			ViewportPanel.SetViewportBounds(canvas, Plotter.Viewport.Visible);

			if (canvas.Parent == null)
				panel.Children.Add(canvas);

			panel.BeginBatchAdd();

			Plotter.Dispatcher.BeginInvoke(() =>
			{
				if (panel.Plotter == null)
					Plotter.Children.Add(panel);
			});

			DataRect dataBounds = DataSource.GetContentBounds(points, Plotter.Viewport.Visible);

			Viewport2D.SetContentBounds(this, dataBounds);
		}

		private void UpdateUIRepresentation()
		{
			Viewport2D viewport = Plotter.Viewport;

			DataSourceEnvironment environment = EnvironmentPlugin.CreateEnvironment(viewport);

			bool visibleChangedSignificantly = !DataRect.EqualsEpsSizes(environment.Visible, VisibleWhileCreation, rectanglesEps);
			bool outputChangedSignificantly = !DataRect.EqualsEpsSizes(new DataRect(environment.Output), new DataRect(OutputWhileCreation), rectanglesEps);
			bool visibleIsOutOfVisibleWhileCreation = !VisibleWhileCreation.Contains(viewport.Visible);

			if (visibleChangedSignificantly || outputChangedSignificantly || visibleIsOutOfVisibleWhileCreation)
			{
				DestroyUIRepresentation();
				CreateUIRepresentation();
				return;
			}
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
