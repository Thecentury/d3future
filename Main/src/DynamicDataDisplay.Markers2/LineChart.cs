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
using System.Windows.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	/// <summary>
	/// Represents a WPF path-based line chart.
	/// </summary>
	public class LineChart : LineChartBase
	{
		private readonly ViewportHostPanel panel = new ViewportHostPanel();
		private readonly Panel canvas = new Canvas();
		private readonly ResourcePool<Path> pathsPool = new ResourcePool<Path>();
		private readonly Binding strokeBinding;
		private readonly Binding strokeThicknessBinding;
		private readonly Binding strokeDashArrayBinding;
		private readonly Binding zIndexBinding;
		private const int pathLength = 500;

		/// <summary>
		/// Initializes a new instance of the <see cref="LineChart"/> class.
		/// </summary>
		public LineChart()
		{
			strokeBinding = new Binding(StrokeProperty.Name) { Source = this };
			strokeThicknessBinding = new Binding(StrokeThicknessProperty.Name) { Source = this };
			strokeDashArrayBinding = new Binding(StrokeDashArrayProperty.Name) { Source = this };
			zIndexBinding = new Binding("(Panel.ZIndex)") { Source = this };
		}

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
			DestroyUIRepresentation(detaching: true);

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

			var screenPoints = points.DataToScreen(Plotter.Viewport.Transform);

			var parts = screenPoints.Split(pathLength);

			Point? lastPoint = null;
			foreach (var part in parts)
			{
				List<Point> list = part.ToList();
				if (list.Count == 0)
					continue;

				StreamGeometry geometry = new StreamGeometry();
				using (var context = geometry.Open())
				{
					var start = lastPoint ?? list[0];

					context.BeginFigure(start, isFilled: false, isClosed: false);
					context.PolyLineTo(list, isStroked: true, isSmoothJoin: false);
				}

				lastPoint = list.Last();

				Path path = pathsPool.GetOrCreate();

				if (path.CacheMode == null)
					path.CacheMode = new BitmapCache();

				path.SetBinding(Path.StrokeProperty, strokeBinding);
				path.SetBinding(Path.StrokeThicknessProperty, strokeThicknessBinding);
				path.SetBinding(Path.StrokeDashArrayProperty, strokeDashArrayBinding);
				path.SetBinding(Panel.ZIndexProperty, zIndexBinding);

				path.Data = geometry;

				canvas.Children.Add(path);
			}

			ViewportPanel.SetViewportBounds(canvas, Plotter.Viewport.Visible);

			if (canvas.Parent == null)
				panel.Children.Add(canvas);

			// switching off content bounds calculation for children of ViewportHostPanel.
			panel.BeginBatchAdd();

			Plotter.Dispatcher.BeginInvoke(() =>
			{
				if (panel.Plotter == null)
					Plotter.Children.Add(panel);
			});

			DataRect bounds = DataRect.Empty;
			if (environment.ContentBounds != null)
				bounds = environment.ContentBounds.Value;
			
			Viewport2D.SetContentBounds(this, bounds);
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

		private void DestroyUIRepresentation(bool detaching = false)
		{
			pathsPool.ReleaseAll(canvas.Children.Cast<Path>());
			canvas.Children.Clear();

			if (Plotter != null && detaching)
				Plotter.CentralGrid.Children.Remove(panel);
		}
	}
}
