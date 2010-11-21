using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Diagnostics.Contracts;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.ColorMaps
{
	public sealed class ColorMapServer : SourceTileServer
	{
		private readonly IDataSource2D<double> dataSource = null;

		public ColorMapServer(IDataSource2D<double> dataSource)
		{
			Contract.Requires(dataSource != null);

			this.dataSource = dataSource;
		}

		public override bool Contains(TileIndex id)
		{
			throw new NotImplementedException();
		}

		public override void BeginLoadImage(TileIndex id)
		{
			throw new NotImplementedException();
		}
	}
}
