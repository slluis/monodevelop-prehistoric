using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for ParenExpr.
	/// </summary>
	public class ParenExpr : Expression
	{
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}
        

		public ParenExpr(Expression expr, Location l) {
			this.expr = expr;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forParenExpr(this, inp);
		}
	}
}
