using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common;
using System.Windows.Markup;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Describes a rectangle in viewport or data coordinates.
	/// </summary>
	[Serializable]
	[ValueSerializer(typeof(DataRectSerializer))]
	[TypeConverter(typeof(DataRectConverter))]
	public struct DataRect : IEquatable<DataRect>, IFormattable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double xMin;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double yMin;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double width;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private double height;

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="rect">Source rect.</param>
		public DataRect(Rect rect)
		{
			xMin = rect.X;
			yMin = rect.Y;
			width = rect.Width;
			height = rect.Height;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="size">The size.</param>
		public DataRect(Size size)
		{
			if (size.IsEmpty)
			{
				this = emptyRect;
			}
			else
			{
				xMin = yMin = 0.0;
				width = size.Width;
				height = size.Height;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="size">The size.</param>
		public DataRect(Point location, Size size)
		{
			if (size.IsEmpty)
			{
				this = emptyRect;
			}
			else
			{
				xMin = location.X;
				yMin = location.Y;
				width = size.Width;
				height = size.Height;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="point1">The point1.</param>
		/// <param name="point2">The point2.</param>
		public DataRect(Point point1, Point point2)
		{
			xMin = Math.Min(point1.X, point2.X);
			yMin = Math.Min(point1.Y, point2.Y);
			width = Math.Max((double)(Math.Max(point1.X, point2.X) - xMin), 0);
			height = Math.Max((double)(Math.Max(point1.Y, point2.Y) - yMin), 0);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <param name="vector">The vector.</param>
		public DataRect(Point point, Vector vector) : this(point, point + vector) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DataRect"/> struct.
		/// </summary>
		/// <param name="xMin">The minimal x.</param>
		/// <param name="yMin">The minimal y.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public DataRect(double xMin, double yMin, double width, double height)
		{
			if ((width < 0) || (height < 0))
				throw new ArgumentException(Strings.Exceptions.WidthAndHeightCannotBeNegative);

			this.xMin = xMin;
			this.yMin = yMin;
			this.width = width;
			this.height = height;
		}

		#endregion

		#region Static

		/// <summary>
		/// Creates the DataRect from minimal and maximal 'x' and 'y' coordinates.
		/// </summary>
		/// <param name="xMin">The x min.</param>
		/// <param name="yMin">The y min.</param>
		/// <param name="xMax">The x max.</param>
		/// <param name="yMax">The y max.</param>
		/// <returns></returns>
		public static DataRect Create(double xMin, double yMin, double xMax, double yMax)
		{
			DataRect rect = new DataRect(xMin, yMin, xMax - xMin, yMax - yMin);
			return rect;
		}

		/// <summary>
		/// Creates DataRect from the points.
		/// </summary>
		/// <param name="x1">The x1.</param>
		/// <param name="y1">The y1.</param>
		/// <param name="x2">The x2.</param>
		/// <param name="y2">The y2.</param>
		/// <returns></returns>
		public static DataRect FromPoints(double x1, double y1, double x2, double y2)
		{
			return new DataRect(new Point(x1, y1), new Point(x2, y2));
		}

		/// <summary>
		/// Creates DataRect from the size and coordinates of its center.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <returns></returns>
		public static DataRect FromCenterSize(Point center, double width, double height)
		{
			DataRect rect = new DataRect(center.X - width / 2, center.Y - height / 2, width, height);
			return rect;
		}

		/// <summary>
		/// Creates DataRect from the size and coordinates of its center.
		/// </summary>
		/// <param name="center">The center.</param>
		/// <param name="size">The size.</param>
		/// <returns></returns>
		public static DataRect FromCenterSize(Point center, Size size)
		{
			return FromCenterSize(center, size.Width, size.Height);
		}

		/// <summary>
		/// Creates DataRect from coordinates of center and size.
		/// </summary>
		/// <param name="centerX">The center X.</param>
		/// <param name="centerY">The center Y.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <returns></returns>
		public static DataRect FromCenterSize(double centerX, double centerY, double width, double height)
		{
			return FromCenterSize(new Point(centerX, centerY), width, height);
		}

		/// <summary>
		/// Intersects with the specified rectangle.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns></returns>
		public static DataRect Intersect(DataRect rect1, DataRect rect2)
		{
			rect1.Intersect(rect2);
			return rect1;
		}

		public static implicit operator DataRect(Rect rect)
		{
			return new DataRect(rect);
		}

		#endregion

		/// <summary>
		/// Converts to WPF rect.
		/// </summary>
		/// <returns></returns>
		public Rect ToRect()
		{
			return new Rect(xMin, yMin, width, height);
		}

		/// <summary>
		/// Intersects with the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		public void Intersect(DataRect rect)
		{
			if (!IntersectsWith(rect))
			{
				this = DataRect.Empty;
				return;
			}

			DataRect res = new DataRect();

			double x = Math.Max(this.XMin, rect.XMin);
			double y = Math.Max(this.YMin, rect.YMin);
			res.width = Math.Max((double)(Math.Min(this.XMax, rect.XMax) - x), 0.0);
			res.height = Math.Max((double)(Math.Min(this.YMax, rect.YMax) - y), 0.0);
			res.xMin = x;
			res.yMin = y;

			this = res;
		}

		/// <summary>
		/// Intersects with the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <returns></returns>
		public bool IntersectsWith(DataRect rect)
		{
			if (IsEmpty || rect.IsEmpty)
				return false;

			return ((((rect.XMin <= this.XMax) && (rect.XMax >= this.XMin)) && (rect.YMax >= this.YMin)) && (rect.YMin <= this.YMax));
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get { return width < 0 && height < 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty horizontally.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is empty horizontally; otherwise, <c>false</c>.
		/// </value>
		public bool IsEmptyX
		{
			get { return width < 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty vertically.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is empty vertically; otherwise, <c>false</c>.
		/// </value>
		public bool IsEmptyY
		{
			get { return height < 0; }
		}

		/// <summary>
		/// Gets the minimal y coordinate.
		/// </summary>
		/// <value>The bottom.</value>
		public double YMin
		{
			get { return yMin; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

				yMin = value;
			}
		}

		/// <summary>
		/// Gets the maximal y value.
		/// </summary>
		/// <value>The top.</value>
		public double YMax
		{
			get
			{
				if (IsEmpty)
					return Double.PositiveInfinity;

				return yMin + height;
			}
		}

		/// <summary>
		/// Gets the minimal x value.
		/// </summary>
		/// <value>The left.</value>
		public double XMin
		{
			get { return xMin; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

				xMin = value;
			}
		}

		/// <summary>
		/// Gets the maximal x value.
		/// </summary>
		/// <value>The right.</value>
		public double XMax
		{
			get
			{
				if (IsEmpty)
					return Double.PositiveInfinity;

				return xMin + width;
			}
		}

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		public Point Location
		{
			get { return new Point(xMin, yMin); }
			set
			{
				if (IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

				xMin = value.X;
				yMin = value.Y;
			}
		}

		/// <summary>
		/// Gets the point with coordinates X max Y max.
		/// </summary>
		/// <value>The X max Y max.</value>
		public Point XMaxYMax
		{
			get { return new Point(XMax, YMax); }
		}

		/// <summary>
		/// Gets the point with coordinates X min Y min.
		/// </summary>
		/// <value>The X min Y min.</value>
		public Point XMinYMin
		{
			get { return new Point(xMin, yMin); }
		}

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public Size Size
		{
			get
			{
				if (IsEmpty)
					return Size.Empty;

				return new Size(width, height);
			}
			set
			{
				if (value.IsEmpty)
				{
					this = emptyRect;
				}
				else
				{
					if (IsEmpty)
						throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

					width = value.Width;
					height = value.Height;
				}
			}
		}

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public double Width
		{
			get { return width; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);
				if (value < 0)
					throw new ArgumentOutOfRangeException(Strings.Exceptions.DataRectSizeCannotBeNegative);

				width = value;
			}
		}

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height.</value>
		public double Height
		{
			get { return height; }
			set
			{
				if (this.IsEmpty)
					throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);
				if (value < 0)
					throw new ArgumentOutOfRangeException(Strings.Exceptions.DataRectSizeCannotBeNegative);

				height = value;
			}
		}

		/// <summary>
		/// Gets the horizontal range.
		/// </summary>
		/// <value>The horizontal range.</value>
		public Range<double> HorizontalRange
		{
			get { return new Range<double>(xMin, XMax); }
		}

		/// <summary>
		/// Gets the vertical range.
		/// </summary>
		/// <value>The vertical range.</value>
		public Range<double> VerticalRange
		{
			get { return new Range<double>(yMin, YMax); }
		}

		private static readonly DataRect emptyRect = CreateEmptyRect();

		/// <summary>
		/// Gets the empty rectangle.
		/// </summary>
		/// <value>The empty.</value>
		public static DataRect Empty
		{
			get { return DataRect.emptyRect; }
		}

		private static DataRect CreateEmptyRect()
		{
			DataRect rect = new DataRect();
			rect.xMin = Double.PositiveInfinity;
			rect.yMin = Double.PositiveInfinity;
			rect.width = Double.NegativeInfinity;
			rect.height = Double.NegativeInfinity;
			return rect;
		}

		private static readonly DataRect infinite = new DataRect(Double.MinValue / 2, Double.MinValue / 2, Double.MaxValue, Double.MaxValue);
		/// <summary>
		/// Gets the infinite dataRect.
		/// </summary>
		/// <value>The infinite.</value>
		public static DataRect Infinite
		{
			get { return infinite; }
		}

		#region Object overrides

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (!(obj is DataRect))
				return false;

			DataRect other = (DataRect)obj;

			return Equals(other);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			if (IsEmpty)
				return 0;

			return xMin.GetHashCode() ^
					width.GetHashCode() ^
					yMin.GetHashCode() ^
					height.GetHashCode();
		}

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override string ToString()
		{
			if (IsEmpty)
				return "Empty";

			return String.Format("({0:F};{1:F}) -> {2:F}*{3:F}", xMin, yMin, width, height);
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(DataRect rect1, DataRect rect2)
		{
			return rect1.Equals(rect2);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(DataRect rect1, DataRect rect2)
		{
			return !rect1.Equals(rect2);
		}

		/// <summary>
		/// Checks if sizes of specified rectangles are equal with eps ratio.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <param name="eps">The eps.</param>
		/// <returns></returns>
		public static bool EqualsEpsSizes(DataRect rect1, DataRect rect2, double eps)
		{
			double least = 1 / (1 + eps);
			double greatest = 1 + eps;

			double widthRatio = rect1.width / rect2.width;
			double heightRatio = rect1.height / rect2.height;

			return least < widthRatio && widthRatio < greatest &&
				least < heightRatio && heightRatio < greatest;
		}

		/// <summary>
		/// Checks if rect1 is similar to rect2 with coefficient eps.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <param name="eps">The eps.</param>
		/// <returns></returns>
		public static bool EqualEps(DataRect rect1, DataRect rect2, double eps)
		{
			double width = Math.Min(rect1.width, rect2.width);
			double height = Math.Min(rect1.height, rect2.height);
			return Math.Abs(rect1.xMin - rect2.xMin) < width * eps &&
				   Math.Abs(rect1.XMax - rect2.XMax) < width * eps &&
				   Math.Abs(rect1.yMin - rect2.yMin) < height * eps &&
				   Math.Abs(rect1.YMax - rect2.YMax) < height * eps;
		}

		#endregion

		#region IEquatable<DataRect> Members

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(DataRect other)
		{
			if (this.IsEmpty)
				return other.IsEmpty;

			return xMin == other.xMin &&
					width == other.width &&
					yMin == other.yMin &&
					height == other.height;
		}

		#endregion

		/// <summary>
		/// Determines whether this DataRect contains point with specified coordinates.
		/// </summary>
		/// <param name="x">The x coordinate of point.</param>
		/// <param name="y">The y coordinate of point.</param>
		/// <returns>
		/// 	<c>true</c> if contains point with specified coordinates; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(double x, double y)
		{
			if (this.IsEmpty)
				return false;

			return x >= xMin &&
				x <= XMax &&
				y >= yMin &&
				y <= YMax;
		}

		/// <summary>
		/// Determines whether rectangle contains the specified point.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified point]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(Point point)
		{
			return Contains(point.X, point.Y);
		}

		/// <summary>
		/// Determines whether rectangle contains the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified rect]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(DataRect rect)
		{
			if (this.IsEmpty || rect.IsEmpty)
				return false;

			return
				this.xMin <= rect.xMin &&
				this.yMin <= rect.yMin &&
				this.XMax >= rect.XMax &&
				this.YMax >= rect.YMax;
		}

		/// <summary>
		/// Offsets at the specified vector.
		/// </summary>
		/// <param name="offsetVector">The offset vector.</param>
		public void Offset(Vector offsetVector)
		{
			if (this.IsEmpty)
				throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

			this.xMin += offsetVector.X;
			this.yMin += offsetVector.Y;
		}

		/// <summary>
		/// Offsets at the specified offset.
		/// </summary>
		/// <param name="offsetX">The offset X.</param>
		/// <param name="offsetY">The offset Y.</param>
		public void Offset(double offsetX, double offsetY)
		{
			if (this.IsEmpty)
				throw new InvalidOperationException(Strings.Exceptions.CannotModifyEmptyDataRect);

			this.xMin += offsetX;
			this.yMin += offsetY;
		}

		/// <summary>
		/// Offsets the specified rect at values.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <param name="offsetX">The offset X.</param>
		/// <param name="offsetY">The offset Y.</param>
		/// <returns></returns>
		public static DataRect Offset(DataRect rect, double offsetX, double offsetY)
		{
			rect.Offset(offsetX, offsetY);
			return rect;
		}

		internal void UnionFinite(DataRect rect)
		{
			if (!rect.IsEmpty)
			{
				if (rect.xMin.IsInfinite())
					rect.xMin = 0;
				if (rect.yMin.IsInfinite())
					rect.yMin = 0;
				if (rect.width.IsInfinite())
					rect.width = 0;
				if (rect.height.IsInfinite())
					rect.height = 0;
			}

			Union(rect);
		}

		/// <summary>
		/// Unions with the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		public void Union(DataRect rect)
		{
			if (IsEmpty)
			{
				this = rect;
				return;
			}
			else if (!rect.IsEmpty)
			{
				double minX = Math.Min(xMin, rect.xMin);
				double minY = Math.Min(yMin, rect.yMin);

				if (rect.width == Double.PositiveInfinity || this.width == Double.PositiveInfinity)
				{
					this.width = Double.PositiveInfinity;
				}
				else
				{
					double maxX = Math.Max(XMax, rect.XMax);
					this.width = Math.Max(maxX - minX, 0.0);
				}

				if (rect.height == Double.PositiveInfinity || this.height == Double.PositiveInfinity)
				{
					this.height = Double.PositiveInfinity;
				}
				else
				{
					double maxY = Math.Max(YMax, rect.YMax);
					this.height = Math.Max(maxY - minY, 0.0);
				}

				this.xMin = minX;
				this.yMin = minY;
			}
		}

		/// <summary>
		/// Unions rect with the specified point.
		/// </summary>
		/// <param name="point">The point.</param>
		public void Union(Point point)
		{
			this.Union(new DataRect(point, point));
		}

		/// <summary>
		/// Unions rect with specified x-coordinate.
		/// </summary>
		/// <param name="x">The x.</param>
		public void UnionX(double x)
		{
			if (Double.IsInfinity(xMin)) // empty
			{
				xMin = x;
			}
			else if (xMin < x)
			{
				width = Math.Max(width, x - xMin);
			}
			else // xMin > x
			{
				width += xMin - x;
				xMin = x;
			}
		}

		/// <summary>
		/// Unions rect with specified x-coordinate.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <param name="x">The x.</param>
		/// <returns></returns>
		public static DataRect UnionX(DataRect rect, double x)
		{
			rect.UnionX(x);
			return rect;
		}

		/// <summary>
		/// Unions rect with specified y-coordinate.
		/// </summary>
		/// <param name="y">The y.</param>
		public void UnionY(double y)
		{
			if (Double.IsInfinity(yMin)) // empty
			{
				yMin = y;
			}
			else if (yMin < y)
			{
				height = Math.Max(height, y - yMin);
			}
			else // yMin > y
			{
				height += yMin - y;
				yMin = y;
			}
		}

		/// <summary>
		/// Unions rect with specified y-coordinate.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <param name="y">The y.</param>
		/// <returns></returns>
		public static DataRect UnionY(DataRect rect, double y)
		{
			rect.UnionY(y);
			return rect;
		}

		/// <summary>
		/// Unions with the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		/// <param name="point">The point.</param>
		/// <returns></returns>
		public static DataRect Union(DataRect rect, Point point)
		{
			rect.Union(point);

			return rect;
		}

		/// <summary>
		/// Unions with the specified rect.
		/// </summary>
		/// <param name="rect1">The rect1.</param>
		/// <param name="rect2">The rect2.</param>
		/// <returns></returns>
		public static DataRect Union(DataRect rect1, DataRect rect2)
		{
			rect1.Union(rect2);

			return rect1;
		}

		internal string ConvertToString(string format, IFormatProvider provider)
		{
			if (IsEmpty)
				return "Empty";

			char listSeparator = TokenizerHelper.GetNumericListSeparator(provider);
			return String.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", listSeparator, xMin, yMin, width, height);
		}

		/// <summary>
		/// Parses the specified string as a DataRect.
		/// </summary>
		/// <remarks>
		/// There are three possible string patterns, that are recognized as string representation of DataRect:
		/// 1) Literal string "Empty" - represents an DataRect.Empty rect;
		/// 2) String in format "d,d,d,d", where d is a floating-point number with '.' as decimal separator - is considered as a string 
		/// of "XMin,YMin,Width,Height";
		/// 3) String in format "d,d d,d", where d is a floating-point number with '.' as decimal separator - is considered as a string
		/// of "XMin,YMin XMax,YMax".
		/// </remarks>
		/// <param name="source">The source.</param>
		/// <returns>DataRect, parsed from the given input string.</returns>
		public static DataRect Parse(string source)
		{
			DataRect rect;
			IFormatProvider cultureInfo = CultureInfo.GetCultureInfo("en-us");

			if (source == "Empty")
			{
				rect = DataRect.Empty;
			}
			else
			{
				// format X,Y,Width,Height
				string[] values = source.Split(',');
				if (values.Length == 4)
				{
					rect = new DataRect(
						Convert.ToDouble(values[0], cultureInfo),
						Convert.ToDouble(values[1], cultureInfo),
						Convert.ToDouble(values[2], cultureInfo),
						Convert.ToDouble(values[3], cultureInfo)
						);
				}
				else
				{
					// format XMin, YMin - XMax, YMax
					values = source.Split(new Char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					rect = DataRect.Create(
						Convert.ToDouble(values[0], cultureInfo),
						Convert.ToDouble(values[1], cultureInfo),
						Convert.ToDouble(values[2], cultureInfo),
						Convert.ToDouble(values[3], cultureInfo)
						);
				}
			}

			return rect;
		}

		#region IFormattable Members

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			return ConvertToString(format, formatProvider);
		}

		#endregion
	}
}
