using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	[Obsolete("Unready", true)]
	public sealed class GurtamServer : NetworkTileServer
	{
		public GurtamServer()
		{
			// todo determine servers num
			UriFormat = "http://t1.maps.gurtam.by/map_gmaps?n=404&x={0}&y={1}&zoom={2}";
			ServerName = "Gurtam";
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int y = MapTileProvider.GetSideTilesCount(index.Level) - 1 - index.Y;
			string uri = String.Format(UriFormat, index.X, y, (18 - index.Level));
			return uri;
		}
	}
}
