using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Implements the typeof operator
	/// </summary>
	public class TypeOf : Expression {
		private readonly CType queriedType;
		public CType QueriedType {
			get {
				return queriedType;
			}
		}

		
		public TypeOf (CType queried_type, Location l) {
			queriedType = queried_type;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forTypeOf(this, inp);
		}

	}

}
