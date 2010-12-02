using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicDataDisplay.Markers.DataSources;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;
using System.Diagnostics.Contracts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Collections.Specialized;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	/// <summary>
	/// Represents a base class for creating marker or line charts.
	/// </summary>
	public abstract class PointChartBase : FrameworkElement, IPlotterElement
	{
		private Plotter2D plotter = null;
		private EnvironmentPlugin environmentPlugin = new DefaultLineChartEnvironmentPlugin();
		private DataRect visibleWhileCreation;
		private Rect outputWhileCreation;
		protected const double rectanglesEps = 0.0005;

		/// <summary>
		/// Initializes a new instance of the <see cref="PointChartBase"/> class.
		/// </summary>
		public PointChartBase()
		{
			Viewport2D.SetIsContentBoundsHost(this, true);
		}

		/// <summary>
		/// Gets the visible rectangle while creation of points to draw.
		/// </summary>
		/// <value>The visible while creation.</value>
		protected DataRect VisibleWhileCreation
		{
			get { return visibleWhileCreation; }
		}

		/// <summary>
		/// Gets the output rectangle while creation of points to draw.
		/// </summary>
		/// <value>The output while creation.</value>
		protected Rect OutputWhileCreation
		{
			get { return outputWhileCreation; }
		}

		#region Helpers

		/// <summary>
		/// Gets or sets the environment plugin.
		/// </summary>
		/// <value>The environment plugin.</value>
		[NotNull]
		public EnvironmentPlugin EnvironmentPlugin
		{
			get { return environmentPlugin; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				environmentPlugin = value;
			}
		}

		/// <summary>
		/// Creates the environment.
		/// </summary>
		/// <returns></returns>
		protected DataSourceEnvironment CreateEnvironment()
		{
			if (plotter == null)
				throw new InvalidOperationException();

			Viewport2D viewport = plotter.Viewport;
			DataSourceEnvironment result = environmentPlugin.CreateEnvironment(viewport);

			visibleWhileCreation = result.Visible;
			outputWhileCreation = result.Output;

			return result;
		}

		#endregion

		#region ItemsSource

		/// <summary>
		/// Gets or sets the items source. This is a DependencyProperty.
		/// </summary>
		/// <value>The items source.</value>
		public object ItemsSource
		{
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
		  "ItemsSource",
		  typeof(object),
		  typeof(PointChartBase),
		  new FrameworkPropertyMetadata(null, OnItemsSourceReplaced));

		private static void OnItemsSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PointChartBase owner = (PointChartBase)d;
			owner.OnItemsSourceReplacedCore(e.OldValue, e.NewValue);
		}

		protected virtual void OnItemsSourceReplacedCore(object oldValue, object newValue)
		{
			object itemsSource = newValue;

			if (itemsSource != null)
			{
				var store = DataSourceFactoryStore.Current;
				var dataSource = store.BuildDataSource(itemsSource);

				if (dataSource != null)
				{
					DataSource = dataSource;
				}
				else
				{
					throw new ArgumentException("Cannot create a DataSource of given ItemsSource. Look into a list of DataSource types to determine what data can be passed.");
				}
			}
			else
			{
				DataSource = null;
			}
		}

		#endregion

		#region DataSource

		/// <summary>
		/// Gets or sets the data source. This is a DependencyProperty.
		/// </summary>
		/// <value>The data source.</value>
		public PointDataSourceBase DataSource
		{
			get { return (PointDataSourceBase)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(PointDataSourceBase),
		  typeof(PointChartBase),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PointChartBase owner = (PointChartBase)d;
			owner.OnDataSourceReplaced((PointDataSourceBase)e.OldValue, (PointDataSourceBase)e.NewValue);
		}

		protected virtual void OnDataSourceReplaced(PointDataSourceBase oldDataSource, PointDataSourceBase newDataSource)
		{
			if (oldDataSource != null)
				oldDataSource.CollectionChanged -= OnDataSource_CollectionChanged;

			if (newDataSource != null)
				newDataSource.CollectionChanged += OnDataSource_CollectionChanged;
		}

		private void OnDataSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(e);
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) { }

		#endregion

		#region IPlotterElement Members

		public virtual void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			this.plotter.Viewport.PropertyChanged += new EventHandler<ExtendedPropertyChangedEventArgs>(Viewport_PropertyChanged);
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			OnViewportPropertyChanged(e);
		}

		protected virtual void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e) { }

		public virtual void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		protected Plotter2D Plotter
		{
			get { return plotter; }
		}

		#endregion

		#region Description property

		/// <summary>
		/// Gets or sets the description of this chart in the legend.
		/// </summary>
		/// <value>The description.</value>
		public string Description
		{
			get { return (string)GetValue(Legend.DescriptionProperty); }
			set { SetValue(Legend.DescriptionProperty, value); }
		}

		#endregion

		#region DetailedDescription property

		/// <summary>
		/// Gets or sets the detailed description of this chart in the legend.
		/// </summary>
		/// <value>The detailed description.</value>
		public string DetailedDescription
		{
			get { return (string)GetValue(Legend.DetailedDescriptionProperty); }
			set { SetValue(Legend.DetailedDescriptionProperty, value); }
		}

		#endregion

		#region IndexRange property

		public static Range<int> GetIndexRange(DependencyObject obj)
		{
			return (Range<int>)obj.GetValue(IndexRangeProperty);
		}

		public static void SetIndexRange(DependencyObject obj, Range<int> value)
		{
			obj.SetValue(IndexRangeProperty, value);
		}

		public static readonly DependencyProperty IndexRangeProperty = DependencyProperty.RegisterAttached(
		  "IndexRange",
		  typeof(Range<int>),
		  typeof(PointChartBase),
		  new FrameworkPropertyMetadata(new Range<int>(IndexWrapper.Empty, IndexWrapper.Empty)));

		#endregion

		#region ContentBounds property

		public static DataRect GetContentBounds(DependencyObject obj)
		{
			return (DataRect)obj.GetValue(ContentBoundsProperty);
		}

		public static void SetContentBounds(DependencyObject obj, DataRect value)
		{
			obj.SetValue(ContentBoundsProperty, value);
		}

		public static readonly DependencyProperty ContentBoundsProperty = DependencyProperty.RegisterAttached(
		  "ContentBounds",
		  typeof(DataRect),
		  typeof(PointChartBase),
		  new FrameworkPropertyMetadata(DataRect.Empty));

		#endregion
	}
}
