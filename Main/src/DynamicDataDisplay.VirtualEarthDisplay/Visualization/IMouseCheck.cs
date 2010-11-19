using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    interface IMouseCheck
    {
        bool CheckIntersection(LatLonAlt location);
        void ShowIntersectedValues();
    }
}
