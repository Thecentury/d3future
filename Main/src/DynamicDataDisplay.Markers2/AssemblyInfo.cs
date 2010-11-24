using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay.Markers2;

[assembly: XmlnsDefinition(NewD3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Markers2")]
[assembly: XmlnsPrefix(NewD3AssemblyConstants.DefaultXmlNamespace, "df")]

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	public static class NewD3AssemblyConstants
	{
		public const string DefaultXmlNamespace = "http://research.microsoft.com/DynamicDataDisplay/1.1";
	}
}
