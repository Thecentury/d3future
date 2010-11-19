using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidCurrentModelling2.DataStructures
{
    class PurlinMatrix
    {
        int size;
        /// <summary>
        /// Ширина и высота матрицы
        /// </summary>
        public int Size
        {
            get { return size; }
        }
        double[] downRow, middleRow, upperRow;

        public double[] UpperRow
        {
            get { return upperRow; }
        }

        public double[] MiddleRow
        {
            get { return middleRow; }
        }

        public double[] DownRow
        {
            get { return downRow; }
        }

        public double this[int i, int j]
        {
            get
            {
                if (i > size || j > size || Math.Abs(i - j) > 1)
                    return 0;
                else if (i == j)
                    return middleRow[i];
                else if (i < j)
                    return upperRow[i];
                else
                    return downRow[i];
            }
            set
            {
                if (i > size || j > size || Math.Abs(i - j) > 1)
                    throw new ArgumentException("Invalid indexer. PurlinMatrix is 3-diagonal matrix");
                else if (i == j)
                    middleRow[i] = value;
                else if (i < j)
                    upperRow[i] = value;
                else
                    downRow[i] = value;
            }
        }

        public PurlinMatrix(Matrix2D matrix)
        {
            if (matrix.Width != matrix.Height)
                throw new ArgumentException("Purlin Matrix should be square");
            else
            {
                this.size = matrix.Width;
                downRow = new double[size];
                upperRow = new double[size];
                middleRow = new double[size];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (Math.Abs(i - j) > 1 && matrix[i, j] != 0)
                            throw new ArgumentException("Invalid Matrix. Purlin matrix should be 3-diagonal");
                        else if (i == j)
                            middleRow[i] = matrix[i, j];
                        else if (i > j)
                            upperRow[i] = matrix[i, j];
                        else
                            downRow[j] = matrix[i, j];
                    }
                }
            }
        }

        public PurlinMatrix(int size)
        {
            this.size = size;
            this.downRow = new double[size];
            this.middleRow = new double[size];
            this.upperRow = new double[size];
        }

        public PurlinMatrix(double[] downRow, double[] middleRow, double[] upperRow)
        {
            int size = downRow.Length;
            if (size != middleRow.Length || size != middleRow.Length)
                throw new ArgumentException("All vectors must have the same length");
            else
            {
                this.size = size;
                this.downRow = downRow;
                this.middleRow = middleRow;
                this.upperRow = upperRow;
            }
        }

        public override string ToString()
        {
            string[] rows = new string[size];
            for (int i = 0; i < size; i++)
            {
                rows[i] = "";
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    rows[i] = rows[i] + " " + this[i, j];
                }
            }
            string result = "";
            for (int i = 0; i < size; i++)
            {
                result = result + rows[i] + "\n";
            }
            return result;

        }
    }
}
