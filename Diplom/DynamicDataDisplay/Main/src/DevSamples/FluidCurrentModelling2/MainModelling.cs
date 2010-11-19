using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidCurrentModelling2.DataStructures;
using FluidCurrentModelling2.ModellingMath;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Factory;
using Microsoft.Research.Science.Data.Proxy;
//using Microsoft.Research.Science.Data.NetCDF4;

namespace FluidCurrentModelling2
{
    public class MainModelling
    {
		private static FluidCurrentSolver solver;
		public static FluidCurrentSolver Solver
		{
			get { return MainModelling.solver; }
		}

        public static void Main(string[] args)
        {
			var port = ProxyDataSet.Create("msds:nc?file=../../../temp.nc");
            NumericalParameters nPar = new NumericalParameters(0.01, 0.02, 0.02, 0.01, 40, 40, 50, 40, 150, 0.78, 1.4);
            solver = new FluidCurrentSolver(nPar);
            //DataSetFactory.Register(typeof(NetCDFDataSet));
			solver.SolveAll("msds:nc?file=../../../temp.nc");
            Console.WriteLine("Done!");
        }
    }
}
