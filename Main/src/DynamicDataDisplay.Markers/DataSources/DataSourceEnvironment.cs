using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources
{
	public class DataSourceEnvironment
	{
		public Rect Output { get; internal set; }
		public DataRect Visible { get; internal set; }
		public CoordinateTransform Transform { get; internal set; }

		public DataRect? ContentBounds { get; set; }
	}
}
