using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows;
using Petzold.Media3D;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Maps.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class ConvolutionStack : ModelVisual3D
	{
		private readonly List<Billboard> billboards = new List<Billboard>();

		#region Properties

		#region DataSource property

		public IUniformDataSource3D<Vector3D> DataSource
		{
			get { return (IUniformDataSource3D<Vector3D>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IUniformDataSource3D<Vector3D>),
		  typeof(ConvolutionStack),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ConvolutionStack owner = (ConvolutionStack)d;
			owner.UpdateUI();
		}

		#endregion DataSource property

		#region SectionHeight property

		public double SectionHeight
		{
			get { return (double)GetValue(SectionHeightProperty); }
			set { SetValue(SectionHeightProperty, value); }
		}

		public static readonly DependencyProperty SectionHeightProperty = DependencyProperty.Register(
		  "SectionHeight",
		  typeof(double),
		  typeof(ConvolutionStack),
		  new FrameworkPropertyMetadata(1.0, OnSectionHeightReplaced));

		private static void OnSectionHeightReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ConvolutionStack owner = (ConvolutionStack)d;
			owner.UpdateUI();
		}

		#endregion SectionHeight property

		#region SectionCount property

		public int SectionCount
		{
			get { return (int)GetValue(SectionCountProperty); }
			set { SetValue(SectionCountProperty, value); }
		}

		public static readonly DependencyProperty SectionCountProperty = DependencyProperty.Register(
		  "SectionCount",
		  typeof(int),
		  typeof(ConvolutionStack),
		  new FrameworkPropertyMetadata(20, OnSecitionCountReplaced));

		private static void OnSecitionCountReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ConvolutionStack owner = (ConvolutionStack)d;
			owner.UpdateUI();
		}

		#endregion SectionCount property

		#region RenderingProgress

		public double RenderingProgress
		{
			get { return (double)GetValue(RenderingProgressProperty); }
			set { SetValue(RenderingProgressProperty, value); }
		}

		public static readonly DependencyProperty RenderingProgressProperty = DependencyProperty.Register(
		  "RenderingProgress",
		  typeof(double),
		  typeof(ConvolutionStack),
		  new FrameworkPropertyMetadata(0.0));

		#endregion RenderingProgress

		#region Value property

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
		  "Value",
		  typeof(double),
		  typeof(ConvolutionStack),
		  new FrameworkPropertyMetadata(1.0, OnValueReplaced));

		private static void OnValueReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ConvolutionStack owner = (ConvolutionStack)d;
			owner.OnValueUpdate();
		}

		#endregion Value property

		#endregion Properties

		private void OnValueUpdate()
		{
			var value = Value;

			if (billboards.Count < value)
				return;

			int visibleImages = (int)Math.Floor(value * SectionCount);
			double opacity = value * SectionCount - visibleImages;

			for (int i = SectionCount - 1; i >= visibleImages + 1; i--)
			{
				SetOpacity(i, 0);
			}

			SetOpacity(visibleImages, opacity);

			for (int i = 0; i < visibleImages; i++)
			{
				SetOpacity(i, 1);
			}
		}

		private void SetOpacity(int index, double opacity)
		{
			Brush brush = ((DiffuseMaterial)billboards[index].Material).Brush;
			brush.Opacity = opacity;
		}

		private void UpdateUI()
		{
			var dataSource = DataSource;
			if (dataSource == null)
				return;

			int sectionCount = SectionCount;
			double sectionHeight = SectionHeight;

			// one noize for all slices
			int[] noize = ImageHelper.CreateWhiteNoizeImage(dataSource.Width, dataSource.Depth);

			Task.Factory.StartNew(() =>
			{
				for (int iy = 0; iy < sectionCount; iy++)
				{
					double y = sectionHeight * iy / (double)sectionCount;

					var sectionDataSource = dataSource.CreateSectionXZ(y);
					Dispatcher.BeginInvoke(() =>
					{
						VectorFieldConvolutionChart chart = new VectorFieldConvolutionChart { DataSource = sectionDataSource, WhiteNoize = noize };
						chart.AddHandler(BackgroundRenderer.RenderingFinished, new RoutedEventHandler(OnRenderingFinished));
						Material material = new DiffuseMaterial(new ImageBrush(chart.Image.Source));
						Billboard billboard = new Billboard
						{
							LowerLeft = new Point3D(-1, y, -1),
							LowerRight = new Point3D(1, y, -1),
							UpperLeft = new Point3D(-1, y, 1),
							UpperRight = new Point3D(1, y, 1),
							Material = material,
							BackMaterial = material
						};

						billboards.Add(billboard);
						Children.Add(billboard);
					}, DispatcherPriority.Background);
				}
			});
		}

		private int renderingFinishedCount = 0;
		private void OnRenderingFinished(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(() =>
			{
				RenderingProgress = renderingFinishedCount / (SectionCount - 1.0) + renderingFinishedCount / SectionCount;
				renderingFinishedCount++;
			});
		}
	}
}
