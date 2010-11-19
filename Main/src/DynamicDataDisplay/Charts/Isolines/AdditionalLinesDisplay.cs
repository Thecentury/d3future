using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	public sealed class AdditionalLinesDisplay : IsolineGraphBase
	{
		protected override void OnPlotterAttached()
		{
			base.OnPlotterAttached();

			Plotter2D.Viewport.PropertyChanged += Viewport_PropertyChanged;
		}

		protected override void OnPlotterDetaching()
		{
			Plotter2D.Viewport.PropertyChanged -= Viewport_PropertyChanged;

			base.OnPlotterDetaching();
		}

		void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (Plotter2D == null) return;
			if (DataSource == null) return;
			if (Collection == null) return;
			if (Collection.Lines.Count == 0)
			{
				IsolineBuilder.DataSource = DataSource;
			}

			var dc = drawingContext;
			var dataSource = DataSource;
			var localMinMax = dataSource.GetMinMax();
			var globalMinMax = dataSource.Range.Value;
			double lengthsRatio = globalMinMax.GetLength() / localMinMax.GetLength();

			if (lengthsRatio > 16)
			{
				double log = Math.Round(Math.Log(lengthsRatio, 2));
				double number = 2 * Math.Pow(2, log);
				double delta = globalMinMax.GetLength() / number;

				double start = Math.Floor((localMinMax.Min - globalMinMax.Min) / delta) * delta + globalMinMax.Min;
				double end = localMinMax.Max;

				var transform = Plotter2D.Transform;
				var strokeThickness = StrokeThickness;

				double x = start;
				while (x < end)
				{
					var collection = IsolineBuilder.BuildIsoline(x);

					foreach (LevelLine line in collection)
					{
						StreamGeometry lineGeometry = new StreamGeometry();
						using (var context = lineGeometry.Open())
						{
							context.BeginFigure(line.StartPoint.ViewportToScreen(transform), false, false);
							context.PolyLineTo(line.OtherPoints.ViewportToScreen(transform).ToArray(), true, true);
						}
						lineGeometry.Freeze();

						var paletteRatio = (line.RealValue - globalMinMax.Min) / globalMinMax.GetLength();
						Pen pen = new Pen(new SolidColorBrush(Palette.GetColor(paletteRatio)), strokeThickness);

						dc.DrawGeometry(null, pen, lineGeometry);
					}

					x += delta;
				}
			}
			//dc.DrawRectangle(Brushes.Green.MakeTransparent(0.3), null, new Rect(RenderSize));
		}
	}
}
