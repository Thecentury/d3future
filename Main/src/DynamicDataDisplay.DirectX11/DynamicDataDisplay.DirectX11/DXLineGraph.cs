using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D11;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D;
using System.IO;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DirectX11.VertexStructures;
using Microsoft.Research.DynamicDataDisplay.DirectX11.DirectXInternalHelpers;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using Microsoft.WindowsAPICodePack.DirectX.DirectXUtilities;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.DirectX11
{
    public class DXLineGraph : DXPlotterElement, IOneDimensionalChart
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private GeometryShader geometryShader;
        private D3DBuffer vertexBuffer, wvpBuffer, lcBuffer;
        private int count = 0;

        #region DataSource

        public IPointDataSource DataSource
        {
            get { return (IPointDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(
              "DataSource",
              typeof(IPointDataSource),
              typeof(DXLineGraph),
              new FrameworkPropertyMetadata
              {
                  DefaultValue = null,
                  PropertyChangedCallback = OnDataSourceChangedCallback
              }
            );

        private static void OnDataSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DXLineGraph graph = (DXLineGraph)d;
            if (e.NewValue != e.OldValue)
            {
                graph.DetachDataSource(e.OldValue as IPointDataSource);
                graph.AttachDataSource(e.NewValue as IPointDataSource);
            }
            graph.OnDataSourceChanged(e);
        }

        private void AttachDataSource(IPointDataSource source)
        {
            if (source != null)
            {
                source.DataChanged += OnDataChanged;
            }
        }

        private void DetachDataSource(IPointDataSource source)
        {
            if (source != null)
            {
                source.DataChanged -= OnDataChanged;
            }
        }

        private void OnDataChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }

        protected virtual void OnDataChanged()
        {
            UpdateBounds(DataSource);

            RaiseDataChanged();
            Update();
        }

        private void RaiseDataChanged()
        {
            if (DataChanged != null)
            {
                DataChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler DataChanged;

        protected virtual void OnDataSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            IPointDataSource newDataSource = (IPointDataSource)args.NewValue;

            if (device != null)
            {
                UpdateBounds(DataSource);

                var vertices = from p in DataSource.GetPoints() select new VertexPosition { Vertex = new Vector3F((float)p.X, (float)p.Y, 0) };
                count = vertices.Count();
                vertexBuffer = BufferHelper.CreateBuffer<VertexPosition>(device, vertices.ToArray());
                deviceContext.IA.SetVertexBuffers(0, new D3DBuffer[] { vertexBuffer }, new uint[] { VertexPosition.Size }, new uint[] { 0 });

                Update();
            }
        }

        
        private void UpdateBounds(IPointDataSource dataSource)
        {
            if (this.Plotter != null)
            {
                var transform = ((Plotter2D)Plotter).Viewport.Transform;
                DataRect bounds = BoundsHelper.GetViewportBounds(dataSource.GetPoints(), transform.DataTransform);
                Viewport2D.SetContentBounds(this, bounds);
            }
        }

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        struct LineColorBufferElement
        {
            public ColorRgba LineColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct VSMatrix
        {
            public Matrix4x4F wvp;
        }

        protected override void RenderScene()
        {
            deviceContext.ClearRenderTargetView(renderTargetView, new ColorRgba(0.0f, 0.0f, 0.0f, 0.0f));
            if (count != 0)
            {
                deviceContext.Draw((uint)count, 0);
                swapChain.Present(0, 0);
            }
        }

        protected override void OnDeviceInitialized()
        {
            if (DataSource != null)
                OnDataSourceChanged(new DependencyPropertyChangedEventArgs());
        }

        protected override void Initialize()
        {
            deviceContext.IA.SetPrimitiveTopology(PrimitiveTopology.LineStrip);

            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.DirectX11.Shaders.Precompiled.Basic.Render.vs"))
            {
                vertexShader = device.CreateVertexShader(stream);
                deviceContext.VS.SetShader(vertexShader);
               
                stream.Position = 0;
                InputLayout inputLayout = device.CreateInputLayout(
                    VertexPosition.InputElementsDescription,
                    stream);
                deviceContext.IA.SetInputLayout(inputLayout);

            }

            // Open precompiled vertex shader
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.DirectX11.Shaders.Precompiled.Basic.Render.ps"))
            {
                pixelShader = device.CreatePixelShader(stream);
            }
            deviceContext.PS.SetShader(pixelShader, null);

            SetShaderMatrices();
            SetShaderLineColor(new ColorRgba(1, 0, 0, 1));

            Viewport2D.SetIsContentBoundsHost(this, true);

        }

        private void SetShaderMatrices()
        {
            VSMatrix vsm = new VSMatrix();
            vsm.wvp = DXMatrixHelper.Transpose(MatrixExtensions.ToMatrix4x4F(worldMatrix));

            if (wvpBuffer == null)
            {
                wvpBuffer = device.CreateBuffer(
                new BufferDescription
                    {
                        BindFlags = BindFlag.ConstantBuffer,
                        CpuAccessFlags = CpuAccessFlag.Write,
                        MiscFlags = ResourceMiscFlag.Undefined,
                        Usage = Usage.Dynamic,
                        ByteWidth = (uint)Marshal.SizeOf(vsm)
                    });
            }

            MappedSubresource mappedResource = (MappedSubresource)deviceContext.Map(wvpBuffer, 0, Map.WriteDiscard, MapFlag.Unspecified);
            Marshal.StructureToPtr(vsm, mappedResource.Data, true);
            deviceContext.Unmap(wvpBuffer, 0);
            deviceContext.VS.SetConstantBuffers(0, new D3DBuffer[] { wvpBuffer });
        }

        private void SetShaderLineColor(ColorRgba color)
        {
            LineColorBufferElement bufElem = new LineColorBufferElement { LineColor = color };
            if (lcBuffer == null)
            {
                lcBuffer = device.CreateBuffer(
                new BufferDescription
                {
                    BindFlags = BindFlag.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlag.Write,
                    MiscFlags = ResourceMiscFlag.Undefined,
                    Usage = Usage.Dynamic,
                    ByteWidth = (uint)Marshal.SizeOf(bufElem)
                });
            }

            MappedSubresource mappedResource = (MappedSubresource)deviceContext.Map(lcBuffer, 0, Map.WriteDiscard, MapFlag.Unspecified);
            Marshal.StructureToPtr(bufElem, mappedResource.Data, true);
            deviceContext.Unmap(lcBuffer, 0);
            deviceContext.PS.SetConstantBuffers(0, new D3DBuffer[] { lcBuffer });
        }

        protected override void UpdateCore()
        {
            SetShaderMatrices();
        }
    }
}
