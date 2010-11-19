using System.Linq;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Specialized;

namespace Microsoft.Research.DynamicDataDisplay.ViewportConstraints
{
	/// <summary>
	/// Represents a collection of <see cref="ViewportConstraint"/>s.
	/// <remarks>
	/// ViewportConstraint that is being added should not be null.
	/// </remarks>
	/// </summary>
	public sealed class ConstraintCollection : D3Collection<ViewportConstraint>
	{
		private readonly Viewport2D viewport;
		internal ConstraintCollection(Viewport2D viewport)
		{
			if (viewport == null)
				throw new ArgumentNullException("viewport");

			this.viewport = viewport;
		}

		protected override void OnItemAdding(ViewportConstraint item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
		}

		protected override void OnItemAdded(ViewportConstraint item)
		{
			item.Changed += OnItemChanged;
			ISupportAttachToViewport attachable = item as ISupportAttachToViewport;
			if (attachable != null)
			{
				attachable.Attach(viewport);
			}
		}

		private void OnItemChanged(object sender, EventArgs e)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnItemRemoving(ViewportConstraint item)
		{
			ISupportAttachToViewport attachable = item as ISupportAttachToViewport;
			if (attachable != null)
			{
				attachable.Detach(viewport);
			}
			item.Changed -= OnItemChanged;
		}

		internal DataRect Apply(DataRect oldVisible, DataRect newVisible, Viewport2D viewport)
		{
			DataRect res = newVisible;
			foreach (var constraint in this)
			{
				res = constraint.Apply(oldVisible, res, viewport);
			}
			return res;
		}
	}
}
