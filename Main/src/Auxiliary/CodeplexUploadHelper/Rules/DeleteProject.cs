using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeplexUploadHelper.Rules;
using System.IO;

namespace CodeplexUploadHelper
{
	public class DeleteProject : Rule
	{
		public string ProjectName { get; set; }

		public override void Apply(ExecutionEnvironment env)
		{
			var sln = File.ReadAllLines(Path.Combine(env.WorkingDirectory, @"sln\DynamicDataDisplay\DynamicDataDisplay.sln")).ToList();

			string guid = "";
			for (int i = 0; i < sln.Count; i++)
			{
				var line = sln[i];
				if (line.StartsWith("Project"))
				{
					var subLine = line.Substring(53);
					var lineParts = subLine.Split('"');

					var name = lineParts[0];
					guid = lineParts[4];

					if (name == ProjectName)
					{
						sln.RemoveAt(i);
						sln.RemoveAt(i);
						break;
					}
				}
			}

			if (guid == "")
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Project \"{0}\" not found.", ProjectName);
				Console.ForegroundColor = ConsoleColor.White;
				return;
			}

			int j = 0;
			while (j < sln.Count)
			{
				var line = sln[j];
				if (line.Contains(guid))
				{
					sln.RemoveAt(j);
				}
				else
				{
					j++;
				}
			}

			File.WriteAllLines(Path.Combine(env.WorkingDirectory, @"sln\DynamicDataDisplay\DynamicDataDisplay.sln"), sln.ToArray());

			Console.WriteLine("Removed project \"{0}\"", ProjectName);
		}
	}
}
