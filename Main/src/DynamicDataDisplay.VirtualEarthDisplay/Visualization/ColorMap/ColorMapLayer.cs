using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    class ColorMapLayer
    {
        public string LayerName { get; set; }
        public string LayerID { get; set; }
        public Guid Guid { get; set; }
        public bool IsVisible { get; set; }
        public object Field { get; set; }
        public double LayerOpacity { get; set; }
        public double MinT { get; set; }
        public double MaxT { get; set; }
    }
}
