using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    interface IVisualizationProvider
    {
        void AddLayer(VisualizationDataSource dataSource);
        void RemoveLayer(VisualizationDataSource dataSource);
        bool ContainsLayer(Guid guid);
        Algorithms GetAlgorithmType();
        bool IsDynamicProvider { get; set; }
        Guid ProviderGuid { get; set; }
    }
}
