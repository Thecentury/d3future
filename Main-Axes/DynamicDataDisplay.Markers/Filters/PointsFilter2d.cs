using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Filters;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Filters
{
	public abstract class PointsFilter2d : DependencyObject, IDisposable, IWeakEventListener
	{
		private IDataSourceEnvironment environment;
		protected internal IDataSourceEnvironment Environment
		{
			get { return environment; }
			set
			{
				environment = value;

				environment.Plotter.Dispatcher.Invoke(() =>
				{
					Viewport = environment.Plotter.Viewport;
				}, DispatcherPriority.Send);
			}
		}

		private Viewport2D viewport;
		protected Viewport2D Viewport
		{
			get { return viewport; }
			set
			{
				if (viewport != value)
				{
					viewport = value;
					// Use weak events to prevent memory leak. Fast, but not best solution
					ExtendedPropertyChangedEventManager.AddListener(viewport, this);
				}

				viewport.Dispatcher.Invoke(() =>
				{
					Visible = viewport.Visible;
					Output = viewport.Output;
					Transform = viewport.Transform;
				}, DispatcherPriority.Send);
			}
		}

		protected virtual void OnViewportPropertyChanged(ExtendedPropertyChangedEventArgs e) { }

		protected CoordinateTransform Transform { get; private set; }

		protected DataRect Visible { get; private set; }

		protected Rect Output { get; private set; }

		protected static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PointsFilter2d filter = (PointsFilter2d)d;
			filter.RaiseChanged();
		}

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}

		protected internal event EventHandler Changed;

		protected internal abstract IEnumerable<IndexWrapper<Point>> Filter(IEnumerable<IndexWrapper<Point>> series);

		#region IDisposable Members

        public void Dispose() // Do this method ever called?
		{
            ExtendedPropertyChangedEventManager.RemoveListener(viewport, this);
		}

		#endregion

        #region IWeakEventListener Members

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(ExtendedPropertyChangedEventManager))
            {
                OnViewportPropertyChanged((ExtendedPropertyChangedEventArgs)e);
                return true;
            }
            return false;
        }

        #endregion
    }

    public class ExtendedPropertyChangedEventManager : WeakEventManager
    {
        private ExtendedPropertyChangedEventManager() { }

        public static void AddListener(Viewport2D vp, IWeakEventListener listener)
        {
            ExtendedPropertyChangedEventManager.CurrentManager.ProtectedAddListener(vp, listener);
        }

        public static void RemoveListener(Viewport2D vp, IWeakEventListener listener)
        {
            ExtendedPropertyChangedEventManager.CurrentManager.ProtectedRemoveListener(vp, listener);
        }

        protected override void StartListening(object source)
        {
            Viewport2D vp = (Viewport2D)source;
            vp.PropertyChanged += ViewportPropertyChanged;
        }

        void ViewportPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            base.DeliverEvent(sender, e);
        }

        protected override void StopListening(object source)
        {
            Viewport2D vp = (Viewport2D)source;
            vp.PropertyChanged -= ViewportPropertyChanged; 
        }

        private static ExtendedPropertyChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(ExtendedPropertyChangedEventManager);
                ExtendedPropertyChangedEventManager manager = (ExtendedPropertyChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new ExtendedPropertyChangedEventManager();
                    WeakEventManager.SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }
    }
}
