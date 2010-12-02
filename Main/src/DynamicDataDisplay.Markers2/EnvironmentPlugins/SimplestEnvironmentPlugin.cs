using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources;

namespace Microsoft.Research.DynamicDataDisplay.Markers2
{
	/// <summary>
	/// Represenys an EnvironmentPlugin which does almost nothing, just passes Viewport's output, visible and transform to the
	/// DataSourceEnvironment.
	/// </summary>
	public sealed class SimplestEnvironmentPlugin : EnvironmentPlugin
	{
		/// <summary>
		/// Creates the environment.
		/// </summary>
		/// <param name="viewport">The viewport.</param>
		/// <returns></returns>
		public override DataSourceEnvironment CreateEnvironment(Viewport2D viewport)
		{
			DataSourceEnvironment result = new DataSourceEnvironment
			{
				Output = viewport.Output,
				Visible = viewport.Visible,
				RealVisible = viewport.Visible,
				Transform = viewport.Transform
			};

			return result;
		}
	}
}
