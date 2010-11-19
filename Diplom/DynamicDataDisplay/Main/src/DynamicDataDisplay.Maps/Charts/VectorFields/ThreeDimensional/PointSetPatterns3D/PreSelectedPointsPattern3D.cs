using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public class PreSelectedPointsPattern3D : PointSetPattern3D
	{
		public PreSelectedPointsPattern3D()
		{
			points.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnPoints_CollectionChanged);
		}

		private readonly ObservableCollection<Point3D> points = new ObservableCollection<Point3D>();
		public ObservableCollection<Point3D> Points
		{
			get { return points; }
		} 

		private void OnPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseChanged();
		}

		public override IEnumerable<Point3D> GeneratePoints()
		{
			foreach (var pt in points)
			{
				yield return pt;
			}
		}
	}
}
