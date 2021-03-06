using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class ReturnStatement : Statement
	{
		Expression returnExpression;
		
		public Expression ReturnExpression {
			get {
				return returnExpression;
			}
			set {
				returnExpression = value;
			}
		}
		
		public ReturnStatement(Expression returnExpression)
		{
			this.returnExpression = returnExpression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ReturnStatement: ReturnExpression={0}]", 
			                     returnExpression);
		}
	}
}
