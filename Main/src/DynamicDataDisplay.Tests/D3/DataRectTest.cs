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
		public void TestInfiniteRectContainsEverything()
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


	}
}
