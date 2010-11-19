using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Petzold.Media3D;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class VectorChart3D : VectorChartModel3DBase
	{
		#region Properties

		private IPalette palette = new HsbPalette();
		public IPalette Palette
		{
			get { return palette; }
			set { palette = value; }
		}

		#endregion Properties

		private double minLength;
		private double maxLength;
		protected override void RebuildUI()
		{
			Children.Clear();

			if (DataSource == null)
				return;

			var dataSource = DataSource;
			int width = dataSource.Width;
			int height = dataSource.Height;
			int depth = dataSource.Depth;


			// searching for min and max lengths of vectors
			minLength = Double.MaxValue;
			maxLength = -Double.MaxValue;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < depth; k++)
					{
						var length = dataSource.Data[i, j, k].Length;
						if (length < minLength) minLength = length;
						if (length > maxLength) maxLength = length;
					}
				}
			}

			for (int i = 0; i < width; i++)
			{
				int iLocal = i;

				Dispatcher.BeginInvoke(() =>
				{
					for (int j = 0; j < height; j++)
					{
						for (int k = 0; k < depth; k++)
						{
							WireLine line = CreateLine(dataSource.Grid[iLocal, j, k], dataSource.Data[iLocal, j, k]);
							Children.Add(line);
						}
					}
				}, DispatcherPriority.Background);
			}
		}

		private WireLine CreateLine(Point3D position, Vector3D direction)
		{
			var length = direction.Length;
			var colorRatio = (length - minLength) / (maxLength - minLength);
			if (colorRatio.IsNaN() || colorRatio.IsInfinite())
				colorRatio = 0;

			direction.Normalize();

			WireLine line = new WireLine
			{
				Point1 = position,
				Point2 = position + direction,
				Color = palette.GetColor(colorRatio)
			};

			return line;
		}
	}
}
