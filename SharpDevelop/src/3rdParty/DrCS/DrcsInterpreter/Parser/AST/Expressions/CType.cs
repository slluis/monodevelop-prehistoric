using System;

namespace Rice.Drcsharp.Parser.AST.Expressions {
	/// <summary>
	///   Fully resolved expression that evaluates to a type
	/// </summary>
	public class CType {

		private string name;
		public string Name {
			get { return name; }
			set { name = value;	}
		}

		public CType (string t) {
			this.name = t;
		}

		public override string ToString() {
			return name;
		}
	
	}

}
