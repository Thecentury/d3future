using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidCurrentModelling2.DataStructures;

namespace FluidCurrentModelling2.ModellingMath
{
    class Auxiliaries
    {
        public static double GetPhiX(DoubleMatrix3D u, DoubleMatrix3D v, DoubleMatrix3D w, int i, int j, int k, NumericalParameters numPar)
        {
            double u_x = GetDX(u, i, j, k, numPar);
            double v_x = GetDX(v, i, j, k, numPar);
            double w_x = GetDX(w, i, j, k, numPar);
            double u_y = GetDY(u, i, j, k, numPar);
            double u_z = GetDZ(u, i, j, k, numPar);

            return 2 * u_x * u_x + v_x * v_x + w_x * w_x + v_x * u_y + w_x * u_z;
        }

        public static double GetPhiY(DoubleMatrix3D u, DoubleMatrix3D v, DoubleMatrix3D w, int i, int j, int k, NumericalParameters numPar)
        {
            double u_y = GetDY(u, i, j, k, numPar);
            double v_y = GetDY(v, i, j, k, numPar);
            double w_y = GetDY(w, i, j, k, numPar);
            double v_x = GetDX(v, i, j, k, numPar);
            double v_z = GetDZ(v, i, j, k, numPar);

            return u_y * u_y + 2 * v_y * v_y + w_y * w_y + u_y * v_x + w_y * v_z;
        }

        public static double GetPhiZ(DoubleMatrix3D u, DoubleMatrix3D v, DoubleMatrix3D w, int i, int j, int k, NumericalParameters numPar)
        {
            double u_z = GetDZ(u, i, j, k, numPar);
            double v_z = GetDZ(v, i, j, k, numPar);
            double w_z = GetDZ(w, i, j, k, numPar);
            double w_x = GetDX(w, i, j, k, numPar);
            double w_y = GetDY(w, i, j, k, numPar);

            return u_z * u_z + v_z * v_z + 2 * w_z * w_z + u_z * w_x + w_y * v_z;
        }

        public static double GetValue(DoubleMatrix3D data, int i, int j, int k)
        {
            return (data.FirstMatrix[i, j, k] + data.SecondMatrix[i, j, k]) / 2.0;
        }

        public static Matrix3D Error(Matrix3D u, Matrix3D v, Matrix3D w, double dx, double dy, double dz)
        {
            Matrix3D result = new Matrix3D(u.Width, u.Height, u.Thickness);
            for (int i = 1; i < u.Width; i++)
            {
                for (int j = 1; j < u.Height; j++)
                {
                    for (int k = 1; k < u.Thickness; k++)
                    {
                        result[i, j, k] = Math.Abs((u[i, j, k] - u[i - 1, j, k]) / dx + (v[i, j, k] - v[i, j - 1, k]) / dy + (w[i, j, k] - w[i, j, k - 1]) / dz);
                    }
                }
            }
            return result;
        }

        private static double GetDX(DoubleMatrix3D data, int i, int j, int k, NumericalParameters numPar)
        {
            if (i == 0)
                return (GetValue(data, i + 1, j, k) - GetValue(data, i, j, k)) / numPar.Dx;
            else if (i == numPar.Nx - 1)
                return (GetValue(data, i, j, k) - GetValue(data, i - 1, j, k)) / numPar.Dx;
            else
                return (GetValue(data, i + 1, j, k) - GetValue(data, i - 1, j, k)) / (2 * numPar.Dx);
        }

        private static double GetDY(DoubleMatrix3D data, int i, int j, int k, NumericalParameters numPar)
        {
            if (j == 0)
                return (GetValue(data, i, j + 1, k) - GetValue(data, i, j, k)) / numPar.Dy;
            else if (j == numPar.Ny - 1)
                return (GetValue(data, i, j, k) - GetValue(data, i, j - 1, k)) / numPar.Dy;
            else
                return (GetValue(data, i, j + 1, k) - GetValue(data, i, j - 1, k)) / (2 * numPar.Dy);
        }

        private static double GetDZ(DoubleMatrix3D data, int i, int j, int k, NumericalParameters numPar)
        {
            if (k == 0)
                return (GetValue(data, i, j, k + 1) - GetValue(data, i, j, k)) / numPar.Dz;
            else if (k == numPar.Nz - 1)
                return (GetValue(data, i, j, k) - GetValue(data, i, j, k - 1)) / numPar.Dz;
            else
                return (GetValue(data, i, j, k + 1) - GetValue(data, i, j, k - 1)) / (2 * numPar.Dz);
        }


    }

}
