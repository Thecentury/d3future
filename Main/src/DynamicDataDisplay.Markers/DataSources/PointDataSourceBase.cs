using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Specialized;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Charts.Filters;
using System.Threading;

namespace DynamicDataDisplay.Markers.DataSources
{
	public abstract class PointDataSourceBase : INotifyCollectionChanged
	{
		#region Protected

		/// <summary>
		/// Tries to subscribe on collection changed event, if collection supports it.
		/// </summary>
		/// <param name="collection">The collection.</param>
		protected void TrySubscribeOnCollectionChanged(object collection)
		{
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
				observableCollection.CollectionChanged += OnCollectionChanged;
		}

		/// <summary>
		/// Tries to unsubscribe from collection changed event, if collection supports it.
		/// </summary>
		/// <param name="collection">The collection.</param>
		protected void TryUnsubscribeFromCollectionChanged(object collection)
		{
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
				observableCollection.CollectionChanged -= OnCollectionChanged;
		}

		/// <summary>
		/// Called when collection changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged.Raise(this, e);
		}

		#endregion // end of Protected

		/// <summary>
		/// Core method for getting data.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <returns></returns>
		protected abstract IEnumerable GetDataCore(DataSourceEnvironment environment);

		#region Public interface

		/// <summary>
		/// Occurs when the underlying collection changes.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Raises the collection reset event.
		/// Can be used to notify about collection changed event when collection doesn't implement INotifyCollectionChanged interface.
		/// </summary>
		public void RaiseCollectionReset()
		{
			CollectionChanged.Raise(this);
		}

		/// <summary>
		/// Gets the data, used by marker chart.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <returns></returns>
		public IEnumerable GetData(DataSourceEnvironment environment)
		{
			IEnumerable data = GetDataCore(environment);

			return data;
		}

		/// <summary>
		/// Gets the point data, used by line chart.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <returns></returns>
		public IEnumerable<Point> GetPointData(DataSourceEnvironment environment)
		{
			IEnumerable data = GetDataCore(environment);

			return data.Cast<object>().Select(o => DataToPoint(o));
		}

		public DataRect GetContentBounds(IEnumerable data, DataRect visible)
		{
			// todo probably throw an exception or swall this case when DataToPoint delegate is null.
			var visibleData = data.Cast<object>().Select(o => DataToPoint(o));

			DataRect bounds = GetContentBounds(visibleData, visible);

			return bounds;
		}

		public virtual DataRect GetContentBounds(IEnumerable<Point> data, DataRect visible)
		{
			var bounds = data.Aggregate(DataRect.Empty, (rect, point) => DataRect.Union(rect, point));
			return bounds;
		}

		/// <summary>
		/// Gets the type of the data.
		/// </summary>
		/// <returns></returns>
		public virtual object GetDataType()
		{
			return typeof(Object);
		}

		#region Conversion delegates

		/// <summary>
		/// Gets or sets the data to point conversion delegate.
		/// </summary>
		/// <value>The data to point.</value>
		public Func<object, Point> DataToPoint { get; set; }

		/// <summary>
		/// Gets or sets the point to data conversion delegate.
		/// </summary>
		/// <value>The point to data.</value>
		public Func<Point, object> PointToData { get; set; }

		#endregion // end of Conversion delegates

		#endregion // end of Public interface
	}
}
