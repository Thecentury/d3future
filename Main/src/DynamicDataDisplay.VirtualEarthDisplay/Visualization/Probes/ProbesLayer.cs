using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes
{
    class ProbesLayer
    {
        public Guid Guid { get; set; }
        public bool IsVisible { get; set; }
        public ProbesDataSource DataSource { get; set; }
        public string LayerName { get; set; }
        public string LayerID { get; set; }
    }
}
