using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    class MathHelper
    {
        public static void GetMaxMin(double[,] data, out double max, out double min)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (double num in data)
            {
                if (num < min)
                    min = num;
                if (num > max)
                    max = num;
            }
        }

        public static void GetMaxMin(int[,] data, out double max, out double min)
        {
            min = int.MaxValue;
            max = int.MinValue;

            foreach (int num in data)
            {
                if (num < min)
                    min = num;
                if (num > max)
                    max = num;
            }
        }

        public static void GetMaxMin(float[,] data, out double max, out double min)
        {
            min = float.MaxValue;
            max = float.MinValue;

            foreach (float num in data)
            {
                if (num < min)
                    min = num;
                if (num > max)
                    max = num;
            }
        }
    }
}
