﻿using System;
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

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	public class PivotSegmentEditor : FrameworkElement, IPlotterElement
	{
		private Plotter2D plotter = null;
		private ViewportHostPanel panel = new ViewportHostPanel();
		private Segment segment = new Segment();
		private DraggablePoint startThumb = new DraggablePoint();
		private DraggablePoint endThumb = new DraggablePoint();
		private Func<double, string> xMapping = d => d.ToString("F");
		private readonly ViewportRay leftRay = new ViewportRay { StrokeDashArray = new DoubleCollection(new double[] { 1, 1 }), Direction = -1 };
		private readonly ViewportRay rightRay = new ViewportRay { StrokeDashArray = new DoubleCollection(new double[] { 1, 1 }), Direction = +1 };

		/// <summary>
		/// Initializes a new instance of the <see cref="PivotSegmentEditor"/> class.
		/// </summary>
		public PivotSegmentEditor()
		{
			Binding p1Binding = new Binding("Point1") { Source = this };
			Binding p2Binding = new Binding("Point2") { Source = this };

			Binding strokeBinding = new Binding(LineStrokeProperty.Name) { Source = this };
			Binding strokeThicknessBinding = new Binding(LineThicknessProperty.Name) { Source = this };

			segment.SetBinding(Segment.StartPointProperty, p1Binding);
			segment.SetBinding(Segment.EndPointProperty, p2Binding);
			segment.SetBinding(Segment.StrokeProperty, strokeBinding);
			segment.SetBinding(Segment.StrokeThicknessProperty, strokeThicknessBinding);

			leftRay.SetBinding(ViewportRay.Point1Property, p1Binding);
			leftRay.SetBinding(ViewportRay.Point2Property, p2Binding);
			leftRay.SetBinding(ViewportRay.StrokeProperty, strokeBinding);
			leftRay.SetBinding(ViewportRay.StrokeThicknessProperty, strokeThicknessBinding);

			rightRay.SetBinding(ViewportRay.Point1Property, p1Binding);
			rightRay.SetBinding(ViewportRay.Point2Property, p2Binding);
			rightRay.SetBinding(ViewportRay.StrokeProperty, strokeBinding);
			rightRay.SetBinding(ViewportRay.StrokeThicknessProperty, strokeThicknessBinding);

			startThumb.SetBinding(DraggablePoint.PositionProperty, new Binding("Point1") { Source = this, Mode = BindingMode.TwoWay });
			endThumb.SetBinding(DraggablePoint.PositionProperty, new Binding("Point2") { Source = this, Mode = BindingMode.TwoWay });

			MultiBinding mBinding = new MultiBinding { Converter = new MValueConverter() };
			mBinding.Bindings.Add(p1Binding);
			mBinding.Bindings.Add(p2Binding);

			TextBlock mText = new TextBlock();
			mText.SetBinding(TextBlock.TextProperty, mBinding);

			CenterPointXConverter centerXConverter = new CenterPointXConverter();
			MultiBinding centerPointXBinding = new MultiBinding { Converter = centerXConverter };
			centerPointXBinding.Bindings.Add(p1Binding);
			centerPointXBinding.Bindings.Add(p2Binding);
			mText.SetBinding(ViewportPanel.XProperty, centerPointXBinding);

			CenterPointYConverter centerYConverter = new CenterPointYConverter();
			MultiBinding centerPointYBinding = new MultiBinding { Converter = centerYConverter };
			centerPointYBinding.Bindings.Add(p1Binding);
			centerPointYBinding.Bindings.Add(p2Binding);
			mText.SetBinding(ViewportPanel.YProperty, centerPointYBinding);

			ViewportPanel.SetViewportHorizontalAlignment(mText, HorizontalAlignment.Left);
			ViewportPanel.SetViewportVerticalAlignment(mText, VerticalAlignment.Top);

			MultiBinding centerPointVerticalAlignmentBinding = new MultiBinding { Converter = new MToVerticalAlignment() };
			centerPointVerticalAlignmentBinding.Bindings.Add(p1Binding);
			centerPointVerticalAlignmentBinding.Bindings.Add(p2Binding);
			mText.SetBinding(ViewportPanel.ViewportVerticalAlignmentProperty, centerPointVerticalAlignmentBinding);

			MultiBinding centerPointVerticalOffsetBinding = new MultiBinding { Converter = new MToVerticalOffsetConverter() };
			centerPointVerticalOffsetBinding.Bindings.Add(p1Binding);
			centerPointVerticalOffsetBinding.Bindings.Add(p2Binding);
			mText.SetBinding(ViewportPanel.ScreenOffsetYProperty, centerPointVerticalOffsetBinding);

			panel.Children.Add(mText);

			GenericValueConverter<Point> pointTextConverter = new GenericValueConverter<Point>(p =>
			{
				var result = p; //String.Format("({0}, {1})", xMapping(p.X), p.Y);
				return result;
			});
			Grid leftPointGrid = new Grid();
			Rectangle leftPointRect = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 1.0 };
			TextBlock leftPointText = new TextBlock { Margin = new Thickness(3) };
			leftPointGrid.Children.Add(leftPointRect);
			leftPointGrid.Children.Add(leftPointText);
			panel.Children.Add(leftPointGrid);
			leftPointText.SetBinding(TextBlock.TextProperty, new Binding("Point1") { Source = this, Converter = pointTextConverter });
			leftPointText.HorizontalAlignment = HorizontalAlignment.Stretch;

			leftPointGrid.SetBinding(ViewportPanel.XProperty, new Binding("Point1.X") { Source = this });
			leftPointGrid.SetBinding(ViewportPanel.YProperty, new Binding("Point1.Y") { Source = this });
			ViewportPanel.SetViewportHorizontalAlignment(leftPointGrid, HorizontalAlignment.Left);
			ViewportPanel.SetViewportVerticalAlignment(leftPointGrid, VerticalAlignment.Top);

			Grid rightPointGrid = new Grid();
			Rectangle rightPointRect = new Rectangle { Stroke = Brushes.Black, StrokeThickness = 1.0 };
			TextBlock rightPointText = new TextBlock { Margin = new Thickness(3) };
			rightPointGrid.Children.Add(rightPointRect);
			rightPointGrid.Children.Add(rightPointText);
			panel.Children.Add(rightPointGrid);
			rightPointText.SetBinding(TextBlock.TextProperty, new Binding("Point2") { Source = this, Converter = pointTextConverter });

			rightPointGrid.SetBinding(ViewportPanel.XProperty, new Binding("Point2.X") { Source = this });
			rightPointGrid.SetBinding(ViewportPanel.YProperty, new Binding("Point2.Y") { Source = this });
			ViewportPanel.SetViewportHorizontalAlignment(rightPointGrid, HorizontalAlignment.Left);
			ViewportPanel.SetViewportVerticalAlignment(rightPointGrid, VerticalAlignment.Top);
		}

		#region Properties

		#region XMapping property

		public Func<double, string> XMapping
		{
			get { return xMapping; }
			set
			{
				Contract.Assert(value != null);

				xMapping = value;
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
			owner.OnPointChanged();
		}

		private void OnPointChanged() { }

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

		#endregion

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;

			plotter.Dispatcher.BeginInvoke(() =>
			{
				plotter.Children.AddMany(
					segment,
					startThumb,
					endThumb,
					panel,
					leftRay,
					rightRay);
			}, DispatcherPriority.Normal);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
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

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}

	internal sealed class CenterPointXConverter : TwoValuesMultiConverter<Point, Point>
	{
		protected override object ConvertCore(Point value1, Point value2, Type targetType, object parameter, CultureInfo culture)
		{
			Point center = value1 + (value2 - value1) / 2;
			double x = center.X;
			return x;
		}
	}

	internal sealed class CenterPointYConverter : TwoValuesMultiConverter<Point, Point>
	{
		protected override object ConvertCore(Point value1, Point value2, Type targetType, object parameter, CultureInfo culture)
		{
			Point center = value1 + (value2 - value1) / 2;
			double y = center.Y;
			return y;
		}
	}

	internal sealed class MValueConverter : TwoValuesMultiConverter<Point, Point>
	{
		protected override object ConvertCore(Point value1, Point value2, Type targetType, object parameter, CultureInfo culture)
		{
			double m = (value1.Y - value2.Y) / (value1.X - value2.X);

			return "m = " + m.ToString("F");
		}
	}

	internal sealed class MToVerticalOffsetConverter : TwoValuesMultiConverter<Point, Point>
	{
		protected override object ConvertCore(Point value1, Point value2, Type targetType, object parameter, CultureInfo culture)
		{
			double m = (value1.Y - value2.Y) / (value1.X - value2.X);

			if (m > 0)
				return 2.0;
			else
				return -2.0;
		}
	}

	internal sealed class MToVerticalAlignment : TwoValuesMultiConverter<Point, Point>
	{
		protected override object ConvertCore(Point value1, Point value2, Type targetType, object parameter, CultureInfo culture)
		{
			double m = (value1.Y - value2.Y) / (value1.X - value2.X);

			if (m > 0)
				return VerticalAlignment.Top;
			else
				return VerticalAlignment.Bottom;
		}
	}
}
