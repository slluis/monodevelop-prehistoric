using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Implements the ternary conditiona operator (?:)
	/// </summary>
	public class Conditional : Expression {
		private Expression expr, trueExpr, falseExpr;
				
		public Conditional (Expression expr, Expression trueExpr, Expression falseExpr, Location l) {
			this.expr = expr;
			this.trueExpr = trueExpr;
			this.falseExpr = falseExpr;
			this.loc = l;
		}

		public Expression Expr {
			get {
				return expr;
			}
		}

		public Expression TrueExpr {
			get {
				return trueExpr;
			}
		}

		public Expression FalseExpr {
			get {
				return falseExpr;
			}
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forConditional(this, inp);
		}
	}

}
