using System;
using Rice.Drcsharp.Parser;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for MissingTypeOrNamespaceException.
	/// </summary>
	public class MissingTypeOrNamespaceException : InterpreterException {

		private string name;
		public MissingTypeOrNamespaceException() : base() {}
		public MissingTypeOrNamespaceException(string s) : base() {
			name = s;
		}

		public MissingTypeOrNamespaceException(string s, Location l) : base(l) {
			name = s;
		}

		public override string ToString() {
			return "The type or namesapce name '" + name + 
				"' could not be found (are you missing a using directive or an assembly reference?)" +
				Environment.NewLine;// + base.ToString();
		}
	}
}
