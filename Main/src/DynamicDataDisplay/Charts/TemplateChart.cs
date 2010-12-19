using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	[ContentProperty("Template")]
	public sealed class TemplateChart : FrameworkElement, IPlotterElement
	{
		private readonly List<IPlotterElement> elements = new List<IPlotterElement>();

		#region Properties

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IEnumerable Items
		{
			get { return (IEnumerable)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
		  "Items",
		  typeof(IEnumerable),
		  typeof(TemplateChart),
		  new FrameworkPropertyMetadata(null, OnItemsReplaced));

		private static void OnItemsReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TemplateChart owner = (TemplateChart)d;

			IEnumerable oldValue = (IEnumerable)e.OldValue;
			IEnumerable newValue = (IEnumerable)e.NewValue;

			owner.DetachOldItems(oldValue);
			owner.AttachNewItems(newValue);
			owner.UpdateItems();
		}

		private void UpdateItems()
		{
			if (plotter == null)
				return;
			if (Template == null)
				return;
			if (Items == null)
				return;

			foreach (var element in elements)
			{
				plotter.Children.Remove(element);
			}
			elements.Clear();

			foreach (var item in Items)
			{
				FrameworkElement chart = (FrameworkElement)Template.LoadContent();
				chart.DataContext = item;

				IPlotterElement plotterElement = (IPlotterElement)chart;
				plotter.Children.Add(plotterElement);
				elements.Add(plotterElement);
			}
		}

		private void AttachNewItems(IEnumerable items)
		{
			INotifyCollectionChanged observable = items as INotifyCollectionChanged;
			if (observable != null)
			{
				observable.CollectionChanged += OnItems_CollectionChanged;
			}
		}

		private void OnItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateItems();
		}

		private void DetachOldItems(IEnumerable items)
		{
			INotifyCollectionChanged observable = items as INotifyCollectionChanged;
			if (observable != null)
			{
				observable.CollectionChanged -= OnItems_CollectionChanged;
			}
		}

		public ControlTemplate Template
		{
			get { return (ControlTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}

		public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register(
		  "Template",
		  typeof(ControlTemplate),
		  typeof(TemplateChart),
		  new FrameworkPropertyMetadata(null, OnTemplateReplaced));

		private static void OnTemplateReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TemplateChart owner = (TemplateChart)d;
			owner.UpdateItems();
		}

		#endregion

		private Plotter2D plotter;

		#region IPlotterElement Members

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			SetBinding(DataContextProperty, new Binding("DataContext") { Source = plotter });
			UpdateItems();
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			foreach (var element in elements)
			{
				plotter.Children.Remove(element);
			}
			BindingOperations.ClearBinding(this, DataContextProperty);
			this.plotter = null;
		}

		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
