using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	/// <summary>
	/// Represents a plugin which changes the way of creating environment.
	/// </summary>
	public abstract class EnvironmentPlugin
	{
		/// <summary>
		/// Creates the environment.
		/// </summary>
		/// <param name="viewport">The viewport.</param>
		/// <returns></returns>
		public abstract DataSourceEnvironment CreateEnvironment(Viewport2D viewport);
	}
}
