using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class WithStatement : Statement
	{
		Expression withExpression;
		BlockStatement body;
		
		public Expression WithExpression {
			get {
				return withExpression;
			}
			set {
				withExpression = value;
			}
		}
		
		public WithStatement(Expression withExpression)
		{
			this.withExpression = withExpression;
			this.body = body;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
