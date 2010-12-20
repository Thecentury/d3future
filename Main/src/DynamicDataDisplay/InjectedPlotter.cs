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
	public class InjectedPlotter : ChartPlotter, IPlotterElement
	{
		private double xScale = 1.0;
		private double xShift = 0.0;
		private double yScale = 1.0;
		private double yShift = 0.0;

		/// <summary>
		/// Initializes a new instance of the <see cref="InjectedPlotter"/> class.
		/// </summary>
		public InjectedPlotter()
			: base(PlotterLoadMode.Empty)
		{
			ViewportPanel = new Canvas { Background = Brushes.IndianRed.MakeTransparent(0.3) };

			Viewport = new InjectedViewport2D(ViewportPanel, this) { CoerceVisibleFunc = CoerceVisible };
		}

		private DataRect CoerceVisible(DataRect newVisible, DataRect baseVisible)
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

		private void CoerceVisible()
		{
			Viewport.CoerceValue(Viewport2D.VisibleProperty);
		}

		private void OuterViewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			CoerceVisible();
		}

		protected override void OnChildAdded(IPlotterElement child)
		{
			base.OnChildAdded(child);

			if (plotter != null && !plotter.Children.Contains(child))
			{
				plotter.PerformChildChecks = false;
				plotter.Children.Add(child);
				plotter.PerformChildChecks = true;
			}
		}

		protected override void OnChildRemoving(IPlotterElement child)
		{
			base.OnChildRemoving(child);

			if (plotter != null && plotter.Children.Contains(child))
			{
				plotter.PerformChildChecks = false;
				plotter.Children.Remove(child);
				plotter.PerformChildChecks = true;
			}
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

		#region IPlotterElement Members

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			this.plotter.Viewport.PropertyChanged += OuterViewport_PropertyChanged;

			plotter.CentralGrid.Children.Add(ViewportPanel);

			HeaderPanel = plotter.HeaderPanel;
			FooterPanel = plotter.FooterPanel;

			LeftPanel = plotter.LeftPanel;
			BottomPanel = plotter.BottomPanel;
			RightPanel = plotter.RightPanel;
			TopPanel = plotter.BottomPanel;

			MainCanvas = plotter.MainCanvas;
			CentralGrid = plotter.CentralGrid;
			MainGrid = plotter.MainGrid;
			ParallelCanvas = plotter.ParallelCanvas;

			OnLoaded();
			ExecuteWaitingChildrenAdditions();
			AddAllChildrenToParentPlotter();
			CoerceVisible();
		}

		private void AddAllChildrenToParentPlotter()
		{
			plotter.PerformChildChecks = false;
			foreach (var child in Children)
			{
				if (plotter.Children.Contains(child))
					continue;

				plotter.Children.Add(child);
			}
			plotter.PerformChildChecks = true;
		}

		protected override bool IsLoadedInternal
		{
			get
			{
				return plotter != null;
			}
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.Children.Remove(ViewportPanel);
			this.plotter.Viewport.PropertyChanged -= OuterViewport_PropertyChanged;
			RemoveAllChildrenFromParentPlotter();

			this.plotter = null;
		}

		private void RemoveAllChildrenFromParentPlotter()
		{
			plotter.PerformChildChecks = false;
			foreach (var child in Children)
			{
				plotter.Children.Remove(child);
			}
			plotter.PerformChildChecks = true;
		}

		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
