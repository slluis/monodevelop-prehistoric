using System;
using System.IO;
using System.Reflection;
using Rice.Drcsharp.Parser;
using Rice.Drcsharp.Parser.AST;

namespace Rice.Drcsharp.Interpreter {
	
	/// <summary>
	/// Summary description for InterpreterProxy.
	/// </summary>
	/// <remarks>How to use this class:
	///  make one copy.
	///  set UserAssemblyPath
	///  set stdout, stdin, stderr
	///  call Reset() when user clicks to recompile
	///  LoadUserAssembly()
	///  call Interpret()
	///  
	///  </remarks>
	public class InterpreterProxy : MarshalByRefObject, IInterpreter {
		
		protected TypeManager tm;
		protected InterpretationVisitor iv;
		
		private string userAssemDir;

		public InterpreterProxy() {
			Reinitialize();
		}

		public void Reinitialize() {
			tm = new TypeManager();
			iv = new InterpretationVisitor(tm);
		}

		public void Reinitialize(string userDir) {
			Reinitialize();
			userAssemDir = userDir;
		}

		public string Interpret(string s) {
			Debug("<Interpret(string)>");
			try {
				CSharpParser parser = new CSharpParser("C#Parser", new StringReader(s));
				Debug("about to parse");
				parser.parse();

				ASTNode retVal = parser.retVal;
			
				if(parser.retStatement) {
					Debug("parsed statment of type: " + retVal.GetType());
				} else if(parser.retUsingDirective) {
					Debug("got using directive: " + retVal.GetType());
				} else if(parser.retExpression) {
					Debug("got expression: " + retVal.GetType());
				}

				object result;
				//execute Visitor
				result = retVal.execute(iv, null);
				
				string retString;
				if(result != null) {
					if(result is VariableInfo) {
						object o = ((VariableInfo)result).Value;
						if(o == null) 
							retString = "null";
						else 
							retString = "" + o;
					} else {
						retString = "" + result;
					}
				} else
					retString = "";

				Debug("retString == " + retString);
				Debug("</Interpret(string)>");
				return retString;
			} catch(ParserException p) {
				Debug("Got ParseException: " + p);
				Debug("</Interpret(string)>");
				return "Parse Exception: " + p.Message;
			} catch(InterpreterException i) {
				Debug("Got InterpreterException: " + i);
				Debug("</Interpret(string)>");
				return "Interpreter Exception: " + i.Message;
			} catch(Exception e) {
				Debug("EXCEPTION CAUGHT!!!");
				Debug(""+e);	 
				Debug("</Interpret(string)>");
				return "INTERNAL EXCEPTION: " + e.Message;
			}
		}

		
		public void SetUserAssemblyPath(string s) {
			tm.UserAssemblyDir = s;	
		}
		
		public void LoadUserAssembly(string assemName) {
			tm.LoadUserAssembly(assemName);
		}

		public void LoadUserAssemblyFullPath(string assemPath) {
			Debug("<LoadUserAssemblyFullPath>");
			Debug("assemPath = " + assemPath);
			
			Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
			
			Debug("userDomain == " + AppDomain.CurrentDomain);
			Debug("---------------------------");
			Debug("loaded[] == ");
			foreach(Assembly ass in loaded) {
				Debug("assembly == " + ass);
				
			}
			Debug("---------------------------");
			tm.LoadUserAssemblyFullPath(assemPath);
			Debug("</LoadUserAssemblyFullPath>");
		}
		
		public void LoadSystemAssembly(string assemName) {
			tm.LoadSystemAssembly(assemName);

		}

		public void AddUsedNamespace(string ns) {
			tm.AddUsedNamespace(ns);
		}

		public void SetStdOut(TextWriter t) {
			if(!DB.toStdOut) {
				DB.output = t;
			}
			Console.SetOut(t);
		}

		public void SetStdErr(TextWriter t) {
			Console.SetError(t);
		}
		
		public void SetStdIn(TextReader t) {
			Console.SetIn(t);
		}

		private void Debug(string s) {
			DB.ip(s);
		}
	
	}
}
