using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows.Input;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	public abstract class PaletteDraggablePoint : PositionalViewportUIContainer
	{
		bool dragging = false;
		Point dragStart;
		Vector shift;
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (Plotter == null)
				return;

			dragStart = e.GetPosition(Plotter.ViewportPanel).ScreenToData(Plotter.Viewport.Transform);
			shift = Position - dragStart;
			dragging = true;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			ReleaseMouseCapture();
			dragging = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!dragging)
			{
				if (IsMouseCaptured)
					ReleaseMouseCapture();

				return;
			}

			if (!IsMouseCaptured)
				CaptureMouse();

			Point mouseInData = e.GetPosition(Plotter.ViewportPanel).ScreenToData(Plotter.Viewport.Transform);

			if (mouseInData != dragStart)
			{
				Position = mouseInData + shift;
				e.Handled = true;
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (dragging)
			{
				dragging = false;
				if (IsMouseCaptured)
				{
					ReleaseMouseCapture();
					e.Handled = true;
				}
			}
		}

	}
}
