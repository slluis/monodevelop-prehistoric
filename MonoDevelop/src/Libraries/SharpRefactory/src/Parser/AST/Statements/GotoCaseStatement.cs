using System;
using System.Collections;

namespace MonoDevelop.SharpRefactory.Parser.AST 
{
	public class GotoCaseStatement : Statement
	{
		Expression caseExpression;
		
		/// <value>null == goto default;</value>
		public Expression CaseExpression {
			get {
				return caseExpression;
			}
			set {
				caseExpression = value;
			}
		}
		
		public bool IsDefaultCase {
			get {
				return caseExpression == null;
			}
		}
		
		public GotoCaseStatement(Expression caseExpression)
		{
			this.caseExpression = caseExpression;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
	}
}
