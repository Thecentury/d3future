using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Petzold.Media3D;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class Section3DChartBase : BillboardChartBase
	{
		public static readonly Color DefaultColor = Color.FromArgb(0xFF, 0x4C, 0x88, 0xFF);
		public static readonly Color SelectedColor = Colors.OrangeRed;

		protected readonly WirePolyline border = new WirePolyline { Thickness = 3, Points = new Point3DCollection(5) };
		protected readonly Billboard billboard = new Billboard();

		protected Section3DChartBase()
		{
			border.Color = DefaultColor;

			Material material = new DiffuseMaterial { Brush = new SolidColorBrush(Color.FromArgb(0x7F, 0x4C, 0x88, 0xFF)) };
			material.Freeze();

			billboard.Material = material;
			billboard.BackMaterial = material;

			Children.Add(billboard);
			Children.Add(border);

			UpdateUI();
		}

		#region Properties

		public WirePolyline Border
		{
			get { return border; }
		}

		#endregion Properties

		public override void UpdateUI()
		{
			border.Points.Clear();
			border.Points.Add(billboard.LowerLeft);
			border.Points.Add(billboard.LowerRight);
			border.Points.Add(billboard.UpperRight);
			border.Points.Add(billboard.UpperLeft);
			border.Points.Add(billboard.LowerLeft);
		}
	}
}
