using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network
{
	public sealed class VERoadMixedServer : VERoadServer
	{
		public VERoadMixedServer()
		{
			resourceServer.ImageLoaded += resourceServer_ImageLoaded;
		}

		private void resourceServer_ImageLoaded(object sender, TileLoadResultEventArgs e)
		{
			if (e.Result == TileLoadResult.Success)
				ReportSuccess(e.Image, e.ID);
			else
				ReportFailure(e.ID);
		}

		private readonly VERoadResourceServer resourceServer = new VERoadResourceServer();

		public override bool CanLoadFast(TileIndex id)
		{
			return resourceServer.Contains(id);
		}

		public override void BeginLoadImage(TileIndex id)
		{
			if (CanLoadFast(id))
				resourceServer.BeginLoadImage(id);
			else
				base.BeginLoadImage(id);
		}
	}
}
