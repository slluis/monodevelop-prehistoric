using System;
using System.Collections;

namespace ICSharpCode.CsVbRefactory.Parser.AST 
{
	public class ThrowStatement : Statement
	{
		Expression expression;
		
//		public Expression ThrowExpression {
//			get {
//				return expression;
//			}
//			set {
//				expression = value;
//			}
//		}
//		
		public Expression Expression {
			get {
				return expression;
			}
			set {
				expression = value;
			}
		}
		
		public ThrowStatement(Expression expression)
		{
			this.expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ThrowStatement: Expression={0}]", 
			                     expression);
		}
	}
}
