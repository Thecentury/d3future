using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using System.Collections;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.Filters;
using System.Diagnostics.Contracts;

namespace DynamicDataDisplay.Markers.DataSources
{
	public sealed class PointArrayDataSource : PointDataSourceBase
	{
		public PointArrayDataSource(Point[] collection)
		{
			Contract.Assert(collection != null);

			this.collection = collection;
		}

		private readonly Point[] collection;
		public Point[] Collection
		{
			get { return collection; }
		} 

		protected override IEnumerable GetDataCore()
		{
			return Filters.Filter(IndexWrapper.Generate(collection), Environment);
		}

		public override IEnumerable GetData(int startingIndex)
		{
			return collection.Skip(startingIndex);
		}

		public override object GetDataType()
		{
			return typeof(Point);
		}

		protected override void OnEnvironmentChanged()
		{
			base.OnEnvironmentChanged();
		}
	}
}
