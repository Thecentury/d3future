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

		public PowerPalette(double power = 2) { Power = power; }

		public PowerPalette(IPalette palette, double power = 2) : base(palette) { Power = power; }

		private double power = 2;
		public double Power
		{
			get { return power; }
			set
			{
				power = value;
				RaiseChanged();
			}
		}

		public override Color GetColor(double t)
		{
			return base.GetColor(Math.Pow(t, power));
		}
	}
}
