using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Windows.Media.Media3D;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	internal static class MathHelper
	{
		public static double GetValue(Vector3D position, double[, ,] array)
		{
			int i = (int)Math.Truncate(position.X);
			int j = (int)Math.Truncate(position.Y);
			int k = (int)Math.Truncate(position.Z);

			if (i >= array.GetLength(0) - 1) i = array.GetLength(0) - 2;
			if (j >= array.GetLength(1) - 1) j = array.GetLength(1) - 2;
			if (k >= array.GetLength(2) - 1) k = array.GetLength(2) - 2;

			double x1 = Interpolate(array[i, j, k], i, array[i + 1, j, k], i + 1, position.X);
			double x2 = Interpolate(array[i, j + 1, k], i, array[i + 1, j + 1, k], i + 1, position.X);
			double x3 = Interpolate(array[i, j, k + 1], i, array[i + 1, j, k + 1], i + 1, position.X);
			double x4 = Interpolate(array[i, j + 1, k + 1], i, array[i + 1, j + 1, k + 1], i + 1, position.X);

			double z1 = Interpolate(x1, k, x3, k + 1, position.Z);
			double z2 = Interpolate(x2, k, x4, k + 1, position.Z);

			double result = Interpolate(z1, j, z2, j + 1, position.Y);

			return result;
		}

		public static double GetValue(Vector position, double[,] array)
		{
			int i = (int)Math.Truncate(position.X);
			int j = (int)Math.Truncate(position.Y);

			if (i >= array.GetLength(0) - 1) i = array.GetLength(0) - 2;
			if (j >= array.GetLength(1) - 1) j = array.GetLength(1) - 2;

			double x1 = Interpolate(array[i, j], i, array[i + 1, j], i + 1, position.X);
			double x2 = Interpolate(array[i, j + 1], i, array[i + 1, j + 1], i + 1, position.X);

			double result = Interpolate(x1, j, x2, j + 1, position.Y);

			return result;
		}

		public static Vector3D GetVector(Vector3D position, Vector3D[, ,] array)
		{
			int i = (int)Math.Truncate(position.X);
			int j = (int)Math.Truncate(position.Y);
			int k = (int)Math.Truncate(position.Z);

			if (i >= array.GetLength(0) - 1) i = array.GetLength(0) - 2;
			if (j >= array.GetLength(1) - 1) j = array.GetLength(1) - 2;
			if (k >= array.GetLength(2) - 1) k = array.GetLength(2) - 2;

			double alpha = position.X - i;
			Vector3D v_x1 = Lerp(array[i, j, k], array[i + 1, j, k], alpha);
			Vector3D v_x2 = Lerp(array[i, j + 1, k], array[i + 1, j + 1, k], alpha);
			Vector3D v_x3 = Lerp(array[i, j, k + 1], array[i + 1, j, k + 1], alpha);
			Vector3D v_x4 = Lerp(array[i, j + 1, k + 1], array[i + 1, j + 1, k + 1], alpha);

			alpha = position.Z - k;
			Vector3D v_z1 = Lerp(v_x1, v_x3, alpha);
			Vector3D v_z2 = Lerp(v_x2, v_x4, alpha);

			alpha = position.Y - j;
			return Lerp(v_z1, v_z2, alpha);
		}

		private static Vector3D Lerp(Vector3D v1, Vector3D v2, double alpha)
		{
			return v1 * (1 - alpha) + v2 * alpha;
		}

		public static bool CheckVector(Vector3D position, Vector3D[, ,] array, double[, ,] values, double missingValue, float numericalStep)
		{
			int i = (int)Math.Truncate(position.X);
			int j = (int)Math.Truncate(position.Y);
			int k = (int)Math.Truncate(position.Z);

			return !(i - numericalStep >= array.GetLength(0) || j - numericalStep >= array.GetLength(1) || k - numericalStep >= array.GetLength(2) || i + numericalStep <= 0 || j + numericalStep <= 0 || k + numericalStep <= 0 || MissingCheck(position, values, missingValue));
		}

		public static bool MissingCheck(Vector3D position, double[, ,] array, double missingValue)
		{
			int i = (int)Math.Truncate(position.X);
			int j = (int)Math.Truncate(position.Y);
			int k = (int)Math.Truncate(position.Z);

			if (i >= array.GetLength(0) - 1) i = array.GetLength(0) - 2;
			if (j >= array.GetLength(1) - 1) j = array.GetLength(1) - 2;
			if (k >= array.GetLength(2) - 1) k = array.GetLength(2) - 2;

			return
				array[i, j, k] == missingValue ||
				array[i + 1, j, k] == missingValue ||
				array[i + 1, j + 1, k] == missingValue ||
				array[i, j + 1, k] == missingValue ||
				array[i, j, k + 1] == missingValue ||
				array[i + 1, j, k + 1] == missingValue ||
				array[i + 1, j + 1, k + 1] == missingValue ||
				array[i, j + 1, k + 1] == missingValue;
		}

		public static double GetValue(Vector3D position, double[,,] array, double missingValue)
		{
			if (MissingCheck(position, array, missingValue))
				return missingValue;
			else
				return GetValue(position, array);

		}

		public static double Interpolate(double v1, double p1, double v2, double p2, double p3)
		{
			double alpha = (p3 - p1) / (p2 - p1);
			alpha = Saturate(alpha);
			return v1 * (1 - alpha) + v2 * alpha;
		}


		public static float GetPatrialDerivation(float frontValue, float backValue, float delta)
		{
			return (frontValue - backValue) / (2.0f * delta);
		}

		//public static double FindMax(INonUniformDataSource3D<double> array)
		//{
		//    double max = array.Data[0, 0, 0];
		//    for (int i = 0; i < arrayWidth; i++)
		//    {
		//        for (int j = 0; j < array.Height; j++)
		//        {
		//            for (int k = 0; k < array.Depth; k++)
		//            {
		//                if (max == (double)array.MissingValue && array.Data[i, j, k] != (double)array.MissingValue)
		//                    max = array.Data[i, j, k];
		//                else
		//                {
		//                    if (max < array.Data[i, j, k] && array.Data[i, j, k] != (double)array.MissingValue)
		//                        max = array.Data[i, j, k];
		//                }
		//            }
		//        }
		//    }
		//    return max;
		//}

		//public static double FindMin(INonUniformDataSource3D<double> array)
		//{
		//    double min = array.Data[0, 0, 0];
		//    for (int i = 0; i < array.Width; i++)
		//    {
		//        for (int j = 0; j < array.Height; j++)
		//        {
		//            for (int k = 0; k < array.Depth; k++)
		//            {
		//                if (min == (double)array.MissingValue && array.Data[i, j, k] != (double)array.MissingValue)
		//                    min = array.Data[i, j, k];
		//                else
		//                {
		//                    if (min > array.Data[i, j, k] && array.Data[i, j, k] != (double)array.MissingValue)
		//                        min = array.Data[i, j, k];
		//                }
		//            }
		//        }
		//    }
		//    return min;
		//}

		//public static Vector3D CalculateNormalToPolygon(Polygon polygon)
		//{
		//    return CalculateNormalToPolygon(polygon.p1, polygon.p2, polygon.p3);
		//}

		public static Vector3D CalculateNormalToPolygon(Vector3D p0, Vector3D p1, Vector3D p2)
		{
			Vector3D v0 = new Vector3D(
				p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
			Vector3D v1 = new Vector3D(
				p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

			var result = Vector3D.CrossProduct(v0, v1);
			result.Normalize();

			return result;
		}

		public static Vector3D CalculateNormalToField(double fX, double fY, double fZ, double[, ,] field)
		{
			Vector3D numericalStep = new Vector3D(1.0f / field.GetLength(0), 1.0f / field.GetLength(1), 1.0f / field.GetLength(2));

			Vector3D rfNormal = new Vector3D();
			rfNormal.X = GetValue(new Vector3D(fX - numericalStep.X, fY, fZ), field) - GetValue(new Vector3D(fX + numericalStep.X, fY, fZ), field);
			rfNormal.Y = GetValue(new Vector3D(fX, fY - numericalStep.Y, fZ), field) - GetValue(new Vector3D(fX, fY + numericalStep.Y, fZ), field);
			rfNormal.Z = GetValue(new Vector3D(fX, fY, fZ - numericalStep.Z), field) - GetValue(new Vector3D(fX, fY, fZ + numericalStep.Z), field);
			if (rfNormal.Length > 0)
				rfNormal.Normalize();
			return rfNormal;
		}

		public static Vector3D CalculateNormalToField(Vector3D position, double[, ,] field)
		{
			return MathHelper.CalculateNormalToField(position.X, position.Y, position.Z, field);
		}

		public static Vector3D Gradient(Vector position, Func<double, double, double> GetData, double[, ,] field)
		{
			Vector3D result = new Vector3D();

			double numericalStep = 0.01f;

			result.X = (GetData(position.X + numericalStep, position.Y) - GetData(position.X - numericalStep, position.Y));
			result.Y = (GetData(position.X, position.Y + numericalStep) - GetData(position.X, position.Y - numericalStep));
			result.Z = 0;

			return result;
		}

		public static double Lerp(double value1, double value2, double alpha)
		{
			if (alpha >= 0 && alpha <= 1)
				return (value1 * alpha + value2 * (1 - alpha));
			else
				throw new ArgumentException("Invalid alpha!");
		}

		public static bool CheckPosition<T>(Vector3D position, T[, ,] array)
		{
			int i = array.GetLength(0);
			int j = array.GetLength(1);
			int k = array.GetLength(2);

			if (position.X >= 0 && position.X < i && position.Y >= 0 && position.Y < j && position.Z >= 0 && position.Z < k)
				return true;
			else
				return false;
		}

		public static bool CheckPosition(Vector3D position, double[, ,] array, double missingValue)
		{
			int i = array.GetLength(0);
			int j = array.GetLength(1);
			int k = array.GetLength(2);

			if (position.X >= 0 && position.X < i && position.Y >= 0 && position.Y < j && position.Z >= 0 && position.Z < k && !MathHelper.MissingCheck(position, array, missingValue))
				return true;
			else
				return false;

		}

		public static float Saturate(float value)
		{
			if (value > 1) return 1.0f;
			if (value < 0) return 0.0f;
			return value;
		}

		public static double Saturate(double value)
		{
			if (value > 1) return 1.0f;
			if (value < 0) return 0.0f;
			return value;
		}

		public static double RungeCuttePositive(Func<double, double> function, double startValue, double delta)
		{
			double k_n1 = delta * function(startValue);
			double k_n2 = delta * function(startValue + k_n1 / 2.0);
			double k_n3 = delta * function(startValue + k_n2 / 2.0);
			double k_n4 = delta * function(startValue + k_n3);

			return startValue + (k_n1 + 2 * k_n2 + 2 * k_n3 + k_n4) / 6.0;
		}

		public static double RungeCutteNegative(Func<double, double> function, double startValue, double delta)
		{
			double k_n1 = -delta * function(startValue);
			double k_n2 = -delta * function(startValue + k_n1 / 2.0);
			double k_n3 = -delta * function(startValue + k_n2 / 2.0);
			double k_n4 = -delta * function(startValue + k_n3);

			return startValue + (k_n1 + 2 * k_n2 + 2 * k_n3 + k_n4) / 6.0;
		}

		public static Vector RungeCuttePositive(Func<Vector, Vector> function, Vector startValue, float delta)
		{
			Vector k_n1 = delta * function(startValue);
			Vector k_n2 = delta * function(startValue + k_n1 * 0.5f);
			Vector k_n3 = delta * function(startValue + k_n2 * 0.5f);
			Vector k_n4 = delta * function(startValue + k_n3);

			return startValue + (k_n1 + 2 * k_n2 + 2 * k_n3 + k_n4) / 6.0f;
		}

		public static Vector RungeCutteNegative(Func<Vector, Vector> function, Vector startValue, float delta)
		{
			Vector k_n1 = -delta * function(startValue);
			Vector k_n2 = -delta * function(startValue + k_n1 * 0.5f);
			Vector k_n3 = -delta * function(startValue + k_n2 * 0.5f);
			Vector k_n4 = -delta * function(startValue + k_n3);

			return startValue + (k_n1 + 2 * k_n2 + 2 * k_n3 + k_n4) / 6.0f;
		}

		private static readonly Random randomGenerator = new Random();
		public static Vector3D GeneratePositionSimple(float x, float y, float z, float width, float height, float depth)
		{
			return new Vector3D((float)randomGenerator.NextDouble() * width + x, (float)randomGenerator.NextDouble() * height + y, (float)randomGenerator.NextDouble() * depth + z);
		}

		public static Vector CreateVelocity(float time)
		{
			Vector result = new Vector((float)Math.Sin(time), (float)Math.Cos(time));
			return result * 100;
		}

		public static float GetGrad(float radian)
		{
			return (float)(radian * 180 / Math.PI);
		}

		public static float GetRadian(float grad)
		{
			return (float)(grad * Math.PI / 180);
		}

		public static W FindGlobalMax<T, W>(T source)
			where T : IDataSource3D<W>
			where W : struct, IComparable
		{
			W max = source.Data[0, 0, 0];
			for (int i = 0; i < source.Width; i++)
			{
				for (int j = 0; j < source.Height; j++)
				{
					for (int k = 0; k < source.Depth; k++)
					{
						if (max.CompareTo(source.Data[i, j, k]) < 0 /*&& source.Data[i, j, k].CompareTo(source.MissingValue) == 0*/)
							max = source.Data[i, j, k];
					}
				}
			}
			return max;
		}

		public static W FindGlobalMin<T, W>(T source)
			where T : IDataSource3D<W>
			where W : struct, IComparable
		{
			W min = source.Data[0, 0, 0];
			for (int i = 0; i < source.Width; i++)
			{
				for (int j = 0; j < source.Height; j++)
				{
					for (int k = 0; k < source.Depth; k++)
					{
						if (min.CompareTo(source.Data[i, j, k]) > 0 /*&& source.Data[i, j, k].CompareTo(source.MissingValue) == 0*/)
							min = source.Data[i, j, k];
					}
				}
			}
			return min;
		}

		public static bool IsClose(double arg1, double arg2, double eps)
		{
			return Math.Abs(arg1 - arg2) < eps;
		}

		public static void OffsetArray(int[] array, int offset)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] += offset;
			}
		}
	}
}
