using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for As.
	/// </summary>
	public class As : Binary {

		private CType wantedType;
		public CType Wantedype {
			get {
				return wantedType;
			}
		}

		public As(Expression left, CType t, Location l)
			: base(Binary.BiOperator.As, left, null, l) {
			wantedType = t;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forAs(this, inp);
		}
	}
}
