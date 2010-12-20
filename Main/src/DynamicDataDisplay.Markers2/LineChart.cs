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
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	/// <summary>
	/// Represents a WPF path-based line chart.
	/// </summary>
	public class LineChart : LineChartBase
	{
		private readonly ViewportHostPanel panel = new ViewportHostPanel();
		private readonly List<Path> drawnPaths = new List<Path>();
		private readonly Panel canvas = new Canvas();
		private readonly ResourcePool<Path> pathsPool = new ResourcePool<Path>();

		private readonly Binding strokeBinding;
		private readonly Binding strokeThicknessBinding;
		private readonly Binding strokeDashArrayBinding;
		private readonly Binding zIndexBinding;
		private readonly Binding isHitTestVisibleBinding;
		private readonly Binding visibilityBinding;
		private readonly Binding tooltipBinding;

		private const int pathLength = 500;
		private readonly MissingValueSplitter missingValueSplitter = new MissingValueSplitter();
		private CoordinateTransform transformWhileCreateUI;

		private Range<int> indexRange = new Range<int>(IndexWrapper.Empty, IndexWrapper.Empty);

		/// <summary>
		/// Initializes a new instance of the <see cref="LineChart"/> class.
		/// </summary>
		public LineChart()
		{
			strokeBinding = new Binding(StrokeProperty.Name) { Source = this };
			strokeThicknessBinding = new Binding(StrokeThicknessProperty.Name) { Source = this };
			strokeDashArrayBinding = new Binding(StrokeDashArrayProperty.Name) { Source = this };
			zIndexBinding = new Binding("(Panel.ZIndex)") { Source = this };
			isHitTestVisibleBinding = new Binding(FrameworkElement.IsHitTestVisibleProperty.Name) { Source = this };
			visibilityBinding = new Binding(VisibilityProperty.Name) { Source = this };
			tooltipBinding = new Binding(ToolTipProperty.Name) { Source = this };
		}

		#region Overrides

		protected override void OnDataSourceReplaced(PointDataSourceBase oldDataSource, PointDataSourceBase newDataSource)
		{
			base.OnDataSourceReplaced(oldDataSource, newDataSource);

			DestroyUIRepresentation();
			CreateUIRepresentation();
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			// todo temp
			HandleCollectionReset();
			return;

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				HandleCollectionReset();
			}
			else if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems != null)
				{
					Range<int> addedRange = e.GetAddedRange();

					if (indexRange.IntersectsWith(addedRange))
					{
						HandleCollectionAdd(e);
					}
					else if (indexRange.Max == addedRange.Min - 1) // item was added into the end of collection
					{
						Path lastPath = drawnPaths.Last();
						int lastCount = LineChartBase.GetPointsCount(lastPath);
						Range<int> requestRange = new Range<int>(addedRange.Min - 1, addedRange.Max);

						// have path with non-filled geometry
						if (lastCount + addedRange.GetLength() <= pathLength)
						{
							canvas.Children.Remove(lastPath);
							drawnPaths.Remove(lastPath);
							pathsPool.Put(lastPath);
							Range<int> lastPathRange = PointChartBase.GetIndexRange(lastPath);
							int min = requestRange.Min;

							if (min % pathLength == 0)
								min -= 1;

							requestRange = new Range<int>(min, addedRange.Max);
						}

						var points = DataSource.GetPointData(requestRange);

						var indexedPoints = IndexWrapper.Generate(points, requestRange.Min);

						// do nothing if there is nothing to draw
						if (!points.Any())
							return;

						int minIndex;
						int maxIndex;
						CreateAndAddPath(indexedPoints, out minIndex, out maxIndex, transformWhileCreateUI);

						this.indexRange = new Range<int>(indexRange.Min, maxIndex);
					}
					else
					{
						// todo
						// do nothing?
					}
				}
				else
				{
					HandleCollectionReset();
				}
			}
		}

		private void HandleCollectionReset()
		{
			DestroyUIRepresentation();
			CreateUIRepresentation();
		}

		private void HandleCollectionAdd(NotifyCollectionChangedEventArgs e)
		{
			Range<int> addedRange = e.GetAddedRange();

			var paths = (from path in drawnPaths
						 let pathRange = PointChartBase.GetIndexRange(path)
						 where pathRange.IntersectsWith(addedRange)
						 let bounds = PointChartBase.GetContentBounds(path)
						 select new { path, bounds }).ToList();

			DataRect unitedContentBounds = paths.Aggregate(
				DataRect.Empty, (rect, other) => DataRect.Union(rect, other.bounds));

			var added = e.NewItems;

			// todo finish
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			DestroyUIRepresentation();
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

		#region Properties

		public bool UseSmoothJoin
		{
			get { return (bool)GetValue(UseSmoothJoinProperty); }
			set { SetValue(UseSmoothJoinProperty, value); }
		}

		public static readonly DependencyProperty UseSmoothJoinProperty = DependencyProperty.Register(
		  "UseSmoothJoin",
		  typeof(bool),
		  typeof(LineChart),
		  new FrameworkPropertyMetadata(false, OnUseSmoothJoinReplaced));

		private static void OnUseSmoothJoinReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LineChart owner = (LineChart)d;
			owner.DestroyUIRepresentation();
			owner.CreateUIRepresentation();
		}

		#endregion

		private void CreateUIRepresentation()
		{
			if (Plotter == null)
				return;

			if (DataSource == null)
				return;

			drawnPaths.Clear();

			DataSourceEnvironment environment = CreateEnvironment();
			var points = DataSource.GetPoints(environment);

			var indexedPoints = IndexWrapper.Generate(points);

			// do nothing if there is nothing to draw
			if (!points.Any())
				return;

			transformWhileCreateUI = Plotter.Viewport.Transform;

			int globalMinIndex;
			int globalMaxIndex;
			CreateAndAddPath(indexedPoints, out globalMinIndex, out globalMaxIndex, transformWhileCreateUI);

			indexRange = new Range<int>(globalMinIndex, globalMaxIndex);

			ViewportPanel.SetViewportBounds(canvas, Plotter.Viewport.Visible);

			// switching off content bounds calculation for children of ViewportHostPanel.
			panel.BeginBatchAdd();

			if (canvas.Parent == null)
				panel.Children.Add(canvas);

			if (panel.Plotter == null)
			{
				Plotter.Dispatcher.BeginInvoke(() =>
				{
					if (panel.Plotter == null && Plotter != null)
						Plotter.Children.Add(panel);
				});
			}

			DataRect bounds = DataRect.Empty;
			if (environment.ContentBounds != null)
			{
				bounds = environment.ContentBounds.Value;
			}
			else
			{
				// todo is this necessary?
				bounds = points.GetBounds();
			}

			Viewport2D.SetContentBounds(this, bounds);
		}

		private void CreateAndAddPath(IEnumerable<IndexWrapper<Point>> indexedPoints,
			out int globalMinIndex, out int globalMaxIndex, CoordinateTransform transform)
		{
			var screenPoints = indexedPoints.DataToScreen(transform);

			var parts = screenPoints.Split(pathLength);

			var splittedParts = from part in parts
								select missingValueSplitter.SplitMissingValue(part);

			bool isSmoothJoin = UseSmoothJoin;

			Point? lastPoint = null;

			globalMinIndex = Int32.MaxValue;
			globalMaxIndex = Int32.MinValue;

			foreach (var shortSegment in splittedParts)
			{
				foreach (var part in shortSegment)
				{
					if (part.MinIndex < globalMinIndex)
						globalMinIndex = part.MinIndex;
					if (part.MaxIndex > globalMaxIndex)
						globalMaxIndex = part.MaxIndex;

					List<Point> list = part.GetPoints().ToList();
					if (list.Count == 0)
						continue;

					if (part.Splitted)
						lastPoint = null;

					StreamGeometry geometry = new StreamGeometry();
					using (var context = geometry.Open())
					{
						var start = lastPoint ?? list[0];

						context.BeginFigure(start, isFilled: false, isClosed: false);
						context.PolyLineTo(list, isStroked: true, isSmoothJoin: isSmoothJoin);
					}

					lastPoint = list.Last();

					Path path = pathsPool.GetOrCreate();
					drawnPaths.Add(path);

					PointChartBase.SetIndexRange(path, new Range<int>(part.MinIndex, part.MaxIndex));
					LineChartBase.SetPointsCount(path, list.Count);

					DataRect localBounds = list.GetBounds();
					PointChartBase.SetContentBounds(path, localBounds);

					//if (path.CacheMode == null)
					//    path.CacheMode = new BitmapCache();

					// todo for debug purpose
					//path.Stroke = ColorHelper.RandomBrush;

					path.SetBinding(Path.StrokeProperty, strokeBinding);
					path.SetBinding(Path.StrokeThicknessProperty, strokeThicknessBinding);
					path.SetBinding(Path.StrokeDashArrayProperty, strokeDashArrayBinding);
					path.SetBinding(Panel.ZIndexProperty, zIndexBinding);
					path.SetBinding(Path.IsHitTestVisibleProperty, isHitTestVisibleBinding);
					path.SetBinding(Path.VisibilityProperty, visibilityBinding);
					path.SetBinding(Path.ToolTipProperty, tooltipBinding);

					path.Data = geometry;

					canvas.Children.Add(path);
				}
			}
		}

		private void UpdateUIRepresentation()
		{
			Viewport2D viewport = Plotter.Viewport;

			DataSourceEnvironment environment = EnvironmentPlugin.CreateEnvironment(viewport);

			bool visibleChangedSignificantly = !DataRect.EqualsEpsSizes(
				environment.Visible, VisibleWhileCreation, rectanglesEps);
			bool outputChangedSignificantly = !DataRect.EqualsEpsSizes(
				new DataRect(environment.Output), new DataRect(OutputWhileCreation), rectanglesEps);
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
			pathsPool.ReleaseAll(drawnPaths);
			canvas.Children.Clear();

			if (Plotter != null && detaching)
				Plotter.CentralGrid.Children.Remove(panel);
		}
	}
}
