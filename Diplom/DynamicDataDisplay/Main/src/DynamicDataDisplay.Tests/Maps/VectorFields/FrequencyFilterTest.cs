using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.StreamLine2D.Filters;

namespace DynamicDataDisplay.Tests.Maps.VectorFields
{
	[TestClass]
	public class FrequencyFilterTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestOnNarrowSequence()
		{
			var seq = new List<Point>();
			seq.Add(new Point(0, 0));
			seq.Add(new Point(0.3, 0.3));
			seq.Add(new Point(0.4, 0.5));
			seq.Add(new Point(2, 2));

			FrequencyFilter filter = new FrequencyFilter();
			var filtered = filter.Filter(seq).ToArray();
			Assert.IsTrue(filtered.Length == 2);
		}

		[TestMethod]
		public void TestOnWideSequence()
		{
			var seq = new List<Point>();
			seq.Add(new Point(0, 0));
			seq.Add(new Point(1, 1));
			seq.Add(new Point(2, 2));
			seq.Add(new Point(3, 3));

			FrequencyFilter filter = new FrequencyFilter();
			var filtered = filter.Filter(seq).ToArray();
			Assert.IsTrue(filtered.Length == 4);
		}

		[TestMethod]
		public void TestOnNarrowAndWideSequence()
		{
			var seq = new List<Point>();
			seq.Add(new Point(0, 0));
			seq.Add(new Point(0.5, 0.5));
			seq.Add(new Point(1, 1));
			seq.Add(new Point(1.5, 1.5));
			seq.Add(new Point(2, 2));
			seq.Add(new Point(2.5, 2.5));
			seq.Add(new Point(3, 3));

			FrequencyFilter filter = new FrequencyFilter();
			var filtered = filter.Filter(seq).ToArray();
			Assert.IsTrue(filtered.Length == 4);
		}
	}
}
