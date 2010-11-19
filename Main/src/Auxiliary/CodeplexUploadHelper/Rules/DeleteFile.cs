using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeplexUploadHelper.Rules;
using System.IO;

namespace CodeplexUploadHelper
{
	public class DeleteFile : Rule
	{
		public string FilePath { get; set; }

		public override void Apply(ExecutionEnvironment env)
		{
			string filePath = Path.Combine(env.WorkingDirectory, FilePath);

			if (File.Exists(filePath))
			{
				File.Delete(filePath);

				Console.WriteLine("Deleted file \"{0}\"", FilePath);
			}
		}
	}
}
