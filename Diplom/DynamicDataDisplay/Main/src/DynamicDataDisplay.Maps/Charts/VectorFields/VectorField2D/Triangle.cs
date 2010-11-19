using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed class Triangle : ModelVisual3D
	{
		private readonly GeometryModel3D model = new GeometryModel3D();
		private readonly MeshGeometry3D mesh = new MeshGeometry3D();
		private readonly DiffuseMaterial material = new DiffuseMaterial(Brushes.Red);
		public Triangle()
		{
			model.Material = material;
			model.BackMaterial = material;

			mesh.TextureCoordinates.Add(new Point(0, 0));
			mesh.TextureCoordinates.Add(new Point(1, 0));
			mesh.TextureCoordinates.Add(new Point(0.5, 1));

			mesh.TriangleIndices.Add(0);
			mesh.TriangleIndices.Add(1);
			mesh.TriangleIndices.Add(2);

			UpdatePosition();

			model.Geometry = mesh;
	
			Content = model;
		}

		#region Properties

		#region Point properties

		public Point3D Point1
		{
			get { return (Point3D)GetValue(Point1Property); }
			set { SetValue(Point1Property, value); }
		}

		public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
		  "Point1",
		  typeof(Point3D),
		  typeof(Triangle),
		  new FrameworkPropertyMetadata(new Point3D(-1, 0, 0), OnPointChanged));

		public Point3D Point2
		{
			get { return (Point3D)GetValue(Point2Property); }
			set { SetValue(Point2Property, value); }
		}

		public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
		  "Point2",
		  typeof(Point3D),
		  typeof(Triangle),
		  new FrameworkPropertyMetadata(new Point3D(1, 0, 0), OnPointChanged));

		public Point3D Point3
		{
			get { return (Point3D)GetValue(Point3Property); }
			set { SetValue(Point3Property, value); }
		}

		public static readonly DependencyProperty Point3Property = DependencyProperty.Register(
		  "Point3",
		  typeof(Point3D),
		  typeof(Triangle),
		  new FrameworkPropertyMetadata(new Point3D(0, 1, 0), OnPointChanged));

		private static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Triangle owner = (Triangle)d;
			owner.UpdatePosition();
		}

		#endregion Point properties

		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
		  "Fill",
		  typeof(Brush),
		  typeof(Triangle),
		  new FrameworkPropertyMetadata(Brushes.Red, OnFillChanged));

		private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Triangle owner = (Triangle)d;
			owner.material.Brush = (Brush)e.NewValue;
		}

		#endregion Properties

		private void UpdatePosition()
		{
			mesh.Positions.Clear();
			mesh.Positions.Add(Point1);
			mesh.Positions.Add(Point2);
			mesh.Positions.Add(Point3);
		}
	}
}
