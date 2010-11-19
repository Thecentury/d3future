using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    public class PointSetElement
    {
        private object value;

        public object Value
        {
            get { return this.value; }
        }

        private double latitude;

        public double Latitude
        {
            get { return latitude; }
        }
        private double longitude;

        public double Longitude
        {
            get { return longitude; }
        }

        public PointSetElement(double latitude, double longitude, object value)
        {
            if (value is int || value is float)
                this.value = Convert.ToDouble(value);
            else
                this.value = value;
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public PointSetElement(double latitude, double longitude)
        {
            this.value = null;
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
