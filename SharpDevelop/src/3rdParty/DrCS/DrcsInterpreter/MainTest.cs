using System;
using System.Reflection;
using System.IO;
using System.Collections;
using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Parser;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp
{
	/// <summary>
	/// Summary description for Main.
	/// </summary>
	public class MainTest
	{
		public MainTest() {}

		[STAThread]
		static void Main(string[] args) {
			bool redirect = false;
			
			if(redirect) {
				DB.redirect();
			}

			try {
				TypeManager tm = new TypeManager();
				//DoubleLit x = new DoubleLit(2.2);
				//IntLit y = new IntLit(5);
				//FloatLit y = new FloatLit((float)1.1);
				BoolLit x = BoolLit.TrueInstance;
				BoolLit y = BoolLit.FalseInstance;


				Binary b = new Binary(Binary.BiOperator.Equality, x, y, Location.Null);
				InterpretationVisitor iv = new InterpretationVisitor(tm);
				object retVal = b.execute(iv, null);
				Console.WriteLine("retVal = " + retVal);
			}
			catch(MissingTypeOrNamespaceException e) {
				Console.WriteLine("Error: " + e);
			}
			catch(Exception e) {
				Console.WriteLine(e);
			}

			if(redirect)
				DB.unredirect();

			Console.WriteLine("done");
		}

	}
}
