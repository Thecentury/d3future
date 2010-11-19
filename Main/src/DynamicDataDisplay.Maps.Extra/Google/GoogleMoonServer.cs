using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public sealed class GoogleMoonServer : NetworkTileServer
	{
		public GoogleMoonServer()
		{
			ServerName = "Google Moon";
			FileExtension = ".jpg";
			ServersNum = 2;
			MinServer = 1;
			UriFormat = "http://mw{0}.google.com/mw-planetary/lunar/lunarmaps_v1/apollo/{1}/{2}/{3}.jpg";
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int level = (int)index.Level;
            var y = MapTileProvider.GetSideTilesCount(level) / 2 + index.Y;
            var x = MapTileProvider.GetSideTilesCount(level) / 2 + index.X;
			string uri = String.Format(UriFormat, CurrentServer, level, x, y);
			return uri;
		}
	}
}
