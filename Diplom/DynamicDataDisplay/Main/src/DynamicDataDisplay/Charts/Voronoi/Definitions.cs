using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	[DebuggerDisplay("Site: {Coord}, #{SiteNumber}")]
	public class Site : IComparable<Site>, IEquatable<Site>
	{
		public Point Coord { get; set; }
		public int SiteNumber { get; set; }
		public int refCount;

		#region IComparable<Site> Members

		public int CompareTo(Site other)
		{
			if (Coord.Y < other.Coord.Y)
				return -1;
			if (Coord.Y > other.Coord.Y)
				return 1;
			if (Coord.X < other.Coord.X)
				return -1;
			if (Coord.X > other.Coord.X)
				return 1;
			return 0;
		}

		#endregion

		public override int GetHashCode()
		{
			return SiteNumber.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as Site;
			if (other == null)
				return false;

			return Equals(other);
		}

		#region IEquatable<Site> Members

		public bool Equals(Site other)
		{
			return SiteNumber == other.SiteNumber;
		}

		#endregion
	}

	[DebuggerDisplay("Edge: {a}x+{b}y={c}")]
	public class Edge
	{
		public double a, b, c;
		public Site[] EndPoints = new Site[2];
		public Site[] reg = new Site[2];
		public int edgeNumber;

		public static readonly Edge Deleted = new Edge();
	}

	public static class EndPoint
	{
		public static readonly int Left = 0;
		public static readonly int Right = 1;
	}

	class HalfEdge
	{
		public HalfEdge Left;
		public HalfEdge Right;
		public Edge Edge;
		public int RefCount;
		public int ELpm; //** was char
		public Site Vertex;
		public double ystar;
		public HalfEdge PQnext;
	}
}
