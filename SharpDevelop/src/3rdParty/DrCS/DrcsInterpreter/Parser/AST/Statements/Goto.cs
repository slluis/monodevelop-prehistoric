using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Goto.
	/// </summary>
	public class Goto : Statement {
		public enum GotoType : byte {
			Ident,
			Case,
			Default
		}

		private GotoType gType;
		public GotoType GType {
			get {
				return gType;
			}
		}

		private string ident;
		public string Ident {
			get {
				return ident;
			}
		}

		private Expression constExpr;
		public Expression ConstExpr {
			get{
				return constExpr;
			}
		}

		/*
		public override bool Resolve (EmitContext ec) {
			return true;
		}
		*/

		public Goto (string ident, Location l) {
			this.ident = ident;
			loc = l;
			gType = GotoType.Ident;
		}

		public Goto(Expression exp, Location l) {
			this.constExpr = exp;
			loc = l;
			gType = GotoType.Case;
		}

		public Goto(Location l) {
			loc = l;
			gType = GotoType.Default;
		}

		public override object execute(IStatementVisitor v, object inp) {
			return v.forGoto(this,inp);
		}
	}

}
