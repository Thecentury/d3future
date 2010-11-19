using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media.Effects;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using System.Windows.Markup;
using System.Windows.Threading;


namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Streamlines
{
	[ContentProperty("Pattern")]
	public class StreamLineChart : StreamLineChartBase
	{
		static StreamLineChart()
		{
			LineLengthFactorProperty.OverrideMetadata(typeof(StreamLineChart), new FrameworkPropertyMetadata(3.0));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StreamLineChart"/> class.
		/// </summary>
		public StreamLineChart()
		{

		}

		private PointSetPattern pattern = new XPattern();
		public PointSetPattern Pattern
		{
			get { return pattern; }
			set { pattern = value; }
		}


		protected override void RebuildUICore()
		{
			Pattern.PointsCount = LinesCount;
			foreach (var point in Pattern.GeneratePoints())
			{
				Point p = point;
				Dispatcher.BeginInvoke(() =>
				{
					DrawLine(p);
				}, DispatcherPriority.Background);
			}
		}
	}
}
