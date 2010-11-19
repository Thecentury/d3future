using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public abstract class PopupTipElement : IPlotterElement
	{
		/// <summary>Shows tooltips.</summary>
		private PopupTip popup;

		protected PopupTip GetPopupTipWindow()
		{
			if (popup != null)
				return popup;

			foreach (var item in Plotter.Children)
			{
				if (item is ViewportUIContainer)
				{
					ViewportUIContainer container = (ViewportUIContainer)item;
					if (container.Content is PopupTip)
						return popup = (PopupTip)container.Content;
				}
			}

			popup = new PopupTip();
			popup.Placement = PlacementMode.Relative;
			popup.PlacementTarget = plotter.CentralGrid;
			Plotter.Children.Add(popup);
			return popup;
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			GetPopupTipWindow().Hide();
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var popup = GetPopupTipWindow();
			if (popup.IsOpen)
				popup.Hide();

			Point screenPoint = e.GetPosition(plotter.CentralGrid);
			Point viewportPoint = screenPoint.ScreenToData(plotter.Transform);

			var tooltip = GetTooltipForPoint(viewportPoint);
			if (tooltip == null) return;

			popup.VerticalOffset = screenPoint.Y + 20;
			popup.HorizontalOffset = screenPoint.X;

			popup.ShowDelayed(TimeSpan.FromSeconds(0));

			Grid grid = new Grid();

			Rectangle rect = new Rectangle
			{
				Stroke = Brushes.Black,
				Fill = SystemColors.InfoBrush
			};

			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Vertical;
			panel.Children.Add(tooltip);
			panel.Margin = new Thickness(4, 2, 4, 2);

			var textBlock = new TextBlock();
			textBlock.Text = String.Format("Location: {0:F2}, {1:F2}", viewportPoint.X, viewportPoint.Y);
			textBlock.Foreground = SystemColors.GrayTextBrush;
			panel.Children.Add(textBlock);

			grid.Children.Add(rect);
			grid.Children.Add(panel);
			grid.Measure(SizeHelper.CreateInfiniteSize());
			popup.Child = grid;
		}

		protected virtual UIElement GetTooltipForPoint(Point viewportPosition)
		{
			return null;
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.CentralGrid.MouseMove += OnMouseMove;
			plotter.CentralGrid.MouseLeave += OnMouseLeave;
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.CentralGrid.MouseMove -= OnMouseMove;
			plotter.CentralGrid.MouseLeave -= OnMouseLeave;
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
