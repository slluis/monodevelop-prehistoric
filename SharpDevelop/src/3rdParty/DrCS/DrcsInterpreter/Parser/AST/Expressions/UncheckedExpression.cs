using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for UncheckedExpression.
	/// </summary>
	public class UncheckedExpression : Expression
	{
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		public UncheckedExpression(Expression e, Location l)
		{
			expr = e;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forUncheckedExpression(this, inp);
		}
	}
}
