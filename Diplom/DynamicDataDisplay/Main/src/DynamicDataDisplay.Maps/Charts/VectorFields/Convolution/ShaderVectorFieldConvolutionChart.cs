using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<System.Windows.Vector>;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.DynamicDataDisplay.Maps.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts
{
	public class ShaderVectorFieldConvolutionChart : FrameworkElement, IPlotterElement
	{
		private readonly ViewportHostPanel panel = new ViewportHostPanel();
		private readonly ConvolutionEffect effect = new ConvolutionEffect();

		public ShaderVectorFieldConvolutionChart()
		{
			// panel is within visual tree, and the very chart is not
			SetBinding(DataContextProperty, new Binding { Path = new PropertyPath("DataContext"), Source = panel });
			this.Effect = effect;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(Brushes.White, null, new Rect(RenderSize));
		}

		#region Properties

		#region DataSource property

		public DataSource DataSource
		{
			get { return (DataSource)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(DataSource),
		  typeof(ShaderVectorFieldConvolutionChart),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ShaderVectorFieldConvolutionChart owner = (ShaderVectorFieldConvolutionChart)d;
			owner.OnDataSourceReplaced((DataSource)e.OldValue, (DataSource)e.NewValue);
		}

		private void OnDataSourceReplaced(DataSource prevDataSource, DataSource currDataSource)
		{
			if (prevDataSource != null)
				prevDataSource.Changed -= DataSource_OnChanged;
			if (currDataSource != null)
				currDataSource.Changed += DataSource_OnChanged;

			UpdateUI();
		}

		private void DataSource_OnChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		#endregion

		#region Palette property

		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(IPalette),
		  typeof(ShaderVectorFieldConvolutionChart),
		  new FrameworkPropertyMetadata(new PowerPalette(new UniformLinearPalette(Colors.Green, Colors.GreenYellow, Colors.Red), 0.1)));

		#endregion

		#endregion

		private DataRect contentBounds = DataRect.Empty;
		private Range<double> minMaxLength = new Range<double>();
		private Range<double> minMaxX = new Range<double>();
		private Range<double> minMaxY = new Range<double>();
		private ImageBrush noizeBrush;
		private int prevWidth;
		private int prevHeight;
		private void UpdateUI()
		{
			var dataSource = this.GetValueSync<DataSource>(DataSourceProperty);
			if (dataSource == null)
				return;

			var contentBounds = dataSource.Grid.GetGridBounds();
			if (Parent != null)
				Viewport2D.SetContentBounds(Parent, contentBounds);
			ViewportPanel.SetViewportBounds(this, contentBounds);


			var minMaxLength = dataSource.GetMinMaxLength();
			effect.MinLength = (float)minMaxLength.Min;
			effect.MaxLength = (float)minMaxLength.Max;

			effect.Palette = ImageHelper.CreatePaletteBrush(Palette, width: 256);

			var minMaxX = dataSource.GetMinMax(v => v.X);
			var minMaxY = dataSource.GetMinMax(v => v.Y);

			effect.MinX = (float)minMaxX.Min;
			effect.MinY = (float)minMaxY.Min;
			effect.MaxX = (float)minMaxX.Max;
			effect.MaxY = (float)minMaxY.Max;

			int width = dataSource.Width;
			int height = dataSource.Height;

			effect.Width = 10 * width;
			effect.Height = -10 * height;

			if (width != prevWidth || height != prevHeight)
			{
				noizeBrush = new ImageBrush(BitmapFrame.Create(width, height, 96, 96, PixelFormats.Bgra32, null,
					ImageHelper.CreateWhiteNoizeImage(width, height), (width * PixelFormats.Bgra32.BitsPerPixel + 7) / 8));

				prevWidth = width;
				prevHeight = height;
			}
			effect.Noize = noizeBrush;

			var field = dataSource.Data;
			uint[] pixels = new uint[width * height];

			var minX = minMaxX.Min;
			var minY = minMaxY.Min;
			var lenX = minMaxX.GetLength();
			var lenY = minMaxY.GetLength();

			ConcurrentBag<uint> bagX = new ConcurrentBag<uint>();
			ConcurrentBag<uint> bagY = new ConcurrentBag<uint>();

			Parallel.For(0, height, iy =>
			{
				for (int ix = 0; ix < width; ix++)
				{
					// todo убрать checked
					checked
					{
						int i = iy * width + ix;
						var vector = field[ix, height - 1 - iy];

						double xRatio = (vector.X - minX) / lenX;
						double yRatio = (vector.Y - minY) / lenY;

						if (xRatio.IsNaN())
							xRatio = 0.5;
						if (yRatio.IsNaN())
							yRatio = 0.5;

						uint x = (byte)(xRatio * 0xFF);
						uint y = (byte)(yRatio * 0xFF);

						double xFrac = xRatio * 0xFF - x;
						double yFrac = yRatio * 0xFF - y;

						Debug.Assert(0 <= xFrac && xFrac <= 1);
						Debug.Assert(0 <= yFrac && yFrac <= 1);

						uint xFracByte = (uint)(xFrac * 0xFF);
						uint yFracByte = (uint)(yFrac * 0xFF);

						bagX.Add(x);
						bagY.Add(y);

						uint pixel = 0;
						//pixel |= xFracByte << 24;
						//pixel |= yFracByte << 16;
						pixel |= (uint)255 << 24;
						pixel |= y << 8;
						pixel |= x;
						//pixel |= ((uint)255) << 24; // alpha

						pixels[i] = pixel;
					}
				}
			});

			Dictionary<uint, int> xdict = new Dictionary<uint, int>();
			Dictionary<uint, int> ydict = new Dictionary<uint, int>();
			foreach (var item in bagX)
			{
				if (xdict.ContainsKey(item))
					xdict[item]++;
				else
					xdict[item] = 1;
			}

			foreach (var item in bagY)
			{
				if (ydict.ContainsKey(item))
					ydict[item]++;
				else
					ydict[item] = 1;
			}

			ImageBrush fieldBrush = new ImageBrush(BitmapFrame.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, (width * PixelFormats.Bgra32.BitsPerPixel + 7) / 8));
			effect.Field = fieldBrush;
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.Children.BeginAdd(panel);

			panel.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			panel.Children.Remove(this);

			plotter.Children.BeginRemove(panel);
			this.plotter = null;
		}

		public Plotter Plotter
		{
			get { return plotter; }
		}

		#endregion // IPlotterElement Members
	}
}
