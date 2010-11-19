using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Diagnostics;
using forms = System.Windows.Forms;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Interaction logic for DiscretePaletteEditor.xaml
	/// </summary>
	[ContentProperty("Palette")]
	public partial class DiscretePaletteEditor : UserControl
	{
		public DiscretePaletteEditor()
		{
			InitializeComponent();

			paletteControl.SetBinding(PaletteControl.PaletteProperty, new Binding("Palette") { Source = this });

			Loaded += new RoutedEventHandler(DiscretePaletteEditor_Loaded);
		}

		private void DiscretePaletteEditor_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.Children.Remove(plotter.MouseNavigation);
			plotter.Children.Remove(plotter.KeyboardNavigation);
			plotter.Children.Remove(plotter.DefaultContextMenu);
		}

		#region Properties

		#region Palette property

		public DiscretePalette Palette
		{
			get { return (DiscretePalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(DiscretePalette),
		  typeof(DiscretePaletteEditor),
		  new FrameworkPropertyMetadata(null, OnPaletteReplaced));

		private static void OnPaletteReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			DiscretePaletteEditor owner = (DiscretePaletteEditor)d;
			owner.OnPaletteReplaced();
		}

		private void OnPaletteReplaced()
		{
			RebuildSplitters();
		}

		#endregion

		#region Index attached DP

		public static int GetIndex(DependencyObject obj)
		{
			return (int)obj.GetValue(IndexProperty);
		}

		public static void SetIndex(DependencyObject obj, int value)
		{
			obj.SetValue(IndexProperty, value);
		}

		public static readonly DependencyProperty IndexProperty = DependencyProperty.RegisterAttached(
		  "Index",
		  typeof(int),
		  typeof(DiscretePaletteEditor),
		  new FrameworkPropertyMetadata(0));

		#endregion Index attached DP

		#endregion Properties

		private readonly List<PaletteThumb> thumbs = new List<PaletteThumb>();
		private void RebuildSplitters()
		{
			plotter.Children.RemoveAll<PaletteThumb>();
			hostPanel.Children.Clear();

			if (Palette == null) return;

			var palette = Palette;
			int count = palette.Steps.Count;

			int counter = 0;

			// adding thumbs
			foreach (var colorStep in palette.Steps.Take(count - 1))
			{
				PaletteThumb thumb = new PaletteThumb { Position = new Point(colorStep.Offset, 0.5) };
				SetIndex(thumb, counter);
				thumb.PositionChanged += new EventHandler<Charts.PositionChangedEventArgs>(thumb_PositionChanged);
				counter++;

				thumbs.Add(thumb);
				plotter.Children.Add(thumb);
			}

			// adding space between thumbs
			double prevOffset = 0;
			foreach (var colorStep in palette.Steps)
			{
				Rectangle rect = new Rectangle { Fill = Brushes.Transparent };
				Rect bounds = new Rect(prevOffset, 0, colorStep.Offset, 1);
				ViewportPanel.SetViewportBounds(rect, bounds);

				hostPanel.Children.Add(rect);

				prevOffset = colorStep.Offset;
			}
		}

		void thumb_PositionChanged(object sender, PositionChangedEventArgs e)
		{
			int index = GetIndex((DependencyObject)sender);
			UpdatePaletteView(index);
		}

		private void UpdatePaletteView(int index)
		{
			DiscretePalette palette = new DiscretePalette();

			Palette.Steps[index].Offset = thumbs[index].Position.X;
			foreach (var colorStep in Palette.Steps)
			{
				palette.Steps.Add(new LinearPaletteColorStep(colorStep.Color, colorStep.Offset));
			}

			paletteControl.Palette = palette;
		}

		//    private ContextMenu BuildContextMenu() {
		//        ContextMenu menu = new ContextMenu();
		//            MenuItem changeColorMenuItem = new MenuItem { Header = Properties.Resources.PaletteEditor_ChangeColor, Tag = rect };
		//            changeColorMenuItem.Click += (s, e) =>
		//            {
		//                int index = GetIndexFromTag(s);
		//                forms.ColorDialog dialog = new forms.ColorDialog();
		//                if (dialog.ShowDialog() == forms.DialogResult.OK)
		//                {
		//                    var c = dialog.Color;
		//                    Color wpfColor = Color.FromRgb(c.R, c.G, c.B);
		//                    Palette.Steps[index / 2].Color = wpfColor;
		//                    UpdatePaletteLook();
		//                }
		//            };
		//            menu.Items.Add(changeColorMenuItem);

		//            MenuItem deleteColorMenuItem = new MenuItem { Header = Properties.Resources.PaletteEditor_DeleteColor, Tag = rect };
		//            deleteColorMenuItem.Click += (s, e) =>
		//            {
		//                int index = GetIndexFromTag(s);

		//                Palette.Steps.RemoveAt(index / 2);
		//                RebuildSplitters();
		//                UpdatePaletteLook();
		//            };
		//            menu.Items.Add(deleteColorMenuItem);

		//            MenuItem addColorMenuItem = new MenuItem { Header = Properties.Resources.PaletteEditor_AddColor, Tag = rect };
		//            addColorMenuItem.Click += (s, e) =>
		//            {
		//                var x = Mouse.GetPosition(splitterGrid).X;
		//                var ratio = x / (ActualWidth);
		//                int insertIndex = Palette.Steps.Select((step, i) => new { Offset = step.Offset, Index = i }).First(step => step.Offset > ratio).Index;

		//                forms.ColorDialog dialog = new forms.ColorDialog();
		//                if (dialog.ShowDialog() == forms.DialogResult.OK)
		//                {
		//                    var c = dialog.Color;
		//                    Color wpfColor = Color.FromRgb(c.R, c.G, c.B);
		//                    Palette.Steps.Insert(insertIndex, new LinearPaletteColorStep(wpfColor, ratio));
		//                }
		//    }


		//const int splitterWidth = 3;
		//private void RebuildSplitters()
		//{
		//    splitterGrid.ColumnDefinitions.Clear();
		//    splitterGrid.Children.Clear();

		//    int rectColumnIndex = 0;
		//    foreach (var step in Palette.Steps)
		//    {
		//        double width = step.Offset;
		//        if (rectColumnIndex > 1)
		//            width = Palette.Steps[rectColumnIndex / 2].Offset - Palette.Steps[rectColumnIndex / 2 - 1].Offset;

		//        rectColumnIndex = AddColumn(rectColumnIndex, width);

		//        ColumnDefinition splitterColumn = new ColumnDefinition { Width = new GridLength(splitterWidth) };
		//        splitterGrid.ColumnDefinitions.Add(splitterColumn);
		//        GridSplitter splitter = new GridSplitter
		//        {
		//            Width = splitterWidth,
		//            VerticalAlignment = VerticalAlignment.Stretch,
		//            HorizontalAlignment = HorizontalAlignment.Center,
		//            Background = Brushes.DarkGray,
		//            ToolTip = Properties.Resources.PaletteEditor_DragToChangeOffset
		//        };
		//        Grid.SetColumn(splitter, rectColumnIndex);
		//        rectColumnIndex++;
		//        splitterGrid.Children.Add(splitter);
		//    }

		//    AddColumn(rectColumnIndex, 1.0 - Palette.Steps[Palette.Steps.Count - 1].Offset);
		//}

		//private int AddColumn(int rectColumnIndex, double width)
		//{
		//    Rectangle rect = new Rectangle
		//    {
		//        Stretch = Stretch.Fill,
		//        Fill = Brushes.Transparent,
		//        Tag = rectColumnIndex
		//    };

		//    ContextMenu menu = new ContextMenu();
		//    MenuItem changeColorMenuItem = new MenuItem { Header = Properties.Resources.PaletteEditor_ChangeColor, Tag = rect };
		//    changeColorMenuItem.Click += (s, e) =>
		//    {
		//        int index = GetIndexFromTag(s);
		//        forms.ColorDialog dialog = new forms.ColorDialog();
		//        if (dialog.ShowDialog() == forms.DialogResult.OK)
		//        {
		//            var c = dialog.Color;
		//            Color wpfColor = Color.FromRgb(c.R, c.G, c.B);
		//            Palette.Steps[index / 2].Color = wpfColor;
		//            UpdatePaletteLook();
		//        }
		//    };
		//    menu.Items.Add(changeColorMenuItem);

		//    MenuItem deleteColorMenuItem = new MenuItem { Header = Properties.Resources.PaletteEditor_DeleteColor, Tag = rect };
		//    deleteColorMenuItem.Click += (s, e) =>
		//    {
		//        int index = GetIndexFromTag(s);

		//        Palette.Steps.RemoveAt(index / 2);
		//        RebuildSplitters();
		//        UpdatePaletteLook();
		//    };
		//    menu.Items.Add(deleteColorMenuItem);

		//    MenuItem addColorMenuItem = new MenuItem { Header = Properties.Resources.PaletteEditor_AddColor, Tag = rect };
		//    addColorMenuItem.Click += (s, e) =>
		//    {
		//        var x = Mouse.GetPosition(splitterGrid).X;
		//        var ratio = x / (ActualWidth);
		//        int insertIndex = Palette.Steps.Select((step, i) => new { Offset = step.Offset, Index = i }).First(step => step.Offset > ratio).Index;

		//        forms.ColorDialog dialog = new forms.ColorDialog();
		//        if (dialog.ShowDialog() == forms.DialogResult.OK)
		//        {
		//            var c = dialog.Color;
		//            Color wpfColor = Color.FromRgb(c.R, c.G, c.B);
		//            Palette.Steps.Insert(insertIndex, new LinearPaletteColorStep(wpfColor, ratio));
		//        }

		//        RebuildSplitters();
		//        UpdatePaletteLook();
		//    };
		//    menu.Items.Add(addColorMenuItem);

		//    ColumnDefinition stepColumn = new ColumnDefinition { Width = new GridLength(width, GridUnitType.Star) };
		//    splitterGrid.ColumnDefinitions.Add(stepColumn);

		//    rect.ContextMenu = menu;

		//    Dispatcher.BeginInvoke(() => rect.SizeChanged += new SizeChangedEventHandler(rect_SizeChanged), DispatcherPriority.Background);

		//    Grid.SetColumn(rect, rectColumnIndex);
		//    splitterGrid.Children.Add(rect);
		//    rectColumnIndex++;
		//    return rectColumnIndex;
		//}

		//private static int GetIndexFromTag(object s)
		//{
		//    FrameworkElement owner = (FrameworkElement)((FrameworkElement)s).Tag;
		//    int index = (int)owner.Tag;

		//    return index;
		//}

		//private void rect_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//    Rectangle rect = (Rectangle)sender;
		//    int index = (int)rect.Tag;
		//    index /= 2;
		//    if (index < Palette.Steps.Count)
		//    {
		//        double width = rect.ActualWidth / splitterGrid.ActualWidth;

		//        if (index > 0)
		//            width += Palette.Steps[index - 1].Offset;

		//        Palette.Steps[index].Offset = width;
		//    }

		//    UpdatePaletteLook();
		//}

		//private void UpdatePaletteLook()
		//{
		//    DiscretePalette palette = new DiscretePalette();
		//    palette.EndColor = Palette.EndColor;

		//    double offset = 0;
		//    for (int i = 0; i < Palette.Steps.Count; i++)
		//    {
		//        offset += splitterGrid.ColumnDefinitions[2 * i].Width.Value / (ActualWidth - Palette.Steps.Count * splitterWidth);
		//        palette.Steps.Add(new LinearPaletteColorStep(Palette.Steps[i].Color, offset));
		//    }

		//    paletteControl.Palette = palette;
		//}
	}
}

