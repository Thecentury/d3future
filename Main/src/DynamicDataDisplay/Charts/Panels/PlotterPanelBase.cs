using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public abstract class PlotterPanelBase : ContentControl, IPlotterElement
	{
		private Plotter2D plotter;

		protected abstract Panel GetPanel(Plotter plotter);

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			
			Panel panel = GetPanel(plotter);
			panel.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			Panel panel = GetPanel(plotter);
			panel.Children.Remove(this);

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
	}
}
