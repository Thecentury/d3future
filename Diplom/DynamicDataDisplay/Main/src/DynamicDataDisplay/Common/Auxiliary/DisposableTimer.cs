using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public sealed class DisposableTimer : IDisposable
	{
		private readonly bool printStart = false;
		private readonly bool isActive = true;
		private readonly string name;
		Stopwatch timer;

		public DisposableTimer(string name, bool isActive = true, bool printStart = false)
		{
			this.name = name;
			this.isActive = isActive;
			this.printStart = printStart;
			if (isActive)
			{
				timer = Stopwatch.StartNew();
				if (printStart)
					Trace.WriteLine(name + ": started " + DateTime.Now.TimeOfDay);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (isActive)
			{
				var duration = timer.ElapsedMilliseconds;
				Trace.WriteLine(String.Format("{0}: elapsed {1} ms.", name, duration.ToString()));
				timer.Stop();
			}
		}

		#endregion
	}
}
