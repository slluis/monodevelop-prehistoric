using System;
using System.Reflection;
using System.Collections;
using Rice.Drcsharp.Interpreter;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for TryVisitor.
	/// </summary>
	public class TryVisitor : InterpretationVisitor {
		protected InterpretationVisitor iv;

		public TryVisitor(InterpretationVisitor i) {
			name = "TryVisitor";
			this.iv = i;
			this.env = i.Env;
			this.tm = i.Tm;
		}

		private Exception exception = null;

		public override object forThrow(Throw t, object inp) {
			if(t.Expr == null && exception != null) {
				throw exception;
			}
			else {
				throw (Exception)t.Expr.execute(iv, inp);
			}
		}

		public override object forTry(Try t, object inp) {
			DB.tp("<forTry>");
			object ret = null;
			try {
				ret = t.Block.execute(iv, inp);
				DB.tp("going to return: " + ret);
				return ret;
			}
			catch(Exception e) {
				DB.tp("exception generated");

				bool foundSpecCatch = false;
				exception = e;
				ArrayList catchList = t.SpecCatchList;
				foreach(Catch c in catchList) {
					CatchArgs ca = c.CatchArgs;
					Type eType = processCType(ca.WantedType);
					DB.tp("spec catch type == " + eType);

					if(e.GetType() == eType || e.GetType().IsSubclassOf(eType)) {
						//add exception to env and execute block.
						foundSpecCatch = true;
						env.NewScope();
						VariableInfo vi = new VariableInfo(ca.Ident, eType, e);
						env.AddVariable(vi);
						ret = null;
						try {
							ret = c.Block.execute(this, inp);
						}
						catch(Exception ex) {
							env.LeaveScope();
							throw ex;
						}
						env.LeaveScope();
						return ret;
					}
				}
				if(!foundSpecCatch) {
					if(t.GeneralCatch != null) {
						Catch c = t.GeneralCatch;
						return c.Block.execute(this, inp);
					}
				}
			}
			finally {
				DB.tp("in finally block");
				//execute finally block
				//cannot have any return value
				if(t.FinallyBlock != null) 
					t.FinallyBlock.execute(iv, inp);
			}
			return null;
		}

	}
}
