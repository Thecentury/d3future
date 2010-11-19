using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    public class NonUniformDataSource2D<T> : IRectDataSource2D<T>
    {
        private double[] gridX;
        private double[] gridY;
        private Point[,] grid;
        private T[,] data;

        public NonUniformDataSource2D(T[,] data, double[] lats, double[] lons)
        {
            if (data == null)
                throw new ArgumentException("data");
            if (lats == null)
                throw new ArgumentException("lats");
            if (lons == null)
                throw new ArgumentException("lons");

            if (lons.Length != data.GetLength(0) || lats.Length != data.GetLength(1))
                throw new ArgumentException("dimension");

            this.data = data;
            this.gridX = lons;
            this.gridY = lats;

            grid = new Point[lons.Length, lats.Length];
            for (int i = 0; i < lons.Length; i++)
            {
                for (int j = 0; j < lats.Length; j++)
                {
                    grid[i, j] = new Point(lons[i], lats[j]);
                }
            }

        }
        
        #region IRectDataSource2D<T> Members

        public double[] X
        {
            get { return gridX; }
        }

        public double[] Y
        {
            get { return gridY; }
        }

        #endregion

        #region IDataSource2D<T> Members

        public T[,] Data
        {
            get { return data; }
        }

        public IDataSource2D<T> GetSubset(int x0, int y0, int countX, int countY, int stepX, int stepY)
        {
            throw new NotImplementedException();
        }

        public void ApplyMappings(DependencyObject marker, int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGridSource2D Members

        public Point[,] Grid
        {
            get { return grid; }
        }

        public int Width
        {
            get { return gridX.Length; }
        }

        public int Height
        {
            get { return gridY.Length; }
        }

        public event EventHandler Changed;

        #endregion
    }
}
