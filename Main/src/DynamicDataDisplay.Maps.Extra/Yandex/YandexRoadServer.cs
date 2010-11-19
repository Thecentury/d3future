using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.Yandex
{
	public sealed class YandexRoadServer : YandexServerBase
	{
		public YandexRoadServer()
		{
			UriFormat = "http://vec0{3}.maps.yandex.net/tiles?l=map&v=2.3.2&x={0}&y={1}&z={2}";
			ServerName = "Yandex";
		}
	}
}
