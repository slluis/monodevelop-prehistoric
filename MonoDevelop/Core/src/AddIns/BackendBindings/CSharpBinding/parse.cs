using System;
using System.IO;

using ICSharpCode.SharpRefactory.Parser;
using CSharpBinding.Parser;

// recurse a dir for .cs files
// and print out parsing errors
class parse
{
	static void Main (string[] args)
	{
		if (args.Length == 1 && Directory.Exists (args[0]))
			new parse (args[0]);	
		else
			PrintUsage ();
	}

	static void PrintUsage ()
	{
		Console.WriteLine ("usage: parse.exe <dir>");
	}

	parse (string dir)
	{
		string[] files = Directory.GetFiles (dir, "*.cs");
		if (files.Length < 1)
			return;

		Parser p = new Parser ();
		Lexer lexer;

		foreach (string f in files) {
			lexer = new Lexer (new FileReader (f));
			p.Parse (lexer);
			CSharpVisitor v = new CSharpVisitor ();
			v.Visit (p.compilationUnit, null);
			v.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			if (v.Cu.ErrorsDuringCompile)
				Console.WriteLine ("errors in parsing " + f);
			foreach (ErrorInfo error in p.Errors.ErrorInformation)
				Console.WriteLine (error.ToString ());
		}
	}
}

