using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	class VoronoiGeometry
	{
		public static double deltax, deltay;
		public static int nedges, sqrt_nsites, nvertices;

		public static void geominit()
		{
			//** freeinit
			nvertices = nedges = 0;
			sqrt_nsites = (int)Math.Sqrt(sqrt_nsites + 4);
			deltay = VoronoiMain.ymax - VoronoiMain.ymin;
			deltax = VoronoiMain.xmax - VoronoiMain.xmin;
		}

		internal static Edge bisect(Site s1, Site s2)
		{
			if (s1 == null)
				throw new ArgumentNullException("s1");
			if (s2 == null)
				throw new ArgumentNullException("s2");


			double dx, dy;
			double adx, ady; // absolute dx and dy - |.|
			Edge newedge = new Edge(); //** getfree
			newedge.reg[0] = s1;
			newedge.reg[1] = s2;

			VoronoiGeometry.Ref(s1);
			VoronoiGeometry.Ref(s2);

			newedge.EndPoints[0] = null;
			newedge.EndPoints[1] = null;

			dx = s2.Coord.X - s1.Coord.X;
			dy = s2.Coord.Y - s1.Coord.Y;
			adx = Math.Abs(dx);
			ady = Math.Abs(dy);

			newedge.c = s1.Coord.X * dx + s1.Coord.Y * dy + 0.5 * (dx * dx + dy * dy);

			if (adx > ady)
			{
				newedge.a = 1;
				newedge.b = dy / dx;
				newedge.c /= dx;
			}
			else
			{
				newedge.b = 1;
				newedge.a = dx / dy;
				newedge.c /= dy;
			}

			newedge.edgeNumber = nedges;
			Output.out_bisector(newedge);
			nedges++;

			return newedge;
		}

		internal static Site intersect(HalfEdge el1, HalfEdge el2)
		{
			Edge e1 = el1.Edge;
			Edge e2 = el2.Edge;
			Edge e;
			HalfEdge el;

			if (e1 == null || e2 == null)
				return null;
			if (e1.reg[1] == e2.reg[1])
				return null;

			double d = e1.a * e2.b - e1.b * e2.a;
			if (-1E-10 < d && d < 1E-10)
				return null;

			double xint = (e1.c * e2.b - e2.c * e1.b) / d;
			double yint = (e2.c * e1.a - e1.c * e2.a) / d;

			if (e1.reg[1].Coord.Y < e2.reg[1].Coord.Y ||
				e1.reg[1].Coord.Y == e2.reg[1].Coord.Y &&
				e1.reg[1].Coord.X < e2.reg[1].Coord.X)
			{
				el = el1;
				e = e1;
			}
			else
			{
				el = el2;
				e = e2;
			}

			bool right_of_site = xint >= e.reg[1].Coord.X;
			if (right_of_site && el.ELpm == EndPoint.Left ||
				!right_of_site && el.ELpm == EndPoint.Right)
				return null;

			Site v = new Site(); //** Memory.getfree(
			v.refCount = 0;
			v.Coord = new Point(xint, yint);

			return v;
		}

		public static bool right_of(HalfEdge el, Point p)
		{
			Edge e = el.Edge;
			Site topsite = e.reg[1];
			bool right_of_site = p.X > topsite.Coord.X;
			if (right_of_site && el.ELpm == EndPoint.Left)
				return true;

			bool above;

			if (!right_of_site && el.ELpm == EndPoint.Right)
				return false;
			if (e.a == 1)
			{
				double dyp = p.Y - topsite.Coord.Y;
				double dxp = p.X - topsite.Coord.X;
				bool fast = false;

				if (!right_of_site && e.b < 0 || right_of_site && e.b >= 0)
					fast = above = dyp >= e.b * dxp;
				else
				{
					above = (p.X + p.Y * e.b) > e.c;
					if (e.b < 0)
						above = !above;
					if (!above)
						fast = true;
				}

				if (!fast)
				{
					double dxs = topsite.Coord.X - e.reg[0].Coord.X;
					above = (e.b * (dxp * dxp - dyp * dyp)) <
						(dxs * dyp * (1 + 2 * dxp / dxs + e.b * e.b));
					if (e.b < 0)
						above = !above;
				}
			}
			else
			{
				// e.b == 1

				double y1 = e.c - e.a * p.X;
				double t1 = p.Y - y1;
				double t2 = p.X - topsite.Coord.X;
				double t3 = y1 - topsite.Coord.Y;
				above = t1 * t1 > (t2 * t2 + t3 * t3);
			}
			return el.ELpm == EndPoint.Left ? above : !above;
		}

		internal static void endpoint(Edge e, int lr, Site s)
		{
			e.EndPoints[lr] = s;
			VoronoiGeometry.Ref(s);

			if (e.EndPoints[EndPoint.Right - lr] == null)
				return;

			Output.OutputEndPoint(e);
			VoronoiGeometry.deref(e.reg[EndPoint.Left]);
			VoronoiGeometry.deref(e.reg[EndPoint.Right]);

			//** makefree((Freenode)e, (Freelist)efl);
		}

		internal static double dist(Site s, Site t)
		{
			double dx = s.Coord.X - t.Coord.X;
			double dy = s.Coord.Y - t.Coord.Y;

			return Math.Sqrt(dx * dx + dy * dy);
		}

		internal static void makevertex(Site v)
		{
			v.SiteNumber = nvertices++;
			Output.OutputVertex(v);
		}

		public static void deref(Site v)
		{
			if (--(v.refCount) == 0)
			{
				//** makefree((Freenode)v, (Freelist)sfl);
			}
		}

		public static void Ref(Site v)
		{
			++(v.refCount);
		}

	} // end of class
}
