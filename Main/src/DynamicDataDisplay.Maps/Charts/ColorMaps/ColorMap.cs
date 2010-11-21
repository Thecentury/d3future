using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public class ColorMap : Map, IPlotterElement
	{
		private Plotter2D plotter;

		#region Properties

		public IDataSource2D<double> DataSource
		{
			get { return (IDataSource2D<double>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource2D<double>),
		  typeof(ColorMap),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ColorMap owner = (ColorMap)d;

		}

		#endregion

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.CentralGrid.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(this);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
