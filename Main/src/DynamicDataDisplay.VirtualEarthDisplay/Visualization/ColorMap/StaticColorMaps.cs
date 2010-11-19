using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.MapPoint.Rendering3D;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.MapPoint.Rendering3D.Utility;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    class StaticColorMaps : IVisualizationProvider
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
            foreach (ColorMapLayer cmLayer in this.colorMapLayers)
            {
                if (cmLayer.Guid == guid)
                    return true;
            }
            return false;
        }

        public void AddLayer(VisualizationDataSource dataSource)
        {
            AddColorMapLayer(dataSource);
        }

        public void RemoveLayer(VisualizationDataSource dataSource)
        {
            RemoveColorMapLayer(dataSource);
        }


        public Algorithms GetAlgorithmType()
        {
            return Algorithms.ColorMap;
        }

        #endregion

        private IPalette palette;
        private List<ColorMapLayer> colorMapLayers;
        private Host host;



        public StaticColorMaps(ObservableCollection<VisualizationDataSource> dataSources, Host host)
        {
            dataSources.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(dataSources_CollectionChanged);
            this.host = host;
            colorMapLayers = new List<ColorMapLayer>();
            palette = new LinearPalette();
        }


        private void RemoveColorMapLayer(VisualizationDataSource dataSource)
        {
            ColorMapLayer colorMapLayer = colorMapLayers.Find(cml => cml.Guid == dataSource.Guid);
            if (colorMapLayer != null)
            {
                if (colorMapLayer.IsVisible)
                {
                    host.DataSources.Remove(colorMapLayer.LayerName, colorMapLayer.LayerID);
                }
                colorMapLayers.Remove(colorMapLayer);
            }
        }

        private void AddColorMapLayer(VisualizationDataSource dataSource)
        {
            if (dataSource.Algorithm == Algorithms.ColorMap && !dataSource.IsDynamic)
            {
                if (dataSource.Data is IDataSource2D<double>)
                {
                    AddDataSource(dataSource.Data, dataSource.Guid, double.MinValue, double.MaxValue);
                }
                
            }
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
                        else if (newDataSource.Algorithm == Algorithms.ColorMap)
                        {
                            AddColorMapLayer(newDataSource);
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
                            RemoveColorMapLayer(removedDataSource);
                        }
                    }
                    break;
                default: break;
            }
        }

        private void AddDataSource(object field, Guid guid, double  minT, double maxT)
        {

            colorMapLayers.Add(
                new ColorMapLayer
                {
                    Guid = guid,
                    LayerName = Guid.NewGuid().ToString(),
                    LayerID = Guid.NewGuid().ToString(),
                    IsVisible = true,
                    Field = field,
                    LayerOpacity = 0.75,
                    MaxT = maxT,
                    MinT = minT
                });

            AddLayerToHost(colorMapLayers[colorMapLayers.Count - 1]);
        }

        private void AddLayerToHost(ColorMapLayer colorMapLayer)
        {
            ColorMapDataSource cmds;
            if (colorMapLayer.Field is NonUniformDataSource2D<double>)
            {
                cmds = new ColorMapDataSource(colorMapLayer.Field as NonUniformDataSource2D<double>, this.host, colorMapLayer.MinT, colorMapLayer.MaxT);
            }
            else
            {
                cmds = new ColorMapDataSource(colorMapLayer.Field as WarpedDataSource2D<double>, this.host, colorMapLayer.MinT, colorMapLayer.MaxT);
            }
            host.DataSources.Add(new DataSourceLayerData(colorMapLayer.LayerName, colorMapLayer.LayerID, cmds, DataSourceUsage.TextureMap, 101, colorMapLayer.LayerOpacity));

        }

        private double Interpolate(double C00, double C01, double C10, double C11, double alpha, double beta)
        {
            return C00 * (1 - alpha) * (1 - beta) + C10 * alpha * (1 - beta) + C01 * (1 - alpha) * beta + C11 * alpha * beta;
        }

    }

    class GeoToBitmap
    {
        public int Knot { get; set; }
        public double Proportion { get; set; }
    }
}
