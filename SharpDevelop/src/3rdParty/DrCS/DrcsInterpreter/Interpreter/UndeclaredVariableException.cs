using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Exception thrown when a variable is used without being first declared
	/// </summary>
	public class UndeclaredVariableException : InterpreterException {
		private string name;

		public UndeclaredVariableException(string n) : base() {
			name = n;
		}

		public UndeclaredVariableException(string n, Location l) : base(l) {
			name = n;
		}
 
		public override string ToString() {
			return "UndeclaredVariableException: '" + name + "' is undeclared" + Environment.NewLine;
		}
	}
}
