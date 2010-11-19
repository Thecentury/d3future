using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	class VoronoiAlgorithm
	{
		public static void voronoi(Func<Site> nextsite)
		{
			Heap.PQinitialize();
			EdgeList.Bottomsite = nextsite();
			Output.OutputSite(EdgeList.Bottomsite);
			EdgeList.ELinitialize();
			Site newsite = nextsite();

			HalfEdge lbnd, rbnd, llbnd, rrbnd, bisector;
			Site bottom, top, temp, p, v;
			Point newintstar = new Point();
			int pm;
			Edge e;
			while (true)
			{
				if (!Heap.PQempty())
					newintstar = Heap.PQ_min();

				if (newsite != null && (Heap.PQempty() || newsite.Coord.Y < newintstar.Y ||
					(newsite.Coord.Y == newintstar.Y && newsite.Coord.X < newintstar.X)))
				{
					// new site is smallest
					Output.OutputSite(newsite);

					lbnd = EdgeList.ELleftbnd(newsite.Coord);
					rbnd = EdgeList.ELright(lbnd);
					bottom = EdgeList.rightreg(lbnd);
					e = VoronoiGeometry.bisect(bottom, newsite);
					bisector = EdgeList.HEcreate(e, EndPoint.Left);
					EdgeList.ELinsert(lbnd, bisector);
					p = VoronoiGeometry.intersect(lbnd, bisector);
					if (p != null)
					{
						Heap.PQdelete(lbnd);
						Heap.PQinsert(lbnd, p, VoronoiGeometry.dist(p, newsite));
					}
					lbnd = bisector;
					bisector = EdgeList.HEcreate(e, EndPoint.Right);
					EdgeList.ELinsert(lbnd, bisector);
					p = VoronoiGeometry.intersect(bisector, rbnd);
					if (p != null)
						Heap.PQinsert(bisector, p, VoronoiGeometry.dist(p, newsite));

					newsite = nextsite();
				}
				else if (!Heap.PQempty())   /* intersection is smallest */
				{
					lbnd = Heap.PQextractmin();
					llbnd = EdgeList.ELleft(lbnd);
					rbnd = EdgeList.ELright(lbnd);
					rrbnd = EdgeList.ELright(rbnd);
					bottom = EdgeList.leftreg(lbnd);
					top = EdgeList.rightreg(rbnd);

					Output.out_triple(bottom, top, EdgeList.rightreg(lbnd));

					v = lbnd.Vertex;
					VoronoiGeometry.makevertex(v);
					VoronoiGeometry.endpoint(lbnd.Edge, lbnd.ELpm, v);
					VoronoiGeometry.endpoint(rbnd.Edge, rbnd.ELpm, v);
					EdgeList.ELdelete(lbnd);
					Heap.PQdelete(rbnd);
					EdgeList.ELdelete(rbnd);
					pm = EndPoint.Left;
					if (bottom.Coord.Y > top.Coord.Y)
					{
						temp = bottom;
						bottom = top;
						top = temp;
						pm = EndPoint.Right;
					}
					e = VoronoiGeometry.bisect(bottom, top);
					bisector = EdgeList.HEcreate(e, pm);
					EdgeList.ELinsert(llbnd, bisector);
					VoronoiGeometry.endpoint(e, EndPoint.Right - pm, v);
					VoronoiGeometry.deref(v);
					p = VoronoiGeometry.intersect(llbnd, bisector);
					if (p != null)
					{
						Heap.PQdelete(llbnd);
						Heap.PQinsert(llbnd, p, VoronoiGeometry.dist(p, bottom));
					}
					p = VoronoiGeometry.intersect(bisector, rrbnd);
					if (p != null)
					{
						Heap.PQinsert(bisector, p, VoronoiGeometry.dist(p, bottom));
					}
				}
				else
				{
					break;
				}
			}

			for (lbnd = EdgeList.ELright(EdgeList.ELleftend);
				 lbnd != EdgeList.ELrightend;
				 lbnd = EdgeList.ELright(lbnd))
			{
				e = lbnd.Edge;
				Output.OutputEndPoint(e);
			}

		} // end of while
	}
}
