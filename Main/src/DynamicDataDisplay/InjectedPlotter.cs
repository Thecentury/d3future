using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Data;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a nested plotter, which can control the way how its children are drawn in dependency of parent ChartPlotter Visible rect.
	/// This plotter is designed to work inside of some other plotter.
	/// <remarks>
	/// There are 8 properties (ParentXMin, SelfXMin, etc) which can be used to tune the size and position of inner plotter
	/// in dependence on parent Plotter's Visible rect.
	/// For example, you can specify that when parent x coordinates are from 0.0 to 1.0, inner plotter's visible x coordinates are
	/// from -1.0 to 2.0. This data will be used to calculate next positions of inner plotter's children charts.
	/// </remarks>
	/// </summary>
	[SkipPropertyCheck]
	public class InjectedPlotter : InjectedPlotterBase, IPlotterElement
	{
		private double xScale = 1.0;
		private double xShift = 0.0;
		private double yScale = 1.0;
		private double yShift = 0.0;

		/// <summary>
		/// Initializes a new instance of the <see cref="InjectedPlotter"/> class.
		/// </summary>
		public InjectedPlotter() : base() { }

		protected override DataRect CoerceVisible(DataRect newVisible, DataRect baseVisible)
		{
			DataRect result = newVisible;

			if (Plotter == null)
				return baseVisible;

			DataRect outerVisible = Plotter.Viewport.Visible;

			double xMin = outerVisible.XMin * xScale + xShift;
			double xMax = outerVisible.XMax * xScale + xShift;
			double yMin = outerVisible.YMin * yScale + yShift;
			double yMax = outerVisible.YMax * yScale + yShift;

			outerVisible = DataRect.Create(xMin, yMin, xMax, yMax);

			switch (ConjunctionMode)
			{
				case ViewportConjunctionMode.None:
					result = baseVisible;
					break;
				case ViewportConjunctionMode.X:
					result = new DataRect(outerVisible.XMin, baseVisible.YMin, outerVisible.Width, baseVisible.Height);
					break;
				case ViewportConjunctionMode.Y:
					result = new DataRect(baseVisible.XMin, outerVisible.YMin, baseVisible.Width, outerVisible.Height);
					break;
				case ViewportConjunctionMode.XY:
					result = outerVisible;
					break;
				default:
					break;
			}

			return result;
		}

		private void UpdateTransform()
		{
			xScale = (SelfXMax - SelfXMin) / (ParentXMax - ParentXMin);
			xShift = SelfXMin - ParentXMin;

			yScale = (SelfYMax - SelfYMin) / (ParentYMax - ParentYMin);
			yShift = SelfYMin - ParentYMin;
		}

		#region Properties

		#region ConjunctionMode property

		/// <summary>
		/// Gets or sets the conjunction mode - the way of how inner plotter calculates its Visible rect in dependence of outer plotter's Visible.
		/// This is a DependencyProperty.
		/// </summary>
		/// <value>The conjunction mode.</value>
		public ViewportConjunctionMode ConjunctionMode
		{
			get { return (ViewportConjunctionMode)GetValue(ConjunctionModeProperty); }
			set { SetValue(ConjunctionModeProperty, value); }
		}

		public static readonly DependencyProperty ConjunctionModeProperty = DependencyProperty.Register(
		  "ConjunctionMode",
		  typeof(ViewportConjunctionMode),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(ViewportConjunctionMode.XY, OnConjunctionModeReplaced));

		private static void OnConjunctionModeReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InjectedPlotter owner = (InjectedPlotter)d;
			owner.CoerceVisible();
		}

		#endregion

		#region Conversion properties

		public double ParentXMin
		{
			get { return (double)GetValue(ParentXMinProperty); }
			set { SetValue(ParentXMinProperty, value); }
		}

		public static readonly DependencyProperty ParentXMinProperty = DependencyProperty.Register(
		  "ParentXMin",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(0.0, OnTransformChanged));

		public double ParentXMax
		{
			get { return (double)GetValue(ParentXMaxProperty); }
			set { SetValue(ParentXMaxProperty, value); }
		}

		public static readonly DependencyProperty ParentXMaxProperty = DependencyProperty.Register(
		  "ParentXMax",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(1.0, OnTransformChanged));

		public double SelfXMin
		{
			get { return (double)GetValue(SelfXMinProperty); }
			set { SetValue(SelfXMinProperty, value); }
		}

		public static readonly DependencyProperty SelfXMinProperty = DependencyProperty.Register(
		  "SelfXMin",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(0.0, OnTransformChanged));

		public double SelfXMax
		{
			get { return (double)GetValue(SelfXMaxProperty); }
			set { SetValue(SelfXMaxProperty, value); }
		}

		public static readonly DependencyProperty SelfXMaxProperty = DependencyProperty.Register(
		  "SelfXMax",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(1.0, OnTransformChanged));

		public double ParentYMin
		{
			get { return (double)GetValue(ParentYMinProperty); }
			set { SetValue(ParentYMinProperty, value); }
		}

		public static readonly DependencyProperty ParentYMinProperty = DependencyProperty.Register(
		  "ParentYMin",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(0.0, OnTransformChanged));

		public double ParentYMax
		{
			get { return (double)GetValue(ParentYMaxProperty); }
			set { SetValue(ParentYMaxProperty, value); }
		}

		public static readonly DependencyProperty ParentYMaxProperty = DependencyProperty.Register(
		  "ParentYMax",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(1.0, OnTransformChanged));

		public double SelfYMin
		{
			get { return (double)GetValue(SelfYMinProperty); }
			set { SetValue(SelfYMinProperty, value); }
		}

		public static readonly DependencyProperty SelfYMinProperty = DependencyProperty.Register(
		  "SelfYMin",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(0.0, OnTransformChanged));

		public double SelfYMax
		{
			get { return (double)GetValue(SelfYMaxProperty); }
			set { SetValue(SelfYMaxProperty, value); }
		}

		public static readonly DependencyProperty SelfYMaxProperty = DependencyProperty.Register(
		  "SelfYMax",
		  typeof(double),
		  typeof(InjectedPlotter),
		  new FrameworkPropertyMetadata(1.0, OnTransformChanged));

		private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InjectedPlotter plotter = (InjectedPlotter)d;
			plotter.UpdateTransform();
		}

		#endregion

		#endregion
	}
}
