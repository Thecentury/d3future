using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	public class PowerPalette : DecoratorPaletteBase
	{
		public PowerPalette() { }

		public PowerPalette(IPalette palette) : base(palette) { }

		public override Color GetColor(double t)
		{
			// todo create a property for power base setting
			return base.GetColor(Math.Pow(t, 0.1));
		}
	}
}
