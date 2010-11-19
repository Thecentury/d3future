using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Threading;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Displays a grid of ellipses on grid from 2D data source.
	/// </summary>
	public class GridChart2D : FrameworkElement, IPlotterElement
	{
		private readonly ViewportHostPanel panel = new ViewportHostPanel();

		/// <summary>
		/// Initializes a new instance of the <see cref="GridChart2D"/> class.
		/// </summary>
		public GridChart2D()
		{
			SetBinding(DataContextProperty, new Binding("DataContext") { Source = panel });
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GridChart2D"/> class.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public GridChart2D(IGridSource2D grid)
			: this()
		{
			this.Grid = grid;
		}

		#region Properties

		#region Grid property

		/// <summary>
		/// Gets or sets the grid.
		/// </summary>
		/// <value>The grid.</value>
		public IGridSource2D Grid
		{
			get { return (IGridSource2D)GetValue(GridProperty); }
			set { SetValue(GridProperty, value); }
		}

		public static readonly DependencyProperty GridProperty = DependencyProperty.Register(
		  "Grid",
		  typeof(IGridSource2D),
		  typeof(GridChart2D),
		  new FrameworkPropertyMetadata(null, OnGridReplaced));

		private static void OnGridReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			GridChart2D owner = (GridChart2D)d;
			owner.UpdateUI();
		}

		#endregion Grid property

		#endregion Properties

		private void UpdateUI()
		{
			panel.Children.Clear();

			if (plotter == null)
				return;

			var grid = Grid;
			if (grid == null)
				return;

			var bounds = grid.Grid.GetGridBounds();
			Viewport2D.SetContentBounds(this, bounds);

			int width = grid.Width;
			int height = grid.Height;

			// todo not the best way to determine size of markers in case of warped grids.
			double deltaX = (grid.Grid[width - 1, 0].X - grid.Grid[0, 0].X) / width;
			double deltaY = (grid.Grid[0, height - 1].Y - grid.Grid[0, 0].Y) / height;

			for (int ix = 0; ix < width; ix++)
			{
				int localX = ix;
				Dispatcher.BeginInvoke(() =>
				{
					for (int iy = 0; iy < height; iy++)
					{
						Ellipse ellipse = new Ellipse { MaxWidth = 10, MaxHeight = 10 };
						var position = grid.Grid[localX, iy];
						ViewportPanel.SetX(ellipse, position.X);
						ViewportPanel.SetY(ellipse, position.Y);
						ViewportPanel.SetViewportWidth(ellipse, deltaX / 2);
						ViewportPanel.SetViewportHeight(ellipse, deltaY / 2);

						if (localX % 10 == 0 && iy % 10 == 0)
							ellipse.Fill = Brushes.Black;
						else
							ellipse.Fill = Brushes.Gray.MakeTransparent(0.7);

						panel.Children.Add(ellipse);
					}
				}, DispatcherPriority.Background);
			}
		}

		#region IPlotterElement Members

		private Plotter2D plotter;
		public Plotter2D Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = (Plotter2D)plotter;
			plotter.Children.BeginAdd(panel);
			UpdateUI();
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			plotter.Children.BeginRemove(panel);
			this.plotter = null;
		}

		Plotter IPlotterElement.Plotter
		{
			get { return plotter; }
		}

		#endregion // IPlotterElement Members
	}
}
