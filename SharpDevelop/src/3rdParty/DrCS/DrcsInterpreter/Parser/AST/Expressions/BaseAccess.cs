using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for BaseAccess.
	/// </summary>
	public class BaseAccess : Expression
	{
		private string ident;
		public string Ident {
			get {
				return ident;
			}
		}

		public BaseAccess(string id, Location l)
		{
			ident = id;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forBaseAccess(this, inp);
		}
	}
}
