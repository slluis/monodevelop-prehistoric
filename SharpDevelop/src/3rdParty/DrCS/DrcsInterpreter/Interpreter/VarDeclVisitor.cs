using System;
using System.Reflection;
using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Adds a variable (with assignment) to the environment,
	/// called from a VariableDeclaration's <code>LocalConstDecl</code>
	/// or <code>LocalVarDecl</code> array of declarations
	/// </summary>
	public class VarDeclVisitor : InterpretationVisitor {
		protected InterpretationVisitor iv;

		public VarDeclVisitor(InterpretationVisitor i) {
			name = "VarDeclVisitor";
			iv = i;
			env = i.Env;
			tm = i.Tm;
		}

		/// <summary>
		/// Overridden branch corresponding to the assignment 
		/// </summary>
		/// <param name="a">the Assignment object</param>
		/// <param name="inp">the Type of the assigned variable</param>
		/// <returns>a VariableInfo object that was (attempted) to be
		/// placed in the environment, representing the variable involved in
		/// the assignment and its value</returns>
		public override object forAssignment(Assignment a, object inp) {
			Expression lVal = a.LExpr;
			SimpleName lValObj = null;
			Type type = (Type)inp;
			if(lVal is SimpleName) {
				lValObj = (SimpleName) lVal;
			}
			else {
				throw new InterpreterException("variable names must be an identifier");
			}
			object rValRes = a.RExpr.execute(this, inp);
			
			//must be VariableInfo
			VariableInfo lValRes = new VariableInfo(lValObj.Name, type, rValRes);
			
			//note, we are not doing any kind of type checking here!!!
			//make assignment in env
			env.AddVariable(lValRes);

			return rValRes;
		}

	}
}
