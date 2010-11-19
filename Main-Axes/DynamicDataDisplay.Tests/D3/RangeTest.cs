using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class RangeTest
	{
		[TestMethod]
		public void CompareLessDouble()
		{
			Range<double> left = new Range<double>(1, 2);
			Range<double> right = new Range<double>(0, 3);

			Assert.IsTrue(left < right);
			Assert.IsTrue(right > left);
		}

		[TestMethod]
		public void CompareNotIncluded()
		{
			Range<double> left = new Range<double>(0, 1);
			Range<double> right = new Range<double>(-1, 0.5);

			Assert.IsFalse(left < right);
			Assert.IsFalse(left < right);
		}

		[TestMethod]
		public void ThrowsExceptionIfNotComparable()
		{
			Range<IntPtr> left = new Range<IntPtr>();
			Range<IntPtr> right = new Range<IntPtr>();

			try
			{
				bool less = left < right;
				bool more = left > right;

				Assert.Fail("Cannot compare not comparable ranges.");
			}
			catch (InvalidOperationException)
			{

			}
		}

		[TestMethod]
		public void NotComparableIfOverlapping()
		{
			Range<double> one = new Range<double>(0, 2);
			Range<double> two = new Range<double>(1, 3);

			Assert.IsFalse(one < two);
			Assert.IsFalse(one > two);
		}
	}
}
