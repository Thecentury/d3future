using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class FuncExtensions
	{
		private const int sizeToUseParallel = 1000;

		public static T[,] ToGrid<T>(this Func<int, int, T> func, int width = 100, int height = 100)
		{
			if (func == null)
				throw new ArgumentNullException("func");
			if (width < 2)
				throw new ArgumentOutOfRangeException("width");
			if (height < 2)
				throw new ArgumentOutOfRangeException("height");


			T[,] result = new T[width, height];

			if (width < sizeToUseParallel)
			{
				for (int ix = 0; ix < width; ix++)
				{
					for (int iy = 0; iy < height; iy++)
					{
						result[ix, iy] = func(ix, iy);
					}
				}
			}
			else
			{
				Parallel.For(0, width, ix =>
				{
					for (int iy = 0; iy < height; iy++)
					{
						result[ix, iy] = func(ix, iy);
					}
				});
			}

			return result;
		}
	}
}
