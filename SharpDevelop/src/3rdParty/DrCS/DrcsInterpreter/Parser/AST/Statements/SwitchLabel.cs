using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for SwitchLabel.
	/// </summary>
	public class SwitchLabel {
		
		private Expression expr;
		
		/// <summary>
		/// constant expression
		/// </summary>
		/// <remarks>if Expr is null, then this is the default case</remarks>
		public Expression Expr {
			get {
				return expr;
			}
		}

		private Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}
					 	
		//
		// if expr == null, then it is the default case.
		//
		public SwitchLabel (Expression expr, Location l) {
			this.expr = expr;
			loc = l;
		}


		
		
	}

}
