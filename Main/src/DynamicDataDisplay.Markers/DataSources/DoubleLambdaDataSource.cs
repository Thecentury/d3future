using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources
{
	/// <summary>
	/// Represents a data source whcih creates points using delegate Func&lt;double, double&gt; as a generator.
	/// </summary>
	public sealed class DoubleLambdaDataSource : PointDataSourceBase
	{
		private readonly Func<double, double> func = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleLambdaDataSource"/> class.
		/// </summary>
		public DoubleLambdaDataSource()
		{
			this.DataToPoint = o => (Point)o;
			this.PointToData = p => p;
		}

		/// <summary>
		/// Gets the func.
		/// </summary>
		/// <value>The func.</value>
		public Func<double, double> Func
		{
			get { return func; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleLambdaDataSource"/> class.
		/// </summary>
		/// <param name="func">The func.</param>
		public DoubleLambdaDataSource(Func<double, double> func)
			: this()
		{
			Contract.Assert(func != null);

			this.func = func;
		}

		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			DataRect visible = environment.Visible;
			Rect output = environment.Output;
			CoordinateTransform transform = environment.Transform;

			double yMin = Double.PositiveInfinity;
			double yMax = Double.NegativeInfinity;

			double step = visible.Width / output.Width;

			for (double x = visible.XMin; x <= visible.XMax; x += step)
			{
				// todo should here go ScreenToData?
				//double dataX = transform.ScreenToViewport(new Point(x, 0)).X;
				double dataX = x;
				double viewportY = func(dataX);

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

		public override object GetDataType()
		{
			return typeof(Point);
		}

		public override DataRect GetContentBounds(IEnumerable<Point> data, DataRect visible)
		{
			DataRect bounds = data.Aggregate(DataRect.Empty, (rect, point) => DataRect.UnionY(rect, point.Y));

			return bounds;
		}
	}
}
