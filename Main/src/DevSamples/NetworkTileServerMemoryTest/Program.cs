using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network;

namespace NetworkTileServerMemoryTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ReadLine();

			var server = new OpenStreetMapServer();
			WeakReference reference = new WeakReference(server);
			server.Dispose();
			server = null;

			GC.Collect(2);
			GC.WaitForPendingFinalizers();
			GC.Collect(2);

			bool alive = reference.IsAlive;

			Console.WriteLine(alive);
			Console.ReadLine();
		}
	}
}
