using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for ElementAccess.
	/// </summary>
	public class ElementAccess : Expression
	{
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		private ArrayList exprList;
		public ArrayList ExprList {
			get{
				return exprList;
			}
		}

		public ElementAccess(Expression expr, ArrayList exprList, Location l)
		{
			this.expr = expr;
			this.exprList = exprList;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forElementAccess(this, inp);
		}
	}
}
