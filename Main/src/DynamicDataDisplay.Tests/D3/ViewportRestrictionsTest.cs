using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Test.D3
{
	[TestClass]
	public class ViewportConstraintsTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestAddingNull()
		{
			ChartPlotter plotter = new ChartPlotter();
			bool thrown = false;
			try
			{
				plotter.Viewport.Constraints.Add(null);
			}
			catch (ArgumentNullException)
			{
				thrown = true;
			}

			Assert.IsTrue(thrown);
		}
	}
}
