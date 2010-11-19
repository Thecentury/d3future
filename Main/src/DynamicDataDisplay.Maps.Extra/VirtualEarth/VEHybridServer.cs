using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public class VEHybridServer : VEServerBase
	{
		public VEHybridServer()
		{
			UriFormat = "http://h{0}.ortho.tiles.virtualearth.net/tiles/h{1}.jpeg?g=275&mkt=en-us";
			FileExtension = ".jpg";
			ServerName = "Virtual Earth Hybrid";
		}
	}
}
