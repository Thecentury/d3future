using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    public class PointSet
    {
        public Guid Guid { get; set; }

        private List<PointSetElement> data;

        internal List<PointSetElement> Data
        {
            get { return data; }
        }

        private Dictionary<string, string> metadata;

        public Dictionary<string, string> Metadata
        {
            get { return metadata; }
        }

        public PointSet(List<PointSetElement> data)
        {
            this.data = data;
            metadata = new Dictionary<string, string>();
        }

        public double MaxValue
        {
            get
            {
                if (data[0].Value is double)
                {
                    double max = (double)data[0].Value;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if ((double)data[i].Value > max)
                            max = (double)data[i].Value;
                    }
                    return max;
                }
                else
                    return Double.NaN;
            }
        }


        public double MinValue
        {
            get
            {
                if (data[0].Value is double)
                {
                    double min = (double)data[0].Value;
                    for (int i = 0; i < data.Count; i++)
                    {
                        if ((double)data[i].Value < min)
                            min = (double)data[i].Value;
                    }
                    return min;
                }
                else
                    return Double.NaN;
            }
        }

        public event PointSetUpdatedHandler PointSetUpdated;

        public void AddElement(PointSetElement element)
        {
            data.Add(element);
            if (PointSetUpdated != null)
                PointSetUpdated(this, new PointSetUpdatedEventArgs());
        }

    }

    public delegate void PointSetUpdatedHandler(PointSet pointSet, PointSetUpdatedEventArgs e);

    public class PointSetUpdatedEventArgs
    {

    }
}
