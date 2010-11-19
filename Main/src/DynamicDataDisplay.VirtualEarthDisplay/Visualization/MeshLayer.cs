using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Geometry;
using Microsoft.MapPoint.Rendering3D.GraphicsProxy;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    class MeshLayer
    {
        /// <summary>
        /// MeshLayer Guid
        /// </summary>
        public Guid Guid { get; set; }

        public bool IsVisible { get; set; }

        private MeshGraphicsObject<Vertex.PositionColored, ushort> mesh;

        /// <summary>
        /// Rendering mesh of the layer
        /// </summary>
        public MeshGraphicsObject<Vertex.PositionColored, ushort> Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }
        private double layerAltitude;

        /// <summary>
        /// Layer altitude
        /// </summary>
        public double LayerAltitude
        {
            get { return layerAltitude; }
            set { layerAltitude = value; }
        }

        public object ScalarField { get; set; }

        /// <summary>
        /// For discrete meshes (for example, in probes algorithm)
        /// </summary>
        public double Step { get; set; }

    }



    class LayerHelper
    {
        public LayerHelper(double startAltitude)
        {
            this.startAltitude = startAltitude;

            freeAltitudes = new List<double>();
            freeAltitudes.Add(startAltitude);

            reservedAltitudes = new List<double>();
        }

        private const double altitudeStep = 100;

        private double startAltitude;

        public double StartAltitude
        {
            get { return startAltitude; }
        }

        private List<double> freeAltitudes;
        private List<double> reservedAltitudes;

        public double GetNewAltitude()
        {
            if (freeAltitudes.Count != 0)
            {
                double result = freeAltitudes[0];
                freeAltitudes.Remove(result);
                reservedAltitudes.Add(result);
                return result;
            }
            else
            {
                double maxAltitude = reservedAltitudes[0];
                foreach (double altitude in reservedAltitudes)
                {
                    if (altitude > maxAltitude)
                        maxAltitude = altitude;
                }
                maxAltitude += altitudeStep;
                reservedAltitudes.Add(maxAltitude);
                return maxAltitude;
            }
        }

        public void RemoveAltitude(double altitude)
        {
            if (reservedAltitudes.Contains(altitude))
            {
                reservedAltitudes.Remove(altitude);
                freeAltitudes.Add(altitude);
            }
            else
                throw new ArgumentException("Invalid altitude value");
        }
    }
}
