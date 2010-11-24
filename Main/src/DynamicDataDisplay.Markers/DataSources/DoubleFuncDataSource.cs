using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;
using System.Diagnostics.Contracts;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources
{
	class DoubleFuncDataSource : PointDataSourceBase
	{
		private readonly Func<double, double> func = null;

		public DoubleFuncDataSource(Func<double, double> func)
		{
			Contract.Assert(func != null);

			this.func = func;
		}

		protected override IEnumerable GetDataCore()
		{
			throw new NotImplementedException();
		}

		public override object GetDataType()
		{
			return typeof(double);
		}
	}
}
