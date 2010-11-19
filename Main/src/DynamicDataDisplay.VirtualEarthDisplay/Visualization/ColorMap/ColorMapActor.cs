using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.Steps.Actors;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using swm = System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.MapPoint.Geometry;
using Microsoft.MapPoint.Rendering3D.GraphicsProxy;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.Geometry.VectorMath;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Drawing;
using Microsoft.MapPoint.Rendering3D.State;
using Microsoft.MapPoint.Binding;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    class ColorMapActor : Actor, IVisualizationProvider
    {
        public ColorMapActor(double layerAltitude, BindingsSource bs, ObservableCollection<VisualizationDataSource> dataSources, Host host)
            : base(bs)
        {
            LinearPalette palette = new LinearPalette(swm.Colors.Blue, swm.Colors.Green, swm.Colors.Red);
            palette.IncreaseBrightness = false;
            this.palette = palette;

            meshLayers = new List<MeshLayer>();
            layerHelper = new LayerHelper(layerAltitude);

            this.host = host;

            dataSources.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(dataSources_CollectionChanged);
        }

        private const byte opacity = 50;
        private Host host;
        private List<MeshLayer> meshLayers;
        private LayerHelper layerHelper;
        private IPalette palette;

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
                        else if (newDataSource.Algorithm == Algorithms.ColorMap)
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
                        else if (removedDataSource.Algorithm == Algorithms.ColorMap)
                        {
                            RemoveActorLayer(removedDataSource);
                        }
                    }
                    break;
                default: break;
            }
        }

        private void AddDataSource(IDataSource2D<double> field, Guid guid)
        {
            double minT, maxT;

            //if (colorMapControl.PaletteType == PaletteType.FromMaxMin)
            //{
            //    minT = colorMapControl.MinValue;
            //    maxT = colorMapControl.MaxValue;
            //}
            //else
            //{
            MathHelper.GetMaxMin(field.Data, out maxT, out minT);

            //colorMapControl.MinValue = minT;
            //colorMapControl.MaxValue = maxT;
            //}

            double k = 1.0 / (maxT - minT);

            List<Vertex.PositionColored> vertices = new List<Vertex.PositionColored>();

            double altitude = layerHelper.GetNewAltitude();

            for (int i = 0; i < field.Width; i++)
            {
                for (int j = 0; j < field.Height; j++)
                {
                    float x = (float)field.Grid[i, j].X;
                    float y = (float)field.Grid[i, j].Y;
                    LatLonAlt position = LatLonAlt.CreateUsingDegrees(y, x, altitude);
                    Vector3F vec = new Vector3F(position.GetVector());
                    System.Windows.Media.Color color = palette.GetColor((field.Data[i, j] - minT) * k);
                    vertices.Add(new Vertex.PositionColored(vec, Color.FromArgb(opacity, color.R, color.G, color.B).ToArgb()));
                }
            }

            MeshGraphicsObject<Vertex.PositionColored, ushort> mesh = new MeshGraphicsObject<Vertex.PositionColored, ushort>(
                GraphicsBufferUsage.Static,
                GraphicsBufferUsage.Static,
                vertices.Count,
                vertices.Count * 6,
                true);

            Material material = new Material { AmbientColor = Color.White, DiffuseColor = Color.White, SpecularColor = Color.White };
            int id = mesh.Materials.Add(material);

            mesh.Vertices.AddData(vertices.ToArray());

            for (int i = 0; i < field.Width - 1; i++)
            {
                for (int j = 0; j < field.Height - 1; j++)
                {

                    mesh.Indices.AddData(

                        (ushort)(i * field.Height + j),
                        (ushort)(i * field.Height + j + 1),

                        (ushort)((i + 1) * field.Height + j),

                        PrimitiveType.TriangleList, id);

                    mesh.Indices.AddData(
                        (ushort)(i * field.Height + j + 1),
                        (ushort)((i + 1) * field.Height + j + 1),
                        (ushort)((i + 1) * field.Height + j),

                        PrimitiveType.TriangleList, id);
                }
            }


            mesh.RenderState.Lighting.Enabled = false;
            mesh.RenderState.Alpha.Enabled = true;
            mesh.RenderState.Alpha.AlphaTestEnable = true;
            mesh.RenderState.Alpha.SourceBlend = Blend.BothInvSourceAlpha;
            mesh.RenderState.Alpha.DestinationBlend = Blend.BothInvSourceAlpha;
            mesh.RenderState.Cull.Enabled = false;

            meshLayers.Add(new MeshLayer { LayerAltitude = altitude, Mesh = mesh, Guid = guid, IsVisible = true, ScalarField = field });

        }

        public override void Update(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            base.Update(sceneState);
        }

        public override void Render(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            if (meshLayers.Count != 0)
            {
                CameraData data;
                if (sceneState.TryGetData<CameraData>(out data))
                {
                    RenderQueues renderQueues = sceneState.GetData<RenderQueues>();
                    lock (meshLayers)
                    {
                        for (int i = 0; i < meshLayers.Count; i++)
                        {
                            if (meshLayers[i].IsVisible)
                            {
                                renderQueues.AddAlphaRenderable(data.Snapshot.Position.Altitude - meshLayers[i].LayerAltitude, meshLayers[i].Mesh);
                            }
                        }
                    }
                }
            }
        }

        private void AddActorLayer(VisualizationDataSource dataSource)
        {
            if (dataSource.Algorithm == Algorithms.ColorMap && dataSource.IsDynamic)
            {
                if (dataSource.Data is IDataSource2D<double>)
                    AddDataSource(dataSource.Data as IDataSource2D<double>, dataSource.Guid);
                host.NeedUpdate();
            }
        }

        private void RemoveActorLayer(VisualizationDataSource dataSource)
        {
            MeshLayer meshLayer = meshLayers.Find(ml => ml.Guid == dataSource.Guid);
            if (meshLayer != null)
            {
                meshLayers.Remove(meshLayer);
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

        bool IVisualizationProvider.ContainsLayer(Guid guid)
        {
            foreach (MeshLayer meshLayer in meshLayers)
            {
                if (meshLayer.Guid == guid)
                    return true;
            }
            return false;
        }

        void IVisualizationProvider.AddLayer(VisualizationDataSource dataSource)
        {
            AddActorLayer(dataSource);
        }

        void IVisualizationProvider.RemoveLayer(VisualizationDataSource dataSource)
        {
            RemoveActorLayer(dataSource);
        }

        Algorithms IVisualizationProvider.GetAlgorithmType()
        {
            return Algorithms.ColorMap;
        }

        #endregion
    }
}
