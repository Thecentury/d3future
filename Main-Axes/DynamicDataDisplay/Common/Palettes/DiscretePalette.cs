using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	[ContentProperty("Steps")]
	public class DiscretePalette : IPalette
	{
		private readonly ObservableCollection<LinearPaletteColorStep> steps = new ObservableCollection<LinearPaletteColorStep>();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableCollection<LinearPaletteColorStep> Steps
		{
			get { return steps; }
		}

		public DiscretePalette() { }
		public DiscretePalette(params LinearPaletteColorStep[] steps)
		{
			this.steps.AddMany(steps);
		}

		public Color GetColor(double t)
		{
			if (t <= 0) return Steps[0].Color;
			if (t >= Steps.Last().Offset) return steps.Last().Color;

			int i = 0;
			double x = 0;
			while (x < t && i < steps.Count)
			{
				x = Steps[i].Offset;
				i++;
			}

			Color result = Steps[i - 1].Color;
			return result;
		}

		#region IPalette Members

		public event EventHandler Changed;

		#endregion
	}
}
