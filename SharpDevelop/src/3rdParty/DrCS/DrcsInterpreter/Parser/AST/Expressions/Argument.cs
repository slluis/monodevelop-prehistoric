using System;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Used for arguments to New(), Invocation()
	/// </summary>
	public class Argument {
		public enum ArgType : byte {
			Expression,
			Ref,
			Out
		};

		private readonly ArgType aType;
		
		public ArgType AType {
			get {
				return aType;
			}
		}
        		
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}
		
		public Argument (Expression expr, ArgType type) {
			this.expr = expr;
			this.aType = type;
		}
	
	}

}
