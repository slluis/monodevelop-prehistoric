using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class HandlesClause
	{
		ArrayList eventNames;
		
		public HandlesClause()
		{
			this.eventNames = new ArrayList();
		}
		
		public ArrayList EventNames {
			get {
				return eventNames;
			}
			set {
				eventNames = value;
			}
		}
		
	}
}
