using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	
	///<summary>Exception thrown when a variable is already declared</summary>
	public class AlreadyDeclaredVariableException : InterpreterException {
		private string name;

		public AlreadyDeclaredVariableException(string n) : base() {
			name = n;
		}

		public AlreadyDeclaredVariableException(string n, Location l) : base(l) {
			name = n;
		}
 
		public override string ToString() {
			return "variable: '" + name + "' is already declared in current scope" + 
				Environment.NewLine;// + base.ToString();
		}
	}
}
