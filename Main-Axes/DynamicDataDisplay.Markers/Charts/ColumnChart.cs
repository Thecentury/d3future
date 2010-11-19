using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Markers;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.MarkupExtensions;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class ColumnChart : ColumnChartBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ColumnChart"/> class.
		/// </summary>
		public ColumnChart()
		{
		}

		protected override void SetCommonBindings(FrameworkElement marker)
		{
			base.SetCommonBindings(marker);

			marker.SetValue(ViewportPanel.YProperty, 0.0);
			marker.SetBinding(ViewportPanel.ViewportHeightProperty, DependentValueBinding);
			marker.SetBinding(ViewportPanel.ViewportWidthProperty, ColumnWidthBinding);
			marker.SetValue(ViewportPanel.ViewportVerticalAlignmentProperty, VerticalAlignment.Bottom);
			marker.SetBinding(ViewportPanel.XProperty, IndexBinding);
		}
	}
}
