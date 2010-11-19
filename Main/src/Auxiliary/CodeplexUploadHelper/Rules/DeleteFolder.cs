using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using CodeplexUploadHelper.Rules;

namespace CodeplexUploadHelper
{
	public class DeleteFolder : Rule
	{
		public string FolderPath { get; set; }

		public override void Apply(ExecutionEnvironment env)
		{
			var pathToDelete = Path.Combine(env.WorkingDirectory, FolderPath);

			if (Directory.Exists(pathToDelete))
			{
				Directory.Delete(pathToDelete, true);
				Console.WriteLine("Deleted folder \"{0}\"", FolderPath);
			}
		}
	}
}
