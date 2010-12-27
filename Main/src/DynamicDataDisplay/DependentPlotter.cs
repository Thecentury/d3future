using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Represents a special embedded kind of plotter.
	/// Follows an outer's plotter Visible change when this is general panning or zooming via MouseNavigation.
	/// Does not reacts on panning or zooming only in one direction (via AxisNavigation or KeyboardNavigation.
	/// </summary>
	public class DependentPlotter : InjectedPlotterBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DependentPlotter"/> class.
		/// </summary>
		public DependentPlotter() : base() { }

		protected override DataRect CoerceVisible(DataRect newVisible, DataRect baseVisible)
		{
			return baseVisible;
		}

		protected override void OuterViewport_PropertyChanged(ExtendedPropertyChangedEventArgs e)
		{
			if(e.PropertyName != Viewport2D.VisiblePropertyName)
				return;

			if (e.ChangeType == ChangeType.Pan || e.ChangeType == ChangeType.Zoom)
			{
				DataRect newRect = (DataRect)e.NewValue;
				DataRect oldRect = (DataRect)e.OldValue;

				double ratioX = newRect.Width / oldRect.Width;
				double ratioY = newRect.Height / oldRect.Height;
				double shiftX = (newRect.XMin - oldRect.XMin) / oldRect.Width;
				double shiftY = (newRect.YMin - oldRect.YMin) / oldRect.Height;

				DataRect visible = Viewport.Visible;

				visible.XMin += shiftX * visible.Width;
				visible.YMin += shiftY * visible.Height;
				visible.Width *= ratioX;
				visible.Height *= ratioY;

				Viewport.Visible = visible;
			}
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			Plotter.Viewport.FittedToView += Viewport_FittedToView;
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			Plotter.Viewport.FittedToView -= Viewport_FittedToView;

			base.OnPlotterDetaching(plotter);
		}

		private void Viewport_FittedToView(object sender, EventArgs e)
		{
			FitToView();
		}
	}
}
