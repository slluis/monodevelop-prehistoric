using System;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Continue.
	/// </summary>
	public class Continue : Statement {
		
		public Continue (Location l) {
			loc = l;
		}


		public override object execute(IStatementVisitor v, object inp) {
			return v.forContinue(this, inp);
		}
	}
	
}
