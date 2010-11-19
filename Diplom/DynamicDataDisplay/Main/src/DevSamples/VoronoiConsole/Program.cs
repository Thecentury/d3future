using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi;
using System.Windows;

namespace VoronoiConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			VoronoiMain.main(new Point[] { new Point(-1, -1), new Point(1, -1)
				, new Point(-1, 1), new Point(1, 1) 
			});

			Console.ReadLine();
		}
	}
}
