using System;
using System.Reflection;
using System.Collections;
using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for LoopVisitor.
	/// </summary>
	public class LoopVisitor : InterpretationVisitor {
		protected InterpretationVisitor iv;

		public LoopVisitor(InterpretationVisitor i) : base() {
			name = "LoopVisitor";
			iv = i;
			this.env = i.Env;
			this.tm = i.Tm;
		}
		
		//		public override object forBreak(Break b, object inp) {
		//			//throw break exception
		//			return null;
		//		}
		//
		//		public override object forContinue(Continue c, object inp) {
		//			//throw continue exception
		//			return null;
		//		}

		public override object forReturn(Return r, object inp) {
			//ReturnException???
			if(r.Expr != null) {
				object ret =  r.Expr.execute(this, inp);
				throw new ReturnException(ret);
			}
			throw new ReturnException();
		}

		public override object forDo(Do d, object inp) {
			//object ret = null;
			//env.NewScope();
			try {
				bool doLoop = true;
				Statement stmt = d.EmbeddedStatement;
				while(doLoop) {	
					//ret = 
					stmt.execute(iv, inp);
					doLoop = testBoolExp(d.Expr, inp);
				}
			}
			catch(BreakException) {
				return null;
			}
			catch(ContinueException) {
				//hmmm
				//check bool statement and re-execute
				if(testBoolExp(d.Expr, inp))
					return d.execute(this, inp);
				else
					return null;
			}
			catch(ReturnException r) {
				return r.RetVal;
			}
			//finally {
			//	env.LeaveScope();
			//}
			return null;
		}

		
		public override object forFor(For f, object inp) {
			//new scope
			env.NewScope();

			try {
				object forInit = f.DeclOrInit;
				if(forInit != null) {
					if(forInit is ArrayList) {
						ArrayList forInitList = (ArrayList)forInit;
						foreach(Expression exp in forInitList) {
							exp.execute(iv, inp);
						}
					}
					else if(forInit is LocalVarDecl) {
						LocalVarDecl varDecl = (LocalVarDecl)forInit;
						varDecl.execute(iv, inp);
					}
					else {
						throw new InterpreterException("invalid 'for-initializer'",f.Loc);
					}
				}

				Expression test = f.Test;
				ArrayList forIt = f.ForIterator;
				return forForLoop(f.Statement, inp, test, f.ForIterator);
			}
			finally {
				env.LeaveScope();
			}
			//return null;
		}

		public object forForLoop(Statement stmt, object inp, Expression test, ArrayList forIt) {
			DB.fp("<forForLoop>");
			
			//object ret = null;
			
			try {
				bool doLoop;
				if(test != null) {
					doLoop = testBoolExp(test, inp);
				}
				else {
					doLoop = true;
				}

				//Statement stmt = w.Statement;
				while(doLoop) {	
					DB.fp("starting loop iteration");

					//ret = 
					stmt.execute(iv, inp);
					
					//for-iterator
					if(forIt != null) {
						DB.fp("executing for-iterator");
						foreach(Expression exp in forIt) {
							exp.execute(iv, inp);
						}
					}

					//test-condition
					if(test != null) {
						DB.fp("checking test-condition");
						doLoop = testBoolExp(test, inp);
					}
					else {
						doLoop = true;
					}
				}
			}
			catch(BreakException) {
				DB.fp("caught break exception");
				return null;
			}
			catch(ContinueException) {
				DB.fp("caught continue exception");
				//hmmm
				//check bool statement and re-execute
				
				//for-iterator
				if(forIt != null) {
					DB.fp("executing for-iterator");
					foreach(Expression exp in forIt) {
						exp.execute(iv, inp);
					}
				}

				return forForLoop(stmt, inp, test, forIt);
			}
			catch(ReturnException r) {
				return r.RetVal;
			}
			return null;
		}

		public override object forForeach(Foreach f, object inp) {
			Type itType = processCType(f.WantedType);
			string localVar = f.LocalVariable;

			object collExp = extractVariableValue(f.Expr.execute(iv, inp));
			if(collExp == null) {
				throw new NullReferenceException("enumerator in foreach statement cannot be null");
			}
			Type collType = collExp.GetType();
			object enumeration = null;
			try {
				enumeration = collType.InvokeMember("GetEnumerator",BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, collExp, null);
			}
			catch(MissingMethodException) {
				throw new InterpreterException("invalid collection type in foreach statement", f.Loc);
			}
			if(enumeration == null) {
				throw new NullReferenceException("enumerator in foreach statement cannot be null");
			}
		
			Type enumType = enumeration.GetType();
			

			VariableInfo vi = new VariableInfo(localVar, itType);
			//new scope
			env.NewScope();
			//insert as variable in env
			env.AddVariable(vi);
			try {
				return forForeachLoop(vi, f.Statement, enumType, enumeration, inp);
			}
			finally {
				env.LeaveScope();
			}
		}

		public object forForeachLoop(VariableInfo vi, Statement stmt, Type enumType, object enumeration, object inp) {
			bool hasNext = tryMoveNext(enumType, enumeration);
			try{ 
				while(hasNext) {
					object current = null;
					try {
						vi.ReadOnly = false;
						current = enumType.InvokeMember("Current", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, enumeration, null);
						vi.Value = current;
						//make sure it's readonly
						vi.ReadOnly = true;
						
						stmt.execute(iv, inp);
					}
					catch(MissingFieldException) {
						throw new InterpreterException("error enumerating collection in foreach statement");
					}					
					hasNext = tryMoveNext(enumType, enumeration);
				}
			}
			catch(BreakException) {
				return null;
			}
			catch(ContinueException) {
				return forForeachLoop(vi, stmt, enumType, enumeration, inp);
			}
			catch(ReturnException r) {
				return r.RetVal;
			}
			return null;
		}

		public bool tryMoveNext(Type enumType, object enumeration) {
			try {
				object hasNext = enumType.InvokeMember("MoveNext",BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, enumeration, null);
				if(hasNext is bool)
					return (bool)hasNext;
				else 
					throw new Exception();
			}
			catch {
				throw new InterpreterException("error with enumerator in foreach statement");
			}
		}

		public override object forWhile(While w, object inp) {
			//object ret = null;
			
			try {
				bool doLoop = testBoolExp(w.Expr, inp);
				Statement stmt = w.Statement;
				while(doLoop) {	
					//ret = 
					stmt.execute(iv, inp);
					doLoop = testBoolExp(w.Expr, inp);
				}
			}
			catch(BreakException) {
				return null;
			}
			catch(ContinueException) {
				//hmmm
				//check bool statement and re-execute
				return w.execute(this, inp);

			}
			catch(ReturnException r) {
				return r.RetVal;
			}
			return null;
		}

		private bool testBoolExp(Expression exp, object inp) {
			object test = extractVariableValue(exp.execute(iv, inp));
			if(test is bool) {
				bool loop = (bool)test;
				return loop;
			}
			else {
				throw new InterpreterException("test statement loop evaluate to a bool", exp.Loc);
			}
		}

	}
}
