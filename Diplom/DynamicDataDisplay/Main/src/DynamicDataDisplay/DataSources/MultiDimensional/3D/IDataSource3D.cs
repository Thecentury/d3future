using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public interface IDataSource3D<T> : IGridSource3D
	{
		IData3D<T> Data { get; }

		event EventHandler Changed;
	}

	public interface IGridSource3D
	{
		int Width { get; }
		int Height { get; }
		int Depth { get; }

		IGrid3D Grid { get; }
	}

	public interface IGrid3D
	{
		Point3D this[int i, int j, int k] { get; }
	}

	public interface IData3D<T>
	{
		T this[int i, int j, int k] { get; }
	}
}
