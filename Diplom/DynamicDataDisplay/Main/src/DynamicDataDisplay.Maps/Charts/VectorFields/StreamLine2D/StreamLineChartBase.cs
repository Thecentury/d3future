using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<System.Windows.Vector>;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media.Effects;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.StreamLine2D.Filters;
using Microsoft.Research.DynamicDataDisplay.Maps.Auxiliary.Extensions;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Streamlines
{
	public abstract class StreamLineChartBase : VectorFieldChartBase
	{
		protected StreamLineChartBase()
		{
			lineEffectBinding = new Binding { Path = new PropertyPath("LineEffect"), Source = this };
		}

		#region LineEffect property

		public Effect LineEffect
		{
			get { return (Effect)GetValue(LineEffectProperty); }
			set { SetValue(LineEffectProperty, value); }
		}

		public static readonly DependencyProperty LineEffectProperty = DependencyProperty.Register(
		  "LineEffect",
		  typeof(Effect),
		  typeof(StreamLineChartBase),
		  new FrameworkPropertyMetadata(new DropShadowEffect { Color = Colors.Black, Direction = 245, BlurRadius = 3 }));

		#endregion

		#region ApplyLineEffect property

		public bool ApplyLineEffect
		{
			get { return (bool)GetValue(ApplyLineEffectProperty); }
			set { SetValue(ApplyLineEffectProperty, value); }
		}

		public static readonly DependencyProperty ApplyLineEffectProperty = DependencyProperty.Register(
		  "ApplyLineEffect",
		  typeof(bool),
		  typeof(StreamLineChartBase),
		  new FrameworkPropertyMetadata(true, OnApplyLineEffectReplaced));

		private static void OnApplyLineEffectReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StreamLineChartBase owner = (StreamLineChartBase)d;
			bool value = (bool)e.NewValue;
			if (value)
				owner.LineEffect = new DropShadowEffect { Color = Colors.Black, Direction = 245, BlurRadius = 3 };
			else
				owner.LineEffect = null;
		}

		#endregion

		#region LinesCount property

		public int LinesCount
		{
			get { return (int)GetValue(LinesCountProperty); }
			set { SetValue(LinesCountProperty, value); }
		}

		public static readonly DependencyProperty LinesCountProperty = DependencyProperty.Register(
		  "LinesCount",
		  typeof(int),
		  typeof(StreamLineChartBase),
		  new FrameworkPropertyMetadata(50, OnLinesCountReplaced));

		private static void OnLinesCountReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StreamLineChartBase owner = (StreamLineChartBase)d;
			owner.RebuildUI();
		}

		#endregion LinesCount property

		#region LineStroke property

		public Brush LineStroke
		{
			get { return (Brush)GetValue(LineStrokeProperty); }
			set { SetValue(LineStrokeProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeProperty = DependencyProperty.Register(
		  "LineStroke",
		  typeof(Brush),
		  typeof(StreamLineChartBase),
		  new FrameworkPropertyMetadata(Brushes.Orange));

		#endregion LineStroke property

		#region LineLengthFactor property

		public double LineLengthFactor
		{
			get { return (double)GetValue(LineLengthFactorProperty); }
			set { SetValue(LineLengthFactorProperty, value); }
		}

		public static readonly DependencyProperty LineLengthFactorProperty = DependencyProperty.Register(
		  "LineLengthFactor",
		  typeof(double),
		  typeof(StreamLineChartBase),
		  new FrameworkPropertyMetadata(10.0));

		#endregion LineLengthFactor property

		#region LineThickness property

		public double LineThickness
		{
			get { return (double)GetValue(LineThicknessProperty); }
			set { SetValue(LineThicknessProperty, value); }
		}

		public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(
		  "LineThickness",
		  typeof(double),
		  typeof(StreamLineChartBase),
		  new FrameworkPropertyMetadata(1.0));

		#endregion LineThickness property

		protected int width;
		protected int height;
		protected DataRect bounds;
		protected UniformField2DWrapper fieldWrapper;
		protected override void RebuildUI()
		{
			if (Plotter == null)
				return;
			if (DataSource == null)
				return;

			width = DataSource.Width;
			height = DataSource.Height;
			fieldWrapper = new UniformField2DWrapper(DataSource.Data);

			bounds = DataSource.GetGridBounds();

			polylinesPool.PutAll(panel.Children.OfType<Polyline>());
			panel.Children.Clear();

			RebuildUICore();
		}

		protected abstract void RebuildUICore();

		private readonly Binding lineEffectBinding;
		private ResourcePool<Polyline> polylinesPool = new ResourcePool<Polyline>();
		private readonly FrequencyFilter frequencyFilter = new FrequencyFilter();
		protected Polyline CreateLine(IEnumerable<Point> track)
		{
			Polyline line;
			if (polylinesPool.IsEmpty)
			{
				line = new Polyline
				{
					Stroke = LineStroke,
					StrokeThickness = LineThickness,
					Stretch = Stretch.Fill,
					IsHitTestVisible = false
				};
			}
			else
			{
				line = polylinesPool.Get();

				line.Stroke = LineStroke;
				line.StrokeThickness = LineThickness;
				line.Stretch = Stretch.Fill;
				line.IsHitTestVisible = false;
			}

			line.Points = new PointCollection(track.DataToScreen(Plotter.Transform).Filter(frequencyFilter));

			// todo should I remove this binding when clearing panel's children?
			line.SetBinding(EffectProperty, lineEffectBinding);

			return line;
		}

		private Point PointToViewport(Point p)
		{
			Point result = new Point();
			result.X = p.X * bounds.Width + bounds.XMin;
			result.Y = p.Y * bounds.Height + bounds.YMin;

			return result;
		}

		protected void DrawLine(Point point)
		{
			double maxLength = LineLengthFactor * Math.Max(DataSource.Width, DataSource.Height);
			const int maxIterations = 300;

			Action<double, List<Point>> pointTracking = (directionSign, track) =>
			{
				int i = 0;
				var position = point;
				double length = 0;
				bool finished = false;
				do
				{
					double x = position.X;
					double y = position.Y;
					if (x < 0 || x > 1 || y < 0 || y > 1 || x.IsNaN() || y.IsNaN())
						break;

					double xFactor = 1.0 / width;
					double yFactor = 1.0 / height;

					var K1 = fieldWrapper.GetVector(position).ChangeLength(xFactor, yFactor);
					var K2 = fieldWrapper.GetVector(position + K1 / 2).ChangeLength(xFactor, yFactor);
					var K3 = fieldWrapper.GetVector(position + K2 / 2).ChangeLength(xFactor, yFactor);
					var K4 = fieldWrapper.GetVector(position + K3).ChangeLength(xFactor, yFactor);

					var shift = (K1 + 2 * K2 + 2 * K3 + K4) / 6;
					if (shift.X.IsNaN() || shift.Y.IsNaN())
						break;

					//shift /= 10;
					Point next = position + directionSign * shift;
					Point viewportPoint = PointToViewport(next);
					track.Add(viewportPoint);

					position = next;
					length += shift.Length;
					i++;

					finished = !(length < maxLength && i < maxIterations);
				} while (!finished);
			};

			var forwardTrack = new List<Point>();
			forwardTrack.Add(PointToViewport(point));
			pointTracking(+1, forwardTrack);
			Polyline forwardLine = CreateLine(forwardTrack);
			ViewportPanel.SetViewportBounds(forwardLine, forwardTrack.GetBounds());
			if (forwardTrack.Count > 1)
				panel.Children.Add(forwardLine);

			var backwardTrack = new List<Point>();
			backwardTrack.Add(PointToViewport(point));
			pointTracking(-1, backwardTrack);
			var backwardLine = CreateLine(backwardTrack);
			ViewportPanel.SetViewportBounds(backwardLine, backwardTrack.GetBounds());
			if (backwardTrack.Count > 1)
				panel.Children.Add(backwardLine);
		}
	}
}
