using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	public static class Output
	{
		public static bool triangulate;
		public static bool plot;
		public static bool debug = true;

		public static double ymax;
		public static double ymin;
		public static double xmax;
		public static double xmin;

		public static double pxmin;
		public static double pxmax;
		public static double pymin;
		public static double pymax;
		public static double cradius;

		public static LineCollection LineCollection = new LineCollection();
		public static VertexCollection VertexCollection = new VertexCollection();
		public static EndPointCollection EndPointCollection = new EndPointCollection();

		/// <summary>
		/// Open plot.
		/// </summary>
		public static void openpl() { }
		public static void line(double ax, double ay, double bx, double by)
		{
		}
		public static void circle(double ax, double ay, double radius) { }
		public static void range(double pxmin, double pxmax, double pymin, double pymax)
		{
		}

		internal static void out_bisector(Edge e)
		{
			if (triangulate && plot && !debug)
				line(e.reg[0].Coord.X, e.reg[0].Coord.Y, e.reg[1].Coord.X, e.reg[1].Coord.Y);
			if (!triangulate && !plot && !debug)
				Console.WriteLine(String.Format("line {0} {1} {2}", e.a, e.b, e.c));
			if (debug)
				Console.WriteLine(String.Format("line edgeNumber: {0}, {1}x+{2}y={3}, bisecting sites # {4} {5}",
					e.edgeNumber, e.a, e.b, e.c, e.reg[EndPoint.Left].SiteNumber, e.reg[EndPoint.Right].SiteNumber));

			LineCollection.Add(new VoronoiLine
			{
				A = e.a,
				B = e.b,
				C = e.c,
				EdgeNumber = e.edgeNumber,
				Site1 = e.reg[EndPoint.Left],
				Site2 = e.reg[EndPoint.Right]
			});
		}

		/// <summary>
		/// Output the endpoint.
		/// </summary>
		/// <param name="e">The e.</param>
		internal static void OutputEndPoint(Edge e)
		{
			if (!triangulate && plot)
				clip_line(e);
			if (!triangulate && !plot)
			{
				Console.Write(String.Format("endpoint edgeNumber: {0} ", e.edgeNumber));
				Console.Write("left: ");
				Console.Write(e.EndPoints[EndPoint.Left] != null ? e.EndPoints[EndPoint.Left].SiteNumber : -1);
				Console.Write(" right: ");
				Console.WriteLine(e.EndPoints[EndPoint.Right] != null ? e.EndPoints[EndPoint.Right].SiteNumber : -1);
			}

			EndPointCollection.Add(e);
		}

		internal static void OutputVertex(Site site)
		{
			if (!triangulate && !plot && !debug)
				Console.WriteLine(String.Format("vertex {0}, {1}", site.Coord.X, site.Coord.Y));
			if (debug)
				Console.WriteLine(String.Format("vertex({0}) at {1}, {2}", site.SiteNumber, site.Coord.X, site.Coord.Y));

			VertexCollection.Add(new VoronoiVertex(site.Coord.X, site.Coord.Y, site.SiteNumber));
		}

		internal static void OutputSite(Site s)
		{
			if (!triangulate && plot && !debug)
				circle(s.Coord.X, s.Coord.Y, cradius);
			if (!triangulate && !plot && !debug)
				Console.WriteLine(String.Format("site {0} {1}", s.Coord.X, s.Coord.Y));
			if (debug)
				Console.WriteLine(String.Format("site ({0}) at {1} {2}", s.SiteNumber, s.Coord.X, s.Coord.Y));
		}

		internal static void out_triple(Site s1, Site s2, Site s3)
		{
			if (triangulate && !plot && !debug)
				Console.WriteLine(String.Format("triple {0} {1} {2}", s1.SiteNumber, s2.SiteNumber, s3.SiteNumber));
			if (debug)
				Console.WriteLine(String.Format("circle through left={0} right={1} bottom={2}", s1.SiteNumber, s2.SiteNumber, s3.SiteNumber));
		}

		internal static void plotinit()
		{
			double dy = ymax - ymin;
			double dx = xmax - xmin;
			double d = Math.Max(dx, dy) * 1.1;

			pxmin = xmin - (d - dx) / 2;
			pxmax = xmax + (d - dx) / 2;
			pymin = ymin - (d - dy) / 2;
			pymax = ymax + (d - dy) / 2;
			cradius = (pxmax - pxmin) / 350;

			openpl();
			range(pxmin, pymin, pxmax, pymax);
		}

		internal static void clip_line(Edge e)
		{
			Site s1, s2;
			double x1, x2, y1, y2;

			if (e.a == 1.0 && e.b >= 0.0)
			{
				s1 = e.EndPoints[1];
				s2 = e.EndPoints[0];
			}
			else
			{
				s1 = e.EndPoints[0];
				s2 = e.EndPoints[1];
			}
			if (e.a == 1.0)
			{
				y1 = pymin;
				if (s1 != null && s1.Coord.Y > pymin)
				{
					y1 = s1.Coord.Y;
				}
				if (y1 > pymax)
				{
					return;
				}
				x1 = e.c - e.b * y1;
				y2 = pymax;
				if (s2 != null && s2.Coord.Y < pymax)
				{
					y2 = s2.Coord.Y;
				}
				if (y2 < pymin)
				{
					return;
				}
				x2 = e.c - e.b * y2;
				if (((x1 > pxmax) && (x2 > pxmax)) || ((x1 < pxmin) && (x2 < pxmin)))
				{
					return;
				}
				if (x1 > pxmax)
				{
					x1 = pxmax;
					y1 = (e.c - x1) / e.b;
				}
				if (x1 < pxmin)
				{
					x1 = pxmin;
					y1 = (e.c - x1) / e.b;
				}
				if (x2 > pxmax)
				{
					x2 = pxmax;
					y2 = (e.c - x2) / e.b;
				}
				if (x2 < pxmin)
				{
					x2 = pxmin;
					y2 = (e.c - x2) / e.b;
				}
			}
			else
			{
				x1 = pxmin;
				if (s1 != null && s1.Coord.X > pxmin)
					x1 = s1.Coord.X;

				if (x1 > pxmax)
					return;

				y1 = e.c - e.a * x1;
				x2 = pxmax;
				if (s2 != null && s2.Coord.X < pxmax)
					x2 = s2.Coord.X;

				if (x2 < pxmin)
					return;

				y2 = e.c - e.a * x2;
				if (((y1 > pymax) && (y2 > pymax)) || ((y1 < pymin) && (y2 < pymin)))
				{
					return;
				}
				if (y1 > pymax)
				{
					y1 = pymax;
					x1 = (e.c - y1) / e.a;
				}
				if (y1 < pymin)
				{
					y1 = pymin;
					x1 = (e.c - y1) / e.a;
				}
				if (y2 > pymax)
				{
					y2 = pymax;
					x2 = (e.c - y2) / e.a;
				}
				if (y2 < pymin)
				{
					y2 = pymin;
					x2 = (e.c - y2) / e.a;
				}
			}

			line(x1, y1, x2, y2);
		}

	} // end of class
}
