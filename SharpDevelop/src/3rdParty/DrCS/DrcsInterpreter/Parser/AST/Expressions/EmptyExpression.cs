using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// This class is not in the AST. It serves as a type carrier during type checking and conversion.
	/// </summary>
	public class EmptyExpression : Expression
	{
		
		public EmptyExpression()
		{}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forEmptyExpression(this, inp);
		}
	}
}
