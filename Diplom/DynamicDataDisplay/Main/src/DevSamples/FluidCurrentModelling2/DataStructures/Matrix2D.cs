using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidCurrentModelling2.DataStructures
{
    class Matrix2D
    {
        private double[,] data;
        private int width;
        /// <summary>
        /// Количество столбцов в матрице
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        private int height;
        /// <summary>
        /// Количество строк в матрице
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        public double this[int i, int j]
        {
            get
            {
                return data[i, j];
            }
            set
            {
                data[i, j] = value;
            }
        }

        public Matrix2D(int width, int height)
        {
            this.data = new double[width, height];
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Получить строчку матрицы с заданным номером
        /// </summary>
        /// <param name="index">номер строки</param>
        /// <returns></returns>
        public double[] GetRow(int index)
        {
            if (index < height)
            {
                double[] result = new double[width];
                for (int i = 0; i < width; i++)
                {
                    result[i] = data[i, index];
                }
                return result;
            }
            else
                throw new ArgumentException("Invalid row indexer");
        }

        /// <summary>
        /// Получить столбец матрицы с заданным номером
        /// </summary>
        /// <param name="index">номер столбца</param>
        /// <returns></returns>
        public double[] GetColumn(int index)
        {
            if (index < width)
            {
                double[] result = new double[height];
                for (int i = 0; i < height; i++)
                {
                    result[i] = data[index, i];
                }
                return result;
            }
            else
                throw new ArgumentException("Invalid column indexer");
        }
    }
}
