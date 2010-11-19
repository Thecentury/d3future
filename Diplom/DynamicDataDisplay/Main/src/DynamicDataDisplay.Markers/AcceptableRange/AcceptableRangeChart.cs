using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class AcceptableRangeChart : MarkerChart
	{
		#region DataSource

		#region YMinMapping property

		public string YMinMapping
		{
			get { return (string)GetValue(YMinMappingProperty); }
			set { SetValue(YMinMappingProperty, value); }
		}

		public static readonly DependencyProperty YMinMappingProperty = DependencyProperty.Register(
			"YMinMapping",
			typeof(string),
			typeof(AcceptableRangeChart),
			new FrameworkPropertyMetadata(null, OnYMappingChanged));

		private static void OnYMappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AcceptableRangeChart chart = (AcceptableRangeChart)d;
			// todo
		}

		#endregion // end of YMinMapping

		#endregion

	}
}
