using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Drawing;
using SlimDX;

namespace Microsoft.Research.DynamicDataDisplay.DirectX2D.VectorFieldConvolution
{
	public class VectorFieldConvolutionChart : DirectXChart
	{
		VertexBuffer vertices;
		Effect effect;

		protected override void OnDirectXRender()
		{
			base.OnDirectXRender();

			var device = Device;
			if (effect == null)
				effect = SlimDX.Direct3D9.Effect.FromStream(Device, 
					GetType().Assembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.DirectX2D.VectorFieldConvolution.ConvolutionShader.fx"), 
					ShaderFlags.None);

			Device.SetRenderState(SlimDX.Direct3D9.RenderState.Ambient, Color.White.ToArgb());
			Device.SetStreamSource(0, vertices, 0, 20);
			Device.SetTransform(TransformState.World, Matrix.Identity);
			//Device.SetTransform(TransformState.Projection, Matrix.OrthoOffCenterLH(0, device.Viewport.Width, device.Viewport.Height, 0, 0, 2.0f));
			Device.SetTransform(TransformState.View, Matrix.Identity);
			effect.SetValue("wvp", Matrix.Identity);
			Device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;

			effect.Begin();
			effect.BeginPass(0);
			Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			effect.EndPass();
			effect.End();
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);

			//effect = SlimDX.Direct3D9.Effect.FromStream(Device, GetType().Assembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.DirectX2D.VectorFieldConvolution.ConvolutionShader.fx"), ShaderFlags.None);

			float size = .9f;
			vertices = new VertexBuffer(Device, 2 * 3 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
			vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new VertexPosition4Color() { Color = Color.Red.ToArgb(), Position = new Vector4(0.0f, 0.0f, 0.5f, 1.0f) },
                new VertexPosition4Color() { Color = Color.Blue.ToArgb(), Position = new Vector4(size, 0.0f, 0.5f, 1.0f) },
                new VertexPosition4Color() { Color = Color.Green.ToArgb(), Position = new Vector4(size, size, 0.5f, 1.0f) },
                new VertexPosition4Color() { Color = Color.Green.ToArgb(), Position = new Vector4(size, size, 0.5f, 1.0f) },
                new VertexPosition4Color() { Color = Color.Green.ToArgb(), Position = new Vector4(0.0f, size, 0.5f, 1.0f) },
                new VertexPosition4Color() { Color = Color.Green.ToArgb(), Position = new Vector4(0.0f, 0.0f, 0.5f, 1.0f) }
            });
			vertices.Unlock();
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			base.OnPlotterDetaching(plotter);
		}
	}
}
