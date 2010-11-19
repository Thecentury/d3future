using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	/// <summary>
	/// Represents an editor of points' position of ViewportPolyline or ViewportPolygon.
	/// </summary>
	[ContentProperty("Polyline")]
	public class PolylineEditor : FrameworkElement, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PolylineEditor"/> class.
		/// </summary>
		public PolylineEditor()
		{
			SetBinding(PointsProperty, new Binding("Polyline.Points") { Source = this });
		}

		public ViewportPolylineBase Polyline
		{
			get { return (ViewportPolylineBase)GetValue(polylineProperty); }
			set { SetValue(polylineProperty, value); }
		}

		public static readonly DependencyProperty polylineProperty = DependencyProperty.Register(
		  "polyline",
		  typeof(ViewportPolylineBase),
		  typeof(PolylineEditor),
		  new FrameworkPropertyMetadata(null));

		//private ViewportPolylineBase polyline;
		///// <summary>
		///// Gets or sets the polyline, to edit points of which.
		///// </summary>
		///// <value>The polyline.</value>
		//[NotNull]
		//public ViewportPolylineBase Polyline
		//{
		//    get { return polyline; }
		//    set
		//    {
		//        if (value == null)
		//            throw new ArgumentNullException("Polyline");

		//        if (polyline != value)
		//        {
		//            polyline = value;
		//            var descr = DependencyPropertyDescriptor.FromProperty(ViewportPolylineBase.PointsProperty, typeof(ViewportPolylineBase));
		//            descr.AddValueChanged(polyline, OnPointsReplaced);

		//            if (plotter != null)
		//            {
		//                AddLineToPlotter(false);
		//            }
		//        }
		//    }
		//}

		public PointCollection Points
		{
			get { return (PointCollection)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

		public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
		  "Points",
		  typeof(PointCollection),
		  typeof(PolylineEditor),
		  new FrameworkPropertyMetadata(null, OnPointsChanged));

		private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PolylineEditor owner = (PolylineEditor)d;
			// todo 
		}

		bool pointsAdded = false;
		private void OnPointsReplaced(object sender, EventArgs e)
		{
			if (plotter == null)
				return;
			if (pointsAdded)
				return;

			ViewportPolylineBase line = (ViewportPolylineBase)sender;

			pointsAdded = true;
			List<IPlotterElement> draggablePoints = new List<IPlotterElement>();
			CreateDraggablePoints(draggablePoints);

			foreach (var point in draggablePoints)
			{
				plotter.Children.Add(point);
			}
		}

		private void AddLineToPlotter(bool async)
		{
			if (!async)
			{
				foreach (var item in GetAllElementsToAdd())
				{
					plotter.Children.Add(item);
				}
			}
			else
			{
				plotter.Dispatcher.BeginInvoke(((Action)(() => { AddLineToPlotter(false); })), DispatcherPriority.Send);
			}
		}

		private List<IPlotterElement> GetAllElementsToAdd()
		{
			var result = new List<IPlotterElement>(1 + Polyline.Points.Count);
			result.Add(Polyline);

			CreateDraggablePoints(result);

			return result;
		}

		private void CreateDraggablePoints(List<IPlotterElement> collection)
		{
			for (int i = 0; i < Polyline.Points.Count; i++)
			{
				DraggablePoint point = new DraggablePoint();
				point.SetBinding(DraggablePoint.PositionProperty, new Binding
				{
					Source = Polyline,
					Path = new PropertyPath("Points[" + i + "]"),
					Mode = BindingMode.TwoWay
				});
				collection.Add(point);
			}
		}

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			if (Polyline != null)
			{
				AddLineToPlotter(true);
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
		}

		private Plotter2D plotter;
		/// <summary>
		/// Gets the parent plotter of chart.
		/// Should be equal to null if item is not connected to any plotter.
		/// </summary>
		/// <value>The plotter.</value>
		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
