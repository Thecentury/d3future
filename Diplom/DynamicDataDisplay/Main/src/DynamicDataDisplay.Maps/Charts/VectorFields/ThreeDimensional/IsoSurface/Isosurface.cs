using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public sealed partial class IsoSurface : ModelVisual3D
	{
		private readonly GeometryModel3D model = new GeometryModel3D
		{
			Material = new DiffuseMaterial(Brushes.LightGreen),
			BackMaterial = new DiffuseMaterial(Brushes.LightSeaGreen)
		};
		private readonly MeshGeometry3D mesh = new MeshGeometry3D();
		private List<IsoSurfaceVertex> vertices = new List<IsoSurfaceVertex>();
		private List<int> indices = new List<int>();
		private List<IsoSurfaceIndex> edges = new List<IsoSurfaceIndex>();

		private int sizeX;
		private int sizeY;
		private int sizeZ;

		private double[, ,] data;

		private double potential;

		#region Properties

		#region DataSource property

		public IDataSource3D<double> DataSource
		{
			get { return (IDataSource3D<double>)GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}

		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
		  "DataSource",
		  typeof(IDataSource3D<double>),
		  typeof(IsoSurface),
		  new FrameworkPropertyMetadata(null, OnDataSourceReplaced));

		private static void OnDataSourceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			IsoSurface owner = (IsoSurface)d;
			owner.UpdateUI();
		}

		#endregion DataSource property

		#region Potential property

		public double Potential
		{
			get { return (double)GetValue(PotentialProperty); }
			set { SetValue(PotentialProperty, value); }
		}

		public static readonly DependencyProperty PotentialProperty = DependencyProperty.Register(
		  "Potential",
		  typeof(double),
		  typeof(IsoSurface),
		  new FrameworkPropertyMetadata(1.0, OnPotentialReplaced));

		private static void OnPotentialReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			IsoSurface owner = (IsoSurface)d;
			owner.UpdateUI();
		}

		#endregion Potential property

		#endregion Properties

		private void UpdateUI()
		{
			if (DataSource == null)
				return;

			var dataSource = DataSource;
			sizeX = DataSource.Width;
			sizeY = DataSource.Height;
			sizeZ = DataSource.Depth;
			model.Geometry = mesh;
			potential = Potential;

			Visual3DModel = model;

			Task.Factory.StartNew(() =>
			{
				data = dataSource.DataToArray();
				MarchingCubes(dataSource, potential);

				Dispatcher.BeginInvoke(() => OnCompletion());
			});
		}

		private void OnCompletion()
		{
			mesh.Positions = new Point3DCollection(vertices.Select(vertex => (Point3D)vertex.Position));
			int count = mesh.Positions.Count;
			mesh.TriangleIndices.AddMany(Enumerable.Range(0, count));
		}

		public void MarchingCubes(IDataSource3D<double> source, double potential)
		{
			vertices = new List<IsoSurfaceVertex>(10000);
			indices = new List<int>();
			edges = new List<IsoSurfaceIndex>();

			for (int ix = 0; ix < sizeX - 1; ix++)
				for (int iy = 0; iy < sizeY - 1; iy++)
					for (int iz = 0; iz < sizeZ - 1; iz++)
					{
						MarchCube(ix, iy, iz, potential, source);
					}
		}

		double GetOffset(double value1, double value2, double valueDesired)
		{
			double fDelta = value2 - value1;

			if (fDelta == 0.0)
			{
				return 0.5f;
			}
			return (valueDesired - value1) / fDelta;
		}

		double Sample(double x, double y, double z)
		{
			return MathHelper.GetValue(new Vector3D(x, y, z), data);
		}

		//vGetColor fins color via point and normal
		void vGetColor(ref Vector3D color, Vector3D position, Vector3D normal)
		{
			double fX = normal.X;
			double fY = normal.Y;
			double fZ = normal.Z;
			color.X = (fX > 0.0 ? fX : 0.0) + (fY < 0.0 ? -0.5 * fY : 0.0) + (fZ < 0.0 ? -0.5 * fZ : 0.0);
			color.Y = (fY > 0.0 ? fY : 0.0) + (fZ < 0.0 ? -0.5 * fZ : 0.0) + (fX < 0.0 ? -0.5 * fX : 0.0);
			color.Z = (fZ > 0.0 ? fZ : 0.0) + (fX < 0.0 ? -0.5 * fX : 0.0) + (fY < 0.0 ? -0.5 * fY : 0.0);
		}

		//Find part of Surface for current voxel
		void MarchCube(double x, double y, double z, double scale, IDataSource3D<double> source)
		{
			int iCorner, iVertex, iVertexTest, iEdge, iTriangle, iFlagIndex, iEdgeFlags;
			double fOffset;
			double[] afCubeValue = new double[8];
			Vector3D[] asEdgeVertex = new Vector3D[12];
			Vector3D[] asEdgeNorm = new Vector3D[12];

			//Find value in voxel's knots
			for (iVertex = 0; iVertex < 8; iVertex++)
			{
				afCubeValue[iVertex] = (float)source.Data[(int)(x + a2fVertexOffset[iVertex, 0]), (int)(y + a2fVertexOffset[iVertex, 1]), (int)(z + a2fVertexOffset[iVertex, 2])];
				//if (afCubeValue[iVertex] == (double)source.MissingValue) return;
			}

			//Checking intersections via table
			iFlagIndex = 0;
			for (iVertexTest = 0; iVertexTest < 8; iVertexTest++)
			{
				if (afCubeValue[iVertexTest] <= potential)
					iFlagIndex |= 1 << iVertexTest;
			}

			//Finally, get our surface configuration
			iEdgeFlags = aiCubeEdgeFlags[iFlagIndex];

			//if our surface doesn't intersect voxel
			if (iEdgeFlags == 0)
				return;

			//Building vertices
			for (iEdge = 0; iEdge < 12; iEdge++)
			{
				if ((iEdgeFlags & (1 << iEdge)) != 0)
				{
					fOffset = GetOffset(afCubeValue[a2iEdgeConnection[iEdge, 0]],
							   afCubeValue[a2iEdgeConnection[iEdge, 1]], potential);

					asEdgeVertex[iEdge].X = (float)(x + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 0] + fOffset * a2fEdgeDirection[iEdge, 0]) * scale);
					asEdgeVertex[iEdge].Y = (float)(y + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 1] + fOffset * a2fEdgeDirection[iEdge, 1]) * scale);
					asEdgeVertex[iEdge].Z = (float)(z + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 2] + fOffset * a2fEdgeDirection[iEdge, 2]) * scale);

					asEdgeNorm[iEdge] = MathHelper.CalculateNormalToField(asEdgeVertex[iEdge].X, asEdgeVertex[iEdge].Y, asEdgeVertex[iEdge].Z, data);
				}
			}


			//Building triangles
			for (iTriangle = 0; iTriangle < 5; iTriangle++)
			{
				if (a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle] < 0)
					break;

				for (iCorner = 0; iCorner < 3; iCorner++)
				{
					iVertex = a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle + iCorner];

					vertices.Add(new IsoSurfaceVertex
					{
						Position = asEdgeVertex[iVertex],
						Normal = MathHelper.CalculateNormalToPolygon(asEdgeVertex[a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle]], asEdgeVertex[a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle + 1]], asEdgeVertex[a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle + 2]])
						//asEdgeNorm[iVertex]
					});
				}
			}
		}

	}
}
