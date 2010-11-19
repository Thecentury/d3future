using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	public sealed class VEAerialServer : VEServerBase
	{
		public VEAerialServer()
		{
			UriFormat = "http://r{0}.ortho.tiles.virtualearth.net/tiles/a{1}.png?g=275&mkt=en-us&shading=hill";
			ServerName = "Virtual Earth Aerial";
		}
	}
}
