using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public sealed class WikiMapiaServer : NetworkTileServer
	{
		public WikiMapiaServer()
		{
			// todo determine servers number
			UriFormat = "http://p1.wikimapia.org/?lng=1&x={0}&y={1}&zoom={2}";
			ServerName = "WikiMapia";
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int x = index.X;
			int y = MapTileProvider.GetSideTilesCount(index.Level) - 1 - index.Y;
			int z = (int)index.Level;
			string uri = String.Format(UriFormat, x, y, z);
			return uri;
		}
	}
}
