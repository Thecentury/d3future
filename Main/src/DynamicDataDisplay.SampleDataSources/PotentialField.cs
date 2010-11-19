using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.SampleDataSources
{
	public struct PotentialPoint
	{
		public Point Position;
		public double Potential;

		public PotentialPoint(Point position, double potential)
		{
			this.Position = position;
			this.Potential = potential;
		}
	}

	public class PotentialField
	{
		private readonly List<PotentialPoint> points = new List<PotentialPoint>();

		public void AddPoints(IEnumerable<PotentialPoint> points)
		{
			this.points.AddMany(points);
		}

		public void AddPotentialPoint(PotentialPoint point)
		{
			points.Add(point);
		}

		public void AddPotentialPoint(Point position, double potential)
		{
			points.Add(new PotentialPoint(position, potential));
		}

		public void AddPotentialPoint(double x, double y, double potential)
		{
			points.Add(new PotentialPoint(new Point(x, y), potential));
		}

		public Vector GetPotential(Point position)
		{
			Vector3D potential = new Vector3D();
			Vector3D up = new Vector3D(0, 0, 1);

			foreach (var point in points)
			{
				var toPoint = (point.Position - position);
				var length = toPoint.Length;
				var pointer = toPoint * point.Potential / (length * length * length);
				var pointer3D = new Vector3D(pointer.X, pointer.Y, 0);
				potential += Vector3D.CrossProduct(pointer3D, up);
			}

			return new Vector(potential.X, potential.Y);
		}
	}
}
