using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public abstract class YahooMapsServerBase : NetworkTileServer
	{
		// todo if server returned too short response, treat it as failure
		protected YahooMapsServerBase()
		{
			Referer = "http://maps.yahoo.com/";
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int x = index.X;
			int y = index.Y - MapTileProvider.GetSideTilesCount(index.Level) / 2;
			int z = (int)(index.Level + 1);

			string uri = String.Format(UriFormat, x, y, z, CurrentServer);
			return uri;
		}
	}
}
