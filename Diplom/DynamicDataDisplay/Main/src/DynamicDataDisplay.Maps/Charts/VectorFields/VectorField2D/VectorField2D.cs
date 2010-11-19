using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Petzold.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class VectorField2D : VectorFieldChartBase
	{
		private readonly Viewport3D viewport3D = new Viewport3D();
		private readonly ViewportHostPanel hostPanel = new ViewportHostPanel();
		private readonly List<Triangle> triangles = new List<Triangle>();
		private readonly ResourcePool<Triangle> trianglesPool = new ResourcePool<Triangle>();
		private Range<double> minMaxLength = new Range<double>();
		private DataRect bounds = new DataRect();
		private int width = 0;
		private int height = 0;
		private double vectorLength = 1;

		public VectorField2D()
		{
			ViewportPanel.SetViewportBounds(viewport3D, new DataRect(0, 0, 1, 1));

			var camera = new PerspectiveCamera(new Point3D(100, 0, 400), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 45);
			viewport3D.Camera = camera;

			// lights
			Model3DGroup lights = new Model3DGroup();
			lights.Children.Add(new AmbientLight(Color.FromRgb(0x7f, 0x7f, 0x7f)));
			lights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)));
			viewport3D.Children.Add(new ModelVisual3D { Content = lights });

			Rectangle bounds = new Rectangle { Fill = Brushes.PowderBlue.MakeTransparent(0.4) };
			ViewportPanel.SetViewportBounds(bounds, new DataRect(0, 0, 1, 1));
			hostPanel.Children.Add(bounds);

			hostPanel.Children.Add(viewport3D);

			SetBinding(DataContextProperty, new Binding { Path = new PropertyPath("DataContext"), Source = hostPanel });
		}

		#region Properties

		public IPalette Palette
		{
			get { return (IPalette)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}

		public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
		  "Palette",
		  typeof(IPalette),
		  typeof(VectorField2D),
		  new FrameworkPropertyMetadata(new HsbPalette(), OnPaletteReplaced));

		private static void OnPaletteReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VectorField2D owner = (VectorField2D)d;
			owner.RebuildUI();
		}

		#endregion Properties

		protected override void RebuildUI()
		{
			// removing all triangles
			foreach (var triangle in triangles)
			{
				trianglesPool.Put(triangle);
				viewport3D.Children.Remove(triangle);
			}

			var dataSource = DataSource;
			if (dataSource == null)
				return;

			var palette = Palette;
			if (palette == null)
				return;

			width = dataSource.Width;
			height = dataSource.Height;

			minMaxLength = dataSource.GetMinMaxLength();
			bounds = dataSource.GetGridBounds();
			vectorLength = Math.Sqrt(bounds.Width * bounds.Width / (width * width) + bounds.Height * bounds.Height / (height * height));

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					var direction = dataSource.Data[i, j];
					var position = dataSource.Grid[i, j];

					var triangle = CreateTriangle(position, direction);
					triangles.Add(triangle);

					viewport3D.Children.Add(triangle);
				}
			}
		}

		private Triangle CreateTriangle(Point position, Vector direction)
		{
			direction.Normalize();
			direction *= vectorLength;

			Point p1 = position + 1.7 / 3 * direction;
			Point basePoint = position - 0.3333333 * direction;
			var perpendicular = direction.Perpendicular();
			perpendicular.Normalize();
			perpendicular *= 0.166666666666;
			Point p2 = basePoint + perpendicular;
			Point p3 = basePoint - perpendicular;

			Triangle triangle = trianglesPool.GetOrCreate();
			triangle.Point1 = new Point3D(p1.X, p1.Y, 0);
			triangle.Point2 = new Point3D(p2.X, p2.Y, 0);
			triangle.Point3 = new Point3D(p3.X, p3.Y, 0);
			return triangle;
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);
			plotter.Children.BeginAdd(hostPanel);
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			plotter.Children.BeginRemove(hostPanel);

			base.OnPlotterDetaching(plotter);
		}
	}
}
