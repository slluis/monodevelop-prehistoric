using System;
using System.Collections;


namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Container class used to hold an array's meta data in the interpreter.
	/// </summary>
	public class ArrayData {
#region Properties
		/// <summary>
		/// An ArrayList field to keep track of rank and length of each dim
		/// </summary>
		private ArrayList ranks;
		/// <summary>
		/// Ranks property field.
		/// </summary>
		public ArrayList Ranks { 
			get { return ranks; }
			set { ranks = value; }
		}
		
		/// <summary>
		/// A Type field for keep track of the array type.
		/// </summary>
		private Type arrayType;
		/// <summary>
		/// ArrayType property.
		/// </summary>
		public Type ArrayType {
			get { return arrayType; }
			set { arrayType = value; }
		}
		
		/// <summary>
		/// An ArrayData field that keeps track of the next element in the array.
		/// </summary>
		private ArrayData next;
		/// <summary>
		/// Next property.
		/// </summary>
		public ArrayData Next {
			get { return next; }
			set { next = value; }
		}
#endregion

#region Constructor
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ArrayData() {
		}
	#endregion
	}
}
