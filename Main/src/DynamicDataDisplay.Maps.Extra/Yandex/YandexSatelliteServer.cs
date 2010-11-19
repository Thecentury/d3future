using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.Yandex
{
	public sealed class YandexSatelliteServer : YandexServerBase
	{
		public YandexSatelliteServer()
		{
			UriFormat = "http://sat0{3}.maps.yandex.net/tiles?l=sat&v=1.6.0&x={0}&y={1}&z={2}";
			FileExtension = ".jpg";
			ServerName = "Yandex Satellite";
		}
	}
}
