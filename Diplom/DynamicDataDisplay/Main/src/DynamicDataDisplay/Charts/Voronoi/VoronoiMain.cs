using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.Voronoi
{
	public static class VoronoiMain
	{
		public static bool sorted, triangulate, plot, debug;
		public static int nsites, siteidx;
		public static double xmin, xmax, ymin, ymax;
		static List<Site> sites;

		public static void main(IEnumerable<Point> points)
		{
			Output.LineCollection.Clear();

			sorted = triangulate = plot = debug = false;
			Func<Site> next;

			if (sorted)
			{
				//**
			}
			else
			{
				ReadSites(points);
				next = new Func<Site>(() => NextOne());
				siteidx = 0;
				VoronoiGeometry.geominit();
				if (plot)
					Output.plotinit();

				VoronoiAlgorithm.voronoi(new Func<Site>(() =>
				{
					Site nextSite = next();
					return nextSite;
				}));
			}
		}

		/// <summary>
		/// sort sites on y, then x, coord
		/// </summary>
		/// <param name="s1"></param>
		/// <param name="s2"></param>
		/// <returns></returns>
		static int SitesCompare(Point s1, Point s2)
		{
			if (s1.Y < s2.Y)
				return -1;
			if (s1.Y > s2.Y)
				return 1;
			if (s1.X < s2.X)
				return -1;
			if (s1.X > s2.X)
				return 1;
			return 0;
		}

		/// <summary>
		/// return a single in-storage site
		/// </summary>
		/// <returns></returns>
		static Site NextOne()
		{
			if (siteidx < nsites)
			{
				Site s = sites[siteidx++];
				return s;
			}
			else
				return null;
		}

		/// <summary>
		/// read all sites, sort, and compute xmin, xmax, ymin, ymax
		/// </summary>
		static void ReadSites(IEnumerable<Point> points)
		{
			nsites = 0;
			sites = new List<Site>();

			foreach (var point in points)
			{

				sites.Add(new Site());

				sites[nsites].Coord = point;
				sites[nsites].SiteNumber = nsites;
				sites[nsites++].refCount = 0;
			}

			sites.Sort();

			xmin = sites[0].Coord.X;
			xmax = sites[0].Coord.X;

			for (int i = 0; i < nsites; i++)
			{
				if (sites[i].Coord.X < xmin)
					xmin = sites[i].Coord.X;
				if (sites[i].Coord.X > xmax)
					xmax = sites[i].Coord.X;
			}

			ymin = sites[0].Coord.Y;
			ymax = sites[nsites - 1].Coord.Y;
		}

		/// <summary>
		/// reads one site
		/// </summary>
		/// <returns></returns>
		static Site ReadOne(StreamReader reader)
		{
			Site s = new Site();
			s.refCount = 0;
			s.SiteNumber = siteidx++;

			if (reader.EndOfStream)
				return null;

			var str = reader.ReadLine();
			var subStrings = str.Split(' ');

			s.Coord = new Point(Double.Parse(subStrings[0]), Double.Parse(subStrings[1]));
			return s;
		}
	}
}
