using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Implements the sizeof expression
	/// </summary>
	public class SizeOf : Expression {
		private readonly CType queriedType;
		public CType QueriedType {
			get {
				return queriedType;
			}
		}

		public SizeOf (CType queried_type, Location l) {
			this.queriedType = queried_type;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forSizeOf(this, inp);
		}

		/*
		public override Expression DoResolve (EmitContext ec) {
			type_queried = RootContext.LookupType (
				ec.DeclSpace, QueriedType, false, loc);
			if (type_queried == null)
				return null;

			type = TypeManager.int32_type;
			eclass = ExprClass.Value;
			return this;
		}
*/
	}

}
