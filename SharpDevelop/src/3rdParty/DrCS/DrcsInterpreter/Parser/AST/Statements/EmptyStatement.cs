using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	public class EmptyStatement : Statement {
		/*
		public override bool Resolve (EmitContext ec) {
			return true;
		}
		*/
		private static EmptyStatement instance;
		public static EmptyStatement Instance {
			get {
				if(instance == null)
					instance = new EmptyStatement();
				return instance;
			}
		}

		private EmptyStatement() {
		}

		public override object execute(IStatementVisitor v, object inp) {
			return v.forEmptyStatement(this, inp);
		}
	}
}
