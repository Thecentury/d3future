using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class VectorChartModel3DBase : ModelVisual3D
	{
		#region Properties

		#region DataSource

		public IDataSource3D<Vector3D> DataSource
		{
			get { return (IDataSource3D<Vector3D>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource3D<Vector3D>),
		  typeof(VectorChartModel3DBase),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VectorChartModel3DBase owner = (VectorChartModel3DBase)d;
			owner.OnDataSourceReplaced((IDataSource3D<Vector3D>)e.OldValue, (IDataSource3D<Vector3D>)e.NewValue);
		}

		private void OnDataSourceReplaced(IDataSource3D<Vector3D> prevSource, IDataSource3D<Vector3D> currSource)
		{
			if (prevSource != null)
				prevSource.Changed -= OnDataSourceChanged;
			if (currSource != null)
				currSource.Changed += OnDataSourceChanged;
			RebuildUI();
		}

		private void OnDataSourceChanged(object sender, EventArgs e)
		{
			RebuildUI();
		}

		#endregion DataSource

		#endregion Properties

		protected abstract void RebuildUI();
	}
}
