using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    interface IRectDataSource2D<T> : IDataSource2D<T>
    {
        double[] X { get; }
        double[] Y { get; }
    }
}
