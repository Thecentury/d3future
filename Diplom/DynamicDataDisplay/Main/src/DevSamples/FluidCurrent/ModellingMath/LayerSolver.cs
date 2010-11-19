using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidCurrentModelling2.DataStructures;

namespace FluidCurrentModelling2.ModellingMath
{
    class LayerSolver
    {
        //Каждый слой - это набор трехмерных матриц для каджой переменной:
        LayerData oldLayer, prevLayer, intermediateLayer1, intermediateLayer2, nextLayer;
        NumericalParameters numPar;
        double cp;
        bool column;

        //Объект(колонна) внутри канала делит его на сегменты, по которым будут произовдится прогонки
        int Ax = 0, Bx = 0, Cx = 0, Dx = 0;
        int Ay = 0, By = 0, Cy = 0, Dy = 0;
        int Az = 0, Bz = 0, Cz = 0, Dz = 0;


        private int width, height, thickness;

        public LayerSolver(LayerData prevLayer, NumericalParameters numericalParameters)
        {
            width = prevLayer.Width;
            height = prevLayer.Height;
            thickness = prevLayer.Thickness;

            this.prevLayer = prevLayer;
            this.oldLayer = prevLayer;
            this.nextLayer = new LayerData(width, height, thickness);
            this.intermediateLayer1 = new LayerData(width, height, thickness);
            this.intermediateLayer2 = new LayerData(width, height, thickness);

            this.numPar = numericalParameters;
            this.cp = (numericalParameters.Gamma - 1) / (numericalParameters.Gamma * numericalParameters.Re);

            //Параметры колонны
            Ax = Az = 0;
            Dx = width;
            Dz = thickness;
            //if (Nz = 50, Ny = Nx = 40) =>
            Bx = (int)(numericalParameters.Nx / 2.0 - numericalParameters.Nx / 4.0);    //10
            Cx = (int)(numericalParameters.Nx / 2.0 + numericalParameters.Nx / 4.0);    //30
            Bz = (int)(numericalParameters.Nz / 2.0 - numericalParameters.Nz / 5.0);    //15
            Cz = (int)(numericalParameters.Nz / 2.0 + numericalParameters.Nz / 5.0);    //35
        }

        public LayerSolver(LayerData oldLayer, LayerData prevLayer, NumericalParameters numericalParameters)
        {
            width = prevLayer.Width;
            height = prevLayer.Height;
            thickness = prevLayer.Thickness;

            this.oldLayer = oldLayer;
            this.prevLayer = prevLayer;
            this.nextLayer = new LayerData(width, height, thickness);
            this.intermediateLayer1 = new LayerData(width, height, thickness);
            this.intermediateLayer2 = new LayerData(width, height, thickness);

            this.numPar = numericalParameters;
            this.cp = (numericalParameters.Gamma - 1) / (numericalParameters.Gamma * numericalParameters.Re);

            //Параметры колонны
            Ax = Az = 0;
            Dx = width;
            Dz = thickness;
            Bx = (int)(numericalParameters.Nx / 2.0 - numericalParameters.Nx / 4.0);
            Cx = (int)(numericalParameters.Nx / 2.0 + numericalParameters.Nx / 4.0);
            Bz = (int)(numericalParameters.Nz / 2.0 - numericalParameters.Nz / 5.0);
            Cz = (int)(numericalParameters.Nz / 2.0 + numericalParameters.Nz / 5.0);
        }

        private void SetZero(int i, int j, int startIndex, int endIndex, Dimensions dimension, LayerData layer)
        {
            int length = endIndex - startIndex;
            double[] zeroArray = new double[length];
            for (int k = 0; k < length; k++)
            {
                zeroArray[k] = 0;
            }
            layer.U.SetColumn(i, j, startIndex, dimension, zeroArray);
            layer.V.SetColumn(i, j, startIndex, dimension, zeroArray);
            layer.W.SetColumn(i, j, startIndex, dimension, zeroArray);
            layer.T.SetColumn(i, j, startIndex, dimension, zeroArray);
        }

        private void SolveColumnX(int i, int j, int startIndex, int endIndex, LayerData oldLayer, LayerData prevLayer, LayerData nextLayer, bool state)
        {
            DoubleMatrix3D u = new DoubleMatrix3D { FirstMatrix = oldLayer.U, SecondMatrix = prevLayer.U };
            DoubleMatrix3D v = new DoubleMatrix3D { FirstMatrix = oldLayer.V, SecondMatrix = prevLayer.V };
            DoubleMatrix3D w = new DoubleMatrix3D { FirstMatrix = oldLayer.W, SecondMatrix = prevLayer.W };
            DoubleMatrix3D T = new DoubleMatrix3D { FirstMatrix = oldLayer.T, SecondMatrix = prevLayer.T };

            int length = endIndex - startIndex;
            double[] downRow = new double[length];
            double[] middleRow = new double[length];
            double[] upperRow = new double[length];
            double[] f = new double[length];

            //Solving for U
            double h = 1.0 / (numPar.Re * numPar.Dx * numPar.Dx);
            for (int k = 1; k < length - 1; k++)
            {
                if (i == 0 || i == height - 1 || j == Az || j == Dz - 1)
                {
                    downRow[k] = 0.0;
                    middleRow[k] = 1.0;
                    upperRow[k] = 0.0;
                    f[k] = 0.0;
                }
                else
                {
                    downRow[k] = Auxiliaries.GetValue(u, startIndex + k, i, j) / (2 * numPar.Dx) - h;
                    middleRow[k] = 3.0 / numPar.Dt + 2.0 * h;
                    upperRow[k] = -Auxiliaries.GetValue(u, startIndex + k, i, j) / (2.0 * numPar.Dx) - h;
                    f[k] = 3.0 * prevLayer.U[startIndex + k, i, j] / numPar.Dt -
                            (Auxiliaries.GetValue(T, startIndex + k + 1, i, j) - Auxiliaries.GetValue(T, startIndex + k - 1, i, j)) / (2.0 * numPar.Dx);
                }
                if (column)
                    if ((j == Bz || j == Cz - 1) && (k >= Bx && k <= Cx - 1))
                    {
                        downRow[k] = 0.0;
                        middleRow[k] = 1.0;
                        upperRow[k] = 0.0;
                        f[k] = 0.0;
                    }
            }
            if (startIndex == Ax && i != 0 && i != height - 1 && j != Az && j != Dz - 1)
                f[0] = 1.0;
            else
                f[0] = 0.0;
            upperRow[0] = 0.0;
            middleRow[0] = 1.0;
            downRow[0] = 0.0;

            if (!state)
            {
                f[length - 1] = 0.0;
                upperRow[length - 1] = 0.0;
                middleRow[length - 1] = 1.0;
                downRow[length - 1] = 0.0;
            }
            else
            {
                f[length - 1] = 2.0 * prevLayer.U[length - 2, i, j] - prevLayer.U[length - 3, i, j];
                upperRow[length - 1] = 0.0;
                middleRow[length - 1] = 1.0;
                downRow[length - 1] = 0.0;
            }
            
            PurlinMatrix pmatrix = new PurlinMatrix(downRow, middleRow, upperRow);
            PurlinSolver pSolver = new PurlinSolver(pmatrix, f);

            double[] result = pSolver.Solve();
            nextLayer.U.SetColumn(i, j, startIndex, Dimensions.Width, result);

            //if (state)
            //    nextLayer.U[length - 1, i, j] = 2.0 * nextLayer.U[length - 2, i, j] - nextLayer.U[length - 3, i, j];

            //Solving for V
            for (int k = 1; k < length - 1; k++)
            {
                if (i == 0 || i == height - 1 || j == Az || j == Dz - 1)
                    f[k] = 0.0;
                else
                    f[k] = 3 * prevLayer.V[startIndex + k, i, j] / numPar.Dt;
                if (column)
                    if ((j == Bz || j == Cz - 1) && (k >= Bx && k <= Cx - 1))
                        f[k] = 0.0;
            }

            f[0] = 0.0;
            if (!state)
                f[length - 1] = 0.0;
            else
                f[length - 1] = 2.0 * prevLayer.V[length - 2, i, j] - prevLayer.V[length - 3, i, j];

            pSolver = new PurlinSolver(pmatrix, f);

            result = pSolver.Solve();
            nextLayer.V.SetColumn(i, j, startIndex, Dimensions.Width, result);

            //if (state)
            //    nextLayer.V[length - 1, i, j] = 2.0 * nextLayer.V[length - 2, i, j] - nextLayer.V[length - 3, i, j];

            //Solving for W
            for (int k = 1; k < length - 1; k++)
            {
                if (i == 0 || i == height - 1 || j == Az || j == Dz - 1)
                    f[k] = 0.0;
                else
                    f[k] = 3 * prevLayer.W[startIndex + k, i, j] / numPar.Dt;
                if (column)
                    if ((j == Bz || j == Cz - 1) && (k >= Bx && k <= Cx - 1))
                        f[k] = 0.0;
            }
            f[0] = 0.0;
            if (state)
                f[length - 1] = 0.0;
            else
                f[length - 1] = 2.0 * prevLayer.W[length - 2, i, j] - prevLayer.W[length - 3, i, j];

            pSolver = new PurlinSolver(pmatrix, f);

            result = pSolver.Solve();
            nextLayer.W.SetColumn(i, j, startIndex, Dimensions.Width, result);

            //if (state)
            //    nextLayer.W[length - 1, i, j] = 2.0 * nextLayer.W[length - 2, i, j] - nextLayer.W[length - 3, i, j];

            //Solving for T
            h = 1.0 / (numPar.Re * numPar.Pr * numPar.Dx * numPar.Dx);
            for (int k = 1; k < length - 1; k++)
            {
                upperRow[k] = 0.0;
                middleRow[k] = 1.0;
                downRow[k] = 0.0;
                if (i == 0)
                    f[k] = prevLayer.T[k, i + 1, j];
                else if (i == height - 1)
                    f[k] = prevLayer.T[k, i - 1, j];
                else if (j == Az)
                    f[k] = oldLayer.T[k, i, j + 1];
                else if (j == Dz - 1)
                    f[k] = oldLayer.T[k, i, j - 1];
                else
                {
                    downRow[k] = Auxiliaries.GetValue(u, startIndex + k, i, j) / (2 * numPar.Dx) - h;
                    middleRow[k] = 3.0 / numPar.Dt + 2.0 * h;
                    upperRow[k] = -Auxiliaries.GetValue(u, startIndex + k, i, j) / (2.0 * numPar.Dx) - h;
                    f[k] = 3 * prevLayer.T[startIndex + k, i, j] / numPar.Dt + cp * Auxiliaries.GetPhiX(u, v, w, startIndex + k, i, j, numPar);
                }
                if (column)
                {
                    if ((j == Bz) && (k >= Bx && k <= Cx - 1))
                        f[k] = oldLayer.T[k, i, j - 1];
                    else if ((j == Cz - 1) && (k >= Bx && k <= Cx - 1))
                        f[k] = oldLayer.T[k, i, j + 1];
                }
            }
            if (startIndex == Ax && i != 0 && i != height - 1 && j != Az && j != Dz - 1)
            {
                f[0] = 1.0;
                upperRow[0] = 0.0;
                middleRow[0] = 1.0;
                downRow[0] = 0.0;
            }
            else
            {
                f[0] = 0.0;
                upperRow[0] = 1.0;
                middleRow[0] = -1.0;
                downRow[0] = 0.0;
            }
            if (!state)
            {
                upperRow[length - 1] = 0.0;
                middleRow[length - 1] = -1.0;
                downRow[length - 1] = 1.0;
                f[length - 1] = 0.0;
            }
            else
            {
                upperRow[length - 1] = 0.0;
                middleRow[length - 1] = 1.0;
                downRow[length - 1] = 0.0;
                f[length - 1] = 2.0 * prevLayer.T[length - 2, i, j] - prevLayer.T[length - 3, i, j];
            }

            pmatrix = new PurlinMatrix(downRow, middleRow, upperRow);
            pSolver = new PurlinSolver(pmatrix, f);

            result = pSolver.Solve();
            nextLayer.T.SetColumn(i, j, startIndex, Dimensions.Width, result);

            //if (state)
            //    nextLayer.T[length - 1, i, j] = 2.0 * nextLayer.T[length - 2, i, j] - nextLayer.T[length - 3, i, j];
        }

        private void SolveColumnY(int i, int j, int startIndex, int endIndex, LayerData oldLayer, LayerData prevLayer, LayerData nextLayer)
        {
            DoubleMatrix3D u = new DoubleMatrix3D { FirstMatrix = oldLayer.U, SecondMatrix = prevLayer.U };
            DoubleMatrix3D v = new DoubleMatrix3D { FirstMatrix = oldLayer.V, SecondMatrix = prevLayer.V };
            DoubleMatrix3D w = new DoubleMatrix3D { FirstMatrix = oldLayer.W, SecondMatrix = prevLayer.W };
            DoubleMatrix3D T = new DoubleMatrix3D { FirstMatrix = oldLayer.T, SecondMatrix = prevLayer.T };

            int length = endIndex - startIndex;
            double[] downRow = new double[length];
            double[] middleRow = new double[length];
            double[] upperRow = new double[length];
            double[] f = new double[length];

            //Solving for U
            double h = 1.0 / (numPar.Re * numPar.Dy * numPar.Dy);

            for (int k = 1; k < length - 1; k++)
            {
                downRow[k] = Auxiliaries.GetValue(v, i, startIndex + k, j) / (2.0 * numPar.Dy) - h;
                middleRow[k] = 3.0 / numPar.Dt + 2.0 * h;
                upperRow[k] = -Auxiliaries.GetValue(v, i, startIndex + k, j) / (2.0 * numPar.Dy) - h;
                f[k] = 3.0 * prevLayer.U[i, startIndex + k, j] / numPar.Dt;
            }
            upperRow[0] = 0.0;
            middleRow[0] = 1.0;
            downRow[0] = 0.0;
            f[0] = 0.0;
            upperRow[length - 1] = 0.0;
            middleRow[length - 1] = 1.0;
            downRow[length - 1] = 0.0;
            f[length - 1] = 0.0;
            
            PurlinMatrix pmatrix = new PurlinMatrix(downRow, middleRow, upperRow);
            PurlinSolver pSolver = new PurlinSolver(pmatrix, f);

            double[] result = pSolver.Solve();
            nextLayer.U.SetColumn(i, j, startIndex, Dimensions.Height, result);

            //Solving for V
            for (int k = 1; k < length - 1; k++)
            {
                f[k] = 3.0 * prevLayer.V[i, startIndex + k, j] / numPar.Dt -
                    (Auxiliaries.GetValue(T, i, startIndex + k + 1, j) - Auxiliaries.GetValue(T, i, startIndex + k - 1, j)) / (2.0 * numPar.Dy);
            }
            f[0] = 0.0;
            f[length - 1] = 0;
            
            pSolver = new PurlinSolver(pmatrix, f);
            result = pSolver.Solve();
            nextLayer.V.SetColumn(i, j, startIndex, Dimensions.Height, result);

            //Solving for W
            for (int k = 1; k < length - 1; k++)
            {
                f[k] = 3.0 * prevLayer.W[i, startIndex + k, j] / numPar.Dt;
            }
            f[0] = 0.0;
            f[length - 1] = 0;
            
            pSolver = new PurlinSolver(pmatrix, f);
            result = pSolver.Solve();
            nextLayer.W.SetColumn(i, j, startIndex, Dimensions.Height, result);

            //Solving for T
            h = 1.0 / (numPar.Re * numPar.Pr * numPar.Dy * numPar.Dy);
            for (int k = 1; k < length - 1; k++)
            {
                downRow[k] = Auxiliaries.GetValue(v, i, startIndex + k, j) / (2.0 * numPar.Dy) - h;
                middleRow[k] = 3.0 / numPar.Dt + 2.0 * h;
                upperRow[k] = -Auxiliaries.GetValue(v, i, startIndex + k, j) / (2.0 * numPar.Dy) - h;
                f[k] = 3.0 * prevLayer.T[i, startIndex + k, j] / numPar.Dt - cp * Auxiliaries.GetPhiY(u, v, w, i, startIndex + k, j, numPar);
            }
            upperRow[0] = 1.0;
            middleRow[0] = -1.0;
            downRow[0] = 0.0; 
            f[0] = 0.0;
            upperRow[length - 1] = 0.0;
            middleRow[length - 1] = -1.0;
            downRow[length - 1] = 1.0;
            f[length - 1] = 0.0;
            
            pSolver = new PurlinSolver(pmatrix, f);
            result = pSolver.Solve();
            nextLayer.T.SetColumn(i, j, startIndex, Dimensions.Height, result);
        }

        private void SolveColumnZ(int i, int j, int startIndex, int endIndex, LayerData oldLayer, LayerData prevLayer, LayerData nextLayer)
        {
            DoubleMatrix3D u = new DoubleMatrix3D { FirstMatrix = oldLayer.U, SecondMatrix = prevLayer.U };
            DoubleMatrix3D v = new DoubleMatrix3D { FirstMatrix = oldLayer.V, SecondMatrix = prevLayer.V };
            DoubleMatrix3D w = new DoubleMatrix3D { FirstMatrix = oldLayer.W, SecondMatrix = prevLayer.W };
            DoubleMatrix3D T = new DoubleMatrix3D { FirstMatrix = oldLayer.T, SecondMatrix = prevLayer.T };

            int length = endIndex - startIndex;
            double[] downRow = new double[length];
            double[] middleRow = new double[length];
            double[] upperRow = new double[length];
            double[] f = new double[length];

            //Solving for U
            double h = 1.0 / (numPar.Dz * numPar.Dz * numPar.Re);
            for (int k = 1; k < length - 1; k++)
            {
                downRow[k] = Auxiliaries.GetValue(w, i, j, startIndex + k) / (2.0 * numPar.Dz) - h;
                middleRow[k] = 3.0 / numPar.Dt + 2.0 * h;
                upperRow[k] = -Auxiliaries.GetValue(w, i, j, startIndex + k) / (2.0 * numPar.Dz) - h;
                f[k] = 3.0 * prevLayer.U[i, j, startIndex + k] / numPar.Dt;
            }
            upperRow[0] = 0.0;
            middleRow[0] = 1.0;
            downRow[0] = 0.0;
            f[0] = 0.0;
            upperRow[length - 1] = 0.0;
            middleRow[length - 1] = 1.0;
            downRow[length - 1] = 0.0;
            f[length - 1] = 0.0;
            
            PurlinMatrix pmatrix = new PurlinMatrix(downRow, middleRow, upperRow);
            PurlinSolver pSolver = new PurlinSolver(pmatrix, f);

            double[] result = pSolver.Solve();
            nextLayer.U.SetColumn(i, j, startIndex, Dimensions.Thickness, result);

            //Solving for V
            for (int k = 1; k < length - 1; k++)
            {
                f[k] = 3.0 * prevLayer.V[i, j, startIndex + k] / numPar.Dt;
            }
            f[0] = 0.0;
            f[length - 1] = 0.0;
            
            pSolver = new PurlinSolver(pmatrix, f);
            result = pSolver.Solve();
            nextLayer.V.SetColumn(i, j, startIndex, Dimensions.Thickness, result);

            //Solving for W
            for (int k = 1; k < length - 1; k++)
            {
                f[k] = 3.0 * prevLayer.W[i, j, startIndex + k] / numPar.Dt -
                    (Auxiliaries.GetValue(T, i, j, startIndex + k + 1) - Auxiliaries.GetValue(T, i, j, startIndex + k - 1)) / (2.0 * numPar.Dz);
            }
            f[0] = 0.0;
            f[length - 1] = 0.0;
            
            pSolver = new PurlinSolver(pmatrix, f);
            result = pSolver.Solve();
            nextLayer.W.SetColumn(i, j, startIndex, Dimensions.Thickness, result);

            //Solving for T
            h = 1.0 / (numPar.Dz * numPar.Dz * numPar.Re * numPar.Pr);
            for (int k = 1; k < length - 1; k++)
            {
                downRow[k] = Auxiliaries.GetValue(w, i, j, startIndex + k) / (2.0 * numPar.Dz) - h;
                middleRow[k] = 3.0 / numPar.Dt + 2.0 * h;
                upperRow[k] = -Auxiliaries.GetValue(w, i, j, startIndex + k) / (2.0 * numPar.Dz) - h;
                f[k] = 3.0 * prevLayer.T[i, j, startIndex + k] / numPar.Dt +
                   cp * Auxiliaries.GetPhiZ(u, v, w, i, j, startIndex + k, numPar);
            }
            upperRow[0] = 1.0;
            middleRow[0] = -1.0;
            downRow[0] = 0.0; 
            f[0] = 0.0;
            upperRow[length - 1] = 0.0; 
            middleRow[length - 1] = -1.0;
            downRow[length - 1] = 1.0;
            f[length - 1] = 0;
            
            pSolver = new PurlinSolver(pmatrix, f);
            result = pSolver.Solve();
            nextLayer.T.SetColumn(i, j, startIndex, Dimensions.Thickness, result);
        }
        
        private void SolveX()
        {
            if (!column)
            {
                for (int i = 0; i < thickness; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnX(j, i, 0, width, intermediateLayer1, intermediateLayer2, nextLayer, true);
                    }
                }
            }
            else
            {
                for (int i = Az; i < Bz; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnX(j, i, Ax, Dx, intermediateLayer1, intermediateLayer2, nextLayer, true);
                    }
                }
                for (int i = Bz; i < Cz; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnX(j, i, Ax, Bx, intermediateLayer1, intermediateLayer2, nextLayer, false);
                        SetZero(j, i, Bx, Cx, Dimensions.Width, nextLayer);
                        SolveColumnX(j, i, Cx, Dx, intermediateLayer1, intermediateLayer2, nextLayer, true);
                    }
                }
                for (int i = Cz; i < Dz; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnX(j, i, Ax, Dx, intermediateLayer1, intermediateLayer2, nextLayer, true);
                    }
                }
            }
        }

        private void SolveY()
        {
            if (!column)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < thickness; j++)
                    {
                        SolveColumnY(i, j, 0, height, prevLayer, intermediateLayer1, intermediateLayer2);
                    }
                }
            }
            else
            {
                for (int i = Ax; i < Bx; i++)
                {
                    for (int j = Az; j < Dz; j++)
                    {
                        SolveColumnY(i, j, 0, height, prevLayer, intermediateLayer1, intermediateLayer2);
                    }
                }
                for (int i = Bx; i < Cx; i++)
                {
                    for (int j = Az; j < Bz; j++)
                    {
                        SolveColumnY(i, j, 0, height, prevLayer, intermediateLayer1, intermediateLayer2);
                    }
                    for (int j = Bz; j < Cz; j++)
                    {
                        SetZero(i, j, 0, height, Dimensions.Height, intermediateLayer2);
                    }
                    for (int j = Cz; j < Dz; j++)
                    {
                        SolveColumnY(i, j, 0, height, prevLayer, intermediateLayer1, intermediateLayer2);
                    }
                }
                for (int i = Cx; i < Dx; i++)
                {
                    for (int j = Az; j < Dz; j++)
                    {
                        SolveColumnY(i, j, 0, height, prevLayer, intermediateLayer1, intermediateLayer2);
                    }
                }
            }
        }

        private void SolveZ()
        {
            if (!column)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnZ(i, j, 0, thickness, oldLayer, prevLayer, intermediateLayer1);
                    }
                }
            }
            else
            {

                for (int i = Ax; i < Bx; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnZ(i, j, Ax, Dz, oldLayer, prevLayer, intermediateLayer1);
                    }
                }
                for (int i = Bx; i < Cx; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnZ(i, j, Az, Bz, oldLayer, prevLayer, intermediateLayer1);
                        SetZero(i, j, Bz, Cz, Dimensions.Thickness, intermediateLayer1);
                        SolveColumnZ(i, j, Cz, Dz, oldLayer, prevLayer, intermediateLayer1);
                    }
                }
                for (int i = Cx; i < Dx; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        SolveColumnZ(i, j, Az, Dz, oldLayer, prevLayer, intermediateLayer1);
                    }
                }
            }
        }

        public LayerData Solve(bool withColumn)
        {
            column = withColumn;
            
            SolveZ();
            SolveY();
            SolveX();

            nextLayer.Div = Auxiliaries.Error(nextLayer.U, nextLayer.V, nextLayer.W, numPar.Dx, numPar.Dy, numPar.Dz);

            return nextLayer;
        }
    }
}
