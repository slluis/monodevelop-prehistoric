using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Represents the `this' construct
	/// </summary>
	/// <remarks>This cannot be found in the interations window.</remarks>
	public class This : Expression {
	
		
		public This (Location loc) {
			this.loc = loc;
		}


		public override object execute(IExpressionVisitor v, object inp) {
			return v.forThis(this, inp);
		}
	}

}
