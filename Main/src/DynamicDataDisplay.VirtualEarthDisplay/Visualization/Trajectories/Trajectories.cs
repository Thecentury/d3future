using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.MapPoint.CoordinateSystems;
using Microsoft.MapPoint.Rendering3D.Utility;
using System.Drawing;
using Microsoft.MapPoint.Geometry;
using Microsoft.MapPoint.Geometry.Geometry2;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Trajectories
{
    class StaticTrajectories : IVisualizationProvider, IMouseCheck
    {
        Host host;
        private List<TrajectoriesLayer> trajectoriesLayers;

        public StaticTrajectories(ObservableCollection<VisualizationDataSource> dataSources, Host host)
        {
            dataSources.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(dataSources_CollectionChanged);
            this.host = host;
            trajectoriesLayers = new List<TrajectoriesLayer>();
            intersectedValues = new List<VisualPushpin>();
        }

        void dataSources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                        else if (newDataSource.Algorithm == Algorithms.Trajectories)
                        {
                            AddLayer(newDataSource);
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
                        else if (removedDataSource.Algorithm == Algorithms.Trajectories)
                        {
                            RemoveLayer(removedDataSource);
                        }
                    }
                    break;
                default: break;
            }
        }

        private void AddDataSource(PointSet pointSet, Guid guid)
        {
            List<Coordinate2D> points = new List<Coordinate2D>();

            foreach (PointSetElement point in pointSet.Data)
            {
                points.Add(new Coordinate2D(point.Longitude, point.Latitude));
            }
            PolyInfo style = PolyInfo.DefaultPolyline;
            style.LineColor = Color.Red;
            style.LineWidth = 0.5;

            string layerId = Guid.NewGuid().ToString();
            string geometryId = Guid.NewGuid().ToString();
            string dataSourceId = Guid.NewGuid().ToString();



            trajectoriesLayers.Add(new TrajectoriesLayer
            {
                DataSource = new TrajectoriesDataSource(host, pointSet),
                Guid = guid,
                GeometryID = geometryId,
                LayerID = layerId,
                DataSourceID = dataSourceId,
                IsVisible = true,
                Geometry = new PolylineGeometry(layerId, geometryId, new Polyline2(Wgs84CoordinateReferenceSystem.Instance,
                   Coordinate2DCollection.CreateUnsafe(points.ToArray())), style)
            });
            AddLayerToHost(trajectoriesLayers[trajectoriesLayers.Count - 1]);


        }

        private void AddLayerToHost(TrajectoriesLayer trajectoriesLayer)
        {
            host.Geometry.AddGeometry(trajectoriesLayer.Geometry);
            host.DataSources.Add(new DataSourceLayerData(trajectoriesLayer.LayerID, trajectoriesLayer.DataSourceID, trajectoriesLayer.DataSource,
                 DataSourceUsage.TextureMap));
        }


        #region IVisualizationProvider Members

        public void AddLayer(VisualizationDataSource dataSource)
        {
            if (dataSource.Algorithm == Algorithms.Trajectories)
            {
                if (dataSource.Data is PointSet)
                {
                    AddDataSource(dataSource.Data as PointSet, dataSource.Guid);
                }
            }
        }

        private void RemoveLayerFromHost(TrajectoriesLayer trLayer)
        {
            host.DataSources.Remove(trLayer.LayerID, trLayer.DataSourceID);
            host.Geometry.RemoveGeometry(trLayer.LayerID, trLayer.GeometryID);
        }


        public void RemoveLayer(VisualizationDataSource dataSource)
        {
            TrajectoriesLayer trLayer = trajectoriesLayers.Find(tl => tl.Guid == dataSource.Guid);
            if (trLayer != null)
            {
                if (trLayer.IsVisible)
                    RemoveLayerFromHost(trLayer);
                trajectoriesLayers.Remove(trLayer);
            }
        }

        public bool ContainsLayer(Guid guid)
        {
            TrajectoriesLayer trajectoriesLayer = trajectoriesLayers.Find(tl => tl.Guid == guid);
            return trajectoriesLayer != null;
        }

        public Algorithms GetAlgorithmType()
        {
            return Algorithms.Trajectories;
        }

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

        #endregion

        private bool CheckEnvirons(LatLonAlt point1, LatLonAlt point2, double eps)
        {
            if (Math.Sqrt(
                Math.Pow(Math.Abs(point1.LongitudeDegrees - point2.LongitudeDegrees), 2) +
                Math.Pow(Math.Abs(point1.LatitudeDegrees - point2.LatitudeDegrees), 2)) < eps)
                return true;
            else
                return false;

        }

        #region IMouseCheck Members

        private List<VisualPushpin> intersectedValues;

        public bool CheckIntersection(Microsoft.MapPoint.Rendering3D.LatLonAlt location)
        {
            bool hasIntersections = false;

            foreach (VisualPushpin pin in intersectedValues)
            {
                host.Geometry.RemoveGeometry(pin.Pushpin.LayerId, pin.Pushpin.Id);
            }
            intersectedValues.Clear();

            lock (trajectoriesLayers)
            {
                foreach (TrajectoriesLayer trajectoriesLayer in trajectoriesLayers)
                {
                    if (trajectoriesLayer.IsVisible)
                    {
                        for (int i = 0; i < trajectoriesLayer.DataSource.PointSet.Data.Count; i++)
                        {
                            LatLonAlt gridPos = LatLonAlt.CreateUsingDegrees(trajectoriesLayer.DataSource.PointSet.Data[i].Latitude, trajectoriesLayer.DataSource.PointSet.Data[i].Longitude, 0);
                            if (CheckEnvirons(gridPos, location, trajectoriesLayer.DataSource.Step))
                            {
                                if (!hasIntersections)
                                    hasIntersections = true;

                                intersectedValues.Add(new VisualPushpin(60, 60, trajectoriesLayer.DataSource.PointSet.Data[i].Value.ToString(), gridPos, null, Guid.NewGuid().ToString()));
                                break;
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
}
