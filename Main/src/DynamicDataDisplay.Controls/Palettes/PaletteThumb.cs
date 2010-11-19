using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Interaction logic for PaletteThumb.xaml
	/// </summary>
	public class PaletteThumb : PaletteDraggablePoint
	{
		public PaletteThumb()
		{
			PositionCoerceCallbacks.Add(new PositionCoerceCallback((container, point) =>
			{
				point.Y = 0.5;
				if (point.X < 0)
					point.X = 0;
				else if (point.X > 1)
					point.X = 1;
				return point;
			}));

			Rectangle content = new Rectangle
			{
				Width = 7,
				Height = 20,
				Fill = Brushes.Transparent,
				Stroke = Brushes.DimGray,
				StrokeThickness = 2,
				VerticalAlignment = VerticalAlignment.Center,
				Cursor = Cursors.ScrollWE
			};
			Content = content;

			Style = null;
		}
	}
}
