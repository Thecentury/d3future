using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace XNAConvolution
{
	class PrimitiveRenderer
	{
		// During the process of constructing a primitive model, vertex
		// and index data is stored on the CPU in these managed lists.
		List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();
		List<ushort> indices = new List<ushort>();

		// Once all the geometry has been specified, the InitializePrimitive
		// method copies the vertex and index data into these buffers, which
		// store it on the GPU ready for efficient rendering.
		VertexDeclaration vertexDeclaration;
		VertexBuffer vertexBuffer;
		IndexBuffer indexBuffer;
		GraphicsDevice device;
		Effect shader;
		public Effect Shader
		{
			get { return shader; }
			set { shader = value; }
		}

		Texture2D noizeTexture;
		Texture2D xTexture;
		Texture2D yTexture;

		private IDataSource2D<Vector2> dataSource;
		public IDataSource2D<Vector2> DataSource
		{
			get { return dataSource; }
			set
			{
				dataSource = value;
				CreateVectorFieldTexture();
			}
		}

		private void CreateVectorFieldTexture()
		{
			if (xTexture != null)
				xTexture.Dispose();

			xTexture = new Texture2D(device, dataSource.Width, dataSource.Height, 0, TextureUsage.None, SurfaceFormat.Single);
			yTexture = new Texture2D(device, dataSource.Width, dataSource.Height, 0, TextureUsage.None, SurfaceFormat.Single);
			float[] xPixels = new float[dataSource.Width * dataSource.Height];
			float[] yPixels = new float[dataSource.Width * dataSource.Height];

			for (int iy = dataSource.Height - 1; iy >= 0; iy--)
			{
				for (int ix = 0; ix < dataSource.Width; ix++)
				{
					//int pixel = 0; // [x], {x}, [y], {y}
					var vector = dataSource.Data[ix, iy];

					//byte integer;
					//byte fractional;
					//GetParts(vector.X, out integer, out fractional);

					//pixel |= integer << 24;
					//pixel |= fractional << 16;

					//GetParts(vector.Y, out integer, out fractional);

					//pixel |= integer << 8;
					//pixel |= fractional;

					xPixels[iy * dataSource.Width + ix] = vector.X;
					yPixels[iy * dataSource.Width + ix] = vector.Y;
				}
			}

			xTexture.SetData(xPixels);
			yTexture.SetData(yPixels);
		}

		static Dictionary<int, int> values = new Dictionary<int, int>();
		private static void GetParts(float value, out byte integer, out byte fractional)
		{
			var floor = (int)(((value - Math.Floor(value)) * 255));
			var freq = values.ContainsKey(floor) ? values[floor] : 0;
			freq++;
			values[floor] = freq;

			integer = (byte)(((int)(Math.Floor(value) + 128)) & 0xFF);

			//double round = value >= 0 ? Math.Floor(value) : Math.Ceiling(value);

			fractional = (byte)(((int)((value - Math.Floor(value)) * 255)) & 0xFF);
		}

		private float GetValue(byte integer, int fractional)
		{
			float x = 256 * integer - 128;
			x += Math.Sign(x) * fractional;
			return x;
		}

		/// <summary>
		/// Once all the geometry has been specified by calling AddVertex and AddIndex,
		/// this method copies the vertex and index data into GPU format buffers, ready
		/// for efficient rendering.
		/// </summary>
		protected void InitializePrimitive(GraphicsDevice graphicsDevice)
		{
			this.device = graphicsDevice;

			// Create a vertex declaration, describing the format of our vertex data.
			vertexDeclaration = new VertexDeclaration(graphicsDevice,
												VertexPositionTexture.VertexElements);

			// Create a vertex buffer, and copy our vertex data into it.
			vertexBuffer = new VertexBuffer(graphicsDevice,
											typeof(VertexPositionTexture),
											vertices.Count, BufferUsage.None);

			vertexBuffer.SetData(vertices.ToArray());

			// Create an index buffer, and copy our index data into it.
			indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort),
										  indices.Count, BufferUsage.None);

			indexBuffer.SetData(indices.ToArray());

			CreateNoizeTexture(graphicsDevice);
		}

		private void CreateNoizeTexture(GraphicsDevice device)
		{
			const int size = 512;
			noizeTexture = new Texture2D(device, size, size);

			int[] noizeData = new int[size * size];
			Random rnd = new Random();
			for (int i = 0; i < size * size; i++)
			{
				HsbColor color = new HsbColor(0, 0, Math.Round(5 * rnd.NextDouble()) / 4);
				int argb = color.ToArgb();
				noizeData[i] = argb;
			}

			noizeTexture.SetData(noizeData);
		}

		/// <summary>
		/// Adds a new vertex to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		protected void AddVertex(Vector3 position, Vector2 texture)
		{
			vertices.Add(new VertexPositionTexture(position, texture));
		}

		/// <summary>
		/// Adds a new index to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		protected void AddIndex(int index)
		{
			if (index > ushort.MaxValue)
				throw new ArgumentOutOfRangeException("index");

			indices.Add((ushort)index);
		}

		/// <summary>
		/// Finalizer.
		/// </summary>
		~PrimitiveRenderer()
		{
			Dispose(false);
		}


		/// <summary>
		/// Frees resources used by this object.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Frees resources used by this object.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (vertexDeclaration != null)
					vertexDeclaration.Dispose();

				if (vertexBuffer != null)
					vertexBuffer.Dispose();

				if (indexBuffer != null)
					indexBuffer.Dispose();

				if (shader != null)
					shader.Dispose();
			}
		}

		#region Draw


		/// <summary>
		/// Draws the primitive model, using the specified effect. Unlike the other
		/// Draw overload where you just specify the world/view/projection matrices
		/// and color, this method does not set any renderstates, so you must make
		/// sure all states are set to sensible values before you call it.
		/// </summary>
		public void Draw(Effect effect)
		{
			GraphicsDevice graphicsDevice = effect.GraphicsDevice;

			// Set our vertex declaration, vertex buffer, and index buffer.
			graphicsDevice.VertexDeclaration = vertexDeclaration;

			graphicsDevice.Vertices[0].SetSource(vertexBuffer, 0,
												 VertexPositionTexture.SizeInBytes);

			graphicsDevice.Indices = indexBuffer;

			// Draw the model, using the specified effect.
			shader.Begin();

			foreach (EffectPass pass in shader.CurrentTechnique.Passes)
			{
				pass.Begin();

				int primitiveCount = indices.Count / 3;

				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
													 vertices.Count, 0, primitiveCount);

				pass.End();
			}

			shader.End();
		}


		/// <summary>
		/// Draws the primitive model, using a BasicEffect shader with default
		/// lighting. Unlike the other Draw overload where you specify a custom
		/// effect, this method sets important renderstates to sensible values
		/// for 3D model rendering, so you do not need to set these states before
		/// you call it.
		/// </summary>
		public void Draw(Matrix world, Matrix view, Matrix projection, Color color)
		{
			// Set BasicEffect parameters.
			shader.Parameters["wvp"].SetValue(world * view * projection);
			shader.Parameters["noizeTexture"].SetValue(noizeTexture);
			shader.Parameters["xTexture"].SetValue(xTexture);
			shader.Parameters["yTexture"].SetValue(yTexture);

			if (dataSource != null)
			{
				shader.Parameters["width"].SetValue(dataSource.Width);
				shader.Parameters["height"].SetValue(dataSource.Height);
			}

			// Draw the model, using BasicEffect.
			Draw(shader);
		}


		#endregion
	}
}
