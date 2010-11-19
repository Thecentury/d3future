using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeplexUploadHelper.Rules;

namespace CodeplexUploadHelper
{
	public class Newline : Rule
	{
		public override void Apply(ExecutionEnvironment env)
		{
			Console.WriteLine();
		}
	}
}
