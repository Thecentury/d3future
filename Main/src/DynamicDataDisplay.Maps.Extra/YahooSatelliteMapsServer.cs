using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public sealed class YahooSatelliteMapsServer : YahooMapsServerBase
	{
		public YahooSatelliteMapsServer()
		{
			// todo determine servers number
			UriFormat = "http://us.maps3.yimg.com/aerial.maps.yimg.com/ximg?v=1.9&t=a&s=256&x={0}&y={1}&z={2}&r=1";
			FileExtension = ".jpg";
			ServerName = "Yahoo Satellite";
		}
	}
}
