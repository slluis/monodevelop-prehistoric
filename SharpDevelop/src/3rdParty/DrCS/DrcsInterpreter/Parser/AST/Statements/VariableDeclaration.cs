using System;

namespace Rice.Drcsharp.Parser.AST.Statements {
	
	// <summary>
	//   A class used to pass around variable declarations and constants
	// </summary>
	public class VariableDeclaration {
		private string ident;
		public string Ident {
			get {
				return ident;
			}
		}

		private object expression_or_array_initializer;
		public object ExpressionOrArrayInit {
			get {
				return this.expression_or_array_initializer;
			}
		}

		private Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}

		public VariableDeclaration (string id, object eoai, Location l){
			this.ident = id;
			this.expression_or_array_initializer = eoai;
			this.loc = l;
		}
	}
}

