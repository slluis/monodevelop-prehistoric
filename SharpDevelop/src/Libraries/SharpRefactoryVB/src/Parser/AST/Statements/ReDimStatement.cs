using System;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.VB;

namespace ICSharpCode.SharpRefactory.Parser.AST.VB
{
	public class ReDimStatement : Statement
	{
		ArrayList reDimClauses;
		
		public ArrayList ReDimClauses
		{
			get {
				return reDimClauses;
			} set {
				reDimClauses = value;
			}
		}
		
		public ReDimStatement(ArrayList reDimClauses)
		{
			this.reDimClauses = reDimClauses;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
	
	public class ReDimClause
	{
		ArrayList initializers;
		string array;
		
		public ArrayList Initializers
		{
			get {
				return initializers;
			}
			set {
				initializers = value;
			}
		}
		
		public string Array
		{
			get {
				return array;
			}
			set {
				array = value;
			}
		}
		
		public ReDimClause(string array)
		{
			this.array = array;
		}
	}
}
