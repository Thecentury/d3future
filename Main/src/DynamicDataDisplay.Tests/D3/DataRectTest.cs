using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class DataRectTest
	{
		[TestMethod]
		public void InfiniteRectContainsEverything()
		{
			var infinite = DataRect.Infinite;

			Assert.IsTrue(infinite.Contains(new DataRect(0, 0, 1, 1)));
			Assert.IsTrue(infinite.Contains(new DataRect(-100, -100, 1, 1)));
			Assert.IsTrue(infinite.Contains(new Point(0, 0)));
		}

		[TestMethod]
		public void Contains_1()
		{
			var big = DataRect.Create(0, 0, 1, 1);
			var small = DataRect.Create(0.1, 0.1, 0.9, 0.9);

			Assert.IsTrue(big.Contains(small));
			Assert.IsTrue(big.IntersectsWith(small));
			Assert.IsTrue(small.IntersectsWith(big));
			Assert.IsFalse(small.Contains(big));
		}

		[TestMethod]
		public void Intersects_1()
		{
			var first = DataRect.Create(0, 0, 1, 1);
			var second = DataRect.Create(-0.5, -0.5, 0.5, 0.5);

			Assert.IsTrue(first.IntersectsWith(second));
			Assert.IsTrue(second.IntersectsWith(first));

			Assert.IsFalse(first.Contains(second));
			Assert.IsFalse(second.Contains(first));

			var intersection1 = first;
			intersection1.Intersect(second);
			var intersection2 = second;
			intersection2.Intersect(first);

			var expected = DataRect.Create(0, 0, 0.5, 0.5);

			Assert.AreEqual(expected, intersection1);
			Assert.AreEqual(expected, intersection2);
		}

		[TestMethod]
		public void CreateInfiniteDataRect()
		{
			DataRect r = DataRect.Create(0, 0, 0, Double.PositiveInfinity);

			Assert.AreEqual(Double.PositiveInfinity, r.Height);
		}

		[TestMethod]
		public void CreateDataRectWithInfiniteMinAndMax()
		{
			DataRect r = DataRect.Create(Double.NegativeInfinity, 0, Double.PositiveInfinity, 0);

			Assert.AreEqual(Double.PositiveInfinity, r.Width);
		}

		[TestMethod]
		public void UniteWithEndlessRect()
		{
			DataRect endless = DataRect.Create(0, 0, Double.PositiveInfinity, 1);
			DataRect r = DataRect.Create(-1, -1, 2, 2);

			DataRect union = endless;
			union.Union(r);

			Assert.AreEqual(DataRect.Create(-1, -1, Double.PositiveInfinity, 2), union);
		}

		[TestMethod]
		public void UniteWithEmptyRect()
		{
			DataRect empty = DataRect.Empty;
			DataRect pointRect = new DataRect(new Point(0, 0), new Vector(0, 0));

			empty.Union(pointRect);

			Assert.AreEqual(new Point(0, 0), empty.Location);
			Assert.AreEqual(0, empty.Width);
			Assert.AreEqual(0, empty.Height);
		}

		[TestMethod]
		public void UniteWithEmptyRectAndFiniteRect()
		{
			DataRect empty = DataRect.Empty;
			DataRect pointRect = new DataRect(new Point(0, 1), new Vector(2, 3));

			empty.Union(pointRect);

			Assert.AreEqual(new Point(0, 1), empty.Location);
			Assert.AreEqual(2, empty.Width);
			Assert.AreEqual(3, empty.Height);
		}

		[TestMethod]
		public void UnionXWithEmpty()
		{
			DataRect empty = DataRect.Empty;

			empty.UnionX(0);

			Assert.AreEqual(0, empty.XMin);

			empty.UnionX(1);

			Assert.AreEqual(0, empty.XMin);
			Assert.AreEqual(1, empty.Width);
		}

		[TestMethod]
		public void UnionYWithEmpty()
		{
			DataRect empty = DataRect.Empty;

			empty.UnionY(0);

			Assert.AreEqual(0, empty.YMin);

			empty.UnionY(1);

			Assert.AreEqual(0, empty.YMin);
			Assert.AreEqual(1, empty.Height);
		}

		[TestMethod]
		public void UnionX()
		{
			DataRect rect = new DataRect(0, 0, 1, 1);

			rect.UnionX(0.5);

			Assert.AreEqual(0, rect.XMin);
			Assert.AreEqual(1, rect.Width);
			Assert.AreEqual(0, rect.YMin);
			Assert.AreEqual(1, rect.Height);

			rect.UnionX(-1);

			Assert.AreEqual(-1, rect.XMin);
			Assert.AreEqual(2, rect.Width);
			Assert.AreEqual(0, rect.YMin);
			Assert.AreEqual(1, rect.Height);

			rect.UnionX(2);

			Assert.AreEqual(-1, rect.XMin);
			Assert.AreEqual(3, rect.Width);
			Assert.AreEqual(0, rect.YMin);
			Assert.AreEqual(1, rect.Height);
		}

		[TestMethod]
		public void UnionY()
		{
			DataRect rect = new DataRect(0, 0, 1, 1);

			rect.UnionY(0.5);

			Assert.AreEqual(0, rect.XMin);
			Assert.AreEqual(1, rect.Width);
			Assert.AreEqual(0, rect.YMin);
			Assert.AreEqual(1, rect.Height);

			rect.UnionY(-1);

			Assert.AreEqual(0, rect.XMin);
			Assert.AreEqual(1, rect.Width);
			Assert.AreEqual(-1, rect.YMin);
			Assert.AreEqual(2, rect.Height);

			rect.UnionY(2);

			Assert.AreEqual(0, rect.XMin);
			Assert.AreEqual(1, rect.Width);
			Assert.AreEqual(-1, rect.YMin);
			Assert.AreEqual(3, rect.Height);
		}
	}
}
