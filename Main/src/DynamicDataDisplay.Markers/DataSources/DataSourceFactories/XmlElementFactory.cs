using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public class XmlElementFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			XmlElement xmlElement = data as XmlElement;
			if (xmlElement != null)
			{
				var dataSource = new XmlElementDataSource(xmlElement);
				return dataSource;
			}

			return null;
		}
	}
}
