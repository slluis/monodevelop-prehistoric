using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST
{
	/// <summary>
	/// Summary description for ASTNode.
	/// </summary>
	public abstract class ASTNode
	{		
		public abstract object execute(IASTVisitor v, object inp);

		public override string ToString() {
			return ToStringVisitor.ASTToString(this);
		}
	}
}
