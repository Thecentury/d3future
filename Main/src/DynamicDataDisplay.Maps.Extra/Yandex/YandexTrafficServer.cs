using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;
using System.Net;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.Yandex
{
	public class YandexTrafficServer : YandexServerBase
	{
		public YandexTrafficServer()
		{
			ServerName = "Yandex Traffic";
			UriFormat = @"http://trf.maps.yandex.net/tiles?l=trf&x={0}&y={1}&z={2}&tm=#3#";
			ServersNum = 1;
			MinServer = 0;
		}

		protected override void AdjustRequest(WebRequest request)
		{
			request.Headers.Add(HttpRequestHeader.CacheControl, "0");
			HttpWebRequest httpRequest = (HttpWebRequest)request;
			Referer = "http://maps.yandex.ru/?ll=37.612161%2C55.742872&z=10&l=trf";
			base.AdjustRequest(request);
		}

		public readonly static int UpdateDelta = 240; // seconds
		public readonly static int MagicSeconds = 1248787201;
		private readonly static DateTime unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0);
		private TimeSpan utcDifference = DateTime.UtcNow - DateTime.Now;

		protected override string CreateRequestUriCore(TileIndex index)
		{
			var uri = base.CreateRequestUriCore(index);

			DateTime now = TrafficTime;

			int oldSeconds = GetRoundedJamsTime(now);

			uri = uri.Replace("#3#", oldSeconds.ToString());
			return uri;
		}

		public int GetRoundedJamsTime(DateTime now)
		{
			now += utcDifference;

			int seconds = (int)Math.Round((now - unixStartTime).TotalSeconds);
			int oldSeconds = MagicSeconds + ((seconds - MagicSeconds) / UpdateDelta) * UpdateDelta;
			return oldSeconds;
		}

		private DateTime trafficTime;
		public DateTime TrafficTime
		{
			get { return trafficTime; }
			set
			{
				trafficTime = value;
				RaiseChangedEvent();
			}
		}
	}
}
