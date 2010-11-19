using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public class ETopoServer : NetworkTileServer
	{
		public ETopoServer()
		{
			ServerName = "ETopo";
			UriFormat = "http://ngdc.noaa.gov/mgg/global/relief/ETOPO1/tiled/ice_surface/{0}/{1}/{2}.jpeg";
			FileExtension = ".jpg";

			MinLevel = 1;
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int level = (int)index.Level;
			int x = index.X;
			int y = index.Y;

			return String.Format(UriFormat, level.ToString(), x.ToString(), y.ToString());
		}
	}
}
