using System;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Exception thrown when an unsupported operation is attempted in Dr.
	/// C#
	/// </summary>
	public class UnsupportedException : Exception {
		public UnsupportedException() : base() {}
		public UnsupportedException(string msg) : base(msg) {}
		public override string ToString() {
			return "The attemped operation in Dr. C# is unsupported";
		}
	}
}
