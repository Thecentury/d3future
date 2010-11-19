using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidCurrentModelling2.DataStructures
{
    class Matrix4D
    {
        private Matrix3D[] data;
        private int width, height, thickness, timeDimension;

        /// <summary>
        /// Четвертое измерение
        /// </summary>
        public int TimeDimension
        {
            get { return timeDimension; }
        }

        /// <summary>
        /// Третье измерение
        /// </summary>
        public int Thickness
        {
            get { return thickness; }
        }

        /// <summary>
        /// Второе измерение
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Первое измерение
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        public Matrix4D(int width, int height, int thickness, int timeDimension)
        {
            this.width = width;
            this.height = height;
            this.thickness = thickness;
            this.timeDimension = timeDimension;

            this.data = new Matrix3D[timeDimension];
            for (int i = 0; i < timeDimension; i++)
            {
                data[i] = new Matrix3D(width, height, thickness);
            }
        }

        public Matrix3D this[int indexer]
        {
            get
            {
                if (indexer < timeDimension)
                    return data[indexer];
                else
                    throw new ArgumentException("Invalid indexer");
            }
            set 
            {
                if (indexer < timeDimension)
                    data[indexer] = value;
                else
                    throw new ArgumentException("Invalid indexer");
            }
        }


    }

    enum Dimensions
    {
        Width = 1,
        Height = 2,
        Thickness = 3,
        TimeDimension = 4
    }
}
