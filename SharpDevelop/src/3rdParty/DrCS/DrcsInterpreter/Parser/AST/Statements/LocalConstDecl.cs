using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for ConstLocalVarDecl.
	/// </summary>
	public class LocalConstDecl : Statement
	{
		private CType wantedType;
		public CType WantedType {
			get {
				return wantedType;
			}
		}

		private ArrayList varDecls;
		public ArrayList VarDecls {
			get {
				return varDecls;
			}
		}

		public LocalConstDecl(CType t, ArrayList varD, Location l) {
			wantedType = t;
			varDecls = varD;
			loc = l;
		}

		public override object execute(IStatementVisitor v, object inp) {
			return v.forLocalConstDecl(this, inp);
		}
	}
}
