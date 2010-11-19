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
	public abstract class PointDataSourceBase : DispatcherObject, INotifyCollectionChanged, IDisposable
	{

		#region Protected

		protected void TrySubscribeOnCollectionChanged(object collection)
		{
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
				observableCollection.CollectionChanged += OnCollectionChanged;
		}

		protected void TryUnsubscribeFromCollectionChanged(object collection)
		{
			INotifyCollectionChanged observableCollection = collection as INotifyCollectionChanged;
			if (observableCollection != null)
				observableCollection.CollectionChanged -= OnCollectionChanged;
		}

		protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged.Raise(this, e);
		}

		protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged.Raise(this, e);
		}

		#endregion // end of Protected

		#region Public interface

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public void RaiseCollectionReset()
		{
			CollectionChanged.Raise(this);
		}

		[Obsolete]
		public void RaiseAdded(object parameters)
		{
			throw new NotImplementedException();
		}

		public IEnumerable GetData()
		{
			return data;
		}

		IEnumerable data;
		public void PrepairData(bool async)
		{
			if (!async)
			{
				data = GetDataCore();
			}
			else
			{
#if !RELEASEXBAP
				ThreadPool.QueueUserWorkItem((unused) =>
				{
					// todo probably get rid of those tries and catches
					try
					{
						data = GetDataCore();
					}
					catch (Exception exc)
					{
						Debug.WriteLine("Exception in PrepairData: " + exc.Message);
						data = new Point[0]; // Empty set of points
					}
					try
					{
						DataPrepaired.Raise(this);
					}
					catch (Exception exc)
					{
						Debug.WriteLine("Exception in DataPrepaired event handler: " + exc.Message);
					}
				});//.WithExceptionThrowingInDispatcher(Dispatcher);
#else 
				data = GetDataCore();
#endif
			}
		}

		public event EventHandler DataPrepaired;

		[Obsolete("Incomplete")]
		public virtual IEnumerable GetData(int startingIndex)
		{
			throw new NotImplementedException();
		}

		protected abstract IEnumerable GetDataCore();

		public abstract object GetDataType();

		private IDataSourceEnvironment environment = null;
		public IDataSourceEnvironment Environment
		{
			get { return environment; }
			set
			{
				environment = value;
				OnEnvironmentChanged();
			}
		}

		protected virtual void OnEnvironmentChanged()
		{
		}

		private readonly NewFilterCollection filters = new NewFilterCollection();

		#region Conversion delegates

		public Func<object, Point> DataToPoint { get; set; }
		public Func<Point, object> PointToData { get; set; }

		#endregion // end of Conversion delegates

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NewFilterCollection Filters { get { return filters; } }

		#endregion // end of Public interface

		#region IDisposable Members

		public void Dispose()
		{
			filters.Clear();
		}

		#endregion
	}
}
