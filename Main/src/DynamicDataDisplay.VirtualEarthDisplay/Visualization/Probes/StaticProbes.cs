using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.MapPoint.Rendering3D.Utility;
using Microsoft.MapPoint.CoordinateSystems;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes
{
    class StaticProbes : IVisualizationProvider, IMouseCheck
    {
        private List<ProbesLayer> probesLayers;
        private Host host;

        public StaticProbes(ObservableCollection<VisualizationDataSource> dataSources, Host host)
        {
            dataSources.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(dataSources_CollectionChanged);
            this.host = host;
            probesLayers = new List<ProbesLayer>();

            intersectedValues = new List<VisualPushpin>();
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
                            AddProbesLayer(newDataSource);
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
                            RemoveProbesLayer(removedDataSource);
                        }
                    }
                    break;
                default: break;
            }
        }

        private void RemoveProbesLayer(VisualizationDataSource removedDataSource)
        {
            RemoveLayer(removedDataSource);
        }

        private void AddProbesLayer(VisualizationDataSource dataSource)
        {
            if (dataSource.Algorithm == Algorithms.Probes && !dataSource.IsDynamic)
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

        private void AddDataSource(PointSet data, Guid guid)
        {
            probesLayers.Add(new ProbesLayer
            {
                Guid = guid,
                IsVisible = true,
                DataSource = new ProbesDataSource(guid, data, host),
                LayerID = Guid.NewGuid().ToString(),
                LayerName = Guid.NewGuid().ToString()
            });
            AddLayerToHost(probesLayers[probesLayers.Count - 1]);
        }

        private void AddDataSource(IDataSource2D<double> field, Guid guid)
        {
            probesLayers.Add(new ProbesLayer
            {
                Guid = guid,
                IsVisible = true,
                DataSource = new ProbesDataSource(guid, field, host),
                LayerID = Guid.NewGuid().ToString(),
                LayerName = Guid.NewGuid().ToString()
            });
            AddLayerToHost(probesLayers[probesLayers.Count - 1]);
        }

        private void AddLayerToHost(ProbesLayer probesLayer)
        {
            host.DataSources.Add(new DataSourceLayerData(probesLayer.LayerID, probesLayer.LayerName, probesLayer.DataSource, DataSourceUsage.TextureMap, 999, 1));
        }

        #region IVisualizationProvider Members

        public void AddLayer(VisualizationDataSource dataSource)
        {
            AddProbesLayer(dataSource);
        }

        public void RemoveLayer(VisualizationDataSource dataSource)
        {
            ProbesLayer probesLayer = probesLayers.Find(pl => pl.Guid == dataSource.Guid);
            if (probesLayer != null)
            {
                host.DataSources.Remove(probesLayer.LayerID, probesLayer.LayerName);
                probesLayers.Remove(probesLayer);
            }
        }

        public Algorithms GetAlgorithmType()
        {
            return Algorithms.Probes;
        }

        public Guid ProviderGuid
        {
            get;
            set;
        }

        public bool IsDynamicProvider
        {
            get;
            set;
        }

        bool IVisualizationProvider.ContainsLayer(Guid guid)
        {
            foreach (ProbesLayer pLayer in this.probesLayers)
            {
                if (pLayer.Guid == guid)
                    return true;
            }
            return false;
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

        public bool CheckIntersection(LatLonAlt location)
        {
            bool hasIntersections = false;

            foreach (VisualPushpin pin in intersectedValues)
            {
                host.Geometry.RemoveGeometry(pin.Pushpin.LayerId, pin.Pushpin.Id);
            }
            intersectedValues.Clear();

            lock (probesLayers)
            {
                foreach (ProbesLayer probesLayer in probesLayers)
                {
                    if (probesLayer.IsVisible)
                    {

                        if (probesLayer.DataSource.Field is IDataSource2D<double>)
                        {
                            //for warped grids
                            IDataSource2D<double> field = probesLayer.DataSource.Field as IDataSource2D<double>;
                            System.Windows.Point[,] grid = field.Grid;

                            Coordinate2D minCoordinate = new Coordinate2D(grid[0, 0].X, grid[0, 0].Y);
                            Coordinate2D maxCoordinate = new Coordinate2D(grid[field.Width - 1, field.Height - 1].X, grid[field.Width - 1, field.Height - 1].Y);

                            bool intersectionFound = false;
                            for (int j = 0; j < field.Height; j++)
                            {
                                for (int i = 0; i < field.Width; i++)
                                {
                                    if (grid[i, j].X < minCoordinate.X)
                                        minCoordinate.X = grid[i, j].X;

                                    if (grid[i, j].X > maxCoordinate.X)
                                        maxCoordinate.X = grid[i, j].X;

                                    if (grid[i, j].Y < minCoordinate.Y)
                                        minCoordinate.Y = grid[i, j].Y;

                                    if (grid[i, j].Y > maxCoordinate.Y)
                                        maxCoordinate.Y = grid[i, j].Y;
                                }
                            }

                            if (location.LatitudeDegrees > minCoordinate.Y && location.LongitudeDegrees > minCoordinate.X && location.LongitudeDegrees < maxCoordinate.X && location.LatitudeDegrees < maxCoordinate.Y)
                            {
                                for (int i = 0; i < field.Width; i++)
                                {
                                    for (int j = 0; j < field.Height; j++)
                                    {
                                        LatLonAlt gridPos = LatLonAlt.CreateUsingDegrees(field.Grid[i, j].Y, field.Grid[i, j].X, 0);
                                        if (CheckEnvirons(gridPos, location, probesLayer.DataSource.Step))
                                        {
                                            if (!hasIntersections)
                                                hasIntersections = true;

                                            intersectedValues.Add(new VisualPushpin(60, 60, field.Data[i, j].ToString(), gridPos, null, Guid.NewGuid().ToString()));
                                            intersectionFound = true;
                                            break;
                                        }
                                    }
                                    if (intersectionFound)
                                        break;
                                }
                            }
                        }
                        else if (probesLayer.DataSource.Field is PointSet)
                        {
                            PointSet pointSet = probesLayer.DataSource.Field as PointSet;
                            if (probesLayer.IsVisible)
                            {
                                for (int i = 0; i < pointSet.Data.Count; i++)
                                {
                                    LatLonAlt gridPos = LatLonAlt.CreateUsingDegrees(pointSet.Data[i].Latitude, pointSet.Data[i].Longitude, 0);
                                    if (CheckEnvirons(gridPos, location, probesLayer.DataSource.Step))
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
}