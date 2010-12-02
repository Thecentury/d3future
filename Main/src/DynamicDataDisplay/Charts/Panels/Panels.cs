using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public sealed class HeaderPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.HeaderPanel;
		}
	}

	public sealed class FooterPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.FooterPanel;
		}
	}

	public sealed class BottomPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.BottomPanel;
		}
	}

	public sealed class TopPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.TopPanel;
		}
	}

	public sealed class LeftPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.LeftPanel;
		}
	}

	public sealed class RightPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.RightPanel;
		}
	}

	public sealed class MainCanvasPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.MainCanvas;
		}
	}

	public sealed class CentralGridPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.CentralGrid;
		}
	}

	public sealed class MainGridPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.MainGrid;
		}
	}

	public sealed class ParallelCanvasPanel : PlotterPanelBase
	{
		protected override Panel GetPanel(Plotter plotter)
		{
			return plotter.ParallelCanvas;
		}
	}
}
