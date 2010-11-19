using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public sealed class YahooMapsServer : YahooMapsServerBase
	{
		public YahooMapsServer()
		{
			ServersNum = 2;
			MinServer = 1;
			UriFormat = "http://us.maps{3}.yimg.com/us.tile.maps.yimg.com/tl?v=4.1&md=2&r=1&x={0}&y={1}&z={2}";
			ServerName = "Yahoo";
		}
	}
}
