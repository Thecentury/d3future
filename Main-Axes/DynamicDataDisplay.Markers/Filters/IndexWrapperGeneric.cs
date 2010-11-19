using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Filters
{
	[DebuggerDisplay("Data={Data}, Index={Index}")]
	public struct IndexWrapper<T>
	{
		public T Data { get; set; }
		public int Index { get; set; }
	}
}
