using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public class VERoadServer : VEServerBase
	{
		public VERoadServer()
		{
			UriFormat = "http://r{0}.ortho.tiles.virtualearth.net/tiles/r{1}.png?g=275&mkt=en-us&shading=hill";
			ServerName = "Virtual Earth Road";
		}
	}
}
