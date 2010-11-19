using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	internal struct IsoSurfaceVertex
	{
		public Vector3D Position;
		public Vector3D Normal;
	}

	internal struct IsoSurfaceIndex
	{
		public int EdgeIndex;
		public int ListPosition;
	}
}
