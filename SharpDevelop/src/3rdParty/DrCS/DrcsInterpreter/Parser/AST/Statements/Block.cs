using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Block.
	/// </summary>
	public class Block : Statement {
		//public readonly Block     Parent;
		//public readonly bool      Implicit;
		//public readonly Location  StartLocation;
		//public Location           EndLocation;

		//
		// The statements in this block
		//
		private ArrayList statements;
		public ArrayList Statements {
			get {
				return statements;
			}
		}

		public Block( ArrayList stmts, Location l ) {
			statements = stmts;
			loc = l;
		}
		//
		// An array of Blocks.  We keep track of children just
		// to generate the local variable declarations.
		//
		// Statements and child statements are handled through the
		// statements.
		//
		//ArrayList children;
		
		//
		// Labels.  (label, block) pairs.
		//
		//Hashtable labels;

		//
		// Keeps track of (name, type) pairs
		//
		//Hashtable variables;

		//
		// Keeps track of constants
		//Hashtable constants;

		//
		// Maps variable names to ILGenerator.LocalBuilders
		//
		//Hashtable local_builders;

		//bool used = false;

		//static int id;

		//int this_id;
		/*
		public Block (Block parent)
			: this (parent, false, Location.Null, Location.Null) {
		  }

		public Block (Block parent, bool implicit_block)
			: this (parent, implicit_block, Location.Null, Location.Null) {
		  }

		public Block (Block parent, Location start, Location end)
			: this (parent, false, start, end) {
		  }

		public Block (Block parent, bool implicit_block, Location start, Location end) {
			if (parent != null)
				parent.AddChild (this);
			
			this.Parent = parent;
			this.Implicit = implicit_block;
			this.StartLocation = start;
			this.EndLocation = end;
			this.loc = start;
			this_id = id++;
			statements = new ArrayList ();
			
		}

		public int ID {
			get {
				return this_id;
			}
		}
		
		void AddChild (Block b) {
			if (children == null)
				children = new ArrayList ();
			
			children.Add (b);
		}

		public void SetEndLocation (Location loc) {
			EndLocation = loc;
		}

		/// <summary>
		///   Adds a label to the current block. 
		/// </summary>
		///
		/// <returns>
		///   false if the name already exists in this block. true
		///   otherwise.
		/// </returns>
		///
		public bool AddLabel (string name, LabeledStatement target) {
			if (labels == null)
				labels = new Hashtable ();
			if (labels.Contains (name))
				return false;
			
			labels.Add (name, target);
			return true;
		}

		public LabeledStatement LookupLabel (string name) {
			if (labels != null){
				if (labels.Contains (name))
					return ((LabeledStatement) labels [name]);
			}

			if (Parent != null)
				return Parent.LookupLabel (name);

			return null;
		}

		public VariableInfo AddVariable (string type, string name, Parameters pars, Location l) {
			if (variables == null)
				variables = new Hashtable ();

			if (GetVariableType (name) != null)
				return null;

			if (pars != null) {
				int idx = 0;
				Parameter p = pars.GetParameterByName (name, out idx);
				if (p != null) 
					return null;
			}
			
			VariableInfo vi = new VariableInfo (type, l);

			variables.Add (name, vi);

			// Console.WriteLine ("Adding {0} to {1}", name, ID);
			return vi;
		}

		public bool AddConstant (string type, string name, Expression value, Parameters pars, Location l) {
			if (AddVariable (type, name, pars, l) == null)
				return false;
			
			if (constants == null)
				constants = new Hashtable ();

			constants.Add (name, value);
			return true;
		}

		public Hashtable Variables {
			get {
				return variables;
			}
		}

		public VariableInfo GetVariableInfo (string name) {
			if (variables != null) {
				object temp;
				temp = variables [name];

				if (temp != null){
					return (VariableInfo) temp;
				}
			}

			if (Parent != null)
				return Parent.GetVariableInfo (name);

			return null;
		}
		
		public string GetVariableType (string name) {
			VariableInfo vi = GetVariableInfo (name);

			if (vi != null)
				return vi.Type;

			return null;
		}

		public Expression GetConstantExpression (string name) {
			if (constants != null) {
				object temp;
				temp = constants [name];
				
				if (temp != null)
					return (Expression) temp;
			}
			
			if (Parent != null)
				return Parent.GetConstantExpression (name);

			return null;
		}
		
		/// <summary>
		///   True if the variable named @name has been defined
		///   in this block
		/// </summary>
		public bool IsVariableDefined (string name) {
			// Console.WriteLine ("Looking up {0} in {1}", name, ID);
			if (variables != null) {
				if (variables.Contains (name))
					return true;
			}
			
			if (Parent != null)
				return Parent.IsVariableDefined (name);

			return false;
		}

		/// <summary>
		///   True if the variable named @name is a constant
		///  </summary>
		public bool IsConstant (string name) {
			Expression e = null;
			
			e = GetConstantExpression (name);
			
			return e != null;
		}
		
		/// <summary>
		///   Use to fetch the statement associated with this label
		/// </summary>
		public Statement this [string name] {
			get {
				return (Statement) labels [name];
			}
		}

		/// <returns>
		///   A list of labels that were not used within this block
		/// </returns>
		public string [] GetUnreferenced () {
			// FIXME: Implement me
			return null;
		}

		public void AddStatement (Statement s) {
			statements.Add (s);
			used = true;
		}

		public bool Used {
			get {
				return used;
			}
		}

		public void Use () {
			used = true;
		}
		

		public void UsageWarning () {
			string name;
			
			if (variables != null){
				foreach (DictionaryEntry de in variables){
					VariableInfo vi = (VariableInfo) de.Value;
					
					if (vi.Used)
						continue;
					
					name = (string) de.Key;
						
					if (vi.Assigned){
						Report.Warning (
							219, vi.Location, "The variable `" + name +
							"' is assigned but its value is never used");
					} else {
						Report.Warning (
							168, vi.Location, "The variable `" +
							name +
							"' is declared but never used");
					} 
				}
			}

			if (children != null)
				foreach (Block b in children)
					b.UsageWarning ();
		}

		public override bool Resolve (EmitContext ec) {
			Block prev_block = ec.CurrentBlock;

			ec.CurrentBlock = this;
			foreach (Statement s in statements){
				if (s.Resolve (ec) == false){
					ec.CurrentBlock = prev_block;
					return false;
				}
			}

			ec.CurrentBlock = prev_block;
			return true;
		}
		*/
		public override object execute(IStatementVisitor v, object inp) {
			return v.forBlock(this, inp);
		}
	}

}
