using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.IO;
using CodeplexUploadHelper.Rules;

namespace CodeplexUploadHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            RulesCollection rules = LoadRules();
            ExecutionEnvironment env = LoadEnvironment(args);

            foreach (var rule in rules)
            {
                rule.Apply(env);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("** Done! **");
            Console.ReadLine();
        }

        private static ExecutionEnvironment LoadEnvironment(string[] args)
        {
            ExecutionEnvironment result = new ExecutionEnvironment();

            if (args.Length == 0)
            {
                DirectoryInfo parentDir = new DirectoryInfo(@"C:\Downloads");
                var srcDirectories = parentDir.GetDirectories("SRC_D3 Main*");
                if (srcDirectories.Length == 1)
                {
                    result.WorkingDirectory = srcDirectories[0].FullName;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unable to find source code directory.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            else
            {
                result.WorkingDirectory = args[0].TrimStart('\"').TrimEnd('\"');
            }
            return result;
        }

        private static RulesCollection LoadRules()
        {
            RulesCollection result = null;
            string fileContent = File.ReadAllText("Rules.xaml");
            fileContent = fileContent.Replace("clr-namespace:CodeplexUploadHelper", "clr-namespace:CodeplexUploadHelper;assembly=CodeplexUploadHelper");

            result = (RulesCollection)XamlReader.Parse(fileContent);
            return result;
        }
    }
}
