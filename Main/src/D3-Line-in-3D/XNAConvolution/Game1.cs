using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;

namespace XNAConvolution
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		PlaneRenderer plane;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		const int size = 200;

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			plane = new PlaneRenderer(GraphicsDevice);
			plane.Shader = Content.Load<Effect>("Convolution");
			plane.DataSource = CreateField();
		}

		private static IDataSource2D<Vector2> CreateField()
		{
			double[] xs = new double[size];
			double[] ys = new double[size];

			Vector2[,] data;
			//CreateCircularField(data);
			data = CreateField(
				(x, y) => (x / 40) % 2 - (y / 40) % 2 == 0 ? new Vector2(1, 0) : new Vector2(0, 1)

				//(x, y) => CreateCircularField(x, y)
				//(x, y) =>
				//{
				//    Vector2 result;

				//    double xc = x - size / 2;
				//    double yc = y - size / 2;
				//    if (xc != 0)
				//    {
				//        double beta = Math.Sqrt(1.0 / (1 + yc * yc / (xc * xc)));
				//        double alpha = -beta * yc / xc;
				//        result = new Vector2((float)alpha, (float)beta);
				//    }
				//    else
				//    {
				//        double alpha = Math.Sqrt(1.0 / (1 + xc * xc / (yc * yc)));
				//        double beta = -alpha * xc / yc;
				//        result = new Vector2((float)alpha, (float)beta);
				//    }

				//    if (Double.IsNaN(result.X))
				//    {
				//        result = new Vector2(0, 0);
				//    }

				//    result *= 2;

				//    return result;
				//}
				//(x, y) => ((int)(x / 20)) % 2 == 0 && ((int)(y / 20)) % 2 == 0 ? new Vector2(1, 0) : new Vector2(0, 1)
				//(x, y) => new Vector2((float)Math.Sin(x / 10.0), (float)Math.Cos(y / 10.0))
				//(x, y) => new Vector2((float)Math.Sin(x/10), y)
				//(x, y) => new Vector2((float)((x - 10) / (y - 12.1)), (float)((x + 12) / (y - 14.2)))
				);

			NonUniformDataSource2D<Vector2> dataSource = new NonUniformDataSource2D<Vector2>(xs, ys, data);
			return dataSource;
		}

		private static Vector2[,] CreateField(Func<int, int, Vector2> func)
		{
			Vector2[,] data = new Vector2[size, size];
			for (int ix = 0; ix < size; ix++)
			{
				for (int iy = 0; iy < size; iy++)
				{
					data[ix, iy] = func(ix, iy);
				}
			}

			return data;
		}

		private static Vector2 CreateCircularField(int x, int y)
		{
			return new Vector2(size / 2f, size / 2f) - new Vector2(x, y);

			Vector3 center = new Vector3(size / 2f, size / 2f, 0);
			Vector3 up = new Vector3(0, 0, 1);
			Vector3 vec = center - new Vector3(x, y, 0);
			Vector3 tangent = Cross(vec, up);
			Vector2 value = new Vector2(tangent.X, tangent.Y);
			if (value.X != 0 || value.Y != 0)
				value.Normalize();

			return value;
		}

		private static Vector3 Cross(Vector3 v1, Vector3 v2)
		{
			// i	j		k
			// v1.x	v1.y	v1.z
			// v2.x	v2.y	v2.z

			float x = v1.Y * v2.Z - v2.Y - v1.Z;
			float y = -(v1.X * v2.Z - v2.X * v1.Z);
			float z = v1.X * v2.Y - v1.Y * v2.X;

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}



		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			Vector3 cameraPosition = new Vector3(0, 0, 2.5f);

			float aspect = GraphicsDevice.Viewport.AspectRatio;

			Matrix world = Matrix.Identity;
			Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 10);

			plane.Draw(world, view, projection, Color.Red);
			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
