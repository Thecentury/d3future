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


namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Streamlines
{
	public sealed class StreamLineChart : FrameworkElement, IPlotterElement
	{
		private readonly ViewportHostPanel panel = new ViewportHostPanel();

		#region Properties

		public IDataSource2D<Vector> DataSource
		{
			get { return (IDataSource2D<Vector>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource2D<Vector>),
		  typeof(StreamLineChart),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			StreamLineChart owner = (StreamLineChart)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(IDataSource2D<Vector> prevDataSource, IDataSource2D<Vector> currDataSource)
		{
			if (prevDataSource != null)
				prevDataSource.Changed -= DataSource_OnChanged;
			if (currDataSource != null)
				currDataSource.Changed += DataSource_OnChanged;

			RebuildUI();
		}

		private void DataSource_OnChanged(object sender, EventArgs e)
		{
			RebuildUI();
		}

		private DataRect bounds;
		private const int occupiedTableWidth = 1000;
		private const int occupiedTableHeight = 1000;
		private int[,] occupiedTable = new int[occupiedTableWidth, occupiedTableHeight];
		private UniformFieldWrapper fieldWrapper;
		private readonly Random rnd = new Random();
		private const int pointsNum = 100;
		private void RebuildUI()
		{
			if (DataSource == null)
			{
				//todo: clear
				return;
			}

			panel.Children.Clear();

			int width = DataSource.Width;
			int height = DataSource.Height;
			bounds = DataSource.Grid.GetGridBounds();
			fieldWrapper = new UniformFieldWrapper(DataSource.Data, width, height);

			List<Point[]> tracks = new List<Point[]>(pointsNum);
			for (int i = 0; i < pointsNum; i++)
			{
				var track = new List<Point>();

				var start = rnd.NextPoint(bounds.XMin, bounds.XMax, bounds.YMin, bounds.YMax);
				track.Add(start);

				int maxLength = Math.Max(width, height);
				var position = start;
				double length = 0;
				do
				{
					var K1 = fieldWrapper.GetVector(position);
					K1.Normalize();
					//var shift = K1;
					var K2 = fieldWrapper.GetVector(position + (K1 / 2).DecreaseLength(width, height));
					K2.Normalize();
					var K3 = fieldWrapper.GetVector(position + (K2 / 2).DecreaseLength(width, height));
					K3.Normalize();
					var K4 = fieldWrapper.GetVector(position + K3.DecreaseLength(width, height));
					K4.Normalize();

					var shift = ((K1 + 2 * K2 + 2 * K3 + K4) / 6);
					//shift.Normalize();
					if (shift.X.IsNaN() || shift.Y.IsNaN())
						break;

					var next = position + shift;
					track.Add(next);

					if (!OccupyCells(position, next, bounds))
						break;

					position = next;
					length += shift.Length;
				} while (length < maxLength);

				Polyline line = new Polyline
				{
					Stroke = Brushes.Orange,
					StrokeThickness = 1,
					Stretch = Stretch.Fill,
					Points = new PointCollection(track),
					Effect = new DropShadowEffect { Color = Colors.Black, Direction = 245, BlurRadius = 3 }
				};
				ViewportPanel.SetViewportBounds(line, track.GetBounds());
				panel.Children.Add(line);
			}
		}

		private bool IsCellOccupied(Point position)
		{
			double xCoeff = bounds.Width / occupiedTableWidth;
			double yCoeff = bounds.Height / occupiedTableHeight;

			int ix = (int)((position.X - bounds.XMin) / xCoeff);
			int iy = (int)((position.Y - bounds.YMin) / yCoeff);

			return occupiedTable[ix, iy] > 1;
		}

		private bool OccupyCells(Point position, Point next, DataRect bounds)
		{
			double xCoeff = bounds.Width / occupiedTableWidth;
			double yCoeff = bounds.Height / occupiedTableHeight;

			int startIx = (int)((position.X - bounds.XMin) / xCoeff);
			int startIy = (int)((position.Y - bounds.YMin) / yCoeff);

			int endIx = (int)((next.X - bounds.XMin) / xCoeff);
			int endIy = (int)((next.Y - bounds.YMin) / yCoeff);

			Vector direction = next - position;
			direction.Normalize();
			Point start = new Point(startIx + 0.5, startIy + 0.5);
			Point end = new Point(endIx + 0.5, endIy + 0.5);

			Vector up = new Vector(0, 1);
			Vector down = new Vector(0, -1);
			Vector left = new Vector(-1, 0);
			Vector right = new Vector(1, 0);

			int ix = startIx;
			int iy = startIy;

			if (ix < 0 || ix >= occupiedTableWidth ||
				iy < 0 || iy >= occupiedTableHeight)
				return false;
			do
			{
				occupiedTable[ix, iy] += 1;

				Point nextPoint = start + direction;

				List<PointDescription> neighbours = new List<PointDescription>();
				AddIfInside(new PointDescription { Point = start + up, DeltaX = 0, DeltaY = 1 }, neighbours);
				AddIfInside(new PointDescription { Point = start + down, DeltaX = 0, DeltaY = -1 }, neighbours);
				AddIfInside(new PointDescription { Point = start + left, DeltaX = -1, DeltaY = 0 }, neighbours);
				AddIfInside(new PointDescription { Point = start + right, DeltaX = 1, DeltaY = 0 }, neighbours);

				foreach (var pt in neighbours)
				{
					var distance = DistanceFromSegmentToPoint(start, end, pt.Point);
					if (distance < 1)
					{
						ix += pt.DeltaX;
						iy += pt.DeltaY;

						if (ix < 0 || ix >= occupiedTableWidth ||
							iy < 0 || iy >= occupiedTableHeight)
							return false;
							

						if (occupiedTable[ix, iy] > 1) return false;

						occupiedTable[ix, iy] += 1;
						break;
					}
				}

			} while (ix != endIx && iy != endIy);

			return true;
		}

		private sealed class PointDescription
		{
			public Point Point;
			public int DeltaX;
			public int DeltaY;
		}

		private double DistanceFromSegmentToPoint(Point start, Point end, Point pt)
		{
			Vector startToEnd = end - start;
			Vector ptToStart = start - pt;
			Vector ptToEnd = end - pt;

			double a = startToEnd.Length;
			double b = ptToStart.Length;
			double c = ptToEnd.Length;

			double halfPerimeter = 0.5 * (a + b + c);
			double square = Math.Sqrt(halfPerimeter * (halfPerimeter - a) * (halfPerimeter - b) * (halfPerimeter - c));
			double distance = 2 * square / a;
			return distance;
		}

		private void AddIfInside(PointDescription pt, IList<PointDescription> list)
		{
			if (IsPointInsideOfTable(pt.Point))
				list.Add(pt);
		}

		private bool IsPointInsideOfTable(Point pt)
		{
			return pt.X > 0 && pt.X < occupiedTableWidth - 1 &&
				pt.Y > 0 && pt.Y < occupiedTableHeight - 1;
		}

		#endregion // end of Properties

		#region IPlotterElement Members

		private Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			this.plotter.Children.BeginAdd(panel);
			RebuildUI();
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Children.BeginRemove(panel);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
