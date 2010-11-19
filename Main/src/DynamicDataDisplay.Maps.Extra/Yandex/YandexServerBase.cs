using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.Yandex
{
	public abstract class YandexServerBase : NetworkTileServer
	{
		protected YandexServerBase()
		{
			Referer = "http://maps.yandex.ru/";
			ServersNum = 4;
			MinServer = 1;
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int x = index.X;
			int y = MapTileProvider.GetSideTilesCount(index.Level) - 1 - index.Y;
			int z = (int)index.Level;

			string uri = String.Format(UriFormat, x, y, z, CurrentServer);
			return uri;
		}
	}
}
