using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.MailRu
{
	public abstract class MailRuServerBase : NetworkTileServer
	{
		protected MailRuServerBase()
		{
			Referer = @"http://maps.mail.ru/";
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int x = index.X - MapTileProvider.GetSideTilesCount(index.Level) / 2;
			int y = index.Y - MapTileProvider.GetSideTilesCount(index.Level) / 2;
			int z = (int)index.Level;

			string uri = String.Format(UriFormat, x, y, z, CurrentServer);
			return uri;
		}
	}
}
