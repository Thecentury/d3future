using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using System.Threading;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Filters
{
	public class ParallelClusteringFilter : GroupFilter
	{
		private int xClustersNum = 10;
		private int yClustersNum = 10;

		protected internal override IEnumerable<IndexWrapper<Point>> Filter(IEnumerable<IndexWrapper<Point>> series)
		{
			var transform = Viewport.Transform;

			// determining bounds of point series
			DataRect bounds = DataRect.Empty;
			foreach (var point in series)
			{
				bounds.Union(point.Data);
			}

			if (bounds.IsEmpty)
				return series;

			double xMin = bounds.XMin;
			double yMin = bounds.YMin;
			double clusterWidth = bounds.Width / xClustersNum;
			double clusterHeight = bounds.Height / yClustersNum;

			int clustersNum = xClustersNum * yClustersNum;
			List<IndexWrapper<Point>>[] seriesParts = new List<IndexWrapper<Point>>[clustersNum];
			for (int i = 0; i < clustersNum; i++)
			{
				seriesParts[i] = new List<IndexWrapper<Point>>();
			}

			foreach (var wrapper in series)
			{
				var point = wrapper.Data;

				int clusterIndexX = (int)Math.Max(0, Math.Min(xClustersNum - 1, (int)(Math.Floor((point.X - xMin) / clusterWidth))));
				int clusterIndexY = (int)Math.Max(0, Math.Min(yClustersNum - 1, (int)(Math.Floor((point.Y - yMin) / clusterHeight))));

				int index = clusterIndexX + clusterIndexY * xClustersNum;
				seriesParts[index].Add(wrapper);
			}

			List<IndexWrapper<Point>>[] filteredParts = new List<IndexWrapper<Point>>[clustersNum];
			for (int i = 0; i < clustersNum; i++)
			{
				filteredParts[i] = new List<IndexWrapper<Point>>();
			}

			var markerSize = MarkerSize;

			for (int i = 0; i < clustersNum; i++)
			{
				var part = seriesParts[i];
				var result = filteredParts[i];

				foreach (var wrapper in part)
				{
					var point = wrapper.Data;
					double minDistance = Double.PositiveInfinity;
					var pointOnScreen = point.DataToScreen(transform);

					foreach (var root in result)
					{
						var rootPoint = root.Data;
						var rootOnScreen = rootPoint.DataToScreen(transform);
						var distance = (rootOnScreen - pointOnScreen).Length;
						if (distance < markerSize)
						{
							minDistance = distance;
							break;
						}
					}

					if (minDistance > markerSize)
						result.Add(wrapper);
				}
			}

			IEnumerable<IndexWrapper<Point>> resultSeries = Enumerable.Empty<IndexWrapper<Point>>();
			foreach (var part in filteredParts)
			{
				resultSeries = resultSeries.Concat(part);
			}

			return resultSeries;
		}
	}
}
