using System;
using System.Collections;

namespace ICSharpCode.CsVbRefactory.Parser.AST 
{
	public class ReturnStatement : Statement
	{
		Expression expression;
		
//		public Expression ReturnExpression {
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
		
		public ReturnStatement(Expression expression)
		{
			this.expression = expression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ReturnStatement: Expression={0}]", 
			                     expression);
		}
	}
}
