using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network
{
	[Obsolete("Unfinished", true)]
	public sealed class NasaBlueMarbleServer : NetworkTileServer
	{
		private string bmng = "http://worldwind25.arc.nasa.gov/tile/tile.aspx?T=bmng.topo.bathy.200404&L={0}&X={1}&Y={2}";
		private string other = "http://worldwind25.arc.nasa.gov/tile/tile.aspx?T=105&L={0}&X={1}&Y={2}";
		public NasaBlueMarbleServer()
		{
			FileExtension = ".jpg";
			ServerName = "Nasa Blue Marble";
		}

		public override bool Contains(TileIndex id)
		{
			return id.Level >= 3 && base.Contains(id);
		}

		protected override string CreateRequestUriCore(TileIndex index)
		{
			int level = (int)index.Level;
			string uriFormat;
			if (index.Level <= 5)
			{
				uriFormat = bmng;
			}
			else
			{
				level -= 2;
				uriFormat = other;
			}

			string uri = String.Format(uriFormat, level, index.X, index.Y);
			return uri;
		}
	}
}
