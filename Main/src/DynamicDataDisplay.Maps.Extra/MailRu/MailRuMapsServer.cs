using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.MailRu
{
	public class MailRuMapsServer : MailRuServerBase
	{
		public MailRuMapsServer()
		{
			UriFormat = "http://maps.mail.ru/TileSender.aspx?ModeKey=tiles&t=Maps&z={2}&x={0}&y={1}";
			FileExtension = ".png";
			ServerName = "Mail.Ru Maps";
		}
	}
}
