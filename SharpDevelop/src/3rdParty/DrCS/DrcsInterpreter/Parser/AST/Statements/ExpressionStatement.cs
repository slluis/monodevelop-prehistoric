using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for StatementExpression.
	/// </summary>
	public class ExpressionStatement: Statement {
		private StatementExpression expr;
		public StatementExpression Expr {
			get {
				return expr;
			}
		}
		
		public ExpressionStatement (StatementExpression expr, Location l) {
			this.expr = expr;
			loc = l;
		}
/*
		public override bool Resolve (EmitContext ec) {
			expr = (Expression) expr.Resolve (ec);
			return expr != null;
		}
		
		public override string ToString () {
			return "StatementExpression (" + expr + ")";
		}
*/
		public override object execute(IStatementVisitor v, object inp) {
			return v.forExpressionStatement(this, inp);
		}
	}

}
