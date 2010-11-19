using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Microsoft.Research.DynamicDataDisplay.SampleDataSources
{
	public struct PotentialPoint3D
	{
		public Point3D Position;
		public double Potential;

		public PotentialPoint3D(Point3D position, double potential)
		{
			this.Position = position;
			this.Potential = potential;
		}
	}

	public sealed class PotentialField3D
	{
		private readonly List<PotentialPoint3D> points = new List<PotentialPoint3D>();
		public IEnumerable<PotentialPoint3D> Points
		{
			get { return points; }
		}

		public void AddPoints(IEnumerable<PotentialPoint3D> points)
		{
			this.points.AddMany(points);
		}

		public void AddPotentialPoint(PotentialPoint3D point)
		{
			points.Add(point);
		}

		public void AddPotentialPoint(Point3D position, double potential)
		{
			points.Add(new PotentialPoint3D(position, potential));
		}

		public void AddPotentialPoint(double x, double y, double z, double potential)
		{
			points.Add(new PotentialPoint3D(new Point3D(x, y, z), potential));
		}

		public void Clear()
		{
			points.Clear();
		}

		public event EventHandler Changed;

		public void RaiseChanged()
		{
			Changed.Raise(this);
		}

		public Vector3D GetVector(Point3D position)
		{
			Vector3D potential = new Vector3D();

			foreach (var point in points)
			{
				var toPoint = (point.Position - position);
				var length = toPoint.Length;
				var pointer = toPoint * point.Potential / (length * length * length);
				potential += pointer;
			}

			return potential;
		}

		public Vector3D GetTangentVector(Point3D position)
		{
			Vector3D potential = new Vector3D();
			Vector3D up = new Vector3D(0, 0, 1);

			for (int i = 0; i < points.Count; i++)
			{
				var point = points[i];
				var toPoint = (point.Position - position);
				var length = toPoint.Length;
				var pointer = toPoint * point.Potential / (length * length * length);
				potential += Vector3D.CrossProduct(pointer, new Vector3D(pointer.Z, pointer.X, pointer.Y));
			}

			return potential;
		}
	}
}
