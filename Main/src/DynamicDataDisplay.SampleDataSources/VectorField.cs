using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.IDataSource2D<System.Windows.Vector>;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.SampleDataSources
{
	public static class VectorField
	{
		private static DataSource CreateVectorField(int width, int height, Vector[,] data)
		{
			double[] xs = Enumerable.Range(0, width).Select(i => (double)i).ToArray();
			double[] ys = Enumerable.Range(0, height).Select(i => (double)i).ToArray();

			return new NonUniformDataSource2D<Vector>(xs, ys, data);
		}

		public static DataSource CreateCheckerboard(int width, int height)
		{
			var vectorArray = DataSource2DHelper.CreateVectorData(width, height, (x, y) =>
			{
				var result = (x / 40) % 2 - (y / 40) % 2 == 0 ? new Vector(x + 1, 0) : new Vector(0, y + 1);
				return result;
			});

			return CreateVectorField(width, height, vectorArray);
		}

		public static DataSource CreateCircularField(int width, int height)
		{
			var vectorArray = DataSource2DHelper.CreateVectorData(width, height, (x, y) =>
			{
				Vector3D center = new Vector3D(width / 2f, height / 2f, 0);
				Vector3D up = new Vector3D(0, 0, 1);
				Vector3D vec = center - new Vector3D(x, y, 0);
				Vector3D tangent = Vector3D.CrossProduct(vec, up);
				Vector value = new Vector(tangent.X, tangent.Y);
				//if (value.X != 0 || value.Y != 0)
				//    value.Normalize();

				return value;
			});

			return CreateVectorField(width, height, vectorArray);
		}

		public static DataSource CreateCircularField2(int width, int height)
		{
			var vectorArray = DataSource2DHelper.CreateVectorData(width, height, (x, y) =>
				{
					Vector result;

					double xc = x - width / 2;
					double yc = y - height / 2;
					if (xc != 0)
					{
						double beta = Math.Sqrt(1.0 / (1 + yc * yc / (xc * xc)));
						double alpha = -beta * yc / xc;
						result = new Vector(alpha, beta);
					}
					else
					{
						double alpha = Math.Sqrt(1.0 / (1 + xc * xc / (yc * yc)));
						double beta = -alpha * xc / yc;
						result = new Vector(alpha, beta);
					}

					if (Double.IsNaN(result.X))
					{
						result = new Vector(0, 0);
					}

					return result;
				});

			return CreateVectorField(width, height, vectorArray);
		}

		public static DataSource CreatePotentialField(int width, int height, params PotentialPoint[] points)
		{
			var potentialField = new PotentialField();
			potentialField.AddPoints(points);
			var vectorArray = DataSource2DHelper.CreateVectorData(width, height, (x, y) =>
			{
				return potentialField.GetPotential(new Point(x, y));
			});

			return CreateVectorField(width, height, vectorArray);
		}
	}
}
