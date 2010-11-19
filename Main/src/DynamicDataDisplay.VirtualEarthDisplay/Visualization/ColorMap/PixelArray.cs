using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    class PixelArray
    {
        private int width;

        public int Width
        {
            get { return width; }
        }
        private int height;

        public int Height
        {
            get { return height; }
        }
        private Color[,] pixels;

        public Color[,] Pixels
        {
            get { return pixels; }
        }

        public PixelArray(int width, int height, Color[,] pixels)
        {
            if (pixels.Length != width * height)
            {
                //Error
            }
            else
            {
                this.pixels = pixels;
                this.width = width;
                this.height = height;
            }
        }

        public PixelArray(int width, int height)
        {
            this.width = width;
            this.height = height;
            pixels = new Color[width, height];
        }
    }
}
