using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Common;

namespace DynamicDataDisplay.Tests.D3
{
	[TestClass]
	public class RingDictionaryTest
	{
		[TestMethod]
		public void CheckCleanup()
		{
			RingDictionary<string> d = new RingDictionary<string>(2);

			Assert.AreEqual(0, d.Count);
			Assert.AreEqual(0, d.Generation - d.StartGeneration);

			d.AddValue("A");

			Assert.AreEqual(1, d.Count);
			Assert.AreEqual(1, d.Generation - d.StartGeneration);
			Assert.IsTrue(d.ContainsValue("A"));

			d.AddValue("B");

			Assert.AreEqual(2, d.Count);
			Assert.AreEqual(2, d.Generation - d.StartGeneration);
			Assert.IsTrue(d.ContainsValue("B"));

			d.AddValue("C");

			Assert.AreEqual(2, d.Count);
			Assert.AreEqual(2, d.Generation - d.StartGeneration);
			Assert.IsTrue(d.ContainsValue("C"));
			Assert.IsTrue(d.ContainsValue("B"));
			Assert.IsFalse(d.ContainsValue("A"));

			d.AddValue("D");

			Assert.AreEqual(2, d.Count);
			Assert.AreEqual(2, d.Generation - d.StartGeneration);
			Assert.IsTrue(d.ContainsValue("D"));
			Assert.IsTrue(d.ContainsValue("C"));
			Assert.IsFalse(d.ContainsValue("B"));
		}
	}
}
