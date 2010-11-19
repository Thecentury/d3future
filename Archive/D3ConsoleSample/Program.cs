using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using System.Diagnostics;

namespace D3ConsoleSample
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			ChartPlotter plotter = new ChartPlotter { Width = 200, Height = 200 };
			plotter.SaveScreenshot("1.png");

			Process.Start("1.png");
		}
	}
}
