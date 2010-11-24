using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicDataDisplay.Markers.DataSources;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	public abstract class MarkerChartBase : DependencyObject, IPlotterElement
	{
		private Plotter2D plotter = null;

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
		  typeof(MarkerChartBase),
		  new FrameworkPropertyMetadata(null, OnItemsSourceReplaced));

		private static void OnItemsSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MarkerChartBase owner = (MarkerChartBase)d;
			owner.OnItemsSourceReplacedCore(e.OldValue, e.NewValue);
		}

		protected virtual void OnItemsSourceReplacedCore(object oldValue, object newValue)
		{
			object itemsSource = newValue;

			if (itemsSource != null)
			{
				var store = DataSourceFactoryStore.Current;
				var dataSource = store.BuildDataSource(itemsSource);

				if (dataSource != null) {
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
		  typeof(MarkerChartBase),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MarkerChartBase owner = (MarkerChartBase)d;
			owner.OnDataSourceReplaced((PointDataSourceBase)e.OldValue, (PointDataSourceBase)e.NewValue);
		}

		protected virtual void OnDataSourceReplaced(PointDataSourceBase oldDataSource, PointDataSourceBase newDataSource) { }

		#endregion

		#region IPlotterElement Members

		public virtual void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
		}

		public virtual void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
