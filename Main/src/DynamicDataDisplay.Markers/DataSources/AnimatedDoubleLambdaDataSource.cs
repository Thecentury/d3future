using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;
using System.Windows.Threading;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources
{
	public sealed class AnimatedDoubleLambdaDataSource : PointDataSourceBase
	{
		private double interval = 30; // ms
		private readonly DispatcherTimer timer;
		private readonly Func<double, double, double> func = null;
		private readonly Stopwatch watch = Stopwatch.StartNew();

		public AnimatedDoubleLambdaDataSource(Func<double, double, double> func)
		{
			Contract.Assert(func != null);

			this.func = func;

			timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(interval)
			};
			timer.Tick += new EventHandler(OnTimer_Tick);
			timer.Start();

			DataToPoint = o => (Point)o;
			PointToData = p => p;
		}

		private void OnTimer_Tick(object sender, EventArgs e)
		{
			RaiseCollectionReset();
		}

		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			double time = watch.Elapsed.TotalSeconds;

			DataRect visible = environment.Visible;
			Rect output = environment.Output;
			CoordinateTransform transform = environment.Transform;

			double yMin = Double.PositiveInfinity;
			double yMax = Double.NegativeInfinity;

			double step = visible.Width / output.Width;

			for (double x = visible.XMin; x <= visible.XMax; x += step)
			{
				double dataX = x;
				double viewportY = func(dataX, time);

				if (viewportY < yMin)
					yMin = viewportY;
				if (viewportY > yMax)
					yMax = viewportY;

				yield return new Point(dataX, viewportY);
			}

			DataRect bounds = DataRect.Empty;
			bounds.UnionY(yMin);
			bounds.UnionY(yMax);

			environment.ContentBounds = bounds;
		}
	}
}
