using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class VectorFieldConvolutionFilter
	{
		public abstract int[] ApplyFilter(int[] pixels, int width, int height, Vector[,] field);

		public event EventHandler Changed;

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
	}
}
