using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	public sealed class LineCollection : Collection<VoronoiLine> { }

	public sealed class VertexCollection : Collection<VoronoiVertex> { }

	public sealed class EndPointCollection : Collection<Edge> { }
}
