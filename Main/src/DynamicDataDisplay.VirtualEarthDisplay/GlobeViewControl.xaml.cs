using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.MapPoint.Rendering3D;
using System.Globalization;
using System.Reflection;
using Microsoft.MapPoint.PlugIns;
using Microsoft.MapPoint.Rendering3D.NavigationControl;
using Microsoft.MapPoint.Rendering3D.Utility;
using System.Windows.Interop;
using DynamicDataDisplay.VirtualEarthDisplay.Visualization;

namespace DynamicDataDisplay.VirtualEarthDisplay
{
    /// <summary>
    /// Interaction logic for GlobeViewControl.xaml
    /// </summary>
    public partial class GlobeViewControl : UserControl
    {
        private Requirements requirements;
        private VirtualEarthViewBase viewBase;
        private PlugInLoader loader;
        private bool doneLoading = false;
        private static bool userClose = false;
        private bool maximized = false;
        Window fullScreenWindow;
        private bool labelsOn = false;
        private bool roadViewOn = false;
        private DataVisualizationPlugin mainPlugin;

        public DataVisualizationPlugin MainPlugin
        {
            get { return mainPlugin; }
        }

        public GlobeViewControl()
        {
            InitializeComponent();
            requirements = new Requirements();

            //Check if VE3D is installed
            Type veCheckType = Type.GetTypeFromProgID("Microsoft.SentinelVirtualEarth3DProxy.SentinelVE3DProxy");
            if (veCheckType != null)
            {
                object veCheckObject = Activator.CreateInstance(veCheckType);
                object veVersion = veCheckType.InvokeMember("CurrentVersion", BindingFlags.GetProperty, null, veCheckObject, null);
                if (veVersion != null)
                {
                    if (double.Parse(veVersion.ToString(), CultureInfo.InvariantCulture) > 4.0)
                    {
                        // create the control and set Forms properties.
                        this.viewBase = new VirtualEarthViewBase();
                        this.viewBase.Name = "globeControl";
                        this.viewBase.TabIndex = 0;
                        this.viewBase.SendToBack();

                        mainHost.Child = viewBase;

                        this.loader = PlugInLoader.CreateLoader(this.viewBase.Host);

                        this.viewBase.Host.RenderEngine.Initialized += EngineInitialized;
                    }
                    else
                    {
                        this.Content = requirements;
                    }
                }
            }
            else
            {
                this.Content = requirements;
            }

        }

        private void EngineInitialized(object sender, EventArgs e)
        {
            // at this point, the control is fully initialized and we can interact with it without worry.

            // set various data sources, here for elevation data, terrain data, and model data.
            this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Elevation", "Elevation", @"http://maps.live.com//Manifests/HD.xml", DataSourceUsage.ElevationMap));
            this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://maps.live.com//Manifests/AT.xml", DataSourceUsage.TextureMap));
            this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Models", "Models", @"http://maps.live.com//Manifests/MO.xml", DataSourceUsage.Model));

            // set some visual display variables
            this.viewBase.Host.WorldEngine.Environment.AtmosphereDisplay = Microsoft.MapPoint.Rendering3D.Atmospherics.EnvironmentManager.AtmosphereStyle.Scattering;
            this.viewBase.Host.WorldEngine.Environment.CelestialDisplay = Microsoft.MapPoint.Rendering3D.Atmospherics.EnvironmentManager.CelestialStyle.Regular;
            this.viewBase.Host.RenderEngine.Graphics.Settings.UseAnisotropicFiltering = true;
            this.viewBase.Host.WorldEngine.Environment.SunPosition = null; // this means to use real-time lighting
            this.viewBase.Host.WorldEngine.EnableInertia = true;

            // Using this event is the proper way to handle loading and activation.
            this.viewBase.Host.CommunicationManager.AttachToEvent(EngineEvents.Group, EngineEvents.OnPlugInLoaded, "Loaded", PlugInLoaded);

            // Plug-ins can also be loaded by path to a dll, but this one is built-in se we reference by type.
            // If loading by path, it is possible to use both filesystem and http paths.
            // Also, if doing that it may be appropriate to execute the LoadPlugIn call on a worker thread,
            // and handle the result in OnPlugInLoaded.
            this.loader.LoadPlugIn(typeof(NavigationPlugIn));

            this.mainPlugin = 
                this.loader.GetPlugInInfo(this.loader.LoadPlugIn(typeof(DataVisualizationPlugin))).PlugIn as DataVisualizationPlugin;

            doneLoading = true;

        }

        private void PlugInLoaded(string functionName, CommunicationParameter param)
        {
            CommunicationParameterSet set = (CommunicationParameterSet)param.Value;
            CommunicationParameter success;
            CommunicationParameter path;
            CommunicationParameter guid;

            if (set.TryGetValue("success", out success) &&
                set.TryGetValue("plugInPath", out path) &&
                set.TryGetValue("guid", out guid) &&
                (bool)success.Value == true)
            {
                this.loader.ActivatePlugIn(new Guid((string)guid.Value), null);
            }
        }

        public override string ToString()
        {
            return "Virtual Earth";
        }

        /// <summary>
        /// Enter Full Screen Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!maximized)
            {
                fullScreenWindow = new Window();
                this.Content = new TextBlock { Text = "Control Content Maximized" };
                fullScreenWindow.Title = "Virtual Earth";
                fullScreenWindow.Loaded += delegate
                {
                    HwndSource source = (HwndSource)PresentationSource.FromDependencyObject(fullScreenWindow);
                    source.AddHook(WindowProc);
                };
                fullScreenWindow.Content = mainGrid;
                fullScreenWindow.WindowStyle = WindowStyle.None;
                fullScreenWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                fullScreenWindow.WindowState = WindowState.Maximized;
                fullScreenWindow.Closing += new System.ComponentModel.CancelEventHandler(w_Closing);
                maximized = true;
                fullScreenWindow.Show();
            }
            else
            {
                fullScreenWindow.Content = null;
                fullScreenWindow.Close();
                this.Content = mainGrid;
                maximized = false;
            }
        }

        /// <summary>
        /// Reset camera position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.viewBase.Host.Navigation.FlyTo(LatLonAlt.CreateUsingDegrees(0, 0, 20000000), -90.0, 0.0);
        }

        /// <summary>
        /// Enable/Disable Labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            this.viewBase.Host.DataSources.Remove("Texture", "Texture");
            if (labelsOn)
            {
                this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://maps.live.com//Manifests/AT.xml", DataSourceUsage.TextureMap));
                labelsOn = false;

                if (roadViewOn)
                    roadViewOn = false;
            }
            else
            {
                this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://maps.live.com//Manifests/HT.xml", DataSourceUsage.TextureMap));
                labelsOn = true;


                if (roadViewOn)
                    roadViewOn = false;
            }
        }

        /// <summary>
        /// Enable/Disable Road View
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuButton_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            this.viewBase.Host.DataSources.Remove("Texture", "Texture");
            if (!roadViewOn)
            {
                this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://maps.live.com//Manifests/RT.xml", DataSourceUsage.TextureMap));
                roadViewOn = true;
            }
            else
            {
                if (labelsOn)
                {
                    this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://maps.live.com//Manifests/HT.xml", DataSourceUsage.TextureMap));
                }
                else
                {
                    this.viewBase.Host.DataSources.Add(new DataSourceLayerData("Texture", "Texture", @"http://maps.live.com//Manifests/AT.xml", DataSourceUsage.TextureMap));
                }
                roadViewOn = false;
            }
        }

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x112:
                    if ((LOWORD((int)wParam) & 0xfff0) == 0xf060)
                        userClose = true;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private static int LOWORD(int n)
        {
            return (n & 0xffff);
        }

        void w_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (userClose)
            {
                e.Cancel = true;
                userClose = false;
            }
        }

        /// <summary>
        /// Indicates if VE Control was correctly initialized
        /// </summary>
        public bool DoneLoading
        {
            get { return doneLoading; }
        }
    }
}
