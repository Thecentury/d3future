using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network;

namespace DynamicDataDisplay.Tests.Maps
{
	[TestClass]
	public class NetworkTileServerTest
	{
		[TestMethod]
		public void GCCollectsTileServer()
		{
			var server = new OpenStreetMapServer();
			WeakReference reference = new WeakReference(server);
			server.Dispose();
			server = null;

			GC.Collect(2);
			GC.WaitForPendingFinalizers();
			GC.Collect(2);

			Assert.IsFalse(reference.IsAlive);
		}
	}
}
