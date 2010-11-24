using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Collections.Specialized;
using System.Windows;
using DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Filters
{
	public sealed class NewFilterCollection : D3Collection<PointsFilter2d>, IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FilterCollection"/> class.
		/// </summary>
		public NewFilterCollection() { }

		protected override void OnItemAdding(PointsFilter2d item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
		}

		protected override void OnItemAdded(PointsFilter2d item)
		{
			item.Changed += OnItemChanged;
		}

		private void OnItemChanged(object sender, EventArgs e)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnItemRemoving(PointsFilter2d item)
		{
			item.Changed -= OnItemChanged;
		}

		internal IEnumerable<IndexWrapper<Point>> Filter(IEnumerable<IndexWrapper<Point>> points, DataSourceEnvironment environment)
		{
			foreach (var filter in Items)
			{
				filter.Environment = environment;
				points = filter.Filter(points);
			}

			return points;
		}

		#region IDisposable Members

		public void Dispose()
		{
			foreach (var filter in this)
			{
				filter.Dispose();
			}
		}

		#endregion
	}
}
