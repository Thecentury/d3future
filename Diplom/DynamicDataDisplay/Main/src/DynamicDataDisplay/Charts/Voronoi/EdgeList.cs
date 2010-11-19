using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	class EdgeList
	{
		public static int ELhashsize;
		
		private static Site bottomsite;
		public static Site Bottomsite
		{
			get { return bottomsite; }
			set { bottomsite = value; }
		}

		public static HalfEdge ELleftend;
		public static HalfEdge ELrightend;
		public static HalfEdge[] ELhash;

		public static int ntry;
		public static int totalsearch;

		public static void ELinitialize()
		{
			ELhashsize = 2 * VoronoiGeometry.sqrt_nsites;
			ELhash = new HalfEdge[ELhashsize];
			for (int i = 0; i < ELhashsize; i++)
			{
				ELhash[i] = null;
			}

			ELleftend = HEcreate(null, 0);
			ELrightend = HEcreate(null, 0);

			ELleftend.Left = null;
			ELleftend.Right = ELrightend;
			ELrightend.Left = ELleftend;
			ELrightend.Right = null;

			ELhash[0] = ELleftend;
			ELhash[ELhashsize - 1] = ELrightend;
		}

		/// <summary>
		/// Half edge create
		/// </summary>
		internal static HalfEdge HEcreate(Edge e, int pm)
		{
			HalfEdge answer = new HalfEdge();//** Memory.getfree(hfl);
			answer.Edge = e;
			answer.ELpm = pm;
			answer.PQnext = null;
			answer.Vertex = null;
			answer.RefCount = 0;

			return answer;
		}

		internal static void ELinsert(HalfEdge lb, HalfEdge New)
		{
			New.Left = lb;
			New.Right = lb.Right;
			lb.Right.Left = New;
			lb.Right = New;
		}

		/// <summary>
		/// Get entry from hash table, pruning any deleted nodes
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static HalfEdge ELgethash(int b)
		{
			HalfEdge he;

			if (b < 0 || b >= ELhashsize)
				return null;

			he = ELhash[b];

			if (he == null || he.Edge != Edge.Deleted)
				return he;

			// Hash table points to deleted half edge. Patch as necessary.
			ELhash[b] = null;
			if (--(he.RefCount) == 0)
			{
				//** Memory.makefree((Freenode)he, (Freelist)hfl);
			}
			return null;
		}

		public static HalfEdge ELleftbnd(Point p)
		{
			int bucket = (int)((p.X - VoronoiMain.xmin) / VoronoiGeometry.deltax * ELhashsize);
			if (bucket < 0)
				bucket = 0;
			if (bucket >= ELhashsize)
				bucket = ELhashsize - 1;

			HalfEdge he = ELgethash(bucket);
			if (he == null)
			{
				int i = 1;
				while (true)
				{
					he = ELgethash(bucket - i);
					if (he != null)
						break;

					he = ELgethash(bucket + i);
					if (he != null)
						break;
					i++;
				}
				totalsearch += i;
			}

			ntry++;

			// Now search linear list of halfedges for the correct one
			if (he == ELleftend || (he != ELrightend && VoronoiGeometry.right_of(he, p)))
			{
				do
				{
					he = he.Right;
				} while (he != ELrightend && VoronoiGeometry.right_of(he, p));
				he = he.Left;
			}
			else
			{
				do
				{
					he = he.Left;
				} while (he != ELleftend && !VoronoiGeometry.right_of(he, p));
			}

			// Update hash table and reference count
			if (bucket > 0 && bucket < (ELhashsize - 1))
			{
				if (ELhash[bucket] != null)
					ELhash[bucket].RefCount--;

				ELhash[bucket] = he;
				ELhash[bucket].RefCount++;
			}

			return he;
		}

		/// <summary>
		/// This delete routine can't reclaim node, since pointers from hash: table may be present.
		/// </summary>
		/// <param name="he"></param>
		public static void ELdelete(HalfEdge he)
		{
			he.Left.Right = he.Right;
			he.Right.Left = he.Left;
			he.Edge = Edge.Deleted;
		}

		internal static HalfEdge ELright(HalfEdge he)
		{
			return he.Right;
		}

		internal static HalfEdge ELleft(HalfEdge he)
		{
			return he.Left;
		}

		internal static Site leftreg(HalfEdge he)
		{
			if (he.Edge == null)
				return Bottomsite;

			return (he.ELpm == EndPoint.Left ? he.Edge.reg[EndPoint.Left] : he.Edge.reg[EndPoint.Right]);
		}

		internal static Site rightreg(HalfEdge he)
		{
			Site result;

			if (he.Edge == null)
				result = Bottomsite;
			else 
				result = (he.ELpm == EndPoint.Left ? he.Edge.reg[EndPoint.Right] : he.Edge.reg[EndPoint.Left]);

			if(result == null)
				throw new InvalidOperationException("Result should not be null.");
			return result;
		}


	} //  end of class
}
