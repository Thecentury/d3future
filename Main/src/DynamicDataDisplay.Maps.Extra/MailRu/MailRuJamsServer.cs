using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.MailRu
{
	public class MailRuJamsServer : MailRuServerBase
	{
		public MailRuJamsServer()
		{
			UriFormat = "http://maps.mail.ru/TileSender.aspx?ModeKey=tiles&t=Jams&z={2}&x={0}&y={1}&478574";  // 758850
			FileExtension = ".png";
			ServerName = "Mail.Ru Jams Maps";
		}
	}
}
