using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidCurrentModelling2.DataStructures
{
    public sealed class NumericalParameters
    {
        private double dx, dy, dz, dt, re, pr, gamma;
        private int nx, ny, nz, nt;
        
        /// <summary>
        /// Теплопроводность
        /// </summary>
        public double Gamma
        {
            get { return gamma; }
        }

        /// <summary>
        /// Число Прандтля
        /// </summary>
        public double Pr
        {
            get { return pr; }
        }
        
        /// <summary>
        /// Число Рейнольца
        /// </summary>
        public double Re
        {
            get { return re; }
        }
        
        /// <summary>
        /// Шаг по X
        /// </summary>
        public double Dx
        {
            get { return dx; }
        }

        /// <summary>
        /// Шаг по Y
        /// </summary>
        public double Dy
        {
            get { return dy; }
        }

        /// <summary>
        /// Шаг по Z
        /// </summary>
        public double Dz
        {
            get { return dz; }
        }

        /// <summary>
        /// Шаг по T 
        /// </summary>
        public double Dt
        {
            get { return dt; }
        }
        /// <summary>
        /// Число узлов сетки по X
        /// </summary>
        public int Nx
        {
            get { return nx; }
        }

        /// <summary>
        /// Число узлов сетки по Y
        /// </summary>
        public int Ny
        {
            get { return ny; }
        }

        /// <summary>
        /// Число узлов сетки по Z
        /// </summary>
        public int Nz
        {
            get { return nz; }
        }

        /// <summary>
        /// Число узлов сетки по T
        /// </summary>
        public int Nt
        {
            get { return nt; }
        }

        public NumericalParameters(double dx, double dy, double dz, double dt, int nx, int ny, int nz, int nt, double re, double pr, double gamma)
        {
            this.dx = dx;
            this.dy = dy;
            this.dz = dz;
            this.dt = dt;

            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.nt = nt;

            this.pr = pr;
            this.re = re;
            this.gamma = gamma;
        }

    }
}
