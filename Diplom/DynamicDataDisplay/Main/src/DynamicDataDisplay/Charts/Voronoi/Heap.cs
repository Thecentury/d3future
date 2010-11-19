using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	class Heap
	{
		public static int PQmin;
		public static int PQcount;
		public static int PQhashsize;
		public static HalfEdge[] PQhash;

		public static void PQinsert(HalfEdge he, Site v, double offset)
		{
			he.Vertex = v;
			VoronoiGeometry.Ref(v);
			he.ystar = v.Coord.Y + offset;
			HalfEdge last = PQhash[PQbucket(he)];

			HalfEdge next;
			while ((next = last.PQnext) != null &&
				(he.ystar > next.ystar || he.ystar == next.ystar && v.Coord.X > next.Vertex.Coord.X))
			{
				last = next;
			}

			he.PQnext = last.PQnext;
			last.PQnext = he;
			PQcount++;
		}

		public static void PQdelete(HalfEdge he)
		{
			HalfEdge last;
			if (he.Vertex != null)
			{
				last = PQhash[PQbucket(he)];
				while (last.PQnext != he)
				{
					last = last.PQnext;
				}
				last.PQnext = he.PQnext;
				PQcount--;
				VoronoiGeometry.deref(he.Vertex);
				he.Vertex = null;
			}
		}

		public static int PQbucket(HalfEdge he)
		{
			int bucket;

			if (he.ystar < VoronoiMain.ymin) bucket = 0;
			else if (he.ystar >= VoronoiMain.ymax) bucket = PQhashsize - 1;
			else bucket = (int)((he.ystar - VoronoiMain.ymin) / VoronoiGeometry.deltay * PQhashsize);

			if (bucket < 0)
				bucket = 0;

			if (bucket >= PQhashsize)
				bucket = PQhashsize - 1;
			if (bucket < PQmin)
				PQmin = bucket;

			return bucket;
		}

		public static bool PQempty()
		{
			return PQcount == 0;
		}

		public static Point PQ_min()
		{
			Point answer = new Point();

			while (PQhash[PQmin].PQnext == null)
				PQmin++;

			answer.X = PQhash[PQmin].PQnext.Vertex.Coord.X;
			answer.Y = PQhash[PQmin].PQnext.ystar;

			return answer;
		}

		public static HalfEdge PQextractmin()
		{
			HalfEdge curr = PQhash[PQmin].PQnext;
			PQhash[PQmin].PQnext = curr.PQnext;
			PQcount--;
			return curr;
		}

		public static void PQinitialize()
		{
			PQcount = PQmin = 0;
			PQhashsize = 4 * VoronoiGeometry.sqrt_nsites;
			PQhash = new HalfEdge[PQhashsize];	//** PQhash = (Halfedge) myalloc(PQhashsize * sizeof(PQhash);
			for (int i = 0; i < PQhashsize; i++)
			{
				PQhash[i] = new HalfEdge();
				PQhash[i].PQnext = null;
			}
		}
	} // end of class
}
