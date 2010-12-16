using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources
{
	/// <summary>
	/// Represents a dataSource in which points are generated from two sequeces of double values.
	/// </summary>
	public sealed class TwoEnumerableOfDoubleDataSource : PointDataSourceBase
	{
		private const string xAndYShouldNotBeNull = "X and Y sequences should not be null.";

		/// <summary>
		/// Initializes a new instance of the <see cref="TwoEnumerableOfDoubleDataSource"/> class.
		/// </summary>
		public TwoEnumerableOfDoubleDataSource()
		{
			this.PointToData = p => p;
			this.DataToPoint = o => (Point)o;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TwoEnumerableOfDoubleDataSource"/> class.
		/// </summary>
		/// <param name="x">The sequence of x values.</param>
		/// <param name="y">The sequence of y values.</param>
		public TwoEnumerableOfDoubleDataSource(IEnumerable<double> x, IEnumerable<double> y)
			: this()
		{
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");

			this.XSequence = x;
			this.YSequence = y;
		}

		#region XSequence property

		public IEnumerable<double> XSequence
		{
			get { return (IEnumerable<double>)GetValue(XSequenceProperty); }
			set { SetValue(XSequenceProperty, value); }
		}

		public static readonly DependencyProperty XSequenceProperty = DependencyProperty.Register(
		  "XSequence",
		  typeof(IEnumerable<double>),
		  typeof(TwoEnumerableOfDoubleDataSource),
		  new FrameworkPropertyMetadata(null, OnSequenceReplaced));

		private static void OnSequenceReplaced(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TwoEnumerableOfDoubleDataSource owner = (TwoEnumerableOfDoubleDataSource)d;
			owner.RaiseCollectionReset();
		}

		#endregion

		#region YSequence property

		public IEnumerable<double> YSequence
		{
			get { return (IEnumerable<double>)GetValue(YSequenceProperty); }
			set { SetValue(YSequenceProperty, value); }
		}

		public static readonly DependencyProperty YSequenceProperty = DependencyProperty.Register(
		  "YSequence",
		  typeof(IEnumerable<double>),
		  typeof(TwoEnumerableOfDoubleDataSource),
		  new FrameworkPropertyMetadata(null, OnSequenceReplaced));

		#endregion

		protected override IEnumerable GetDataCore(DataSourceEnvironment environment)
		{
			if (XSequence == null || YSequence == null)
				throw new InvalidOperationException(xAndYShouldNotBeNull);

			IEnumerator<double> xEnumerator = XSequence.GetEnumerator();
			IEnumerator<double> yEnumerator = YSequence.GetEnumerator();

			double xMin = Double.MaxValue;
			double xMax = Double.MinValue;
			double yMin = Double.MaxValue;
			double yMax = Double.MinValue;

			while (xEnumerator.MoveNext() && yEnumerator.MoveNext())
			{
				double x = xEnumerator.Current;
				double y = yEnumerator.Current;

				if (x < xMin)
					xMin = x;
				else if (x > xMax)
					xMax = x;
				if (y < yMin)
					yMin = y;
				else if (y > yMax)
					yMax = y;

				yield return new Point(x, y);
			}

			environment.ContentBounds = new DataRect(new Point(xMin, yMin), new Point(xMax, yMax));
		}

		public override IEnumerable<Point> GetPointData(Range<int> range)
		{
			if (XSequence is IList<double> && YSequence is IList<double>)
				return GetPointDataFromList(range);
			else
				return GetPointDataFromEnumerable(range);
		}

		private IEnumerable<Point> GetPointDataFromEnumerable(Range<int> range)
		{
			IEnumerable<double> xSeq = XSequence.Skip(range.Min).Take(range.GetLength());
			IEnumerable<double> ySeq = YSequence.Skip(range.Min).Take(range.GetLength());

			IEnumerator<double> xEnumerator = xSeq.GetEnumerator();
			IEnumerator<double> yEnumerator = ySeq.GetEnumerator();

			while (xEnumerator.MoveNext() && yEnumerator.MoveNext())
			{
				yield return new Point(xEnumerator.Current, yEnumerator.Current);
			}
		}

		private IEnumerable<Point> GetPointDataFromList(Range<int> range)
		{
			IList<double> xList = (IList<double>)XSequence;
			IList<double> yList = (IList<double>)YSequence;

			for (int i = range.Min; i < range.Max; i++)
			{
				yield return new Point(xList[i], yList[i]);
			}
		}
	}
}
