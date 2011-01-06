using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Converters;
using System.Globalization;
using System.Windows.Controls;
using System.Diagnostics.Contracts;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	public class PivotSegmentEditor : FrameworkElement, IPlotterElement, INotifyPropertyChanged
	{
		private Plotter2D plotter = null;
		private ViewportHostPanel panel = new ViewportHostPanel();
		private Segment segment;
		private DraggablePoint startThumb;
		private DraggablePoint endThumb;
		private Func<double, string> xMapping = d => d.ToString("F");
		private readonly ViewportRay leftRay;
		private readonly ViewportRay rightRay;

		/// <summary>
		/// Initializes a new instance of the <see cref="PivotSegmentEditor"/> class.
		/// </summary>
		public PivotSegmentEditor()
		{
			ResourceDictionary resources = new ResourceDictionary
			{
				Source = new Uri("/DynamicDataDisplay;component/Charts/Shapes/PivotSegmentEditor.xaml", UriKind.Relative)
			};

			panel.BeginBatchAdd();

			ControlTemplate segmentTemplate = (ControlTemplate)resources["segment"];
			segment = (Segment)segmentTemplate.LoadContent();
			segment.DataContext = this;

			ControlTemplate startThumbTemplate = (ControlTemplate)resources["leftThumb"];
			startThumb = (DraggablePoint)startThumbTemplate.LoadContent();
			startThumb.DataContext = this;

			ControlTemplate endThumbTemplate = (ControlTemplate)resources["rightThumb"];
			endThumb = (DraggablePoint)endThumbTemplate.LoadContent();
			endThumb.DataContext = this;

			ControlTemplate leftRayTemplate = (ControlTemplate)resources["leftRay"];
			leftRay = (ViewportRay)leftRayTemplate.LoadContent();
			leftRay.DataContext = this;

			ControlTemplate rightRayTemplate = (ControlTemplate)resources["rightRay"];
			rightRay = (ViewportRay)rightRayTemplate.LoadContent();
			rightRay.DataContext = this;

			ControlTemplate mTextTemplate = (ControlTemplate)resources["mText"];
			TextBlock mText = (TextBlock)mTextTemplate.LoadContent();
			panel.Children.Add(mText);
			mText.DataContext = this;

			ControlTemplate leftPointGridTemplate = (ControlTemplate)resources["leftPointGrid"];
			Panel leftPointGrid = (Panel)leftPointGridTemplate.LoadContent();
			panel.Children.Add(leftPointGrid);
			leftPointGrid.DataContext = this;

			ControlTemplate rightPointGridTemplate = (ControlTemplate)resources["rightPointGrid"];
			Panel rightPointGrid = (Panel)rightPointGridTemplate.LoadContent();
			panel.Children.Add(rightPointGrid);
			rightPointGrid.DataContext = this;

			ControlTemplate leftTextTemplate = (ControlTemplate)resources["leftText"];
			FrameworkElement leftBorder = (FrameworkElement)leftTextTemplate.LoadContent();
			panel.Children.Add(leftBorder);
			leftBorder.DataContext = this;

			ControlTemplate rightTextTemplate = (ControlTemplate)resources["rightText"];
			FrameworkElement rightBorder = (FrameworkElement)rightTextTemplate.LoadContent();
			panel.Children.Add(rightBorder);
			rightBorder.DataContext = this;
		}

		#region Properties

		#region XMapping property

		[NotNull]
		public Func<double, string> XMapping
		{
			get { return xMapping; }
			set
			{
				Contract.Assert(value != null);

				xMapping = value;
				PropertyChanged.Raise(this, "");
			}
		}

		#endregion

		#region Point1 property

		public Point Point1
		{
			get { return (Point)GetValue(Point1Property); }
			set { SetValue(Point1Property, value); }
		}

		public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
		  "Point1",
		  typeof(Point),
		  typeof(PivotSegmentEditor),
		  new FrameworkPropertyMetadata(new Point(0, 0), OnPointChanged));

		private static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PivotSegmentEditor owner = (PivotSegmentEditor)d;
			owner.CoerceValue(LeftYProperty);
			owner.CoerceValue(RightYProperty);
			owner.OnPointChanged();
		}

		private void OnPointChanged()
		{
			PropertyChanged.Raise(this, "");
		}

		#endregion

		#region Point2 property

		public Point Point2
		{
			get { return (Point)GetValue(Point2Property); }
			set { SetValue(Point2Property, value); }
		}

		public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
		  "Point2",
		  typeof(Point),
		  typeof(PivotSegmentEditor),
		  new FrameworkPropertyMetadata(new Point(1, 1), OnPointChanged));

		#endregion

		#region LineStroke property

		public Brush LineStroke
		{
			get { return (Brush)GetValue(LineStrokeProperty); }
			set { SetValue(LineStrokeProperty, value); }
		}

		public static readonly DependencyProperty LineStrokeProperty = DependencyProperty.Register(
		  "LineStroke",
		  typeof(Brush),
		  typeof(PivotSegmentEditor),
		  new FrameworkPropertyMetadata(Brushes.Black));

		#endregion

		#region LineThickness property

		public double LineThickness
		{
			get { return (double)GetValue(LineThicknessProperty); }
			set { SetValue(LineThicknessProperty, value); }
		}

		public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(
		  "LineThickbess",
		  typeof(double),
		  typeof(PivotSegmentEditor),
		  new FrameworkPropertyMetadata(2.0));

		#endregion

		#region LeftY property

		public double LeftY
		{
			get { return (double)GetValue(LeftYProperty); }
		}

		private static readonly DependencyPropertyKey LeftYPropertyKey = DependencyProperty.RegisterReadOnly(
			"LeftY",
			typeof(double),
			typeof(PivotSegmentEditor),
			new FrameworkPropertyMetadata(null, OnLeftYCoerce));

		public static readonly DependencyProperty LeftYProperty = LeftYPropertyKey.DependencyProperty;

		private Func<double, double> GetLineFunc()
		{
			Viewport2D viewport = plotter.Viewport;
			double deltaX = Point1.X - Point2.X;
			double deltaY = Point1.Y - Point2.Y;
			double m = deltaY / deltaX;
			double b = Point1.Y - Point1.X * deltaY / deltaX;

			Func<double, double> func = x => m * x + b;

			return func;
		}

		private static object OnLeftYCoerce(DependencyObject source, object value)
		{
			PivotSegmentEditor editor = (PivotSegmentEditor)source;
			if (editor.plotter == null)
				return value;

			Func<double, double> func = editor.GetLineFunc();
			double xmin = editor.plotter.Viewport.Visible.XMin;
			double result = func(xmin);

			return result;
		}

		#endregion

		#region RightY property

		public double RightY
		{
			get { return (double)GetValue(RightYProperty); }
		}

		private static readonly DependencyPropertyKey RightYPropertyKey = DependencyProperty.RegisterReadOnly(
			"RightY",
			typeof(double),
			typeof(PivotSegmentEditor),
			new FrameworkPropertyMetadata(null, OnRightYCoerce));

		public static readonly DependencyProperty RightYProperty = RightYPropertyKey.DependencyProperty;

		private static object OnRightYCoerce(DependencyObject source, object value)
		{
			PivotSegmentEditor editor = (PivotSegmentEditor)source;
			if (editor.plotter == null)
				return value;

			Func<double, double> func = editor.GetLineFunc();
			double xmax = editor.plotter.Viewport.Visible.XMax;
			double result = func(xmax);

			return result;
		}
		#endregion

		public double M
		{
			get { return (Point1.Y - Point2.Y) / (Point1.X - Point2.X); }
			set { Debug.WriteLine("M set = " + value.ToString()); }
		}

		public string LeftName
		{
			get
			{
				return String.Format("({0}, {1:F})", xMapping(Point1.X), Point1.Y);
			}
		}

		public string RightName
		{
			get
			{
				return String.Format("({0}, {1:F})", xMapping(Point2.X), Point2.Y);
			}
		}

		public Point Center
		{
			get
			{
				return Point1 + (Point2 - Point1) / 2;
			}
		}

		#endregion

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			this.plotter.Viewport.PropertyChanged += OnViewport_PropertyChanged;

			plotter.Dispatcher.BeginInvoke(() =>
			{
				plotter.Children.AddMany(
					segment,
					startThumb,
					endThumb,
					panel,
					leftRay,
					rightRay);

				CoerceValue(LeftYProperty);
				CoerceValue(RightYProperty);

				PropertyChanged.Raise(this, "");
			}, DispatcherPriority.Normal);
		}

		private void OnViewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			CoerceValue(LeftYProperty);
			CoerceValue(RightYProperty);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter.Viewport.PropertyChanged -= OnViewport_PropertyChanged;

			plotter.Dispatcher.BeginInvoke(() =>
			{
				plotter.Children.RemoveAll(
					segment,
					startThumb,
					endThumb,
					panel,
					leftRay,
					rightRay);
			}, DispatcherPriority.Normal);

			this.plotter = null;
		}

		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}

	public sealed class MToVerticalOffsetConverter : GenericValueConverter<double>
	{
		public override object ConvertCore(double value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value > 0)
				return 2.0;
			else
				return -2.0;
		}
	}

	public sealed class MToVerticalAlignmentInvertedConverter : GenericValueConverter<double>
	{
		public override object ConvertCore(double value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value > 0)
				return VerticalAlignment.Bottom;
			else
				return VerticalAlignment.Top;
		}
	}

	public sealed class MToVerticalAlignmentConverter : GenericValueConverter<double>
	{
		public override object ConvertCore(double value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value > 0)
				return VerticalAlignment.Top;
			else
				return VerticalAlignment.Bottom;
		}
	}
}

