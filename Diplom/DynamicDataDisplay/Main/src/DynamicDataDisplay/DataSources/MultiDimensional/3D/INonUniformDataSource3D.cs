using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional
{
	public interface INonUniformDataSource3D<T> : IDataSource3D<T>
	{
		double[] XCoordinates { get; }
		double[] YCoordinates { get; }
		double[] ZCoordinates { get; }
	}
}
