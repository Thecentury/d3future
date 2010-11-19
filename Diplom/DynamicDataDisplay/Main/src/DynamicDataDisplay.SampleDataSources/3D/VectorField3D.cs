using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSource = Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional.IDataSource3D<System.Windows.Media.Media3D.Vector3D>;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.SampleDataSources
{
	public static class VectorField3D
	{
		public static FuncUniformDataSource3D<Vector3D> CreatePotentialField(int width, int height, int depth, params PotentialPoint3D[] points)
		{
			var potentialField = new PotentialField3D();
			potentialField.AddPoints(points);

			return CreatePotentialField(potentialField, width: width, height: height, depth: depth);
		}

		public static FuncUniformDataSource3D<Vector3D> CreatePotentialField(PotentialField3D potentialField, int width = 100, int height = 100, int depth = 100)
		{
			FuncUniformDataSource3D<Vector3D> dataSource = new FuncUniformDataSource3D<Vector3D>(
				(i, j, k) => potentialField.GetVector(new Point3D(i / (double)width, j / (double)height, k / (double)depth)),
				width: width, height: height, depth: depth);
			return dataSource;
		}

		public static FuncUniformDataSource3D<Vector3D> CreateTangentPotentialField(PotentialField3D potentialField, int width = 100, int height = 100, int depth = 100)
		{
			FuncUniformDataSource3D<Vector3D> dataSource = new FuncUniformDataSource3D<Vector3D>(
				(i, j, k) => potentialField.GetTangentVector(new Point3D(i / (double)width, j / (double)height, k / (double)depth)),
				width: width, height: height, depth: depth);
			return dataSource;
		}

		// lattice point == узел решетки
		public static DataSource CreateSpiral(double width = 1, double height = 1, double depth = 2, int latticeCountX = 100, int latticeCountY = 100, int latticeCountZ = 200)
		{
			FuncDataSource3D<Vector3D> dataSource = new FuncDataSource3D<Vector3D>(latticeCountX, latticeCountY, latticeCountZ,
				(i, j, k) =>
				{
					var xyAngle = 2 * Math.PI * j / (double)(latticeCountY);

					if (xyAngle < 0)
						xyAngle += 2 * Math.PI;
					else if (xyAngle > 2 * Math.PI)
						xyAngle -= 2 * Math.PI;

					Debug.WriteLine(xyAngle);

					Point3D result = new Point3D(
						width / 2.0 / latticeCountX * i * Math.Cos(xyAngle),
						height / 2.0 / latticeCountX * i * Math.Sin(xyAngle),
						depth / latticeCountZ * (k + xyAngle / (2 * Math.PI)));

					return result;
				},
				(i, j, k) =>
				{
					//double x = i - latticeX / 2.0;
					//double y = j - latticeY / 2.0;
					//var xyAngle = Math.Atan2(y, x);

					var xyAngle = 2 * Math.PI * j / (double)(latticeCountY);

					var vector = new Vector3D(
						width / 2.0 / latticeCountX * i * Math.Cos(xyAngle),
						width / 2.0 / latticeCountY * i * Math.Sin(xyAngle),
						0);
					var perpendicular = Vector3D.CrossProduct(vector, new Vector3D(0, 0, depth / latticeCountZ / 2.0));
					perpendicular.Z = 0.1;

					perpendicular.Normalize();
					perpendicular *= 0.1;

					return perpendicular;
				});

			return dataSource;
		}
	}
}
