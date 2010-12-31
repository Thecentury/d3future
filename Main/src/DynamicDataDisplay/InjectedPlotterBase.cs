using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	public abstract class InjectedPlotterBase : ChartPlotter, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InjectedPlotterBase"/> class.
		/// </summary>
		public InjectedPlotterBase()
			: base(PlotterLoadMode.Empty)
		{
			ViewportPanel = new Canvas();

			Viewport = new InjectedViewport2D(ViewportPanel, this) { CoerceVisibleFunc = CoerceVisible };
		}

		protected abstract DataRect CoerceVisible(DataRect newVisible, DataRect baseVisible);

		protected void CoerceVisible()
		{
			Viewport.CoerceValue(Viewport2D.VisibleProperty);
		}

		private void OuterViewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			OuterViewport_PropertyChanged(e);
		}

		protected virtual void OuterViewport_PropertyChanged(ExtendedPropertyChangedEventArgs e)
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
		  typeof(InjectedPlotterBase),
		  new FrameworkPropertyMetadata(ViewportConjunctionMode.XY, OnConjunctionModeReplaced));

		private static void OnConjunctionModeReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InjectedPlotterBase owner = (InjectedPlotterBase)d;
			owner.OnConjunctionModeChanged();
		}

		protected abstract void OnConjunctionModeChanged();

		#endregion
		#endregion

		#region IPlotterElement methods

		public virtual void OnPlotterAttached(Plotter plotter)
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

		public virtual void OnPlotterDetaching(Plotter plotter)
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
