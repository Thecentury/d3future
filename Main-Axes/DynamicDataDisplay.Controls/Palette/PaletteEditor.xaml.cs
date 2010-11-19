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
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	/// <summary>
	/// Interaction logic for PaletteEditor.xaml
	/// </summary>
	public partial class PaletteEditor : UserControl
	{
		public PaletteEditor()
		{
			InitializeComponent();
		}

		#region Properties

		#region Palette property

		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(IPalette),
		  typeof(PaletteEditor),
		  new FrameworkPropertyMetadata(null, OnPaletteReplaced));

		private static void OnPaletteReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PaletteEditor owner = (PaletteEditor)d;

		}

		#endregion Palette property

		#region Range property

		public Range<double> Range
		{
			get { return (Range<double>)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}

		public static readonly DependencyProperty RangeProperty = DependencyProperty.Register(
		  "Range",
		  typeof(Range<double>),
		  typeof(PaletteEditor),
		  new FrameworkPropertyMetadata(new Range<double>(0, 1), OnRangeReplaced));

		private static void OnRangeReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PaletteEditor owner = (PaletteEditor)d;

		}

		#endregion Range property

		#endregion Properties
	}
}
