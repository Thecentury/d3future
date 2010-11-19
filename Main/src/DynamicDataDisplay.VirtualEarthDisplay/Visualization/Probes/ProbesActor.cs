using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.Steps.Actors;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.Binding;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.MapPoint.Rendering3D.State;
using Microsoft.MapPoint.Rendering3D.GraphicsProxy;
using Microsoft.MapPoint.Geometry;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.MapPoint.Geometry.VectorMath;
using System.Drawing;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.MapPoint.CoordinateSystems;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes
{
    class ProbesActor : Actor, IVisualizationProvider, IMouseCheck
    {
        private List<MeshLayer> meshLayers;
        double layerAltitude;
        double step = 0.5;
        Host host;

        public ProbesActor(Host host, double layerAltitude, BindingsSource bs, ObservableCollection<VisualizationDataSource> dataSources)
            : base(bs)
        {
            meshLayers = new List<MeshLayer>();
            this.layerAltitude = layerAltitude;

            this.host = host;

            intersectedValues = new List<VisualPushpin>();

            dataSources.CollectionChanged += new NotifyCollectionChangedEventHandler(dataSources_CollectionChanged);
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
                        else if (newDataSource.Algorithm == Algorithms.Probes)
                        {
                            AddActorLayer(newDataSource);
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
                        else if (removedDataSource.Algorithm == Algorithms.Probes)
                        {
                            RemoveActorLayer(removedDataSource);
                        }
                    }
                    break;
                default: break;
            }
        }

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

        public void AddLayer(VisualizationDataSource dataSource)
        {
            AddActorLayer(dataSource);
        }


        public void RemoveLayer(VisualizationDataSource dataSource)
        {
            RemoveActorLayer(dataSource);
        }


        public Algorithms GetAlgorithmType()
        {
            return Algorithms.Probes;
        }

        bool IVisualizationProvider.ContainsLayer(Guid guid)
        {
            foreach (MeshLayer meshLayer in meshLayers)
            {
                if (meshLayer.Guid == guid)
                    return true;
            }
            return false;
        }

        #endregion

        public override void Render(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            if (meshLayers.Count != 0)
            {
                CameraData data;
                if (sceneState.TryGetData<CameraData>(out data))
                {
                    RenderQueues renderQueues = sceneState.GetData<RenderQueues>();
                    foreach (MeshLayer meshLayer in meshLayers)
                    {
                        if (meshLayer.IsVisible)
                        {
                            renderQueues.AddRenderable(meshLayer.Mesh);
                        }
                    }
                }
            }
        }

        private void AddActorLayer(VisualizationDataSource dataSource)
        {
            if (dataSource.Algorithm == Algorithms.Probes && dataSource.IsDynamic)
            {
                if (dataSource.Data is IDataSource2D<double>)
                {
                    AddDataSource(dataSource.Data as IDataSource2D<double>, dataSource.Guid);
                }
                else if (dataSource.Data is PointSet)
                {
                    AddDataSource(dataSource.Data as PointSet, dataSource.Guid);
                }
            }
        }

        private void AddDataSource(PointSet pointSet, Guid guid)
        {
            double altitude = host.Navigation.CameraPosition.Altitude / 20.0;
            step = 0.3;

            List<Vertex.PositionColored> vertices = BuildVertices(pointSet, altitude);

            MeshGraphicsObject<Vertex.PositionColored, ushort> mesh = new MeshGraphicsObject<Vertex.PositionColored, ushort>(
                GraphicsBufferUsage.Static,
                GraphicsBufferUsage.Static,
                vertices.Count,
                vertices.Count,
                true);

            Material material = new Material { AmbientColor = Color.White, DiffuseColor = Color.White, SpecularColor = Color.White };
            int id = mesh.Materials.Add(material);

            mesh.Vertices.AddData(vertices.ToArray());

            List<ushort> indexData = new List<ushort>();
            for (int i = 0; i < pointSet.Data.Count; i++)
            {
                indexData.Add((ushort)(i * 4));
                indexData.Add((ushort)(i * 4 + 1));
                indexData.Add((ushort)(i * 4 + 2));
                indexData.Add((ushort)(i * 4 + 3));
            }

            mesh.Indices.AddData(indexData.ToArray(),
                        PrimitiveType.LineList,
                        id);

            mesh.RenderState.Lighting.Enabled = false;
            mesh.RenderState.Cull.Enabled = false;

            pointSet.Guid = guid;
            meshLayers.Add(new MeshLayer { LayerAltitude = altitude, Mesh = mesh, Guid = guid, IsVisible = true, ScalarField = pointSet, Step = step });
        }

        private void AddDataSource(IDataSource2D<double> field, Guid guid)
        {
            // Get data and min/max.
            double[,] data = field.Data;
            double minT, maxT;
            MathHelper.GetMaxMin(data, out maxT, out minT);


            double k = 1.0 / (maxT - minT);


            double altitude = layerAltitude;

            //TODO: Perform right calculation depending on grid
            step = 0.3;

            List<Vertex.PositionColored> vertices = new List<Vertex.PositionColored>();
            for (int i = 0; i < field.Width; i++)
            {
                for (int j = 0; j < field.Height; j++)
                {
                    float x = (float)field.Grid[i, j].X;
                    float y = (float)field.Grid[i, j].Y;
                    LatLonAlt position = LatLonAlt.CreateUsingDegrees(y + step, x + step, altitude);
                    Vector3F vec = new Vector3F(position.GetVector());
                    vertices.Add(new Vertex.PositionColored(vec, Color.Red.ToArgb()));

                    position = LatLonAlt.CreateUsingDegrees(y - step, x - step, altitude);
                    vec = new Vector3F(position.GetVector());
                    vertices.Add(new Vertex.PositionColored(vec, Color.Red.ToArgb()));

                    position = LatLonAlt.CreateUsingDegrees(y + step, x - step, altitude);
                    vec = new Vector3F(position.GetVector());
                    vertices.Add(new Vertex.PositionColored(vec, Color.Red.ToArgb()));

                    position = LatLonAlt.CreateUsingDegrees(y - step, x + step, altitude);
                    vec = new Vector3F(position.GetVector());
                    vertices.Add(new Vertex.PositionColored(vec, Color.Red.ToArgb()));

                }
            }



            MeshGraphicsObject<Vertex.PositionColored, ushort> mesh = new MeshGraphicsObject<Vertex.PositionColored, ushort>(
                GraphicsBufferUsage.Static,
                GraphicsBufferUsage.Static,
                vertices.Count,
                vertices.Count,
                true);

            Material material = new Material { AmbientColor = Color.White, DiffuseColor = Color.White, SpecularColor = Color.White };
            int id = mesh.Materials.Add(material);

            mesh.Vertices.AddData(vertices.ToArray());

            List<ushort> indexData = new List<ushort>();
            for (int i = 0; i < field.Width; i++)
            {
                for (int j = 0; j < field.Height; j++)
                {

                    indexData.Add((ushort)((i * field.Height + j) * 4));
                    indexData.Add((ushort)((i * field.Height + j) * 4 + 1));
                    indexData.Add((ushort)((i * field.Height + j) * 4 + 2));
                    indexData.Add((ushort)((i * field.Height + j) * 4 + 3));



                }
            }

            mesh.Indices.AddData(indexData.ToArray(),
                        PrimitiveType.LineList,
                        id);

            mesh.RenderState.Lighting.Enabled = false;
            mesh.RenderState.Cull.Enabled = false;

            meshLayers.Add(new MeshLayer { LayerAltitude = altitude, Mesh = mesh, Guid = guid, IsVisible = true, ScalarField = field, Step = step });
        }



        private void RemoveActorLayer(VisualizationDataSource dataSource)
        {
            MeshLayer meshLayer = meshLayers.Find(ml => ml.Guid == dataSource.Guid);
            if (meshLayer != null)
            {
                meshLayers.Remove(meshLayer);
            }
        }

        private List<Vertex.PositionColored> BuildVertices(PointSet pointSet, double altitude)
        {
            List<Vertex.PositionColored> vertices = new List<Vertex.PositionColored>();

            double minT = 0, maxT = 0, k;
            if (!Double.IsNaN(pointSet.MaxValue))
            {
                minT = pointSet.MinValue;
                maxT = pointSet.MaxValue;
                k = 1.0 / (maxT - minT);
            }
            else
                k = Double.NaN;

            for (int j = 0; j < pointSet.Data.Count; j++)
            {
                float x = (float)pointSet.Data[j].Longitude;
                float y = (float)pointSet.Data[j].Latitude;
                LatLonAlt position = LatLonAlt.CreateUsingDegrees(y + step, x + step, altitude);
                Vector3F vec = new Vector3F(position.GetVector());

                double hue;
                Color color = Color.Red;
                System.Windows.Media.Color tempColor;

                if (!Double.IsNaN(k))
                {
                    hue = 270 * ((maxT - (double)pointSet.Data[j].Value) * k);
                    tempColor = new HsbColor(hue, 1, 1, 0.8).ToArgb();
                    color = Color.FromArgb(tempColor.A, tempColor.R, tempColor.G, tempColor.B);
                }

                vertices.Add(new Vertex.PositionColored(vec, color.ToArgb()));

                position = LatLonAlt.CreateUsingDegrees(y - step, x - step, altitude);
                vec = new Vector3F(position.GetVector());
                vertices.Add(new Vertex.PositionColored(vec, color.ToArgb()));

                position = LatLonAlt.CreateUsingDegrees(y + step, x - step, altitude);
                vec = new Vector3F(position.GetVector());
                vertices.Add(new Vertex.PositionColored(vec, color.ToArgb()));

                position = LatLonAlt.CreateUsingDegrees(y - step, x + step, altitude);
                vec = new Vector3F(position.GetVector());
                vertices.Add(new Vertex.PositionColored(vec, color.ToArgb()));

            }

            return vertices;
        }

        public override void Update(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            if (meshLayers.Count != 0)
            {
                CameraData data;
                if (sceneState.TryGetData<CameraData>(out data))
                {
                    double altitude = data.MetersAboveGround / 100.0;
                    bool needUpdate = false;
                    foreach (MeshLayer meshLayer in meshLayers)
                    {
                        if (meshLayer.LayerAltitude != altitude)
                        {
                            needUpdate = true;
                            if (meshLayer.ScalarField is PointSet)
                            {
                                PointSet pointSet = meshLayer.ScalarField as PointSet;
                                meshLayer.Mesh.Vertices.Clear();
                                meshLayer.Mesh.Vertices.AddData(BuildVertices(pointSet, altitude).ToArray());
                                meshLayer.LayerAltitude = altitude;
                                continue;
                            }
                        }
                    }
                    if (needUpdate)
                        host.NeedUpdate();
                    /*
                    if (mlayer != null && mlayer.LayerAltitude!=altitude)
                    {
                        
                    }*/


                }
            }


            base.Update(sceneState);
        }

        private bool CheckEnvirons(LatLonAlt point1, LatLonAlt point2, double eps)
        {
            if (Math.Sqrt(
                Math.Pow(Math.Abs(point1.LongitudeDegrees - point2.LongitudeDegrees), 2) +
                Math.Pow(Math.Abs(point1.LatitudeDegrees - point2.LatitudeDegrees), 2)) < eps)
                return true;
            else
                return false;

        }



        private List<VisualPushpin> intersectedValues;

        #region IMouseCheck Members

        public bool CheckIntersection(LatLonAlt location)
        {
            bool hasIntersections = false;

            foreach (VisualPushpin pin in intersectedValues)
            {
                host.Geometry.RemoveGeometry(pin.Pushpin.LayerId, pin.Pushpin.Id);
            }
            intersectedValues.Clear();

            lock (meshLayers)
            {
                foreach (MeshLayer meshLayer in meshLayers)
                {
                    if (meshLayer.IsVisible)
                    {

                        if (meshLayer.ScalarField is IDataSource2D<double>)
                        {
                            //for warped grids
                            IDataSource2D<double> field = meshLayer.ScalarField as IDataSource2D<double>;

                            Coordinate2D minCoordinate = new Coordinate2D(field.Grid[0, 0].X, field.Grid[0, 0].Y);
                            Coordinate2D maxCoordinate = new Coordinate2D(field.Grid[field.Width - 1, field.Height - 1].X, field.Grid[field.Width - 1, field.Height - 1].Y);


                            for (int j = 0; j < field.Height; j++)
                            {
                                for (int i = 0; i < field.Width; i++)
                                {
                                    if (field.Grid[i, j].Y < minCoordinate.X)
                                        minCoordinate.X = field.Grid[i, j].X;

                                    if (field.Grid[i, j].X > maxCoordinate.X)
                                        maxCoordinate.X = field.Grid[i, j].X;

                                    if (field.Grid[i, j].Y < minCoordinate.Y)
                                        minCoordinate.Y = field.Grid[i, j].Y;

                                    if (field.Grid[i, j].Y > maxCoordinate.Y)
                                        maxCoordinate.Y = field.Grid[i, j].Y;
                                }
                            }

                            if (location.LatitudeDegrees > minCoordinate.Y && location.LongitudeDegrees > minCoordinate.X && location.LongitudeDegrees < maxCoordinate.X && location.LatitudeDegrees < maxCoordinate.Y)
                            {
                                for (int i = 0; i < field.Width; i++)
                                {
                                    for (int j = 0; j < field.Height; j++)
                                    {
                                        LatLonAlt gridPos = LatLonAlt.CreateUsingDegrees(field.Grid[i, j].Y, field.Grid[i, j].X, meshLayer.LayerAltitude);
                                        if (CheckEnvirons(gridPos, location, meshLayer.Step))
                                        {
                                            if (!hasIntersections)
                                                hasIntersections = true;

                                            intersectedValues.Add(new VisualPushpin(50, 50, field.Data[i, j].ToString(), gridPos, null, Guid.NewGuid().ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        else if (meshLayer.ScalarField is PointSet)
                        {
                            PointSet pointSet = meshLayer.ScalarField as PointSet;
                            if (meshLayer.IsVisible)
                            {
                                for (int i = 0; i < pointSet.Data.Count; i++)
                                {
                                    LatLonAlt gridPos = LatLonAlt.CreateUsingDegrees(pointSet.Data[i].Latitude, pointSet.Data[i].Longitude, 0);
                                    if (CheckEnvirons(gridPos, location, 0.3))
                                    {
                                        if (!hasIntersections)
                                            hasIntersections = true;

                                        intersectedValues.Add(new VisualPushpin(60, 60, pointSet.Data[i].Value.ToString(), gridPos, null, Guid.NewGuid().ToString()));
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return hasIntersections;
        }

        public void ShowIntersectedValues()
        {
            foreach (VisualPushpin pin in intersectedValues)
            {
                host.Geometry.AddGeometry(pin.Pushpin);
            }
        }

        #endregion
    }

    class ScalarValue
    {
        public LatLonAlt Position { get; set; }
        public double Value { get; set; }
    }
}
