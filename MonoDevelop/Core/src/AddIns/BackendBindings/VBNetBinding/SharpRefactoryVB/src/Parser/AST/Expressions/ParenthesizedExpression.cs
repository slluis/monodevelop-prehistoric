using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class ParenthesizedExpression : Expression
	{
		Expression expression;
		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = value;
			}
		}
		
		public ParenthesizedExpression(Expression expression)
		{
			this.expression  = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ParenthesizedExpression: Expression={0}]",
			                     expression);
		}
	}
}
