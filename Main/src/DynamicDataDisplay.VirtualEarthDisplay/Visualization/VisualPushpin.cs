using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.Utility;
using System.Drawing;
using Microsoft.MapPoint.Rendering3D;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    class VisualPushpin
    {
        public VisualPushpin(int width, int height, string value, LatLonAlt position, object tag, string id)
        {
            style = PushpinInfo.Default;

            double num = 0;
            try
            {
                num = Double.Parse(value);
                style.Label = num.ToString("0.##");
            }
            catch
            {
                style.Label = value;
            }
            style.HitDetect = HitDetectMode.None;
            style.ImageWidth = width;
            style.ImageHeight = height;
            style.BackColor = Color.FromArgb(0, Color.Blue);

            pushpin = new PushpinGeometry(
                Guid.NewGuid().ToString(),
                id,
                position,
                style);

        }

        private PushpinGeometry pushpin;

        public PushpinGeometry Pushpin
        {
            get { return pushpin; }
        }

        private PushpinInfo style;
    }
}
