using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class DynamicStreamlineChart3D : StreamlineChartBase3D
	{
		public DynamicStreamlineChart3D()
		{
			LineThickness = 3;
			Pattern = new SinglePointPattern3D();
			Palette = new SingleColorPalette();
		}
	}
}
