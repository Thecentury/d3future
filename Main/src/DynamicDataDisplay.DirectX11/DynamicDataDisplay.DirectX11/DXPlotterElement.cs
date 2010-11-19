using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D11;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D;
using Microsoft.WindowsAPICodePack.DirectX.DirectXUtilities;

namespace Microsoft.Research.DynamicDataDisplay.DirectX11
{
    public abstract class DXPlotterElement : ContentControl, IPlotterElement
    {
        private Plotter2D plotter;
        private D3DImageEx d3dImage;
        private Image image;
        protected Viewport2D viewport;
        protected Vector offset;
        protected Matrix3D worldMatrix = Matrix3D.Identity;

        protected double aspectRatio = 0;


        protected bool dxInitialized = false;

        protected D3DDevice device;
        protected SwapChain swapChain;
        protected RenderTargetView renderTargetView;
        protected DeviceContext deviceContext;

        /// <summary>
        /// Gets the device, assosiated with host
        /// </summary>
        public D3DDevice Device
        {
            get { return device; }
        }

        #region Events

        /// <summary>
        /// Occurs once per iteration of the main loop.
        /// </summary>
        public event EventHandler MainLoop;

        /// <summary>
        /// Occurs when the device is created.
        /// </summary>
        public event EventHandler DeviceCreated;

        /// <summary>
        /// Occurs when the device is destroyed.
        /// </summary>
        public event EventHandler DeviceDestroyed;

        /// <summary>
        /// Occurs when the device is lost.
        /// </summary>
        public event EventHandler DeviceLost;

        /// <summary>
        /// Occurs when the device is reset.
        /// </summary>
        public event EventHandler DeviceReset;

        /// <summary>
        /// Raises the OnInitialize event.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:MainLoop"/> event.
        /// </summary>
        protected virtual void OnMainLoop(EventArgs e)
        {
            if (MainLoop != null)
                MainLoop(this, e);
        }

        /// <summary>
        /// Raises the DeviceCreated event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceCreated(EventArgs e)
        {
            if (DeviceCreated != null)
                DeviceCreated(this, e);
        }

        /// <summary>
        /// Raises the DeviceDestroyed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceDestroyed(EventArgs e)
        {
            if (DeviceDestroyed != null)
                DeviceDestroyed(this, e);
        }

        /// <summary>
        /// Raises the DeviceLost event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceLost(EventArgs e)
        {
            if (DeviceLost != null)
                DeviceLost(this, e);
        }

        /// <summary>
        /// Raises the DeviceReset event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceReset(EventArgs e)
        {
            if (DeviceReset != null)
                DeviceReset(this, e);
        }

        #endregion

        #region IPlotterElement Members

        public void OnPlotterAttached(Plotter plotter)
        {
            this.plotter = (Plotter2D)plotter;
            if (this.plotter == null)
                throw new ArgumentException("Invalid plotter");
            else
            {
                if (!dxInitialized)
                    InitializeDX();
                Initialize();
                this.plotter.CentralGrid.Children.Add(this);
                this.viewport = this.plotter.Viewport;
                this.viewport.PropertyChanged += OnViewportPropertyChanged;
            }
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            this.plotter.CentralGrid.Children.Remove(this);
            this.plotter = null;
            this.viewport.PropertyChanged -= OnViewportPropertyChanged;
            this.viewport = null;
        }

        public Plotter Plotter
        {
            get { return plotter; ; }
        }

        #endregion

        public DXPlotterElement()
        {
            this.image = new Image();
            this.d3dImage = new D3DImageEx();

            this.image.Source = d3dImage;
            this.Content = image;

            SizeChanged += BufferSizeChanged;
        }


        protected abstract void RenderScene();
        protected virtual void Initialize() { }

        protected virtual void BufferSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (dxInitialized)
            {
                Texture2DDescription tdesc = new Texture2DDescription
                {
                    ArraySize = 1,
                    Width = (uint)e.NewSize.Width,
                    Height = (uint)e.NewSize.Height,
                    Format = Format.B8G8R8A8_UNORM,
                    MipLevels = 1,
                    SampleDescription = new SampleDescription { Count = 1, Quality = 0 },
                    Usage = Usage.Default,
                    BindFlags = BindFlag.RenderTarget | BindFlag.ShaderResource,
                    MiscFlags = ResourceMiscFlag.Shared,
                    CpuAccessFlags = 0
                };

                using (Texture2D texture2D = device.CreateTexture2D(tdesc))
                {
                    renderTargetView = device.CreateRenderTargetView(texture2D);
                    deviceContext.OM.SetRenderTargets(new RenderTargetView[] { renderTargetView });

                    /* Set the backbuffer, which is a ID3D11Texture2D pointer */
                    d3dImage.SetBackBufferEx(D3DResourceTypeEx.ID3D11Texture2D, texture2D.NativeInterface);
                }

                // viewport
                SwapChainDescription desc = swapChain.Description;
                Viewport viewport = new Viewport();
                viewport.Width = (float)e.NewSize.Width;
                viewport.Height = (float)e.NewSize.Height;
                viewport.MinDepth = 0.0f;
                viewport.MaxDepth = 1.0f;
                viewport.TopLeftX = 0;
                viewport.TopLeftY = 0;

                deviceContext.RS.SetViewports(new Viewport[] { viewport });

                aspectRatio = e.NewSize.Width / e.NewSize.Height;

                Update();

            }
        }

        protected bool renderingHappens = false;
        private void OnRenderingProccessReady(object sender, EventArgs e)
        {
            renderingHappens = true;
            CompositionTarget.Rendering -= OnRenderingProccessReady;
            OnInnerRendering(null, null);
        }

        private void InitializeDX()
        {
            //perform DX Device and Context initialization
            HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "dxPlotterElement", IntPtr.Zero);
            device = D3DDevice.CreateDeviceAndSwapChain(hwnd.Handle, out swapChain);
            deviceContext = device.GetImmediateContext();

            CompositionTarget.Rendering += OnRenderingProccessReady; 

            Texture2DDescription tdesc = new Texture2DDescription
            {
                ArraySize = 1,
                Width = 1,
                Height = 1,
                Format = Format.B8G8R8A8_UNORM,
                MipLevels = 1,
                SampleDescription = new SampleDescription { Count = 1, Quality = 0 },
                Usage = Usage.Default,
                BindFlags = BindFlag.RenderTarget | BindFlag.ShaderResource,
                MiscFlags = ResourceMiscFlag.Shared,
                CpuAccessFlags = 0
            };

            using (Texture2D texture2D = device.CreateTexture2D(tdesc))
            {
                renderTargetView = device.CreateRenderTargetView(texture2D);
                deviceContext.OM.SetRenderTargets(new RenderTargetView[] { renderTargetView });

                d3dImage.SetBackBufferEx(D3DResourceTypeEx.ID3D11Texture2D, texture2D.NativeInterface);
            }

            dxInitialized = true;
            OnDeviceInitialized();

        }

        protected void OnInnerRendering(object sender, EventArgs e)
        {
            /* Render D3D10 test scene */
            RenderScene();

            /* Invalidate our D3DImage */
            InvalidateD3DImage();
        }


        /// <summary>
        /// Invalidates entire D3DImage area
        /// </summary>
        private void InvalidateD3DImage()
        {
            d3dImage.Lock();
            d3dImage.AddDirtyRect(new Int32Rect()
            {
                X = 0,
                Y = 0,
                Height = d3dImage.PixelHeight,
                Width = d3dImage.PixelWidth
            });
            d3dImage.Unlock();
        }

        private void OnViewportPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
        {
            Update();
        }

        protected void Update() 
        {
            DataRect newRect = ((Plotter2D)plotter).Viewport.Transform.ViewportRect;
            worldMatrix = new TranslateTransform3D(-newRect.XMin, -newRect.YMin, 0).Value *
                new ScaleTransform3D(2 / newRect.Width, 2 / newRect.Height, 1).Value *
                new TranslateTransform3D(-1, -1, 0).Value;

            UpdateCore();

            if (renderingHappens)
                OnInnerRendering(null, null);
        }

        protected virtual void UpdateCore() { }

        protected virtual void OnDeviceInitialized() { }
    }
}
