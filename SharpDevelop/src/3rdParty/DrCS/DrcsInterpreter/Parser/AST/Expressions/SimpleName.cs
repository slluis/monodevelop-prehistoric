using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for SimpleName.
	/// </summary>
	public class SimpleName : Expression 
	{
		private string name;
		public string Name {
			get {
				return name;
			}
        }
		
		public SimpleName(string n, Location l)
		{
			name = n;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forSimpleName(this, inp);
		}
	}
}
