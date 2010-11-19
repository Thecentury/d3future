using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidCurrentModelling2.DataStructures;

namespace FluidCurrentModelling2.ModellingMath
{
    class PurlinSolver
    {
        PurlinMatrix matrix;
        double[] f;

        public PurlinSolver(PurlinMatrix matrix, double[] f)
        {
            if (f.Length != matrix.Size)
                throw new ArgumentException("Mismatch between matrix size and right side vector size");

            this.matrix = matrix;
            this.f = f;
        }

        public double[] Solve()
        {
            double[] result = new double[matrix.Size];
            double[] a = new double[matrix.Size];
            double[] b = new double[matrix.Size];

            //Прямой ход
            a[0] = 0;
            a[1] = -(matrix.UpperRow[0] / matrix.MiddleRow[0]);
            b[0] = 1;
            b[1] = f[0] / matrix.MiddleRow[0];

            for (int i = 1; i < matrix.Size - 1; i++)
            {
                a[i + 1] = -(matrix.UpperRow[i] / (matrix.MiddleRow[i] + matrix.DownRow[i] * a[i]));
                b[i + 1] = (f[i] - b[i] * matrix.DownRow[i]) / (matrix.MiddleRow[i] + matrix.DownRow[i] * a[i]);
            }

            //Обратный ход
            result[matrix.Size - 1] = (f[matrix.Size - 1] - matrix.DownRow[matrix.Size - 1] * b[matrix.Size - 1]) / (matrix.MiddleRow[matrix.Size - 1] + a[matrix.Size - 1] * matrix.DownRow[matrix.Size - 1]);
            for (int i = matrix.Size - 2; i >= 0; i--)
            {
                result[i] = a[i + 1] * result[i + 1] + b[i + 1];
            }

            return result;
        }
    }
}
