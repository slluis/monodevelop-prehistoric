using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Exception thrown when a variable is accessed before initialization
	/// </summary>
	public class UninitializedVariableException : InterpreterException {

		private string name;

		public UninitializedVariableException(string n) : base() {
			name = n;
		}

		public UninitializedVariableException(string n, Location l) : base(l) {
			name = n;
		}

		public override string ToString() {
			return "UninitializedVariableException: '" + name + "' is uninitialized" + Environment.NewLine;
		}
	}
}
