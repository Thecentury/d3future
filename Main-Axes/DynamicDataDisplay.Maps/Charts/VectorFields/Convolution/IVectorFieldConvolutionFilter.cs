using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public interface IVectorFieldConvolutionFilter
	{
		void ApplyFilter(int[] pixels, int width, int height, Vector[,] field);
		event EventHandler Changed;
	}

	public abstract class VectorFieldConvolutionFilter : IVectorFieldConvolutionFilter
	{

		#region IVectorFieldConvolutionFilter Members

		public abstract void ApplyFilter(int[] pixels, int width, int height, Vector[,] field);

		public event EventHandler Changed;

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}

		#endregion
	}
}
