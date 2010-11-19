using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.MapPoint.Rendering3D.Utility;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Isolines
{
    class IsolinesLayer
    {
        public Guid Guid { get; set; }
        public bool IsVisible { get; set; }

        private List<PolylineGeometry> geometry;

        public List<PolylineGeometry> Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }

        public List<VisualPushpin> Labels { get; set; }
    }
}
