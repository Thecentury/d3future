using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	public abstract class LineChartBase : PointChartBase
	{
		#region Stroke property

		/// <summary>
		/// Gets or sets the stroke of lines. This is a DependencyProperty.
		/// </summary>
		/// <value>The stroke.</value>
		public Brush Stroke
		{
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
		  "Stroke",
		  typeof(Brush),
		  typeof(LineChartBase),
		  new FrameworkPropertyMetadata(Brushes.Red));

		#endregion

		#region StrokeThickness property

		/// <summary>
		/// Gets or sets the stroke thickness of lines. This is a DependencyProperty.
		/// </summary>
		/// <value>The stroke thickness.</value>
		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
		  "StrokeThickness",
		  typeof(double),
		  typeof(LineChartBase),
		  new FrameworkPropertyMetadata(1.0));
		
		#endregion
	}
}
