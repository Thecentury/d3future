using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    class GeoRect
    {
        private double x;

        /// <summary>
        /// Left Coordinate
        /// </summary>
        public double X
        {
            get { return x; }
        }
        private double y;

        /// <summary>
        /// Bottom Coordinate
        /// </summary>
        public double Y
        {
            get { return y; }
        }

        private double width;

        /// <summary>
        /// Rect Width
        /// </summary>
        public double Width
        {
            get { return width; }
        }
        private double height;

        /// <summary>
        /// Rect Height
        /// </summary>
        public double Height
        {
            get { return height; }
        }

        public GeoRect(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Bounding Box of Triangle
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        public GeoRect(Point point1, Point point2, Point point3)
        {
            this.x = Math.Min(point1.X, Math.Min(point2.X, point3.X));
            this.y = Math.Min(point1.Y, Math.Min(point2.Y, point3.Y));

            double x2 = Math.Max(point1.X, Math.Max(point2.X, point3.X));
            double y2 = Math.Max(point1.Y, Math.Max(point2.Y, point3.Y));

            this.width = x2 - x;
            this.height = y2 - y;
        }

        public double Bottom
        {
            get { return y; }
        }

        public double Left
        {
            get { return x; }
        }

        public double Right
        {
            get { return x + width; }
        }

        public double Top
        {
            get { return y + height; }
        }

        public static GeoRect Intersect(GeoRect rect1, GeoRect rect2)
        {
            if (IntersectionExist(rect1, rect2))
            {
                double x = Math.Max(rect1.Left, rect2.Left);
                double y = Math.Max(rect1.Bottom, rect2.Bottom);
                double width = Math.Max((double)(Math.Min(rect1.Right, rect2.Right) - x), (double)0.0);
                double height = Math.Max((double)(Math.Min(rect1.Top, rect2.Top) - y), (double)0.0);
                return new GeoRect(x, y, width, height);
            }
            else
            {
                return null;
            }
        }

        public static bool IntersectionExist(GeoRect rect1, GeoRect rect2)
        {
            return (rect1.Left <= rect2.Right && rect1.Top >= rect2.Bottom && rect1.Bottom <= rect2.Top && rect1.Right >= rect2.Left);
        }
    }
}
