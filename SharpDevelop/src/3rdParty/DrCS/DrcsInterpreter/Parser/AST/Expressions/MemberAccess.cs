using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for MemberAccess.
	/// </summary>
	public class MemberAccess : Expression
	{
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		private string ident;
		public string Ident {
			get { 
				return ident;
			}
		}
		
		/*
		private PrimTypes.PrimType primType;
		public PrimTypes.PrimType PrimType {
			get {
				return primType;
			}
		}
*/
//		private SimpleName primName;
//		public SimpleName PrimName {
//			get {
//				return primName;
//			}
//		}

		public MemberAccess(Expression expr, string ident, Location l) {
			this.expr = expr;
			loc = l;
			this.ident = ident;
		}

//		public MemberAccess(SimpleName sn, string ident, Location l) {
//			this.primName = sn;
//			this.ident = ident;
//			loc = l;
//		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forMemberAccess(this, inp);
		}
	}
}
