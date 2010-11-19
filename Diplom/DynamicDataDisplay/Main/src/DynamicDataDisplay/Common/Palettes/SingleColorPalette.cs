using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Palettes
{
	public class SingleColorPalette : IPalette
	{
		private Color color = Colors.Crimson;
		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		#region IPalette Members

		public Color GetColor(double t)
		{
			return color;
		}

		public event EventHandler Changed;

		#endregion
	}
}
