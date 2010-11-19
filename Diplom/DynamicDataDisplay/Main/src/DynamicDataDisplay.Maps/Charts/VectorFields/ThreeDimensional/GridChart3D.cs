using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows;
using Petzold.Media3D;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class GridChart3D : ModelVisual3D
	{
		#region Properties

		#region DataSource

		public IGridSource3D GridSource
		{
			get { return (IGridSource3D)GetValue(GridSourceProperty); }
			set { SetValue(GridSourceProperty, value); }
		}

		public static readonly DependencyProperty GridSourceProperty = DependencyProperty.Register(
		  "GridSource",
		  typeof(IGridSource3D),
		  typeof(GridChart3D),
		  new FrameworkPropertyMetadata(null, OnGridSourceReplaced));

		private static void OnGridSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GridChart3D owner = (GridChart3D)d;
			owner.UpdateUI();
		}

		#endregion DataSource

		#region SphereRadius

		public double SphereRadius
		{
			get { return (double)GetValue(SphereRadiusProperty); }
			set { SetValue(SphereRadiusProperty, value); }
		}

		public static readonly DependencyProperty SphereRadiusProperty = DependencyProperty.Register(
		  "SphereRadius",
		  typeof(double),
		  typeof(GridChart3D),
		  new FrameworkPropertyMetadata(0.01, OnSphereRadiusReplaced));

		private static void OnSphereRadiusReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GridChart3D owner = (GridChart3D)d;
			owner.UpdateUI();
		}

		#endregion SphereRadius

		#endregion Properties

		private void UpdateUI()
		{
			Children.Clear();

			var grid = GridSource;
			if (grid == null)
				return;

			double sphereRadius = SphereRadius;
			Material sphereMaterial = new DiffuseMaterial(Brushes.Cyan);
			sphereMaterial.Freeze();

			for (int k = 0; k < grid.Depth; k++)
			{
				int kLocal = k;
				Dispatcher.BeginInvoke(() =>
				{
					for (int i = 0; i < grid.Width; i++)
					{
						for (int j = 0; j < grid.Height; j++)
						{
							Point3D position = grid.Grid[i, j, kLocal];
							Sphere sphere = new Sphere
							{
								Radius = sphereRadius,
								Center = position,
								Material = sphereMaterial,
								Slices = 4,
								Stacks = 2
							};
							Children.Add(sphere);
						}
					}
				}, DispatcherPriority.Background);
			}
		}
	}
}
