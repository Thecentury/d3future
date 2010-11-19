using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.MarkupExtensions;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class HorizontalColumnChart : ColumnChartBase
	{
		public HorizontalColumnChart()
		{
		}

		protected override void SetCommonBindings(FrameworkElement marker)
		{
			base.SetCommonBindings(marker);

			marker.SetValue(ViewportPanel.XProperty, 0.0);
			marker.SetBinding(ViewportPanel.ViewportWidthProperty, DependentValueBinding);
			marker.SetBinding(ViewportPanel.ViewportHeightProperty, ColumnWidthBinding);
			marker.SetValue(ViewportPanel.ViewportHorizontalAlignmentProperty, HorizontalAlignment.Left);
			marker.SetBinding(ViewportPanel.YProperty, IndexBinding);
		}
	}
}
