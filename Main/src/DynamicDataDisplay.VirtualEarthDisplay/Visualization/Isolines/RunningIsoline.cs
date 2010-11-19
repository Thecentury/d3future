using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.GraphicsProxy;
using Microsoft.MapPoint.Geometry;
using System.Drawing;
using Microsoft.MapPoint.Geometry.VectorMath;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.Rendering3D.State;
using Microsoft.MapPoint.Rendering3D.Steps.Actors;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.MapPoint.Binding;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Isolines
{
    class RunningIsoline : Actor
    {
        Host host;

        bool needUpdate = false;
        LevelLine newSegment, oldSegment;
        public LevelLine Segment
        {
            get { return oldSegment; }
            set
            {
                newSegment = value;
                needUpdate = true;
            }

        }

        private Color lineColor = Color.White;

        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }


        public RunningIsoline(BindingsSource bs, Host host)
            : base(bs)
        {
            this.host = host;
        }

        public override void Update(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            if (needUpdate && newSegment != null)
            {
                CameraData data;
                if (sceneState.TryGetData<CameraData>(out data))
                {
                    double altitude = data.MetersAboveGround / 100.0;

                    List<Vertex.PositionColored> vertices = new List<Vertex.PositionColored>();

                    LatLonAlt position = LatLonAlt.CreateUsingDegrees(newSegment.StartPoint.Y, newSegment.StartPoint.X, altitude);
                    Vector3F vec = new Vector3F(position.GetVector());
                    vertices.Add(new Vertex.PositionColored(vec, lineColor.ToArgb()));

                    foreach (System.Windows.Point point in newSegment.OtherPoints)
                    {
                        position = LatLonAlt.CreateUsingDegrees(point.Y, point.X, altitude);
                        vec = new Vector3F(position.GetVector());
                        vertices.Add(new Vertex.PositionColored(vec, lineColor.ToArgb()));
                    }

                    mesh = new MeshGraphicsObject<Vertex.PositionColored, ushort>(
                        GraphicsBufferUsage.Static,
                        GraphicsBufferUsage.Static,
                        vertices.Count,
                        vertices.Count,
                        true);

                    Material material = new Material { AmbientColor = Color.White, DiffuseColor = Color.White, SpecularColor = Color.White };
                    int id = mesh.Materials.Add(material);

                    mesh.Vertices.AddData(vertices.ToArray());

                    List<ushort> indexData = new List<ushort>();
                    for (int i = 0; i < newSegment.OtherPoints.Count + 1; i++)
                    {
                        indexData.Add((ushort)i);
                    }

                    mesh.Indices.AddData(indexData.ToArray(),
                        PrimitiveType.LineStrip,
                        id);

                    mesh.RenderState.Lighting.Enabled = false;
                    mesh.RenderState.Cull.Enabled = false;

                    oldSegment = newSegment;
                    needUpdate = false;
                    host.NeedUpdate();
                }
            }

            base.Update(sceneState);
        }


        MeshGraphicsObject<Vertex.PositionColored, ushort> mesh;

        public override void Render(Microsoft.MapPoint.Rendering3D.Scene.SceneState sceneState)
        {
            if (mesh != null)
            {
                RenderQueues renderQueues = sceneState.GetData<RenderQueues>();
                renderQueues.AddRenderable(mesh);
            }
        }


    }
}

