
using System;
using System.Reflection;
using MonoDevelop.Services;

public class CodeCompletionDatabaseGeneratorTool
{
	public static int Main (string[] args)
	{
		if (args.Length == 0) {
			Console.WriteLine ("MonoDevelop Code Completion Database Generation Tool");
			Console.WriteLine ("Usage: dbgen <destDirectory> [<assemblyName> | <assemblyPath>]");
			return 0;
		}
		DefaultParserService parserService = new DefaultParserService ();
		parserService.GenerateAssemblyDatabase (args[0], args[1]);
		return 0;
	}
}

