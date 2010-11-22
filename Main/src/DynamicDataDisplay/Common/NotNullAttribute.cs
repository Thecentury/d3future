using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// This attribute indicates that there will be ArgumentNullException thrown if null will be passed to property's setter.
	/// Used in unit tests only.
	/// </summary>
	[Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
	internal sealed class NotNullAttribute : Attribute
	{
	}
}
