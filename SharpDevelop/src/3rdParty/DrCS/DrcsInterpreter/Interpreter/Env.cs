using System;
using System.Collections;
using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for Env.
	/// </summary>
	public class Env {
		private Stack envStack;
		private Hashtable currentScope;
		private Hashtable labeledStatements;
		private Hashtable usingAlias;
		private TypeManager tm;

		public Env(TypeManager t) {
			envStack = new Stack();
			currentScope = new Hashtable();
			labeledStatements = new Hashtable();
			usingAlias = new Hashtable();
			tm = t;
		}

		public void AddVariable(string n, Type t) {
			DB.envp("<AddVariable: string, Type>");
			DB.envp("adding var: " + n + ", type == " + t.FullName);

			if(currentScope[n] == null){
				VariableInfo v = new VariableInfo(n, t);
				currentScope[n] = v;
			}
			else {
				throw new AlreadyDeclaredVariableException(n);
			}
		}

		public void AddVariable(string n, Type t, object val) {
			DB.envp("<AddVariable: string, Type, object>");
			DB.envp("adding var: " + n + ", type == " + t.FullName);

			if(currentScope[n] == null) {
				VariableInfo v = new VariableInfo(n,t,val);
				currentScope[n] = v;
			}
			else {
				throw new AlreadyDeclaredVariableException(n);
			}
		}

		public void AddVariable(VariableInfo vi) {
			DB.envp("<AddVariable: VariableInfo>");
			DB.envp("adding var: " + vi.Name + ", type == " + vi.Type.FullName);
			if(currentScope[vi.Name] == null) {
				currentScope[vi.Name] = vi;
			}
			else {
				throw new AlreadyDeclaredVariableException(vi.Name);
			}
			VariableInfo var = (VariableInfo)currentScope[vi.Name];
			DB.envp("var == " + var);
			DB.envp("</AddVariable: VariableInfo>");
		}

		public VariableInfo LookupVariable(string n) {
			DB.envp("<VariableInfo>");
			foreach(VariableInfo vi in currentScope.Values) {
				DB.envp("currentScope contains: " + vi);
			}

			VariableInfo v;
			v = (VariableInfo)currentScope[n];
			if(v != null) {
				DB.envp("found variable: " + n);
				DB.envp("</VariableInfo>");
				return v;
			}
			DB.envp("checking higher scope");
			foreach(Hashtable h in envStack) {
				v = (VariableInfo)h[n];
				if(v != null) {
					DB.envp("found variable: " + n);
					DB.envp("</VariableInfo>");
					return v;
				}
			}

			DB.envp("didn't find: " + n);
			DB.envp("</VariableInfo>");
			throw new UndeclaredVariableException(n);
		}

		public void AssignVariable(string n, object val) {
			VariableInfo vi = LookupVariable(n);
			vi.Value = val;
		}

		public void NewScope() {
			DB.envp("<NewScope>");
			envStack.Push(currentScope);
			currentScope = new Hashtable();
		}

		public void LeaveScope() {
			DB.envp("<LeaveScope>");
			if(envStack.Count == 0)
				throw new InterpreterException("cannot leave top level scope");

			currentScope = (Hashtable)envStack.Pop();
		}

		public void AddLabeledStatement(string label, Statement stmt) {
			labeledStatements[label] = stmt;
		}

		/// <summary>
		/// Retrieves a given labeled statement if it exists in the environment
		/// </summary>
		/// <param name="label">the label of the statement</param>
		/// <returns>a Statement</returns>
		public Statement GetLabeledStatement(string label) {
			return (Statement)labeledStatements[label];
		}

		public void AddUsingAlias(string alias, string name) {
			if(usingAlias[alias] == null) { //changed from !=
				try {
					Type t = tm.LookupType(name); //either type or namespace
					usingAlias[alias] = new TypeInfo(t);
				}
				catch(UndeclaredVariableException) {
					if(tm.IsValidNamespace(name)) {
						usingAlias[alias] = new NamespaceInfo(name);
					}
					else {
						throw new MissingTypeOrNamespaceException(name);
					}
				}
			}
			else {
				throw new InterpreterException("using alias '" + alias + "' already declared");
			}
		}

		public Info GetUsingAlias(string alias) {
			return (Info)usingAlias[alias];
		}

		//		public Type LookupVariableType(string n) {
		//			return (Type)((VariableInfo)lookupTable[n]).Type;
		//		}
		//		
		//		public VariableInfo LookupVaraibleInfo(string n) {
		//			return (VariableInfo)lookupTable[n];
		//		}
	}
}
