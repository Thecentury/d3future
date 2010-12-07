using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Shapes
{
	internal sealed class SimpleGrid : Panel
	{
		protected override Size MeasureOverride(Size availableSize)
		{
			Size size = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

			double maxWidth = Double.NegativeInfinity;
			double maxheight = Double.NegativeInfinity;
			foreach (UIElement element in InternalChildren)
			{
				if (element != null)
				{
					element.Measure(size);

					maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
					maxheight = Math.Max(maxheight, element.DesiredSize.Height);
				}
			}

			return new Size(maxWidth, maxheight);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			foreach (UIElement element in InternalChildren)
			{
				if (element == null)
					continue;

				element.Arrange(new Rect(element.DesiredSize));
			}

			return finalSize;
		}
	}
}
