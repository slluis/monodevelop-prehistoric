using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for CheckedExpression.
	/// </summary>
	public class CheckedExpression : Expression
	{
		private Expression expr;
		public Expression Expr {
			get{
				return expr;
			}
		}
	
		public CheckedExpression(Expression e, Location l)
		{
			expr = e;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forCheckedExpression(this, inp);
		}
	}
}
