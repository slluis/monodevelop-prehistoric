using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for Is.
	/// </summary>
	public class Is : Binary 
	{
		private CType wantedType;
		public CType WantedType {
			get {
				return wantedType;
			}
		}

		public Is(Expression left, CType t, Location l) : base(Binary.BiOperator.Is, left, null, l)
		{
			wantedType = t;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forIs(this, inp);
		}
	}
}
