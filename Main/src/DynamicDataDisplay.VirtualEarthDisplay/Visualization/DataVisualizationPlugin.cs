using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.PlugIns;
using Microsoft.MapPoint.Rendering3D;
using System.Collections.ObjectModel;
using Microsoft.Ccr.Core;
using DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap;
using Microsoft.MapPoint.Rendering3D.Steps.Actors;
using Microsoft.MapPoint.Binding;
using DynamicDataDisplay.VirtualEarthDisplay.Visualization.Isolines;
using DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes;
using DynamicDataDisplay.VirtualEarthDisplay.Visualization.Trajectories;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    /// <summary>
    /// Visualization PlugIn for VirtualEarth3D
    /// </summary>
    public class DataVisualizationPlugin : PlugIn
    {
        #region Helper elements for inner procceses
        private Port<EmptyValue> mousePort = new Port<EmptyValue>();
        private static DispatcherQueue dispatcherQueue = new DispatcherQueue("vevisplugin");
        #endregion
        
        /// <summary>
        /// Data sources container
        /// </summary>
        private ObservableCollection<VisualizationDataSource> dataSources;

        /// <summary>
        /// Hack for prevent VE3D from rather slow work
        /// </summary>
        private const int DataSourceMaxCount = 21;

        /// <summary>
        /// Hardcore constant
        /// </summary>
        private const double startAltitude = 100000;


        /// <summary>
        /// Availible visualization providers
        /// </summary>
        private List<IVisualizationProvider> pluginVisualizationProviders;

        public override string Name
        {
            get { return "DataVisualization"; }
        }

        public DataVisualizationPlugin(Host host)
            : base(host)
        {
            
        }

        /// <summary>
        /// Activates this plug-in.  This should be written such that it can be called multiple times
        /// along with Deactivate.
        /// </summary>
        /// <param name="activationObject">Passed by the activator, in this case script.</param>
        public override void Activate(object activationObject)
        {
            base.Activate(activationObject);

            Host.BindingsManager.RegisterAction(this.BindingsSource, "GeometryMouse", MouseMove);
            Host.BindingsManager.AddActiveBindingSet(this.BindingsSource, "Geometry");

            dataSources = new ObservableCollection<VisualizationDataSource>();
            pluginVisualizationProviders = new List<IVisualizationProvider>();

            //ColorMap providers
            Guid colorMapGuid = Guid.NewGuid();
            pluginVisualizationProviders.Add(new ColorMapActor(startAltitude, this.BindingsSource, this.dataSources, Host) { IsDynamicProvider = true, ProviderGuid = colorMapGuid });
            pluginVisualizationProviders.Add(new StaticColorMaps(this.dataSources, this.Host) { IsDynamicProvider = false, ProviderGuid = colorMapGuid });

            
            //Probes providers
            Guid probesGuid = Guid.NewGuid();
            pluginVisualizationProviders.Add(new ProbesActor(this.Host, startAltitude, this.BindingsSource, this.dataSources) { IsDynamicProvider = true, ProviderGuid = probesGuid });
            pluginVisualizationProviders.Add(new StaticProbes(this.dataSources, this.Host) { IsDynamicProvider = false, ProviderGuid = probesGuid });
            
            //Isolines providers
            Guid isolinesGuid = Guid.NewGuid();
            pluginVisualizationProviders.Add(new StaticIsolines(this.dataSources, this.Host) { IsDynamicProvider = false, ProviderGuid = isolinesGuid });
            
            //Trajectories providers
            Guid trajectoriesGuid = Guid.NewGuid();
            pluginVisualizationProviders.Add(new StaticTrajectories(this.dataSources, this.Host) { IsDynamicProvider = false, ProviderGuid = trajectoriesGuid });
            
            foreach (IVisualizationProvider item in pluginVisualizationProviders)
            {
                if (item is Actor)
                {
                    Host.Actors.Add(item as Actor);
                }
            }


            Arbiter.Activate(dispatcherQueue, Arbiter.Interleave(
                new TeardownReceiverGroup(),
                new ExclusiveReceiverGroup(
                    Arbiter.Receive<EmptyValue>(true, mousePort,
                            delegate(EmptyValue val)
                            {
                                if (Host.Navigation.PointerPosition != null)
                                {
                                    LatLonAlt location = Host.Navigation.PointerPosition.Location;
                                    foreach (IVisualizationProvider visualizationProvider in pluginVisualizationProviders)
                                    {
                                        if (visualizationProvider is IMouseCheck)
                                        {
                                            IMouseCheck provider = visualizationProvider as IMouseCheck;
                                            if (provider.CheckIntersection(location))
                                            {
                                                provider.ShowIntersectedValues();
                                            }
                                        }
                                    }
                                }
                            })),
                  new ConcurrentReceiverGroup()
                    ));
        }

        /// <summary>
        /// Deactivates this plug-in.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
        }

        public AddDataResult AddNewDataSource(VisualizationDataSource dataSource)
        {
            VisualizationDataSource oldDataSource = null;
            foreach (VisualizationDataSource innerDataSource in dataSources)
            {
                if (innerDataSource.ParameterName == dataSource.ParameterName)
                {
                    oldDataSource = innerDataSource;
                    break;
                }
            }
            if (oldDataSource != null)
            {
                Algorithms algorithm = oldDataSource.Algorithm;
                dataSources.Remove(oldDataSource);
                dataSource.Algorithm = algorithm;
                dataSource.Data = AnalyzeData(dataSource.Data);
                dataSources.Add(dataSource);

                return AddDataResult.Replaced;
            }
            else if (dataSources.Count < DataSourceMaxCount)
            {
                dataSource.Data = AnalyzeData(dataSource.Data);
                dataSources.Add(dataSource);
                return AddDataResult.Added;
            }
            else
            {
                return AddDataResult.Skipped;
            }
        }

        private object AnalyzeData(object sourceData)
        {
            if (sourceData is NonUniformDataSource2D<double>)
            {
                NonUniformDataSource2D<double> field = sourceData as NonUniformDataSource2D<double>;

                double[] lats = new double[field.Height];
                double[] lons = new double[field.Width];
                double[,] data = new double[field.Width, field.Height];

                if (field.X[field.Width - 1] < field.X[0])
                {
                    if (field.Y[field.Height - 1] < field.Y[0])
                    {
                        for (int i = 0; i < field.Width; i++)
                        {
                            for (int j = 0; j < field.Height; j++)
                            {
                                lats[j] = field.Y[field.Height - 1 - j];
                                lons[i] = field.X[field.Width - 1 - i];

                                data[i, j] = field.Data[field.Width - 1 - i, field.Height - 1 - j];
                            }
                        }
                        field = new NonUniformDataSource2D<double>(data, lats, lons);
                    }
                    else
                    {
                        for (int i = 0; i < field.Width; i++)
                        {
                            for (int j = 0; j < field.Height; j++)
                            {
                                lats[j] = field.Y[j];
                                lons[i] = field.X[field.Width - 1 - i];

                                data[i, j] = field.Data[field.Width - 1 - i, j];
                            }
                        }
                        field = new NonUniformDataSource2D<double>(data, lats, lons);
                    }
                }
                else
                {
                    if (field.Y[field.Height - 1] < field.Y[0])
                    {
                        for (int i = 0; i < field.Width; i++)
                        {
                            for (int j = 0; j < field.Height; j++)
                            {
                                lats[j] = field.Y[field.Height - 1 - j];
                                lons[i] = field.X[i];

                                data[i, j] = field.Data[i, field.Height - 1 - j];
                            }
                        }
                        field = new NonUniformDataSource2D<double>(data, lats, lons);
                    }
                }

                int workIndex = field.Width;
                for (int i = 0; i < field.Width; i++)
                {
                    if (field.X[i] > 180)
                    {
                        workIndex = i;
                        break;
                    }
                }

                if (workIndex < field.Width)
                {
                    double[] lats2 = new double[field.Height];
                    double[] lons2 = new double[field.Width];
                    double[,] data2 = new double[field.Width, field.Height];

                    for (int i = 0; i < field.Width; i++)
                    {
                        for (int j = 0; j < field.Height; j++)
                        {
                            if (i < field.Width + 1 - workIndex)
                            {
                                lons2[i] = field.X[workIndex + i - 1] - 360;
                                lats2[j] = field.Y[j];
                                data2[i, j] = field.Data[workIndex + i - 1, j];
                            }
                            else
                            {
                                lons2[i] = field.X[i - (field.Width + 1 - workIndex)];
                                lats2[j] = field.Y[j];
                                data2[i, j] = field.Data[i - (field.Width + 1 - workIndex), j];
                            }
                        }
                    }
                    field = new NonUniformDataSource2D<double>(data2, lats2, lons2);
                    TryToCircle(ref field);
                }

                return field;

            }
            else
                return sourceData;
        }

        private void TryToCircle(ref NonUniformDataSource2D<double> nfield)
        {
            double[] lons = nfield.X;
            if (lons.Length <= 2) return;
            if (lons[0] > lons[1]) return; // we're supporting now only increaing values

            double delta0 = lons[1] - lons[0];
            double delta1 = 360.0 - (lons[lons.Length - 1] - lons[0]);
            if (delta1 > 0 && (delta1 <= 1.2 * delta0))
            {
                // Hooray! Circling it....
                double[] lats = nfield.Y;
                double[,] data = new double[nfield.Width + 1, nfield.Height];
                double[,] sdata = nfield.Data;

                Array.Resize(ref lons, lons.Length + 1);
                lons[lons.Length - 1] = lons[0] + 360.0;

                int i;
                for (i = 0; i < lons.Length - 1; i++)
                    for (int j = 0; j < lats.Length; j++)
                        data[i, j] = sdata[i, j];
                for (int j = 0; j < lats.Length; j++)
                    data[i, j] = sdata[0, j];

                nfield = new NonUniformDataSource2D<double>(data,lats,lons);
            }
        }

        private string bindingsXmlString =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
            "<Bindings>" + Environment.NewLine +
            " <BindingSet Name=\"Geometry\">" + Environment.NewLine +
            "  <Bind Event=\"Mouse.X\" Action=\"GeometryMouse\" />" + Environment.NewLine +
            "  <Bind Event=\"Mouse.Y\" Action=\"GeometryMouse\" />" + Environment.NewLine +
            " </BindingSet>" + Environment.NewLine +
            "</Bindings>";

        public override Bindings GetBindings()
        {
            return new Bindings(Host.BindingsManager.ActionSystem, bindingsXmlString, this.BindingsSource);
        }

        private bool MouseMove(EventData cause)
        {
            if (!cause.Activate) return false;

            mousePort.Post(EmptyValue.SharedInstance);

            return false;
        }

        public void ResetLayers()
        {
            int index = dataSources.Count;
            for (int i = 0; i < index; i++)
            {
                dataSources.RemoveAt(index - 1 - i);
            }
        }

    }

    public enum AddDataResult
    {
        Added,
        Replaced,
        Skipped
    }
}
