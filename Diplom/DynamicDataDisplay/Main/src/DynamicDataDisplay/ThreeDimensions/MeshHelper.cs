using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ThreeDimensions
{
	public static class MeshHelper
	{
		public static void BuildMeshData(Func<int, int, double> data, int width, int height,
			out Point3DCollection points, out PointCollection textureCoordinates, out Int32Collection triangleIndices,
			double textureWidth = 1, double textureHeight = 1)
		{
			int pointCount = width * height;
			points = new Point3DCollection(pointCount);
			textureCoordinates = new PointCollection(pointCount);

			int triangleCount = 2 * (width - 1) * (height - 1);
			triangleIndices = new Int32Collection(3 * triangleCount);

			for (int iy = 0; iy < height; ++iy)
			{
				double yProportion = iy / (height - 1.0);
				double outY = 0.5 - yProportion;
				double textureY = textureHeight * yProportion;

				for (int ix = 0; ix < width; ++ix)
				{
					double xProportion = ix / (width - 1.0);
					double outX = xProportion - 0.5;
					double textureX = textureWidth * xProportion;

					points.Add(new Point3D(outX, outY, data(ix, iy)));
					textureCoordinates.Add(new Point(textureX, textureY));

					if (ix < (width - 1) && iy < (height - 1))
					{
						int topLeftIndex = ix + iy * width;
						int bottomLeftIndex = topLeftIndex + width;

						triangleIndices.Add(bottomLeftIndex);
						triangleIndices.Add(bottomLeftIndex + 1);
						triangleIndices.Add(topLeftIndex);

						triangleIndices.Add(bottomLeftIndex + 1);
						triangleIndices.Add(topLeftIndex + 1);
						triangleIndices.Add(topLeftIndex);
					}
				}
			}
		}

		public static MeshGeometry3D BuildMeshFromPoints(double[,] data, double textureWidth, double textureHeight)
		{
			Point3DCollection points;
			PointCollection textureCoordinates;
			Int32Collection triangleIndices;
			MeshHelper.BuildMeshData((ix, iy) => data[ix, iy], data.GetLength(0), data.GetLength(1),
				out points, out textureCoordinates, out triangleIndices,
				textureWidth, textureHeight);

			points.Freeze();
			textureCoordinates.Freeze();
			triangleIndices.Freeze();

			MeshGeometry3D mesh = new MeshGeometry3D { Positions = points, TextureCoordinates = textureCoordinates, TriangleIndices = triangleIndices };
			return mesh;
		}
	}
}
