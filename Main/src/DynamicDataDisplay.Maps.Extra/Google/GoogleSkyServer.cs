using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
    public sealed class GoogleSkyServer : NetworkTileServer
    {
        public GoogleSkyServer()
        {
            UriFormat = "http://mw{0}.google.com/mw-planetary/sky/skytiles_v1/{1}_{2}_{3}.jpg";
            ServerName = "Google Sky";
            ServersNum = 2;
            MinServer = 1;
            FileExtension = ".jpg";
        }

        protected override string CreateRequestUriCore(TileIndex index)
        {
            var level = (int)index.Level;
            var y = MapTileProvider.GetSideTilesCount(level) / 2 - index.Y - 1;
            var x = MapTileProvider.GetSideTilesCount(level) / 2 + index.X;
            string uri = String.Format(UriFormat, CurrentServer, x, y, level);
            return uri;
        }
    }
}
