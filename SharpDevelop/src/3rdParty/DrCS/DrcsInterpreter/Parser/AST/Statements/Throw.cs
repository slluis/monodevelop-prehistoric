using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Throw.
	/// </summary>
	public class Throw : Statement {
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}
		
		public Throw(Location l) {
			loc = l;
		}

		public Throw (Expression expr, Location l) {
			this.expr = expr;
			loc = l;
		}

		/*
		public override bool Resolve (EmitContext ec) {
			if (expr != null){
				expr = expr.Resolve (ec);
				if (expr == null)
					return false;
			}
			return true;
		}
*/
		public override object execute(IStatementVisitor v, object inp) {
			return v.forThrow(this, inp);
		}

	}


}
