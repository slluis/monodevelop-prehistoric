using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for UsingNamespace.
	/// </summary>
	public class UsingNamespace : Statement
	{
		private string space;
		public string Space {
			get {
				return space;
			}
		}

		public UsingNamespace(string sp, Location l)
		{
			this.space = sp;
			loc = l;
		}

		public override object execute(IStatementVisitor v, object inp) {
			return v.forUsingNamespace(this, inp);
		}
	}
}
