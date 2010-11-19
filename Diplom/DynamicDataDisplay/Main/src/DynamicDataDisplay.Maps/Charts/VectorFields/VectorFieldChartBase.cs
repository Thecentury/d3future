using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Charts;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<System.Windows.Vector>;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class VectorFieldChartBase : FrameworkElement, IPlotterElement
	{
		protected VectorFieldChartBase()
		{
			// panel is within visual tree, and the very chart is not
			SetBinding(DataContextProperty, new Binding { Path = new PropertyPath("DataContext"), Source = panel });
		}

		protected readonly ViewportHostPanel panel = new ViewportHostPanel();

		#region DataSource property

		public DataSource DataSource
		{
			get { return (DataSource)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(DataSource),
		  typeof(VectorFieldChartBase),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var owner = (VectorFieldChartBase)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(DataSource prevDataSource, DataSource currDataSource)
		{
			if (prevDataSource != null)
				prevDataSource.Changed -= DataSource_OnChanged;
			if (currDataSource != null)
				currDataSource.Changed += DataSource_OnChanged;

			RebuildUI();
		}

		protected virtual void DataSource_OnChanged(object sender, EventArgs e)
		{
			RebuildUI();
		}

		protected abstract void RebuildUI();

		#endregion

		#region IPlotterElement Members

		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
			protected set { plotter = value; }
		}

		public virtual void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.Children.BeginAdd(panel);

			SetBinding(DataContextProperty, new Binding("DataContext") { Source = panel });

			RebuildUI();
		}

		public virtual void OnPlotterDetaching(Plotter plotter)
		{
			plotter.Children.BeginRemove(panel);
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion // IPlotterElement Members
	}
}
