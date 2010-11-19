using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidCurrentModelling2.DataStructures
{
    class Matrix3D
    {
        private double[, , ,] data;
        private int width, height, thickness;

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

        public double this[int i, int j, int k]
        {
            get
            {
                return data[i, j, k, 0];
            }

            set
            {
                data[i, j, k, 0] = value;
            }
        }

        public Matrix3D(int width, int height, int thickness)
        {
            this.width = width;
            this.height = height;
            this.thickness = thickness;

            data = new double[width, height, thickness, 1];
        }

        public Matrix2D GetSubMatrix(int index, Dimensions dimsType)
        {
            switch (dimsType)
            {
                case Dimensions.TimeDimension:
                    throw new InvalidOperationException("Can't get submatrix for this dimension");
                case Dimensions.Thickness:
                    if (index < thickness)
                    {
                        Matrix2D result = new Matrix2D(width, height);
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                result[i, j] = this[i, j, index];
                            }
                        }
                        return result;
                    }
                    else
                        throw new ArgumentException("Invalid indexer");
                case Dimensions.Height:
                    if (index < height)
                    {
                        Matrix2D result = new Matrix2D(width, thickness);
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < thickness; j++)
                            {
                                result[i, j] = this[i, index, j];
                            }
                        }
                        return result;
                    }
                    else
                        throw new ArgumentException("Invalid indexer");
                case Dimensions.Width:
                    if (index < width)
                    {
                        Matrix2D result = new Matrix2D(height, thickness);
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < thickness; j++)
                            {
                                result[i, j] = this[index, i, j];
                            }
                        }
                        return result;
                    }
                    else
                        throw new ArgumentException("Invalid indexer");
                default:
                    throw new Exception("Something strange has happened");
            }
        }

        public void SetSubMatrix(Matrix2D matrix, int index, Dimensions dimsType)
        {
            switch (dimsType)
            {
                case Dimensions.TimeDimension:
                    throw new InvalidOperationException("Can't set submatrix for this dimension");
                case Dimensions.Thickness:
                    if (index < thickness)
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                this[i, j, index] = matrix[i, j];
                            }
                        }
                    else
                        throw new ArgumentException("Invalid indexer");
                    break;
                case Dimensions.Height:
                    if (index < height)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < thickness; j++)
                            {
                                this[i, index, j] = matrix[i, j];
                            }
                        }
                    }
                    else
                        throw new ArgumentException("Invalid indexer");
                    break;
                case Dimensions.Width:
                    if (index < width)
                    {
                        Matrix2D result = new Matrix2D(height, thickness);
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < thickness; j++)
                            {
                                this[index, i, j] = matrix[i, j];
                            }
                        }
                    }
                    else
                        throw new ArgumentException("Invalid indexer");
                    break;
                default:
                    throw new Exception("Something strange has happened");
            }
        }       

        public void SetColumn(int i, int j, int startIndex, Dimensions dimsType, double[] data)
        {
            if (CheckDim(dimsType, data.Length))
            {
                switch (dimsType)
                {
                    case Dimensions.Width:
                        for (int k = 0; k < data.Length; k++)
                        {
                            this[startIndex + k, i, j] = data[k];
                        }
                        break;
                    case Dimensions.Height:
                        for (int k = 0; k < data.Length; k++)
                        {
                            this[i, startIndex + k, j] = data[k];
                        }
                        break;
                    case Dimensions.Thickness:
                        for (int k = 0; k < data.Length; k++)
                        {
                            this[i, j, startIndex + k] = data[k];
                        }
                        break;
                    default: throw new InvalidOperationException("Invalid column type");
                }
            }
            else
                throw new InvalidOperationException("Invalid data for insert");
        }

        private bool CheckDim(Dimensions dimType, int value)
        {
            switch (dimType)
            {
                case Dimensions.Width: return value <= width;
                case Dimensions.Height: return value <= height;
                case Dimensions.Thickness: return value <= thickness;
                default: return false;
            }
        }

        public void InitializeData(double value)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < thickness; k++)
                    {
                        this[i, j, k] = value;
                    }
                }
            }
        }

        public Array ToArray()
        {
            return this.data;
        }

    }

}
