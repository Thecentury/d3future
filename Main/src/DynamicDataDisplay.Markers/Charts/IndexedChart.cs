using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class IndexedChart : MarkerChartBase
	{
		DefaultUpdateHandler updateHandler = new DefaultUpdateHandler();
		protected override DataSourceUpdateHandler GetUpdateHandler()
		{
			return updateHandler;
		}

		#region DataSource update handlers

		protected internal override void OnAdded(NotifyCollectionChangedEventArgs e)
		{
			var index = e.NewStartingIndex;

			var indexIncrement = e.NewItems.Count;
			for (int i = index; i < CurrentItemsPanel.Children.Count; i++)
			{
				var marker = CurrentItemsPanel.Children[i];
				SetIndex(marker, i + indexIncrement);
			}

			foreach (var item in e.NewItems)
			{
				CreateAndAddMarker(item, index);
				index++;
			}
		}

		protected internal override void OnRemoved(NotifyCollectionChangedEventArgs e)
		{
			var index = e.OldStartingIndex;
			var removingCount = e.OldItems.Count;

			for (int i = index + removingCount; i < CurrentItemsPanel.Children.Count; i++)
			{
				var marker = CurrentItemsPanel.Children[i];
				SetIndex(marker, i - removingCount);
			}

			for (int i = index; i < index + removingCount; i++)
			{
				CurrentItemsPanel.Children.RemoveAt(i);
			}

			ForceUpdateContentBounds();
		}

		protected internal override void OnReplaced(NotifyCollectionChangedEventArgs e)
		{
			var index = e.NewStartingIndex;
			var marker = (FrameworkElement)CurrentItemsPanel.Children[index];
			marker.DataContext = e.NewItems[0];

			ForceUpdateContentBounds();
		}

		#endregion // end of Update handlers
	}
}
