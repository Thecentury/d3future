using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class VoronoiChart : FrameworkElement, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VoronoiChart"/> class.
		/// </summary>
		public VoronoiChart()
		{
		}

		#region Properties

		public PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
		  "Points",
		  typeof(PointCollection),
		  typeof(VoronoiChart),
		  new FrameworkPropertyMetadata(null, OnPointsReplaced));

		private static void OnPointsReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VoronoiChart owner = (VoronoiChart)d;
			owner.OnPointsReplaced((PointCollection)e.OldValue, (PointCollection)e.NewValue);
		}

		private void OnPointsReplaced(PointCollection prevPoints, PointCollection currPoints)
		{
			if (prevPoints != null)
				prevPoints.Changed -= OnPointsChanged;
			if (currPoints != null)
				currPoints.Changed += OnPointsChanged;

			UpdateVoronoi();
		}

		private void OnPointsChanged(object sender, EventArgs e)
		{
			UpdateVoronoi();
		}

		#endregion

		#region Voronoi

		private void UpdateVoronoi()
		{
			if (Points == null)
				return;

			InvalidateVisual();

			//VoronoiMain.main(Points);

			////var elementsToRemove = plotter.Children.Where(el => ((FrameworkElement)el).Tag == this).ToArray();
			////foreach (var item in elementsToRemove)
			////{
			////    plotter.Children.Remove(item);
			////}

			//Debug.WriteLine("VoronoiChart updated.");

			//var voronoiCollection = Output.Collection;
			//foreach (var item in voronoiCollection.OfType<VoronoiLine>())
			//{
			//    const double coordMin = -10;
			//    const double coordMax = 10;
			//    Point p1, p2;
			//    if (item.B != 0)
			//    {
			//        p1 = new Point(coordMin, item.C - item.A / item.B * coordMin);
			//        p2 = new Point(coordMax, item.C - item.A / item.B * coordMax);
			//    }
			//    else
			//    {
			//        double x = item.C / item.A;
			//        p1 = new Point(x, coordMin);
			//        p2 = new Point(x, coordMax);
			//    }

			//    Segment line = new Segment { Stroke = Brushes.Red, StrokeThickness = 2, StartPoint = p1, EndPoint = p2, Tag = this };
			//    plotter.Children.Add(line);
			//    //ViewportPanel.SetViewportBounds(line, bounds);
			//    //panel.Children.Add(line);
			//}
		}

		private enum InfinityPoint
		{
			None = 0,
			Start,
			End,
		}

		[DebuggerDisplay("{Start} - {End}")]
		private class Segment : IEquatable<Segment>
		{
			public Point Start { get; set; }
			public Point End { get; set; }

			public Site Site1 { get; set; }
			public Site Site2 { get; set; }

			public InfinityPoint InfinityPoint { get; set; }

			public Point GetNonInfinityPoint()
			{
				if (InfinityPoint == VoronoiChart.InfinityPoint.End || InfinityPoint == VoronoiChart.InfinityPoint.None)
					return Start;
				else
					return End;
			}

			public Point GetInfinityPoint()
			{
				if (InfinityPoint == VoronoiChart.InfinityPoint.Start)
					return Start;
				else if (InfinityPoint == VoronoiChart.InfinityPoint.End)
					return End;
				else
					throw new InvalidOperationException("Segment doesn't have infinity point.");
			}

			public bool HasInfinityPoint
			{
				get { return InfinityPoint != VoronoiChart.InfinityPoint.None; }
			}

			public Point GetOtherPoint(Point p)
			{
				if (Start == p)
					return End;
				else
					return Start;
			}

			public override int GetHashCode()
			{
				return Start.GetHashCode() ^
					End.GetHashCode() ^
					Site1.SiteNumber.GetHashCode() ^
					Site2.SiteNumber.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				var other = obj as Segment;
				if (other == null)
					return false;

				return this.Equals(other);
			}

			#region IEquatable<Segment> Members

			public bool Equals(Segment other)
			{
				return Start == other.Start &&
					End == other.End &&
					Site1.SiteNumber == other.Site1.SiteNumber &&
					Site2.SiteNumber == other.Site2.SiteNumber;
			}

			#endregion
		}

		const double coordInfinity = 10;
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			var dc = drawingContext;

			if (plotter == null)
				return;
			if (Points == null)
				return;

			var transform = plotter.Transform;

			VoronoiMain.main(Points);

			var lines = Output.LineCollection.OrderBy(line => line.EdgeNumber).ToArray();
			var vertices = Output.VertexCollection.OrderBy(vertex => vertex.SiteNumber).ToArray();
			var endPoints = Output.EndPointCollection.OrderBy(edge => edge.edgeNumber).ToArray();

			Pen linePen = new Pen(Brushes.Black, 1);

			var segments = new List<Segment>();
			for (int i = 0; i < endPoints.Length; i++)
			{
				var endPoint = endPoints[i];
				var vertexId = endPoint.edgeNumber;
				var line = lines[vertexId];
				var leftId = endPoint.EndPoints[0] != null ? endPoint.EndPoints[0].SiteNumber : -1;
				var rightId = endPoint.EndPoints[1] != null ? endPoint.EndPoints[1].SiteNumber : -1;

				Point pLeft = new Point();
				Point pRight = new Point();

				// at least one endPointId will be != -1
				if (rightId == -1)
				{
					pLeft = GetSegmentCoordinate(vertices, line, leftId, new Point());
					pRight = GetSegmentCoordinate(vertices, line, rightId, pLeft);
					segments.Add(new Segment { Start = pLeft, End = pRight, InfinityPoint = InfinityPoint.End, Site1 = line.Site1, Site2 = line.Site2 });
				}
				else if (leftId == -1)
				{
					pRight = GetSegmentCoordinate(vertices, line, rightId, new Point());
					pLeft = GetSegmentCoordinate(vertices, line, leftId, pRight);
					segments.Add(new Segment { Start = pRight, End = pLeft, InfinityPoint = InfinityPoint.End, Site1 = line.Site1, Site2 = line.Site2 });
				}
				else // leftId != -1 && rightId != -1
				{
					pLeft = GetSegmentCoordinate(vertices, line, leftId, new Point());
					pRight = GetSegmentCoordinate(vertices, line, rightId, pLeft);
					segments.Add(new Segment { Start = pLeft, End = pRight, InfinityPoint = InfinityPoint.None, Site1 = line.Site1, Site2 = line.Site2 });
				}

				pLeft = pLeft.DataToScreen(transform);
				pRight = pRight.DataToScreen(transform);

				dc.DrawLine(linePen, pLeft, pRight);
			}

			var pointsSequences = GetPointsSequences(segments);
			foreach (var sequence in pointsSequences)
			{
				var pathFigure = new PathFigure { IsClosed = true, IsFilled = true };
				pathFigure.StartPoint = sequence.Points.First().DataToScreen(transform);
				pathFigure.Segments.Add(new PolyLineSegment(sequence.Points.Skip(1).DataToScreen(transform), false));
				PathGeometry geometry = new PathGeometry();
				geometry.Figures.Add(pathFigure);

				if (!brushes.ContainsKey(sequence.Site))
					brushes[sequence.Site] = ColorHelper.RandomBrush.MakeTransparent(0.5);

				dc.DrawGeometry(brushes[sequence.Site], null, geometry);
			}
		}

		private readonly Dictionary<Site, Brush> brushes = new Dictionary<Site, Brush>();

		private sealed class SiteBoundsInfo
		{
			public Site Site { get; set; }
			public IEnumerable<Point> Points { get; set; }
		}

		private static IEnumerable<SiteBoundsInfo> GetPointsSequences(IEnumerable<Segment> segments)
		{
			//segments = segments.Distinct();

			var segmentsBySite = new MultiDictionary<Site, Segment>();
			foreach (var segment in segments)
			{
				segmentsBySite.Add(segment.Site1, segment);
				segmentsBySite.Add(segment.Site2, segment);
			}

			var result = new List<SiteBoundsInfo>();
			foreach (var site in segmentsBySite.Keys) // create sequence of points for each site
			{
				var segmentsForSite = segmentsBySite[site];
				List<Point> pointsSequence = new List<Point>();

				HashSet<Segment> segmentsSet = new HashSet<Segment>(segmentsForSite.Distinct());
				var firstInfinitySegment = segmentsSet.FirstOrDefault(s => s.HasInfinityPoint);
				Segment firstSegment;
				Point pointToSearch;
				
				if (firstInfinitySegment == null) // no infinity points
				{
					var first = segmentsSet.First();
					firstSegment = first;

					pointsSequence.Add(first.Start);
					pointsSequence.Add(first.End);

					pointToSearch = first.End;
				}
				else // has infinity point
				{
					firstSegment = firstInfinitySegment;

					pointsSequence.Add(firstInfinitySegment.GetInfinityPoint());
					pointsSequence.Add(firstInfinitySegment.GetNonInfinityPoint());

					pointToSearch = firstInfinitySegment.GetNonInfinityPoint();
				}

				segmentsSet.Remove(firstSegment);

				while (segmentsSet.Count > 0)
				{
					var segment = segmentsSet.Where(s => s.Start == pointToSearch || s.End == pointToSearch).FirstOrDefault();
					if (segment == null)
						break;

					Point otherPoint = segment.GetOtherPoint(pointToSearch);
					pointsSequence.Add(otherPoint);
					segmentsSet.Remove(segment);
					pointToSearch = otherPoint;
				}

				SiteBoundsInfo info = new SiteBoundsInfo { Points = pointsSequence, Site = site };
				result.Add(info);
			}

			return result;
		}

		private static Point GetSegmentCoordinate(VoronoiVertex[] vertices, VoronoiLine line, int endPointId, Point endPoint2)
		{
			Point result = new Point();
			if (endPointId != -1)
			{
				result.X = vertices[endPointId].X;
				result.Y = vertices[endPointId].Y;
			}
			else
			{
				var bisector = new Vector(line.Site1.Coord.X + line.Site2.Coord.X, line.Site1.Coord.Y + line.Site2.Coord.Y);
				if (line.B != 0)
				{
					double y = (line.C - line.A * coordInfinity) / line.B;
					result = new Point(coordInfinity, y);
				}
				else
				{
					double x = line.C / line.A;
					result = new Point(x, coordInfinity);
				}
				var existingDirection = result - endPoint2;
				if (bisector * existingDirection < 0)
				{
					existingDirection = -existingDirection;
					result += 2 * existingDirection;
				}
			}
			return result;
		}

		void Viewport_OnPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			this.InvalidateVisual();
		}


		#endregion

		private ViewportHostPanel panel = new ViewportHostPanel { Background = Brushes.Blue.MakeTransparent(0.1) };

		#region IPlotterElement Members

		private Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.CentralGrid.Children.Add(this);
			this.plotter.Viewport.PropertyChanged += Viewport_OnPropertyChanged;
			//plotter.Children.BeginAdd(panel);

			UpdateVoronoi();
		}


		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Viewport.PropertyChanged -= Viewport_OnPropertyChanged;
			plotter.CentralGrid.Children.Remove(this);
			//plotter.Children.BeginRemove(panel);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
