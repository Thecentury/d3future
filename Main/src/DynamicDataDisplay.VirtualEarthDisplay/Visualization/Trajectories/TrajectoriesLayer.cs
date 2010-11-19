using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.Utility;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Trajectories
{
    class TrajectoriesLayer
    {
        public Guid Guid { get; set; }
        public bool IsVisible { get; set; }
        public TrajectoriesDataSource DataSource { get; set; }
        public PolylineGeometry Geometry { get; set; }
        public string LayerID { get; set; }
        public string DataSourceID { get; set; }
        public string GeometryID { get; set; }
    }
}
