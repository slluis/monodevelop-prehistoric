using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for CompoundAssignment.
	/// </summary>
	public class CompoundAssignment : Expression
	{
		private Binary.BiOperator op;
		public Binary.BiOperator Op {
			get {
				return op;
			}
		}

		private Expression lVal;
		public Expression LVal {
			get {
				return lVal;
			}
		}

		private Expression rVal;
		public Expression RVal {
			get {
				return rVal;
			}
		}

		public CompoundAssignment(Binary.BiOperator bo, Expression lVal, Expression rVal, Location l)
		{
			op = bo;
			this.lVal = lVal;
			this.rVal = rVal;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forCompoundAssignment(this, inp);
		}
	}
}
