using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeplexUploadHelper.Rules
{
	public abstract class Rule
	{
		public abstract void Apply(ExecutionEnvironment env);
	}
}
