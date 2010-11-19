using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace DynamicDataDisplay.Tests.D3
{
	/// <summary>
	/// Summary description for BinarySearchTest
	/// </summary>
	[TestClass]
	public class BinarySearchTest
	{
		public BinarySearchTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestLessThanArray()
		{
			int index = BinarySearch.SearchInterval(new double[] { 1, 2, 3 }, 0);
			Assert.IsTrue(index == BinarySearch.NotFound);
		}

		[TestMethod]
		public void TestGreaterThanArray()
		{
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { 1, 2, 3 }, 4) == BinarySearch.NotFound);
		}

		[TestMethod]
		public void TestSmallArray()
		{
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { }, 1) == BinarySearch.NotFound);
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { 1 }, 1) == BinarySearch.NotFound);
		}

		[TestMethod]
		public void TestSearchOddLength()
		{
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { 1, 2, 3, 4, 5 }, 2.3) == 1);
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { 1, 2, 3, 4, 5 }, 1.3) == 0);
		}

		[TestMethod]
		public void TestSearchEvenLength()
		{
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { 1, 2, 3, 4 }, 1.2) == 0);
			Assert.IsTrue(BinarySearch.SearchInterval(new double[] { 1, 2, 3, 4 }, 3.2) == 2);
		}

		[TestMethod]
		public void TestLargeArray()
		{
			var array = Enumerable.Range(0, 1000).Select(i => (double)i).ToArray();
			var index = BinarySearch.SearchInterval(array, 498.1);
			Assert.IsTrue(index == 498);
		}
	}
}
