using System;
using System.Reflection;
using System.Collections;
using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for SwitchVisitor.
	/// </summary>
	public class SwitchVisitor : InterpretationVisitor {
		protected InterpretationVisitor iv;

		protected Hashtable caseHash;
		protected SwitchSection defaultSection;

		public SwitchVisitor(InterpretationVisitor i) {
			iv = i;
			env = i.Env;
			tm = i.Tm;
			caseHash = new Hashtable();
			defaultSection = null;
		}



		public override object forGoto(Goto g, object inp) {
			Goto.GotoType gType = g.GType;
			switch(gType) {
				case Goto.GotoType.Case:
					object ident = extractVariableValue(g.ConstExpr.execute(iv, inp));
					SwitchSection ss = (SwitchSection)caseHash[ident];
					if(ss == null) {
						throw new InterpreterException("no such case '" + ident + "' in switch statement", g.Loc);
					}
					else {
						return executeStatementList(ss.StatementList, inp);
					}
				case Goto.GotoType.Default:
					if(defaultSection != null) {
						return executeStatementList(defaultSection.StatementList, inp);
					}
					else
						throw new InterpreterException("There is no default case to goto",g.Loc);
				case Goto.GotoType.Ident:
					throw new UnsupportedException("'goto identifier' is not supported");
				default:
					throw new InterpreterException("unknown goto type", g.Loc);
			}
		}

		public override object forSwitch(Switch s, object inp) {
			DB.sp("<forSwitch>");
			//determine governing type
			object sExp = extractVariableValue(s.Expr.execute(this, inp));
			Type sExpType = sExp.GetType();
			DB.sp("expression = " + sExp);

			if(sExpType != TypeManager.bool_type && (sExpType.IsPrimitive || sExpType == TypeManager.string_type || sExpType == TypeManager.enum_type)) {
				ArrayList al = s.Sections;
				//SwitchSection defaultSection = null;
				SwitchSection matchSection = null;
				bool match = false;
				foreach(SwitchSection ss in al) {		
					foreach(SwitchLabel sl in ss.Labels) {
						//what about default???
						if(sl.Expr != null) {
							object label = sl.Expr.execute(this, inp);
							DB.sp("label = " + label);
							caseHash[label] = ss;
							if(sExp.Equals(label)) {
								DB.sp("have match");
								match = true;
								matchSection = ss;
							}
						}
						else {
							DB.sp("found default section");
							defaultSection = ss;
						}
					}
				}
				if(match) {
					return executeStatementList(matchSection.StatementList, inp);
				}
				else if(!match && defaultSection != null) {
					return executeStatementList(defaultSection.StatementList, inp);
				}
				else {
					return null;
				}
			}
			else {
				throw new InterpreterException("invalid governing type for switch statement", s.Loc);
			}
		}

		private object executeStatementList(ArrayList al, object inp) {
			object ret = null;
			try {
				foreach(Statement stmt in al) {
					ret = stmt.execute(this, inp);
				}
			}
			catch(BreakException) {
				return null;
			}
			return ret;
		}
	}
}
