using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows.Media.Media3D;
using Microsoft.Research.Science.Data;

namespace FluidCurrent
{
	public class DataSetSource3D : FuncUniformDataSource3D<Vector3D>
	{
		private readonly Variable u;
		private readonly Variable v;
		private readonly Variable w;

		private readonly DataSet dataSet;

		public DataSetSource3D(DataSet dataSet)
		{
			if (dataSet == null)
				throw new ArgumentNullException("dataSet");

			this.dataSet = dataSet;
			this.u = dataSet["U velocity"];
			this.v = dataSet["V velocity"];
			this.w = dataSet["W velocity"];

			width = u.Dimensions["x"].Length;
			height = u.Dimensions["y"].Length;
			depth = u.Dimensions["z"].Length;

			xSize = 1;
			ySize = 1;
			zSize = 1;

			SetData(new FuncData3D<Vector3D>((i, j, k) => new Vector3D()));

			dataSet.Committed += new DataSetCommittedEventHandler(OnDataSetCommitted);
		}

		private void OnDataSetCommitted(object sender, DataSetCommittedEventArgs e)
		{
			int[] start = new int[] { 0, 0, 0, u.Dimensions[3].Length - 1 };

			double[, , ,] uData = (double[, , ,])u.GetData(start, null);
			double[, , ,] vData = (double[, , ,])v.GetData(start, null);
			double[, , ,] wData = (double[, , ,])w.GetData(start, null);

			SetData(new FuncData3D<Vector3D>((i, j, k) =>
			{
				Vector3D result = new Vector3D(uData[i, j, k, 0], vData[i, j, k, 0], wData[i, j, k, 0]);
				return result;
			}));

			RaiseChanged();
		}
	}
}
