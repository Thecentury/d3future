using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class PatternChart2D : DependencyObject, IPlotterElement
	{
		#region Properties

		#region Pattern property

		public PointSetPattern3D Pattern
		{
			get { return (PointSetPattern3D)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}

		public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(
		  "Pattern",
		  typeof(PointSetPattern3D),
		  typeof(PatternChart2D),
		  new FrameworkPropertyMetadata(new SinglePointPattern3D(), OnPatternReplaced));

		private static void OnPatternReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PatternChart2D owner = (PatternChart2D)d;
			owner.UpdateUI();
		}

		#endregion Pattern property

		#region OutPattern property

		private readonly PreSelectedPointsPattern3D outPattern = new PreSelectedPointsPattern3D();
		public PointSetPattern3D OutPattern
		{
			get { return outPattern; }
		}

		#endregion OutPattern property

		#region Index attached property

		public static int GetIndex(DependencyObject obj)
		{
			return (int)obj.GetValue(IndexProperty);
		}

		public static void SetIndex(DependencyObject obj, int value)
		{
			obj.SetValue(IndexProperty, value);
		}

		public static readonly DependencyProperty IndexProperty = DependencyProperty.RegisterAttached(
		  "Index",
		  typeof(int),
		  typeof(PatternChart2D),
		  new FrameworkPropertyMetadata(0));

		#endregion Index attached property

		private Func<Point3D, Point> project = p3D => new Point(p3D.X, p3D.Y);
		public Func<Point3D, Point> Project
		{
			get { return project; }
			set { project = value; }
		}

		private Func<Point, Point3D> outProject = p2D => new Point3D(p2D.X, p2D.Y, 0.1);
		public Func<Point, Point3D> OutProject
		{
			get { return outProject; }
			set { outProject = value; }
		}

		#endregion Properties

		private readonly ResourcePool<DraggablePoint> pointsPool = new ResourcePool<DraggablePoint>();
		private readonly List<DraggablePoint> points = new List<DraggablePoint>();
		private void UpdateUI()
		{
			foreach (var point in points)
			{
				point.PositionChanged -= OnPoint_PositionChanged;
				pointsPool.Put(point);
				plotter.Children.Remove(point);
			}

			points.Clear();

			int i = 0;
			foreach (var point3D in Pattern.GeneratePoints())
			{
				DraggablePoint draggablePoint = pointsPool.GetOrCreate();
				var position = project(point3D);

				outPattern.Points.Add(point3D);

				draggablePoint.Position = position;
				draggablePoint.PositionChanged += OnPoint_PositionChanged;
				SetIndex(draggablePoint, i);
				plotter.Children.Add(draggablePoint);
				points.Add(draggablePoint);
				i++;
			}
		}

		protected void OnPoint_PositionChanged(object sender, PositionChangedEventArgs e)
		{
			DraggablePoint source = (DraggablePoint)sender;
			int index = GetIndex(source);
			var point = source.Position;
			var point3D = outProject(point);
			outPattern.Points[index] = point3D;
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			Dispatcher.BeginInvoke(() => UpdateUI());
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion // IPlotterElement Members
	}
}
