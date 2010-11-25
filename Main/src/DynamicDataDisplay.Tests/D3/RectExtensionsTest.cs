using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;

namespace DynamicDataDisplay.Tests.D3
{
	/// <summary>
	/// Tests for Rect extensions methods.
	/// </summary>
	[TestClass]
	public class RectExtensionsTest
	{
		[TestMethod]
		public void ZoomOutExtension()
		{
			Rect rect = new Rect(new Point(0, 0), new Size(1, 2));

			Rect zoomedOut = rect.ZoomOutFromCenter(3.0);

			Assert.AreEqual(new Point(-1, -2), zoomedOut.Location);
			Assert.AreEqual(new Size(3, 6), zoomedOut.Size);
		}
	}
}
