using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for ReadonlyVariableException.
	/// </summary>
	public class ReadonlyVariableException : InterpreterException {
		private string name;
		public ReadonlyVariableException(string n) : base() {	
			name = n;
		}
 
		public override string ToString() {
			return "Cannot assign to readonly variable: " + name;//base.ToString() + 
		}
	}
}
