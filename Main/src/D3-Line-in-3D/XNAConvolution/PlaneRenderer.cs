using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAConvolution
{
	class PlaneRenderer : PrimitiveRenderer
	{
		public PlaneRenderer(GraphicsDevice device)
		{
			AddVertex(new Vector3(-1, -1, 0), new Vector2(0, 0));
			AddVertex(new Vector3(-1, 1, 0), new Vector2(0, 1));
			AddVertex(new Vector3(1, -1, 0), new Vector2(1, 0));
			AddVertex(new Vector3(1, 1, 0), new Vector2(1, 1));

			AddIndex(0);
			AddIndex(1);
			AddIndex(3);
			AddIndex(0);
			AddIndex(3);
			AddIndex(2);

			InitializePrimitive(device);
		}
	}
}
