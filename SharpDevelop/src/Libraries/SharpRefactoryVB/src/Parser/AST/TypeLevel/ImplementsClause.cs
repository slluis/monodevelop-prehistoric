using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class ImplementsClause
	{
		ArrayList baseMembers;
		
		public ImplementsClause()
		{
			this.baseMembers = new ArrayList();
		}
		
		public ArrayList BaseMembers
		{
			get {
				return baseMembers;
			}
			set {
				baseMembers = value;
			}
		}
	}
}
