using System;
using System.Collections;

namespace MonoDevelop.CsVbRefactory.Parser.AST 
{
	public class BaseReferenceExpression : Expression
	{
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[BaseReferenceExpression]");
		}
	}
}