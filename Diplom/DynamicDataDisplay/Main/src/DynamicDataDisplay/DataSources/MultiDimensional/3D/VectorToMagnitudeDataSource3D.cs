using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public class VectorToMagnitudeDataSource3D : IDataSource3D<double>
	{
		private readonly IDataSource3D<Vector3D> dataSource;

		public VectorToMagnitudeDataSource3D(IDataSource3D<Vector3D> dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");

			//todo subscribe on dataSource Changed event
			this.dataSource = dataSource;
			this.data = new MagnitudeData3D(dataSource.Data);
		}

		#region IDataSource3D<double> Members

		private readonly MagnitudeData3D data;
		public IData3D<double> Data
		{
			get { return data; }
		}

		private sealed class MagnitudeData3D : IData3D<double>
		{
			private readonly IData3D<Vector3D> data;
			public MagnitudeData3D(IData3D<Vector3D> data)
			{
				this.data = data;
			}

			#region IData3D<double> Members

			public double this[int i, int j, int k]
			{
				get
				{
					double length = data[i, j, k].Length;
					return length;
				}
			}

			#endregion
		}

		public event EventHandler Changed;

		#endregion

		#region IGridSource3D Members

		public int Width
		{
			get { return dataSource.Width; }
		}

		public int Height
		{
			get { return dataSource.Height; }
		}

		public int Depth
		{
			get { return dataSource.Depth; }
		}

		public IGrid3D Grid
		{
			get { return dataSource.Grid; }
		}

		#endregion
	}
}
