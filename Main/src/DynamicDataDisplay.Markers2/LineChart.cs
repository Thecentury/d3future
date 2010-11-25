using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay.Markers.Extensions;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	public class LineChart : LineChartBase
	{
		private readonly List<Shape> shapes = new List<Shape>();

		protected override void OnDataSourceReplaced(PointDataSourceBase oldDataSource, PointDataSourceBase newDataSource)
		{
			base.OnDataSourceReplaced(oldDataSource, newDataSource);

			CreateUIRepresentation();
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			CreateUIRepresentation();
		}

		private void CreateUIRepresentation()
		{
			if (Plotter == null)
				return;

			if (DataSource == null)
				return;

			DataSourceEnvironment environment = CreateEnvironment();
			var points = DataSource.GetPoints(environment);

			// do nothing if there is nothing to draw
			if (!points.Any())
				return;

			var screenPoints = points.DataToScreen(Plotter.Viewport.Transform).ToList();

			StreamGeometry geometry = new StreamGeometry();
			using (var context = geometry.Open())
			{
				context.BeginFigure(screenPoints[0], isFilled: false, isClosed: false);
				context.PolyLineTo(screenPoints, isStroked: true, isSmoothJoin: false);
			}

			Path path = new Path { Stroke = ColorHelper.RandomBrush, StrokeThickness = 3 };
			path.Data = geometry;

			Plotter.CentralGrid.Children.Add(path);
		}
	}
}
