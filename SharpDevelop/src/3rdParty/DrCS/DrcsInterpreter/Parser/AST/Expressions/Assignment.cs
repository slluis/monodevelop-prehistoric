using System;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for Assignment.
	/// </summary>
	public class Assignment : StatementExpression
	{

		private Expression lExpr;
		public Expression LExpr {
			get {
				return lExpr;
			}
		}

		private Expression rExpr;
		public Expression RExpr {
			get {
				return rExpr;
			}
		}

		public Assignment(Expression left, Expression right, Location l)
		{
			lExpr = left;
			rExpr = right;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forAssignment(this, inp); 
		}
	}
}
