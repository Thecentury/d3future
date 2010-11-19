using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Media;

namespace DynamicDataDisplay.Markers
{
	public class BarChart : MarkerChart
	{
		static BarChart()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarChart), new FrameworkPropertyMetadata(typeof(BarChart)));
		}

		public BarChart()
		{
			PropertyMappings[DependentValuePathProperty] = ViewportPanel.ViewportWidthProperty;
			MarkerBuilder = new TemplateMarkerGenerator();
		}
	}
}
