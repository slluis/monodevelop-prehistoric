using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Return.
	/// </summary>
	public class Return : Statement {
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
			set {
				expr = value;
			}
		}
		
		
		public Return (Location l) {
			expr = null;
			loc = l;
		}

		public Return (Expression expr, Location l) {
			this.expr = expr;
			loc = l;
		}
	
		public override object execute(IStatementVisitor v, object inp) {
			return v.forReturn(this, inp);
		}
	}

}
