using System;

namespace Rice.Drcsharp.Parser
{
	/// <summary>
	///   Keeps track of the location in the program
	/// </summary>
	///
	/// <remarks>
	///   This uses a compact representation and a couple of auxiliary
	///   structures to keep track of tokens to (file,line) mappings.
	///
	///   We could probably also keep track of columns by storing those
	///   in 8 bits (and say, map anything after char 255 to be `255+').
	/// </remarks>
	public class Location {
		#region old_code
		/*
		public int token; 
		static Hashtable map;
		static Hashtable sym_docs;
		static ArrayList list;
		static int global_count;
		static int module_base;

		public readonly static Location Null;
		
		static Location () {
			map = new Hashtable ();
			list = new ArrayList ();
			sym_docs = new Hashtable ();
			global_count = 0;
			module_base = 0;
			Null.token = -1;
		}

		static public void Push (string name) {
			map.Remove (global_count);
			map.Add (global_count, name);
			list.Add (global_count);
			module_base = global_count;
		}
		
		public Location (int row) {
			if (row < 0)
				token = -1;
			else {
				token = module_base + row;
				if (global_count < token)
					global_count = token;
			}
		}

		public override string ToString () {
			return Name + ": (" + Row + ")";
		}
		
		/// <summary>
		///   Whether the Location is Null
		/// </summary>
		static public bool IsNull (Location l) {
			return l.token == -1;
		}

		public string Name {
			get {
				int best = 0;
				
				if (token < 0)
					return "Internal";

				foreach (int b in list){
					if (token > b)
						best = b;
				}
				return (string) map [best];
			}
		}

		public int Row {
			get {
				int best = 0;
				
				if (token < 0)
					return 1;
				
				foreach (int b in list){
					if (token > b)
						best = b;
				}
				return token - best;
			}
		}
		*/
		#endregion

		public static Location Null = new Location(-1, -1);

		private int line;
		public int Line {
			get {
				return line;
			}
		}

		private int column;
		public int Column {
			get {
				return column;
			}
		}

		public Location(int l, int col) {
			line = l;
			column = col;
		}
	
		public override string ToString() {
			return "<location line: " + line + ", col: " + column + ">";
		}
	}
}
