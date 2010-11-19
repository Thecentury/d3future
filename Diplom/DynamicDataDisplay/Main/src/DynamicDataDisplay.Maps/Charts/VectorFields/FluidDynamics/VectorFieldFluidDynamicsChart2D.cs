using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<System.Windows.Vector>;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class VectorFieldFluidDynamicsChart2D : InteractiveFluidDynamicsChart2D
	{
		private UniformField2DWrapper wrapper;

		#region Properties

		#region DataSource property

		public DataSource DataSource
		{
			get { return (DataSource)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(DataSource),
		  typeof(VectorFieldFluidDynamicsChart2D),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var owner = (VectorFieldFluidDynamicsChart2D)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(DataSource prevDataSource, DataSource currDataSource)
		{
			if (prevDataSource != null)
				prevDataSource.Changed -= DataSource_OnChanged;
			if (currDataSource != null)
				currDataSource.Changed += DataSource_OnChanged;

			wrapper = new UniformField2DWrapper(DataSource.Data);
		}

		protected virtual void DataSource_OnChanged(object sender, EventArgs e)
		{
		}

		#endregion DataSource property

		#endregion Properties

		protected override void GetFluidData(double[,] dens_prev, double[,] u_prev, double[,] v_prev)
		{
			if (wrapper == null) return;

			base.GetFluidData(dens_prev, u_prev, v_prev);

			int nanCount = 0;
			for (int i = 1; i <= N; i++)
			{
				for (int j = 1; j <= N; j++)
				{
					var vector = wrapper.GetVector(i / (double)N, j / (double)N);

					if (!vector.X.IsNaN() && !vector.Y.IsNaN())
					{
						u_prev[i, j] = vector.X * N;
						v_prev[i, j] = vector.Y * N;
					}
					else
					{
						nanCount++;
					}
				}
			}

			Debug.WriteLine("Nan count " + nanCount.ToString());
		}
	}
}
