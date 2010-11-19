﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	/// <summary>
	/// LevelLine contains all data for one isoline line - its start point, other points and value in field.
	/// </summary>
	public sealed class LevelLine
	{
		/// <summary>
		/// Gets or sets the value of line in limits of [0..1].
		/// </summary>
		/// <value>The value01.</value>
		public double Value01 { get; set; }
		/// <summary>
		/// Gets or sets the real value of line - without scaling to [0..1] segment.
		/// </summary>
		/// <value>The real value.</value>
		public double RealValue { get; set; }

		/// <summary>
		/// Gets or sets the start point of line.
		/// </summary>
		/// <value>The start point.</value>
		public Point StartPoint { get; set; }

		private readonly List<Point> otherPoints = new List<Point>();
		/// <summary>
		/// Gets other points of line, except first point.
		/// </summary>
		/// <value>The other points.</value>
		public List<Point> OtherPoints
		{
			get { return otherPoints; }
		}

		/// <summary>
		/// Gets all points of line, including start point.
		/// </summary>
		/// <value>All points.</value>
		public IEnumerable<Point> AllPoints
		{
			get
			{
				yield return StartPoint;
				for (int i = 0; i < otherPoints.Count; i++)
				{
					yield return otherPoints[i];
				}
			}
		}

		/// <summary>
		/// Gets all the segments of lines.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Range<Point>> GetSegments()
		{
			if (otherPoints.Count < 1)
				yield break;

			yield return new Range<Point>(StartPoint, otherPoints[0]);
			for (int i = 1; i < otherPoints.Count; i++)
			{
				yield return new Range<Point>(otherPoints[i - 1], otherPoints[i]);
			}
		}
	}

	/// <summary>
	/// IsolineTextLabel contains information about one label in isoline - its text, position and rotation.
	/// </summary>
	public sealed class IsolineTextLabel
	{
		/// <summary>
		/// Gets or sets the rotation of isoline text label.
		/// </summary>
		/// <value>The rotation.</value>
		public double Rotation { get; internal set; }
		/// <summary>
		/// Gets or sets the text of isoline label.
		/// </summary>
		/// <value>The text.</value>
		public double Value { get; internal set; }
		/// <summary>
		/// Gets or sets the position of isoline text label.
		/// </summary>
		/// <value>The position.</value>
		public Point Position { get; internal set; }
	}

	/// <summary>
	/// Collection which contains all data generated by <seealso cref="IsolineBuilder"/>.
	/// </summary>
	public sealed class IsolineCollection : IEnumerable<LevelLine>
	{
        private double min;
        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        private double max;
        public double Max
        {
            get { return max; }
            set { max = value; }
        }

		private readonly List<LevelLine> lines = new List<LevelLine>();
		/// <summary>
		/// Gets the list of isoline lines.
		/// </summary>
		/// <value>The lines.</value>
		public List<LevelLine> Lines
		{
			get { return lines; }
		}

		internal void StartLine(Point p, double value01, double realValue)
		{
            LevelLine segment = new LevelLine { StartPoint = p, Value01 = value01, RealValue = realValue };
            if (lines.Count == 0 || lines[lines.Count - 1].OtherPoints.Count > 0)
                lines.Add(segment);
            else
                lines[lines.Count - 1] = segment;
            
		}

		internal void AddPoint(Point p)
		{
			lines[lines.Count - 1].OtherPoints.Add(p);
		}

		#region IEnumerable<LevelLine> Members

		public IEnumerator<LevelLine> GetEnumerator()
		{
			return lines.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
