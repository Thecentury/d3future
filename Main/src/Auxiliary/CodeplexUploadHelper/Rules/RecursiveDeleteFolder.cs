using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeplexUploadHelper.Rules;

namespace CodeplexUploadHelper
{
	public class RecursiveDeleteFolder : Rule
	{
		public string FolderName { get; set; }

		public override void Apply(ExecutionEnvironment env)
		{
			DirectoryInfo dir = new DirectoryInfo(env.WorkingDirectory);

			DeleteFolderRecursively(dir);
		}

		private void DeleteFolderRecursively(DirectoryInfo dir)
		{
			if (dir.Name == FolderName)
			{
				Console.WriteLine("Deleting " + dir.FullName);
				dir.Delete(true);
			}
			else
			{
				foreach (var subDir in dir.GetDirectories())
				{
					DeleteFolderRecursively(subDir);
				}
			}
		}
	}
}
