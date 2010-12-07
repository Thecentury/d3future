using Microsoft.Research.DynamicDataDisplay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Windows;
using System.Threading;
using System.Security.Permissions;
using System;
using System.Linq;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace DynamicDataDisplay.Test
{
	[TestClass]
	public class AxisTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void HorizontalAxisTest()
		{
			ChartPlotter plotter = new ChartPlotter();
			var expected = new HorizontalAxis();
			plotter.MainHorizontalAxis = expected;
			GeneralAxis actual = plotter.MainHorizontalAxis;

			Assert.AreEqual(expected, actual);
			Assert.IsTrue(plotter.Children.OfType<HorizontalAxis>().Count() == 1);
		}

		[TestMethod]
		public void VerticalAxisTest()
		{
			ChartPlotter plotter = new ChartPlotter();
			var expected = new VerticalAxis();
			GeneralAxis actual;
			plotter.MainVerticalAxis = expected;
			actual = plotter.MainVerticalAxis;

			Assert.AreEqual(expected, actual);
			Assert.IsTrue(plotter.Children.OfType<VerticalAxis>().Count() == 1);
		}

		[TestMethod]
		public void HorizontalAxisIsDefaultTest()
		{
			ChartPlotter plotter = new ChartPlotter();
			plotter.PerformLoad();

			HorizontalAxis axis = (HorizontalAxis)plotter.MainHorizontalAxis;
			HorizontalAxis axis2 = new HorizontalAxis();
			plotter.Children.Add(axis2);

			Assert.AreEqual(plotter.MainHorizontalAxis, axis);
			Assert.IsTrue(axis.IsDefaultAxis);
			Assert.AreEqual(2, plotter.Children.OfType<HorizontalAxis>().Count());

			axis2.IsDefaultAxis = true;
			Assert.AreEqual(axis2, plotter.MainHorizontalAxis);
			Assert.IsFalse(axis.IsDefaultAxis);

			axis.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainHorizontalAxis, axis);
			Assert.IsFalse(axis2.IsDefaultAxis);
		}

		[TestMethod]
		public void VerticalAxisIsDefaultTest()
		{
			ChartPlotter plotter = new ChartPlotter();
			plotter.PerformLoad();

			VerticalAxis axis = (VerticalAxis)plotter.MainVerticalAxis;
			VerticalAxis axis2 = new VerticalAxis();
			plotter.Children.Add(axis2);

			Assert.AreEqual(plotter.MainVerticalAxis, axis);
			Assert.IsTrue(axis.IsDefaultAxis);

			axis2.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainVerticalAxis, axis2);
			Assert.IsFalse(axis.IsDefaultAxis);

			axis.IsDefaultAxis = true;
			Assert.AreEqual(plotter.MainVerticalAxis, axis);
			Assert.IsFalse(axis2.IsDefaultAxis);
		}
	}
}
