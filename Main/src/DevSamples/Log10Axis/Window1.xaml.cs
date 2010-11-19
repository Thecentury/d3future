using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric;

namespace Log10Axis
{
    /// <summary>
    /// Interaction logic for D3LineChart.xaml
    /// </summary>
	public partial class Window1 : Window
	{
		private AxisInfo _xAxisInfo;
		private IList<AxisInfo> _yAxisInfoList;
		private IList<AxisInfo> _yAxisInfoSecList;

		private string _chartTitle = string.Empty;
		public string ChartTitle
		{
			get { return _chartTitle; }
			set { _chartTitle = value; }
		}

		public Window1()
		{
			InitializeComponent();
		}

		public Window1(AxisInfo xAxisInfo, IList<AxisInfo> yAxisInfoList)
			: this()
		{
			_xAxisInfo = xAxisInfo;
			_yAxisInfoList = yAxisInfoList;

			if (_xAxisInfo != null && _yAxisInfoList != null)
			{
				DrawPlots();
			}
		}

		public Window1(AxisInfo xAxisInfo, IList<AxisInfo> yAxisInfoList, IList<AxisInfo> yAxisInfoSecList)
		{
			InitializeComponent();
			_xAxisInfo = xAxisInfo;
			_yAxisInfoList = yAxisInfoList;
			_yAxisInfoSecList = yAxisInfoSecList;

			if (_xAxisInfo != null && _yAxisInfoList != null)
			{
				DrawPlots();
			}
		}

		private void DrawPlots()
		{
			HorizontalAxis xAxis = (HorizontalAxis)plotter.MainHorizontalAxis;
			xAxis.TicksProvider = new LogarithmNumericTicksProvider(10);
			xAxis.LabelProvider = new UnroundingLabelProvider();
			xAxis.ShowMajorLabels = true;
			xAxis.ShowMinorTicks = true;
			xAxis.SnapsToDevicePixels = true;

			xAxis.LabelProvider.SetCustomFormatter(info => info.Tick.ToString("#.######e0"));

			HorizontalAxisTitle HAT = new HorizontalAxisTitle()
			{
				Content = _xAxisInfo.AxisTitle
			};

			plotter.MainHorizontalAxis = xAxis;
			plotter.Children.Add(HAT);
			((NumericAxis)plotter.MainHorizontalAxis).AxisControl.TicksPath.Stroke = Brushes.Blue;

			VerticalAxis yAxis = (VerticalAxis)plotter.MainVerticalAxis;
			yAxis.TicksProvider = new NumericTicksProvider();
			yAxis.LabelProvider = new UnroundingLabelProvider();
			yAxis.ShowMajorLabels = true;
			yAxis.ShowMinorTicks = true;
			yAxis.SnapsToDevicePixels = true;


			VerticalAxisTitle VAT = new VerticalAxisTitle()
			{
				Content = _yAxisInfoList[0].AxisTitle
			};

			plotter.MainVerticalAxis = yAxis;
			plotter.Children.Add(VAT);
			plotter.AxisGrid.DrawVerticalMinorTicks = true;
			plotter.AxisGrid.DrawHorizontalMinorTicks = true;
			plotter.MainVerticalAxis.Background = new LinearGradientBrush(Colors.White, Colors.LightGray, 90);

			var xPoints = _xAxisInfo.AxisDataPoints.AsXDataSource();

			// Create the main plot
			foreach (AxisInfo yAxInfo in _yAxisInfoList)
			{
				var yPoints = yAxInfo.AxisDataPoints.AsYDataSource();

				CompositeDataSource plot = xPoints.Join(yPoints);
				plotter.AddLineGraph(plot, yAxInfo.PlotColor, yAxInfo.PlotLineThickness, yAxInfo.AxisLegend);
			}


			// add secondary y-axis plots if any exist
			if (_yAxisInfoSecList != null)
			{
				InjectedPlotter innerPlotter = new InjectedPlotter();
				innerPlotter.SetViewportBinding = false;
				plotter.Children.Add(innerPlotter);

				HorizontalAxis ax = new HorizontalAxis();
				ax.Placement = AxisPlacement.Top;
				ax.TicksProvider = new LogarithmNumericTicksProvider(10);
				ax.LabelProvider = new UnroundingLabelProvider();
				ax.LabelProvider.SetCustomFormatter(info => info.Tick.ToString("#.######e0"));
				ax.ShowMajorLabels = true;
				ax.ShowMinorTicks = true;
				ax.SnapsToDevicePixels = true;
				ax.AxisControl.TicksPath.Stroke = Brushes.Red;
				plotter.Children.Add(ax);


				VerticalAxis yAxisSec = new VerticalAxis()
				{
					TicksProvider = new NumericTicksProvider(),
					LabelProvider = new UnroundingLabelProvider(),
					ShowMinorTicks = true,
					ShowMajorLabels = true,
					SnapsToDevicePixels = true,
					Placement = AxisPlacement.Right
				};

				VerticalAxisTitle VATsecondary = new VerticalAxisTitle()
				{
					Content = _yAxisInfoSecList[0].AxisTitle,
					Placement = AxisPlacement.Right
				};

				innerPlotter.MainVerticalAxis = yAxisSec;
				innerPlotter.Children.Add(VATsecondary);
				innerPlotter.MainVerticalAxis.Background = new LinearGradientBrush(Colors.White, Colors.Red, 90);

				foreach (AxisInfo yAxInfoSec in _yAxisInfoSecList)
				{
					var ySecPoints = yAxInfoSec.AxisDataPoints.AsYDataSource();

					CompositeDataSource plotSec = xPoints.Join(ySecPoints);
					/*innerP*/
					plotter.AddLineGraph(plotSec, yAxInfoSec.PlotColor, yAxInfoSec.PlotLineThickness, yAxInfoSec.AxisLegend);
				}

			}
		}
	}

    public class AxisInfo
    {
        public string AxisTitle;
        public string AxisLegend;
        public double[] AxisDataPoints;
        public Color PlotColor = Colors.Black;
        public int PlotLineThickness = 1;
    }
}