using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
    public class GoogleInfraredSkyServer : NetworkTileServer
    {
        public GoogleInfraredSkyServer()
        {
            ServerName = "Google Sky Infrared";
            FileExtension = ".png";
            UriFormat = "http://mw1.google.com/mw-planetary/sky/mapscontent_v1/overlayTiles/iras/zoom{0}/iras_{1}_{2}.png";
            MinLevel = 2;
        }

        protected override string CreateRequestUriCore(TileIndex index)
        {
            var level = (int)index.Level;
            var y = MapTileProvider.GetSideTilesCount(level) / 2 - index.Y - 1;
            var x = MapTileProvider.GetSideTilesCount(level) / 2 + index.X;
            string uri = String.Format(UriFormat, level, x, y);
            return uri;
        }
    }
}
