using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace ParallelBenchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			Func<int, int, double> func = ((ix, iy) => ix * iy);

			const int width = 1000;
			const int height = 1000;

			using (new DisposableTimer("sequential #1"))
			{
				var grid = func.ToGrid(width, height);
			}

			using (new DisposableTimer("sequential #2"))
			{
				var grid = func.ToGrid(width, height);
			}


			Console.ReadLine();
		}
	}
}
