using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class PointSetUpdateHandler : DefaultUpdateHandler
	{
		private DataRect increasedVisible;
		private PointChartBase chart;

		public PointSetUpdateHandler()
		{
		}

		public override void OnPlotterAttached(Plotter2D plotter, PointChartBase chart)
		{
			this.chart = chart;
			increasedVisible = plotter.Visible.ZoomOutFromCenter(2.0);
			plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;
		}

		public override void OnPlotterDetached(Plotter2D plotter, PointChartBase chart)
		{
			plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
			this.chart = null;
		}

		public override void OnUpdate(NotifyCollectionChangedEventArgs e, PointChartBase chart)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
					if (CheckDataInUpdateBounds(e, chart))
					{
						base.OnUpdate(e, chart);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					base.OnUpdate(e, chart);
					break;
				default:
					break;
			}
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				DataRect currVisible = (DataRect)e.NewValue;
				if (increasedVisible.Contains(currVisible))
				{
					var increasedSquare = increasedVisible.GetSquare();
					if (increasedSquare > 0)
					{
						var squareRatio = increasedSquare / currVisible.GetSquare();
						if (2 < squareRatio && squareRatio < 6)
						{
							// keeping old value of increasedVisible
							return;
						}
					}
				}

				increasedVisible = currVisible.ZoomOutFromCenter(2.0);
				chart.OnReset();
			}
			else if (e.PropertyName == "Output")
			{
				chart.OnReset();
			}
		}

		private bool CheckDataInUpdateBounds(NotifyCollectionChangedEventArgs e, PointChartBase chart)
		{
			var dataToPoint = chart.DataSource.DataToPoint;
			if (dataToPoint == null)
				return true;

			if (e.NewItems != null)
			{
				foreach (var dataItem in e.NewItems)
				{
					var point = dataToPoint(dataItem);
					if (increasedVisible.Contains(point))
						return true;
				}
			}
			if (e.OldItems != null)
			{
				foreach (var dataItem in e.OldItems)
				{
					var point = dataToPoint(dataItem);
					if (increasedVisible.Contains(point))
						return true;
				}
			}

			return false;
		}
	}
}
