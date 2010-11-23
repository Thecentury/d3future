using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network
{
	public sealed class NetworkAvailabilityManager : WeakEventManager
	{
		/// <summary>
		/// Collection of connected listeners, for debug purposes.
		/// </summary>
		private static readonly ObservableCollection<IWeakEventListener> listeners = new ObservableCollection<IWeakEventListener>();

		private NetworkAvailabilityManager()
		{
			NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
		}

		public static void AddListener(IWeakEventListener listener)
		{
			CurrentManager.ProtectedAddListener(typeof(NetworkChange), listener);
			listeners.Add(listener);
		}

		public static void RemoveListener(IWeakEventListener listener)
		{
			CurrentManager.ProtectedRemoveListener(typeof(NetworkChange), listener);
			listeners.Remove(listener);
		}

		protected override void StartListening(object source)
		{
		}

		void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			DeliverEvent(sender, e);
		}

		protected override void StopListening(object source)
		{
		}

		private static NetworkAvailabilityManager CurrentManager
		{
			get
			{
				Type managerType = typeof(NetworkAvailabilityManager);
				NetworkAvailabilityManager currentManager = null;
				try
				{
					currentManager = (NetworkAvailabilityManager)WeakEventManager.GetCurrentManager(managerType);
				}
				catch (InvalidOperationException)
				{
					// nothing to do
				}

				if (currentManager == null)
				{
					currentManager = new NetworkAvailabilityManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}

				return currentManager;
			}
		}
	}
}
