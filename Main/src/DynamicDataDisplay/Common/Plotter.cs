using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.UndoSystem;
using Microsoft.Research.DynamicDataDisplay.Navigation;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>Plotter is a base control for displaying various graphs. It provides
	/// means to draw chart itself and side space for axes, annotations, etc</summary>
	[ContentProperty("Children")]
	[TemplatePart(Name = "PART_HeaderPanel", Type = typeof(StackPanel))]
	[TemplatePart(Name = "PART_FooterPanel", Type = typeof(StackPanel))]

	[TemplatePart(Name = "PART_BottomPanel", Type = typeof(StackPanel))]
	[TemplatePart(Name = "PART_LeftPanel", Type = typeof(StackPanel))]
	[TemplatePart(Name = "PART_RightPanel", Type = typeof(StackPanel))]
	[TemplatePart(Name = "PART_TopPanel", Type = typeof(StackPanel))]

	[TemplatePart(Name = "PART_MainCanvas", Type = typeof(Canvas))]
	[TemplatePart(Name = "PART_CentralGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_MainGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_ContentsGrid", Type = typeof(Grid))]
	[TemplatePart(Name = "PART_ParallelCanvas", Type = typeof(Canvas))]
	public abstract class Plotter : ContentControl
	{
		private PlotterLoadMode loadMode = PlotterLoadMode.Normal;
		protected PlotterLoadMode LoadMode
		{
			get { return loadMode; }
		}

		private static Plotter current;
		/// <summary>
		/// Gets the current plotter. Used in VisualDebug.
		/// </summary>
		/// <value>The current.</value>
		internal static Plotter Current
		{
			get { return Plotter.current; }
		}

		protected Plotter() : this(PlotterLoadMode.Normal) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Plotter"/> class.
		/// </summary>
		protected Plotter(PlotterLoadMode loadMode)
		{
			current = this;

			this.loadMode = loadMode;

			SetPlotter(this, this);

			if (loadMode == PlotterLoadMode.Normal)
			{
				UpdateUIParts();
			}

			children = new PlotterChildrenCollection(this);
			children.CollectionChanged += OnChildrenCollectionChanged;
			Loaded += Plotter_Loaded;
			Unloaded += Plotter_Unloaded;

			genericResources = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay;component/Themes/Generic.xaml", UriKind.Relative));

			ContextMenu = null;

			foreach (var panel in GetAllPanels().Where(panel => panel != null))
			{
				Plotter.SetIsDefaultPanel(panel, true);
			}
		}

		private void Plotter_Unloaded(object sender, RoutedEventArgs e)
		{
			OnUnloaded();
		}

		protected virtual void OnUnloaded() { }

		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new PlotterAutomationPeer(this);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ShouldSerializeContent()
		{
			return false;
		}

		protected override bool ShouldSerializeProperty(DependencyProperty dp)
		{
			// do not serialize context menu if it was created by DefaultContextMenu, because that context menu items contains references of plotter
			if (dp == ContextMenuProperty && children.Any(el => el is DefaultContextMenu)) return false;
			if (dp == TemplateProperty) return false;
			if (dp == ContentProperty) return false;

			return base.ShouldSerializeProperty(dp);
		}

		private const string templateKey = "defaultPlotterTemplate";
		private const string styleKey = "defaultPlotterStyle";
		private void UpdateUIParts()
		{
			ResourceDictionary dict = new ResourceDictionary
			{
				Source = new Uri("/DynamicDataDisplay;component/Common/PlotterStyle.xaml", UriKind.Relative)
			};

			Resources.MergedDictionaries.Add(dict);

			Style = (Style)dict[styleKey];

			ControlTemplate template = (ControlTemplate)dict[templateKey];
			Template = template;
			ApplyTemplate();
		}

		private ResourceDictionary genericResources;
		protected ResourceDictionary GenericResources
		{
			get { return genericResources; }
		}

		/// <summary>
		/// Forces plotter to load.
		/// </summary>
		public void PerformLoad()
		{
			isLoadedIntensionally = true;
			ApplyTemplate();

			Plotter_Loaded(null, null);
		}

		private bool isLoadedIntensionally = false;
		protected virtual bool IsLoadedInternal
		{
			get { return isLoadedIntensionally || IsLoaded; }
		}

		private void Plotter_Loaded(object sender, RoutedEventArgs e)
		{
			ExecuteWaitingChildrenAdditions();

			OnLoaded();
		}

		protected internal void ExecuteWaitingChildrenAdditions()
		{
			executedWaitingChildrenAdding = true;

			foreach (var pair in waitingForExecute)
			{
				pair.Value.Invoke();
			}

			waitingForExecute.Clear();
		}

		protected virtual void OnLoaded()
		{
			// this is done to enable keyboard shortcuts
			Focus();
		}

		protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
		{
			base.OnTemplateChanged(oldTemplate, newTemplate);
		}

		private Grid contentsGrid;
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var headerPanel = GetPart<StackPanel>("PART_HeaderPanel");
			MigrateChildren(this.headerPanel, headerPanel);
			this.headerPanel = headerPanel;

			var footerPanel = GetPart<StackPanel>("PART_FooterPanel");
			MigrateChildren(this.footerPanel, footerPanel);
			this.footerPanel = footerPanel;

			var leftPanel = GetPart<StackPanel>("PART_LeftPanel");
			MigrateChildren(this.leftPanel, leftPanel);
			this.leftPanel = leftPanel;

			var bottomPanel = GetPart<StackPanel>("PART_BottomPanel");
			MigrateChildren(this.bottomPanel, bottomPanel);
			this.bottomPanel = bottomPanel;

			var rightPanel = GetPart<StackPanel>("PART_RightPanel");
			MigrateChildren(this.rightPanel, rightPanel);
			this.rightPanel = rightPanel;

			var topPanel = GetPart<StackPanel>("PART_TopPanel");
			MigrateChildren(this.topPanel, topPanel);
			this.topPanel = topPanel;

			var mainCanvas = GetPart<Canvas>("PART_MainCanvas");
			MigrateChildren(this.mainCanvas, mainCanvas);
			this.mainCanvas = mainCanvas;

			var centralGrid = GetPart<Grid>("PART_CentralGrid");
			MigrateChildren(this.centralGrid, centralGrid);
			this.centralGrid = centralGrid;

			var mainGrid = GetPart<Grid>("PART_MainGrid");
			MigrateChildren(this.mainGrid, mainGrid);
			this.mainGrid = mainGrid;

			var parallelCanvas = GetPart<Canvas>("PART_ParallelCanvas");
			MigrateChildren(this.parallelCanvas, parallelCanvas);
			this.parallelCanvas = parallelCanvas;

			var contentsGrid = GetPart<Grid>("PART_ContentsGrid");
			MigrateChildren(this.contentsGrid, contentsGrid);
			this.contentsGrid = contentsGrid;

			Content = contentsGrid;
			AddLogicalChild(contentsGrid);

			ExecuteWaitingChildrenAdditions();
		}

		private void MigrateChildren(Panel previousParent, Panel currentParent)
		{
			if (previousParent != null && currentParent != null)
			{
				UIElement[] children = new UIElement[previousParent.Children.Count];
				previousParent.Children.CopyTo(children, 0);
				previousParent.Children.Clear();

				foreach (var child in children)
				{
					bool isDefaultPanel = GetIsDefaultPanel(child);
					if (!currentParent.Children.Contains(child) && !isDefaultPanel)
					{
						currentParent.Children.Add(child);
					}
				}
			}
			else if (previousParent != null)
			{
				previousParent.Children.Clear();
			}
		}

		internal virtual IEnumerable<Panel> GetAllPanels()
		{
			yield return headerPanel;
			yield return footerPanel;

			yield return leftPanel;
			yield return bottomPanel;
			yield return rightPanel;
			yield return topPanel;

			yield return mainCanvas;
			yield return centralGrid;
			yield return mainGrid;
			yield return parallelCanvas;
			yield return contentsGrid;
		}

		private T GetPart<T>(string name)
		{
			return (T)Template.FindName(name, this);
		}

		#region Children and add/removed events handling

		private readonly PlotterChildrenCollection children;

		/// <summary>
		/// Provides access to Plotter's children charts.
		/// </summary>
		/// <value>The children.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PlotterChildrenCollection Children
		{
			[DebuggerStepThrough]
			get { return children; }
		}

		private readonly Dictionary<IPlotterElement, Action> waitingForExecute = new Dictionary<IPlotterElement, Action>();

		bool executedWaitingChildrenAdding = false;
		private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (IsLoadedInternal && !executedWaitingChildrenAdding)
			{
				ExecuteWaitingChildrenAdditions();
				executedWaitingChildrenAdding = true;
			}

			if (e.NewItems != null)
			{
				foreach (IPlotterElement item in e.NewItems)
				{
					if (IsLoadedInternal)
					{
						OnChildAdded(item);
					}
					else
					{
						waitingForExecute.Remove(item);
						waitingForExecute.Add(item, () => OnChildAdded(item));
					}
				}
			}
			if (e.OldItems != null)
			{
				foreach (IPlotterElement item in e.OldItems)
				{
					if (IsLoadedInternal)
					{
						OnChildRemoving(item);
					}
					else
					{
						waitingForExecute.Remove(item);
						waitingForExecute.Add(item, () => OnChildRemoving(item));
					}
				}
			}
		}

		bool performChildChecks = true;
		internal bool PerformChildChecks
		{
			get { return performChildChecks; }
			set { performChildChecks = value; }
		}

		private IPlotterElement currentChild = null;
		protected IPlotterElement CurrentChild
		{
			get { return currentChild; }
		}

		protected virtual void OnChildAdded(IPlotterElement child)
		{
			if (child != null)
			{
				currentChild = child;

				if (performChildChecks && child.Plotter != null)
				{
					throw new InvalidOperationException(Strings.Exceptions.PlotterElementAddedToAnotherPlotter);
				}

				if (performChildChecks)
				{
					child.OnPlotterAttached(this);
					if (child.Plotter != this)
					{
						throw new InvalidOperationException(Strings.Exceptions.InvalidParentPlotterValue);
					}
				}

				DependencyObject dependencyObject = child as DependencyObject;
				if (dependencyObject != null)
				{
					SetPlotter(dependencyObject, this);
				}
			}
		}

		protected virtual void OnChildRemoving(IPlotterElement child)
		{
			if (child != null)
			{
				// todo probably here child.Plotter can be null.
				if (performChildChecks && child.Plotter != this)
				{
					//throw new InvalidOperationException(Strings.Exceptions.InvalidParentPlotterValueRemoving);
				}

				if (performChildChecks)
				{
					if (child.Plotter != null)
						child.OnPlotterDetaching(this);

					if (child.Plotter != null)
					{
						throw new InvalidOperationException(Strings.Exceptions.ParentPlotterNotNull);
					}
				}

				DependencyObject dependencyObject = child as DependencyObject;
				if (dependencyObject != null)
				{
					SetPlotter(dependencyObject, null);
				}
			}
		}

		#endregion

		#region Layout zones

		private Panel parallelCanvas = new Canvas();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel ParallelCanvas
		{
			get { return parallelCanvas; }
			protected set { parallelCanvas = value; }
		}

		private Panel headerPanel = new StackPanel();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel HeaderPanel
		{
			get { return headerPanel; }
			protected set { headerPanel = value; }
		}

		private Panel footerPanel = new StackPanel();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel FooterPanel
		{
			get { return footerPanel; }
			protected set { footerPanel = value; }
		}

		private Panel leftPanel = new StackPanel();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel LeftPanel
		{
			get { return leftPanel; }
			protected set { leftPanel = value; }
		}

		private Panel rightPanel = new StackPanel();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel RightPanel
		{
			get { return rightPanel; }
			protected set { rightPanel = value; }
		}

		private Panel topPanel = new StackPanel();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel TopPanel
		{
			get { return topPanel; }
			protected set { topPanel = value; }
		}

		private Panel bottomPanel = new StackPanel();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel BottomPanel
		{
			get { return bottomPanel; }
			protected set { bottomPanel = value; }
		}

		private Panel mainCanvas = new Canvas();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel MainCanvas
		{
			get { return mainCanvas; }
			protected set { mainCanvas = value; }
		}

		private Panel centralGrid = new Grid();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel CentralGrid
		{
			get { return centralGrid; }
			protected set { centralGrid = value; }
		}

		private Panel mainGrid = new Grid();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Panel MainGrid
		{
			get { return mainGrid; }
			protected set { mainGrid = value; }
		}

		#endregion

		#region Screenshots & copy to clipboard

		public BitmapSource CreateScreenshot()
		{
			UIElement parent = (UIElement)Parent;

			Rect renderBounds = new Rect(RenderSize);

			Point p1 = renderBounds.TopLeft;
			Point p2 = renderBounds.BottomRight;

			if (parent != null)
			{
				//p1 = TranslatePoint(p1, parent);
				//p2 = TranslatePoint(p2, parent);
			}

			Int32Rect rect = new Rect(p1, p2).ToInt32Rect();

			return ScreenshotHelper.CreateScreenshot(this, rect);
		}


		/// <summary>Saves screenshot to file.</summary>
		/// <param name="filePath">File path.</param>
		public void SaveScreenshot(string filePath)
		{
			ScreenshotHelper.SaveBitmapToFile(CreateScreenshot(), filePath);
		}

		/// <summary>
		/// Saves screenshot to stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="fileExtension">The file type extension.</param>
		public void SaveScreenshotToStream(Stream stream, string fileExtension)
		{
			ScreenshotHelper.SaveBitmapToStream(CreateScreenshot(), stream, fileExtension);
		}

		/// <summary>Copies the screenshot to clipboard.</summary>
		public void CopyScreenshotToClipboard()
		{
			Clipboard.Clear();
			Clipboard.SetImage(CreateScreenshot());
		}

		#endregion

		#region IsDefaultElement attached property

		protected void SetAllChildrenAsDefault()
		{
			foreach (var child in Children.OfType<DependencyObject>())
			{
				child.SetValue(IsDefaultElementProperty, true);
			}
		}

		/// <summary>Gets a value whether specified graphics object is default to this plotter or not</summary>
		/// <param name="obj">Graphics object to check</param>
		/// <returns>True if it is default or false otherwise</returns>
		public static bool GetIsDefaultElement(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsDefaultElementProperty);
		}

		public static void SetIsDefaultElement(DependencyObject obj, bool value)
		{
			obj.SetValue(IsDefaultElementProperty, value);
		}

		public static readonly DependencyProperty IsDefaultElementProperty = DependencyProperty.RegisterAttached(
			"IsDefaultElement",
			typeof(bool),
			typeof(Plotter),
			new UIPropertyMetadata(false));

		/// <summary>Removes all user graphs from given UIElementCollection, 
		/// leaving only default graphs</summary>
		protected static void RemoveUserElements(IList<IPlotterElement> elements)
		{
			int index = 0;

			while (index < elements.Count)
			{
				DependencyObject d = elements[index] as DependencyObject;
				if (d != null && !GetIsDefaultElement(d))
				{
					elements.RemoveAt(index);
				}
				else
				{
					index++;
				}
			}
		}

		public void RemoveUserElements()
		{
			RemoveUserElements(Children);
		}

		#endregion

		#region IsDefaultAxis

		public static bool GetIsDefaultAxis(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsDefaultAxisProperty);
		}

		public static void SetIsDefaultAxis(DependencyObject obj, bool value)
		{
			obj.SetValue(IsDefaultAxisProperty, value);
		}

		public static readonly DependencyProperty IsDefaultAxisProperty = DependencyProperty.RegisterAttached(
			"IsDefaultAxis",
			typeof(bool),
			typeof(Plotter),
			new UIPropertyMetadata(false, OnIsDefaultAxisChanged));

		private static void OnIsDefaultAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Plotter parentPlotter = null;
			IPlotterElement plotterElement = d as IPlotterElement;
			if (plotterElement != null)
			{
				parentPlotter = plotterElement.Plotter;

				if (parentPlotter != null)
				{
					parentPlotter.OnIsDefaultAxisChangedCore(d, e);
				}
			}
		}

		protected virtual void OnIsDefaultAxisChangedCore(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

		#endregion

		#region Undo

		private readonly UndoProvider undoProvider = new UndoProvider();
		public UndoProvider UndoProvider
		{
			get { return undoProvider; }
		}

		#endregion

		#region Plotter attached property

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static Plotter GetPlotter(DependencyObject obj)
		{
			return (Plotter)obj.GetValue(PlotterProperty);
		}

		public static void SetPlotter(DependencyObject obj, Plotter value)
		{
			obj.SetValue(PlotterProperty, value);
		}

		public static readonly DependencyProperty PlotterProperty = DependencyProperty.RegisterAttached(
		  "Plotter",
		  typeof(Plotter),
		  typeof(Plotter),
		  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnPlotterChanged));

		private static void OnPlotterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement element = d as FrameworkElement;
			Plotter prevPlotter = (Plotter)e.OldValue;
			Plotter currPlotter = (Plotter)e.NewValue;

			// raise Plotter[*] events, where * is Attached, Detaching, Changed.
			if (element != null)
			{
				PlotterChangedEventArgs args = new PlotterChangedEventArgs(prevPlotter, currPlotter, PlotterDetachingEvent);

				if (currPlotter == null && prevPlotter != null)
				{
					RaisePlotterEvent(element, args);
				}
				else if (currPlotter != null)
				{
					args.RoutedEvent = PlotterAttachedEvent;
					RaisePlotterEvent(element, args);
				}

				args.RoutedEvent = PlotterChangedEvent;
				RaisePlotterEvent(element, args);
			}
		}

		private static void RaisePlotterEvent(FrameworkElement element, PlotterChangedEventArgs args)
		{
			element.RaiseEvent(args);
			PlotterEvents.Notify(element, args);
		}

		#endregion

		#region Plotter routed events

		public static readonly RoutedEvent PlotterAttachedEvent = EventManager.RegisterRoutedEvent(
			"PlotterAttached",
			RoutingStrategy.Direct,
			typeof(PlotterChangedEventHandler),
			typeof(Plotter));

		public static readonly RoutedEvent PlotterDetachingEvent = EventManager.RegisterRoutedEvent(
			"PlotterDetaching",
			RoutingStrategy.Direct,
			typeof(PlotterChangedEventHandler),
			typeof(Plotter));

		public static readonly RoutedEvent PlotterChangedEvent = EventManager.RegisterRoutedEvent(
			"PlotterChanged",
			RoutingStrategy.Direct,
			typeof(PlotterChangedEventHandler),
			typeof(Plotter));

		#endregion

		#region DefaultPanel property

		/// <summary>
		/// Gets the value indicating that this is a default panel.
		/// Default panels are those that are contained in plotter by default, like MainGrid or CentralCanvas.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		public static bool GetIsDefaultPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsDefaultPanelProperty);
		}

		public static void SetIsDefaultPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(IsDefaultPanelProperty, value);
		}

		public static readonly DependencyProperty IsDefaultPanelProperty = DependencyProperty.RegisterAttached(
		  "IsDefaultPanel",
		  typeof(bool),
		  typeof(Plotter),
		  new FrameworkPropertyMetadata(false));

		#endregion
	}

	public delegate void PlotterChangedEventHandler(object sender, PlotterChangedEventArgs e);
}

