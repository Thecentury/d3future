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
	/// <summary>
	/// Represents a MVVM-friendly way to manage charts.
	/// </summary>
	[ContentProperty("Template")]
	public sealed class TemplateChart : FrameworkElement, IPlotterElement
	{
		private readonly Dictionary<object, IPlotterElement> cache = new Dictionary<object, IPlotterElement>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TemplateChart"/> class.
		/// </summary>
		public TemplateChart() { }

		#region Properties

		/// <summary>
		/// Gets or sets the data items, used as a dataSource for generated charts. This is a DependencyProperty.
		/// </summary>
		/// <value>The items.</value>
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

			foreach (var element in cache.Values)
			{
				plotter.Children.Remove(element);
			}
			cache.Clear();

			foreach (var item in Items)
			{
				CreateElementAndAdd(item);
			}
		}

		private void CreateElementAndAdd(object item)
		{
			FrameworkElement chart = (FrameworkElement)Template.LoadContent();
			chart.DataContext = item;

			IPlotterElement plotterElement = (IPlotterElement)chart;
			plotter.Children.Add(plotterElement);
			cache.Add(item, plotterElement);
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
			if (plotter == null)
				return;
			if (Template == null)
				return;

			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				UpdateItems();
				return;
			}

			if (e.OldItems != null)
			{
				foreach (var removedData in e.OldItems)
				{
					var plotterElement = cache[removedData];

					plotter.Children.Remove(plotterElement);

					cache.Remove(removedData);
				}
			}

			if (e.NewItems != null)
			{
				foreach (var addedData in e.NewItems)
				{
					CreateElementAndAdd(addedData);
				}
			}
		}

		private void DetachOldItems(IEnumerable items)
		{
			INotifyCollectionChanged observable = items as INotifyCollectionChanged;
			if (observable != null)
			{ 
				observable.CollectionChanged -= OnItems_CollectionChanged;
			}
		}

		/// <summary>
		/// Gets or sets the template, used to generate charts for each data item. This is a DependencyProperty.
		/// </summary>
		/// <value>The template.</value>
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

		/// <summary>
		/// Called when parent plotter is attached.
		/// Allows to, for example, add custom UI parts to ChartPlotter's visual tree or subscribe to ChartPlotter's events.
		/// </summary>
		/// <param name="plotter">The parent plotter.</param>
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			SetBinding(DataContextProperty, new Binding("DataContext") { Source = plotter });
			UpdateItems();
		}

		/// <summary>
		/// Called when item is being detached from parent plotter.
		/// Allows to remove added in OnPlotterAttached method UI parts or unsubscribe from events.
		/// This should be done as each chart can be added only one Plotter at one moment of time.
		/// </summary>
		/// <param name="plotter">The plotter.</param>
		public void OnPlotterDetaching(Plotter plotter)
		{
			foreach (var element in cache.Values)
			{
				plotter.Children.Remove(element);
			}
			BindingOperations.ClearBinding(this, DataContextProperty);
			this.plotter = null;
		}

		/// <summary>
		/// Gets the parent plotter of chart.
		/// Should be equal to null if item is not connected to any plotter.
		/// </summary>
		/// <value>The plotter.</value>
		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		/// <summary>
		/// Gets the parent plotter of chart.
		/// Should be equal to null if item is not connected to any plotter.
		/// </summary>
		/// <value>The plotter.</value>
		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion
	}
}
