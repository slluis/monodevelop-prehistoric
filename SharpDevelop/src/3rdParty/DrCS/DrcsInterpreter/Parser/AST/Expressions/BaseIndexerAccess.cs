using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for BaseIndexerAcccess.
	/// </summary>
	public class BaseIndexerAccess : Expression
	{
		private ArrayList expList;
		public ArrayList ExpList {
			get {
				return expList;
			}
		}

		public BaseIndexerAccess(ArrayList el, Location l)
		{
			expList = el;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forBaseIndexerAccess(this, inp);
		}

	}
}
