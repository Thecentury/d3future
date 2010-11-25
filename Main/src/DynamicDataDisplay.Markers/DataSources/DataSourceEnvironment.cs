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
		public Rect Output { get; set; }
		public DataRect Visible { get; set; }
		public CoordinateTransform Transform { get; set; }

		public DataRect? ContentBounds { get; set; }
	}
}
