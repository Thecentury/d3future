using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Legend_items;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Filters;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Data;
using System.Windows.Threading;


namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a series of points connected by one polyline.
	/// </summary>
	public class NewLineGraph : Canvas, IPlotterElement
	{
		static NewLineGraph()
		{
			Type thisType = typeof(NewLineGraph);

			NewLegend.DescriptionProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata("LineGraph"));
			NewLegend.LegendItemsBuilderProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(new LegendItemsBuilder(DefaultLegendItemsBuilder)));
		}

		private static IEnumerable<FrameworkElement> DefaultLegendItemsBuilder(IPlotterElement plotterElement)
		{
			NewLineGraph lineGraph = (NewLineGraph)plotterElement;

			Line line = new Line { X1 = 0, Y1 = 10, X2 = 20, Y2 = 0, Stretch = Stretch.Fill, DataContext = lineGraph };
			line.SetBinding(Line.StrokeProperty, "Stroke");
			line.SetBinding(Line.StrokeThicknessProperty, "StrokeThickness");
			NewLegend.SetVisualContent(lineGraph, line);

			var legendItem = LegendItemsHelper.BuildDefaultLegendItem(lineGraph);
			yield return legendItem;
		}

		private readonly FilterCollection filters = new FilterCollection();

		/// <summary>
		/// Initializes a new instance of the <see cref="LineGraph"/> class.
		/// </summary>
		public NewLineGraph()
		{
			Legend.SetVisibleInLegend(this, true);

			filters.CollectionChanged += filters_CollectionChanged;

			RenderTransform = layoutTransform;
			Background = Brushes.Green.MakeTransparent(0.3);
		}

		private void filters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// todo
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LineGraph"/> class.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		public NewLineGraph(IPointDataSource pointSource)
			: this()
		{
			DataSource = pointSource;
		}

		/// <summary>Provides access to filters collection</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FilterCollection Filters
		{
			get { return filters; }
		}

		#region Properties

		#region DataSource property

		public IPointDataSource DataSource
		{
			get { return (IPointDataSource)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IPointDataSource),
		  typeof(NewLineGraph),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			NewLineGraph owner = (NewLineGraph)d;
			owner.OnDataSourceReplaced((IPointDataSource)e.OldValue, (IPointDataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(IPointDataSource prevDataSource, IPointDataSource currDataSource)
		{
			if (prevDataSource != null)
			{
				prevDataSource.DataChanged -= OnDataChanged;
			}

			if (currDataSource != null)
			{
				currDataSource.DataChanged += OnDataChanged;
			}
			Update();
		}

		void OnDataChanged(object sender, EventArgs e)
		{
			Update();
		}

		#endregion // end of DataSource property

		#region Stroke property

		public Brush Stroke
		{
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
		  "Stroke",
		  typeof(Brush),
		  typeof(NewLineGraph),
		  new FrameworkPropertyMetadata(Brushes.Blue));

		#endregion // end of Stroke property

		#region StrokeThickness property

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
		  "StrokeThickness",
		  typeof(double),
		  typeof(NewLineGraph),
		  new FrameworkPropertyMetadata(1.0));

		#endregion // end of StrokeThickness property

		#endregion // end of Properties

		private bool smoothLinesJoin = true;
		public bool SmoothLinesJoin
		{
			get { return smoothLinesJoin; }
			set
			{
				smoothLinesJoin = value;
				Update();
			}
		}

		private List<Point> FilterPoints(List<Point> points)
		{
			var filteredPoints = filters.Filter(points, plotter.Viewport.Output);

			return filteredPoints;
		}

		void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				DataRect prevVisible = (DataRect)e.OldValue;
				DataRect currVisible = (DataRect)e.NewValue;

				if (currVisible.Size != prevVisible.Size)
					Update();
				else
					UpdateTransform();

			}
			else
			{
				Update();
			}
		}

		TranslateTransform layoutTransform = new TranslateTransform();
		private void UpdateTransform()
		{
			var currentTransform = plotter.Transform;

			var shift = transformWhenCreated.ViewportRect.Location.ViewportToScreen(currentTransform) - currentTransform.ViewportRect.Location.ViewportToScreen(currentTransform);
			layoutTransform.X = shift.X;
			layoutTransform.Y = shift.Y;

			Debug.WriteLine("X=" + shift.X);
			Debug.WriteLine("Y=" + shift.Y);
		}

		CoordinateTransform transformWhenCreated;
		ResourcePool<Polyline> polylinePool = new ResourcePool<Polyline>();
		private const int pointCount = 500;
		private void Update()
		{
			if (Plotter == null) return;
			if (DataSource == null) return;

			layoutTransform.X = 0;
			layoutTransform.Y = 0;

			var dataSource = DataSource;
			var dataPoints = dataSource.GetPoints();

			transformWhenCreated = plotter.Transform;

			var contentBounds = dataPoints.GetBounds();
			Viewport2D.SetContentBounds(this, contentBounds);

			foreach (Polyline polyline in Children)
			{
				polylinePool.Put(polyline);
			}
			Children.Clear();

			PointCollection pointCollection = new PointCollection();
			foreach (var screenPoint in dataPoints.DataToScreen(plotter.Transform))
			{
				if (pointCollection.Count < pointCount)
				{
					pointCollection.Add(screenPoint);
				}
				else
				{
					var polyline = polylinePool.GetOrCreate();
					polyline.Points = pointCollection;

					SetPolylineBindings(polyline);

					Children.Add(polyline);
					Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
					pointCollection = new PointCollection();
				}
			}
		}

		private void SetPolylineBindings(Polyline polyline)
		{
			polyline.SetBinding(Polyline.StrokeProperty, new Binding { Source = this, Path = new PropertyPath("Stroke") });
			polyline.SetBinding(Polyline.StrokeThicknessProperty, new Binding { Source = this, Path = new PropertyPath("StrokeThickness") });
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			this.plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;

			plotter.CentralGrid.Children.Add(this);

			Update();
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(this);
			this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
