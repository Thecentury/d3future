using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Geometry.VectorMath;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using System.Collections.ObjectModel;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.MapPoint.Rendering3D;
using System.Collections.Specialized;
using swm = System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.MapPoint.Rendering3D.Utility;
using Microsoft.MapPoint.CoordinateSystems;
using System.Windows;
using System.Windows.Media;
using Microsoft.MapPoint.Geometry;
using Microsoft.MapPoint.Geometry.Geometry2;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Isolines
{
    class StaticIsolines : IVisualizationProvider, IMouseCheck
	{
		#region IVisualizationProvider Members

		public bool IsDynamicProvider
		{
			get;
			set;
		}

		public Guid ProviderGuid
		{
			get;
			set;
		}

		bool IVisualizationProvider.ContainsLayer(Guid guid)
		{
			foreach (IsolinesLayer iLayer in this.layers)
			{
				if (iLayer.Guid == guid)
					return true;
			}
			return false;
		}

		public void AddLayer(VisualizationDataSource dataSource)
		{
			AddIsolinesLayer(dataSource);
		}

		public void RemoveLayer(VisualizationDataSource dataSource)
		{
			RemoveIsolinesLayer(dataSource);
		}

		public Algorithms GetAlgorithmType()
		{
			return Algorithms.IsolineMap;
		}

		#endregion

		private List<IsolinesLayer> layers;
		private Host host;
		private IPalette palette;
		IsolineBuilder isolineBuilder = new IsolineBuilder();
		IsolineTextAnnotater annotater = new IsolineTextAnnotater();

        RunningIsoline runningIsoline;

		public StaticIsolines(ObservableCollection<VisualizationDataSource> dataSources, Host host)
		{
			dataSources.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(dataSources_CollectionChanged);
			this.host = host;
			layers = new List<IsolinesLayer>();
            palette = new LinearPalette(swm.Colors.Blue, swm.Colors.Green, swm.Colors.Red);

            runningIsoline = new RunningIsoline(null, host);
            host.Actors.Add(runningIsoline);
		}

		protected virtual void dataSources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (object newItem in e.NewItems)
					{
						VisualizationDataSource newDataSource = newItem as VisualizationDataSource;
						if (newDataSource == null)
						{
							throw new ArgumentException("Something strange has happened");
						}
						else if (newDataSource.Algorithm == Algorithms.IsolineMap)
						{
							AddIsolinesLayer(newDataSource);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (object removedItem in e.OldItems)
					{
						VisualizationDataSource removedDataSource = removedItem as VisualizationDataSource;
						if (removedDataSource == null)
						{
							throw new ArgumentException("Something strange has happened");
						}
						else if (removedDataSource.Algorithm == Algorithms.IsolineMap)
						{
							RemoveIsolinesLayer(removedDataSource);
						}
					}
					break;
				default: break;
			}
		}

		private void RemoveIsolinesLayer(VisualizationDataSource removedDataSource)
		{
			IsolinesLayer layer = layers.Find(il => il.Guid == removedDataSource.Guid);
			if (layer != null)
			{
				RemoveLayerFromHost(layer);
				layers.Remove(layer);
			}
		}

		private void AddIsolinesLayer(VisualizationDataSource dataSource)
		{
			if (dataSource.Algorithm == Algorithms.IsolineMap)
			{
                if (dataSource.Data is IDataSource2D<double>)
                    AddDataSource(dataSource.Data as IDataSource2D<double>, dataSource.Guid);
			}
		}

		private void AddDataSource(IDataSource2D<double> field, Guid guid)
		{
			List<PolylineGeometry> geometry = new List<PolylineGeometry>();
			List<VisualPushpin> labels = new List<VisualPushpin>();

			isolineBuilder.DataSource = field;
			IsolineCollection collection = isolineBuilder.Build();
			annotater.WayBeforeText = 20.0;

			foreach (LevelLine segment in collection.Lines)
			{
				List<Coordinate2D> points = new List<Coordinate2D>();
				points.Add(new Coordinate2D(segment.StartPoint.X, segment.StartPoint.Y));
				foreach (Point point in segment.OtherPoints)
				{
					points.Add(new Coordinate2D(point.X, point.Y));
				}
				PolyInfo style = PolyInfo.DefaultPolyline;
				Color color =  palette.GetColor(segment.Value01);
                style.LineColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

				geometry.Add(new PolylineGeometry(guid.ToString(), Guid.NewGuid().ToString(), new Polyline2(Wgs84CoordinateReferenceSystem.Instance,
					Coordinate2DCollection.CreateUnsafe(points.ToArray())), style));
			}

			foreach (IsolineTextLabel label in annotater.Annotate(collection, new Rect()))
			{
				labels.Add(new VisualPushpin(30, 30, label.Text, LatLonAlt.CreateUsingDegrees(label.Position.Y, label.Position.X, 0), this, Guid.NewGuid().ToString()));
			}

			layers.Add(new IsolinesLayer() { Geometry = geometry, IsVisible = true, Guid = guid, Labels = labels });
			AddLayerToHost(layers[layers.Count - 1]);

		}

		

		private void AddLayerToHost(IsolinesLayer layer)
		{
			if (layer.IsVisible)
			{
				foreach (PolylineGeometry geometry in layer.Geometry)
				{
					host.Geometry.AddGeometry(geometry);
				}
				foreach (VisualPushpin pushpin in layer.Labels)
				{
					host.Geometry.AddGeometry(pushpin.Pushpin);
				}
			}
		}

		private void RemoveLayerFromHost(IsolinesLayer layer)
		{
			if (layer.IsVisible)
			{
				foreach (PolylineGeometry geometry in layer.Geometry)
				{
					host.Geometry.RemoveGeometry(geometry.LayerId, geometry.Id);
				}
				foreach (VisualPushpin pushpin in layer.Labels)
				{
					host.Geometry.RemoveGeometry(pushpin.Pushpin.LayerId, pushpin.Pushpin.Id);
				}
			}
		}

		PolylineGeometry runningLine;

		#region IMouseCheck Members

		public bool CheckIntersection(LatLonAlt location)
		{
			if (layers.Count == 0) return false;
			if (layers[layers.Count - 1] == null)
				return false;

            WarpedDataSource2D<double> dataSource = null;//layers[layers.Count - 1].dataSource;
			if (dataSource == null)
				return false;

			double isolineLayer;
			bool foundVal = Search(dataSource, new Point(location.LongitudeDegrees, location.LatitudeDegrees), out isolineLayer);
			if (foundVal)
			{
				host.Geometry.RemoveGeometry("RunningLine", "rl");
				IsolineCollection collection = isolineBuilder.Build(isolineLayer);
				LevelLine segment = collection.Lines[0];

                //runningIsoline.LineColor = palette.GetColor(segment.Value01);
                //runningIsoline.Segment = segment;
                //List<Coordinate2D> points = new List<Coordinate2D>();
                //points.Add(new Coordinate2D(segment.StartPoint.X, segment.StartPoint.Y));
                //foreach (Point point in segment.OtherPoints)
                //{
                //    points.Add(new Coordinate2D(point.X, point.Y));
                //}
                //PolyInfo style = PolyInfo.DefaultPolyline;
                //style.LineColor = palette.GetColor(segment.Value01);
                //style.LineWidth = 5;
                //runningLine = new PolylineGeometry("RunningLine", "rl", new Polyline2(Wgs84CoordinateReferenceSystem.Instance,
                //        Coordinate2DCollection.CreateUnsafe(points.ToArray())), style);
			}
			return foundVal;
		}

		public void ShowIntersectedValues()
		{
			//if (runningLine != null)
			//{
				//host.Geometry.AddGeometry(runningLine);
			//}
		}

		#endregion

		int foundI = 0;
		int foundJ = 0;
		Quad foundQuad = null;
		private bool Search(WarpedDataSource2D<double> dataSource, Point pt, out double foundVal)
		{
			var grid = dataSource.Grid;

			foundVal = 0;

			int width = dataSource.Width;
			int height = dataSource.Height;
			bool found = false;
			int i = 0, j = 0;
			for (i = 0; i < width - 1; i++)
			{
				for (j = 0; j < height - 1; j++)
				{
					Quad quad = new Quad(
					grid[i, j],
					grid[i, j + 1],
					grid[i + 1, j + 1],
					grid[i + 1, j]);
					if (quad.Contains(pt))
					{
						found = true;
						foundQuad = quad;
						foundI = i;
						foundJ = j;

						break;
					}
				}
				if (found) break;
			}
			if (!found)
			{
				foundQuad = null;
				return false;
			}

			var data = dataSource.Data;

			double x = pt.X;
			double y = pt.Y;
			Vector2D A = new Vector2D(grid[i, j + 1].X, grid[i, j + 1].Y);					// @TODO: in common case add a sorting of points:
			Vector2D B = new Vector2D(grid[i + 1, j + 1].X, grid[i + 1, j + 1].Y);				//   maxA ___K___ B
			Vector2D C = new Vector2D(grid[i + 1, j].X, grid[i + 1, j].Y);					//      |         |
			Vector2D D = new Vector2D(grid[i, j].X, grid[i, j].Y);						//      M    P    N
			double a = data[i, j + 1];						//		|         |
			double b = data[i + 1, j + 1];					//		В ___L____Сmin
			double c = data[i + 1, j];
			double d = data[i, j];

			Vector2D K, L;
			double k, l;
			if (x >= A.X)
				k = Interpolate(A, B, a, b, K = new Vector2D(x, GetY(A, B, x)));
			else
				k = Interpolate(D, A, d, a, K = new Vector2D(x, GetY(D, A, x)));

			if (x >= C.X)
				l = Interpolate(C, B, c, b, L = new Vector2D(x, GetY(C, B, x)));
			else
				l = Interpolate(D, C, d, c, L = new Vector2D(x, GetY(D, C, x)));

			foundVal = Interpolate(L, K, l, k, new Vector2D(x, y));
			return !Double.IsNaN(foundVal);
		}

		private double Interpolate(Vector2D v0, Vector2D v1, double u0, double u1, Vector2D a)
		{
			Vector2D l1 = a - v0;
			Vector2D l = v1 - v0;

			double res = (u1 - u0) / l.Length() * l1.Length() + u0;
			return res;
		}

		private double GetY(Vector2D v0, Vector2D v1, double x)
		{
			double res = v0.Y + (v1.Y - v0.Y) / (v1.X - v0.X) * (x - v0.X);
			return res;
		}
	}
}
