using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public abstract class VEServerBase : NetworkTileServer
	{
		protected VEServerBase()
		{
			ServersNum = 4;
			Referer = "http://maps.live.com/";
			MaxLatitude = 85.28799;
			XCycling = true;
		}

		protected sealed override string CreateRequestUriCore(TileIndex index)
		{
			string indexString = CreateTileIndexString(index);

			string res = String.Format(UriFormat, CurrentServer, indexString);
			return res;
		}

		protected override bool IsGoodTileResponse(WebResponse response)
		{
			if (response.Headers.AllKeys.Contains("X-VE-Tile-Info"))
			{
				return false;
			}
			return base.IsGoodTileResponse(response);
		}

		private string CreateTileIndexString(TileIndex index)
		{
			StringBuilder builder = new StringBuilder();

			checked
			{
				int shift = (int)Math.Pow(2, index.Level - MinLevel);
				int x = index.X + shift;
				int y = index.Y + shift;

				for (int level = MinLevel; level <= index.Level; level++)
				{
					char ch = '0';
					int halfTilesNum = (int)Math.Pow(2, index.Level - level);
					if ((x & halfTilesNum) != 0)
						ch += (char)1;
					if ((y & halfTilesNum) == 0)
						ch += (char)2;
					builder.Append(ch);
				}
			}

			return builder.ToString();
		}
	}
}
