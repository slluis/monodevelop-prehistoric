using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Break.
	/// </summary>
	public class Break : Statement {
		
		public Break (Location l) {
			loc = l;
		}


	
		public override object execute(IStatementVisitor v, object inp) {
			return v.forBreak(this, inp);
		}
	}

}
