using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.MarkupExtensions;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public abstract class ColumnChartBase : IndexedChart
	{
		private Binding columnFillBinding;
		private Binding columnWidthBinding;
		private Binding columnBorderStrokeBinding;
		private Binding columnBorderThicknessBinding;
		private Binding indexBinding;

		static ColumnChartBase()
		{
			Type thisType = typeof(ColumnChartBase);
			MarkerTemplateProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(CreateDefaultColumnTemplate()));
			DependentValuePathProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata("."));
		}

		private static object CreateDefaultColumnTemplate()
		{
			DataTemplate template = new DataTemplate();

			FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Rectangle));
			factory.SetValue(Shape.FillProperty, Brushes.Green);
			template.VisualTree = factory;

			return template;
		}

		protected ColumnChartBase()
		{
			BoundsUnionMode = BoundsUnionMode.Bounds;

			InitializeBindings();
		}

		private void InitializeBindings()
		{
			columnFillBinding = new Binding { Path = new PropertyPath("(0)", ColumnFillProperty), Source = this };
			columnWidthBinding = new Binding { Path = new PropertyPath("(0)", ColumnWidthProperty), Source = this };
			columnBorderStrokeBinding = new Binding { Path = new PropertyPath("(0)", ColumnBorderStrokeProperty), Source = this };
			columnBorderThicknessBinding = new Binding { Path = new PropertyPath("(0)", ColumnBorderThicknessProperty), Source = this };
			indexBinding = new SelfBinding { Path = new PropertyPath("(0)", IndexProperty) };
		}

		protected Binding ColumnWidthBinding
		{
			get { return columnWidthBinding; }
		}

		protected Binding IndexBinding
		{
			get { return indexBinding; }
		}

		#region Dependency Properties

		#region ColumnFill property

		public Brush ColumnFill
		{
			get { return (Brush)GetValue(ColumnFillProperty); }
			set { SetValue(ColumnFillProperty, value); }
		}

		public static readonly DependencyProperty ColumnFillProperty = DependencyProperty.Register(
		  "ColumnFill",
		  typeof(Brush),
		  typeof(ColumnChartBase),
		  new FrameworkPropertyMetadata(Brushes.Green));

		#endregion // end of ColumnFill property

		#region ColumnWidth

		public double ColumnWidth
		{
			get { return (double)GetValue(ColumnWidthProperty); }
			set { SetValue(ColumnWidthProperty, value); }
		}

		public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(
		  "ColumnWidth",
		  typeof(double),
		  typeof(ColumnChartBase),
		  new FrameworkPropertyMetadata(0.8));

		#endregion // end of ColumnWidth

		#region ColumnBorderStroke property

		public Brush ColumnBorderStroke
		{
			get { return (Brush)GetValue(ColumnBorderStrokeProperty); }
			set { SetValue(ColumnBorderStrokeProperty, value); }
		}

		public static readonly DependencyProperty ColumnBorderStrokeProperty = DependencyProperty.Register(
		  "ColumnBorderStroke",
		  typeof(Brush),
		  typeof(ColumnChartBase),
		  new FrameworkPropertyMetadata(null));

		#endregion // end of ColumnBorderStroke property

		#region ColumnBorderThickness

		public double ColumnBorderThickness
		{
			get { return (double)GetValue(ColumnBorderThicknessProperty); }
			set { SetValue(ColumnBorderThicknessProperty, value); }
		}

		public static readonly DependencyProperty ColumnBorderThicknessProperty = DependencyProperty.Register(
		  "ColumnBorderThickness",
		  typeof(double),
		  typeof(ColumnChartBase),
		  new FrameworkPropertyMetadata(0.0));

		#endregion // end of ColumnBorderThickness

		#endregion // end of Properties

		protected override void SetCommonBindings(FrameworkElement marker)
		{
			base.SetCommonBindings(marker);

			if (!BindingOperations.IsDataBound(marker, Shape.FillProperty))
			{
				marker.SetBinding(Shape.FillProperty, columnFillBinding);
			}
			marker.SetBinding(Shape.StrokeProperty, columnBorderStrokeBinding);
			marker.SetBinding(Shape.StrokeThicknessProperty, columnBorderThicknessBinding);
		}
	}
}
