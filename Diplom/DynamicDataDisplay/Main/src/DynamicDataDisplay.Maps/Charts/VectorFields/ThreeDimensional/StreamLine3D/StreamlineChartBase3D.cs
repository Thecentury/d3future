using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;
using Petzold.Media3D;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows.Media;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class StreamlineChartBase3D : VectorChartModel3DBase
	{
		#region Properties

		#region LinesCount

		public int LinesCount
		{
			get { return (int)GetValue(LinesCountProperty); }
			set { SetValue(LinesCountProperty, value); }
		}

		public static readonly DependencyProperty LinesCountProperty = DependencyProperty.Register(
		  "LinesCount",
		  typeof(int),
		  typeof(StreamlineChartBase3D),
		  new FrameworkPropertyMetadata(50, OnLinesCountReplaced));

		private static void OnLinesCountReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StreamlineChartBase3D owner = (StreamlineChartBase3D)d;
			owner.RebuildUI();
		}

		#endregion // LinesCount

		#region Pattern

		public PointSetPattern3D Pattern
		{
			get { return (PointSetPattern3D)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}

		public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(
		  "Pattern",
		  typeof(PointSetPattern3D),
		  typeof(StreamlineChartBase3D),
		  new FrameworkPropertyMetadata(new RandomPattern3D(), OnPatternReplaced));

		private static void OnPatternReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StreamlineChartBase3D owner = (StreamlineChartBase3D)d;

			var prevValue = e.OldValue as PointSetPattern3D;
			if (prevValue != null)
				prevValue.Changed -= owner.OnPatternChanged;

			var currValue = e.NewValue as PointSetPattern3D;
			if (currValue != null)
				currValue.Changed += owner.OnPatternChanged;

			owner.RebuildUI();
		}

		private void OnPatternChanged(object sender, EventArgs e)
		{
			RebuildUI();
		}

		#endregion Pattern

		#region Bounds

		private Rect3D bounds = new Rect3D(new Point3D(-1, -1, -1), new Size3D(2, 2, 2));
		public Rect3D Bounds
		{
			get { return bounds; }
			set { bounds = value; }
		}

		#endregion Bounds

		#region Palette property

		private IPalette palette = new UniformLinearPalette(Colors.Red, Colors.GreenYellow, Colors.CornflowerBlue);
		public IPalette Palette
		{
			get { return palette; }
			set { palette = value; }
		}

		#endregion Palette property

		#region LineThickness

		private double lineThickness = 1;
		public double LineThickness
		{
			get { return lineThickness; }
			set { lineThickness = value; }
		}

		#endregion LineThickness

		#endregion Properties

		protected readonly ResourcePool<WirePolyline> linesPool = new ResourcePool<WirePolyline>();

		private WirePolyline CreatePolyline(IEnumerable<Point3D> points, Point3D start)
		{
			WirePolyline line = linesPool.GetOrCreate();

			double ratio = ((start.X - bounds.X) / bounds.SizeX + (start.Y - bounds.Y) / bounds.SizeY) / 2;

			line.Thickness = lineThickness;
			line.Color = palette.GetColor(ratio);

			line.Points = new Point3DCollection(points);

			return line;
		}

		protected UniformField3DWrapper fieldWrapper;
		protected int width;
		protected int height;
		protected int depth;
		protected void DrawLine(Point3D point)
		{
			// point was in cube [0..1]x[0..1]x[0..1], we are transforming it 
			// to cube [0..width]x[0..height]x[0..depth]

			Point3D start = point.TransformToBounds(bounds);

			int maxLength = 10 * Math.Max(width, Math.Max(height, depth));
			const int maxIterations = 400;
			Size3D boundsSize = new Size3D(1.0 / width, 1.0 / height, 1.0 / depth);

			Action<double, List<Point3D>> pointTracking = (direction, track) =>
			{
				var position01 = point;
				double length = 0;
				int i = 0;
				do
				{
					var K1 = fieldWrapper.GetVector(position01).DecreaseLength(boundsSize);
					var K2 = fieldWrapper.GetVector(position01 + (K1 / 2)).DecreaseLength(boundsSize);
					var K3 = fieldWrapper.GetVector(position01 + (K2 / 2)).DecreaseLength(boundsSize);
					var K4 = fieldWrapper.GetVector(position01 + K3).DecreaseLength(boundsSize);

					var shift = ((K1 + 2 * K2 + 2 * K3 + K4) / 6);
					if (shift.X.IsNaN() || shift.Y.IsNaN() || shift.Z.IsNaN())
						break;

					var next = position01 + direction * shift;
					Point3D viewportPoint = position01.TransformToBounds(bounds);
					track.Add(viewportPoint);

					position01 = next;
					length += shift.Length;
					i++;
				} while (length < maxLength && i < maxIterations);
			};

			var forwardTrack = new List<Point3D>();
			forwardTrack.Add(start);
			pointTracking(+1, forwardTrack);
			if (forwardTrack.Count > 1)
			{
				var forwardLine = CreatePolyline(forwardTrack, start);
				Children.Add(forwardLine);
			}

			var backwardTrack = new List<Point3D>();
			backwardTrack.Add(start);
			pointTracking(-1, backwardTrack);
			if (backwardTrack.Count > 1)
			{
				var backwardLine = CreatePolyline(backwardTrack, start);
				Children.Add(backwardLine);
			}
		}

		protected sealed override void RebuildUI()
		{
			linesPool.PutAll(Children.OfType<WirePolyline>());
			Children.Clear();

			if (DataSource == null)
				return;

			width = DataSource.Width;
			height = DataSource.Height;
			depth = DataSource.Depth;

			fieldWrapper = new UniformField3DWrapper(DataSource.Data, width, height, depth);

			Pattern.PointsCount = LinesCount;
			foreach (var point in Pattern.GeneratePoints())
			{
				DrawLine(point);
			}
		}
	}
}
