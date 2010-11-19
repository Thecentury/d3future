using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.ViewportConstraints
{
	public delegate DataRect ViewportConstraintCallback(DataRect proposedDataRect);

	public class InjectionDelegateConstraint : ViewportConstraint
	{
		public InjectionDelegateConstraint(Viewport2D masterViewport, ViewportConstraintCallback callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (masterViewport == null)
				throw new ArgumentNullException("masterViewport");

			this.callback = callback;
			this.masterViewport = masterViewport;
			masterViewport.PropertyChanged += masterViewport_PropertyChanged;
		}

		void masterViewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				RaiseChanged();
			}
		}

		private Viewport2D masterViewport;
		private ViewportConstraintCallback callback;

		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			return callback(proposedDataRect);
		}
	}
}
