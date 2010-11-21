using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.ColorMaps
{
	public sealed class ColorMapBuilder
	{
		private IDataSource2D<double> dataSource;
		private DataRect visible;
		private IPalette palette;

		public IPalette Palette
		{
			get { return palette; }
			set { palette = value; }
		}

		public DataRect Visible
		{
			get { return visible; }
			set { visible = value; }
		}

		public IDataSource2D<double> DataSource
		{
			get { return dataSource; }
			set { dataSource = value; }
		}
	}
}
