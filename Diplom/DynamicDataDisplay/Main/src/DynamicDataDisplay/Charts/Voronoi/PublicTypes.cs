using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{


	[DebuggerDisplay("{A}x + {B}y = {C}")]
	public class VoronoiLine
	{
		public double A { get; set; }
		public double B { get; set; }
		public double C { get; set; }
		public int EdgeNumber { get; set; }
		public Site Site1 { get; set; }
		public Site Site2 { get; set; }
	}

	[DebuggerDisplay("{X}, {Y}")]
	public class VoronoiVertex : IComparable<VoronoiVertex>
	{
		public VoronoiVertex(double x, double y, int siteNumber)
		{
			this.x = x;
			this.y = y;
			this.siteNumber = siteNumber;
		}

		private readonly double x;
		private readonly double y;
		private readonly int siteNumber;

		public double X { get { return x; } }
		public double Y { get { return y; } }
		public int SiteNumber { get { return siteNumber; } }

		#region IComparable<VoronoiVertex> Members

		public int CompareTo(VoronoiVertex other)
		{
			return siteNumber.CompareTo(other.siteNumber);
		}

		#endregion
	}

	public class VoronoiSegment
	{
		public VoronoiSegment(int l, int v1, int v2)
		{
			this.l = l;
			this.v1 = v1;
			this.v2 = v2;
		}

		public readonly int l;
		public readonly int v1;
		public readonly int v2;

		public int L { get { return l; } }
		public int V1 { get { return v1; } }
		public int V2 { get { return v2; } }
	}
}
