using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;

namespace Microsoft.Research.DynamicDataDisplay.DirectX11
{
    public class LinesDataSource
    {
        private List<SingleLineDataSource> lines =  new List<SingleLineDataSource>();

        public List<SingleLineDataSource> Lines
        {
            get { return lines; }
        }
    }

    public class SingleLineDataSource
    {
        private List<LinePoint> points = new List<LinePoint>();

        public List<LinePoint> Points
        {
            get { return points; }
        }
    }

    public class LinePoint
    {
        public Vector2F Position { get; set; }
        public ColorRgba Color { get; set; }
    }
}
