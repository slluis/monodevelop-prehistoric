using System;
using System.Collections;
using Rice.Drcsharp.Parser.AST;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;


namespace Rice.Drcsharp.Parser.AST.Statements
{
	/// <summary>
	/// Summary description for Statement.
	/// </summary>
	public abstract class Statement : ASTNode {
		protected Location loc;
		public Location Loc {
			get {
				return loc;
			}
		}
		
		/*
		///
		/// Resolves the statement, true means that all sub-statements
		/// did resolve ok.
		//
		public virtual bool Resolve (EmitContext ec) {
			return true;
		}
		*/
		// <summary>
		// Return value indicates whether all code paths emitted return.
		// </summary>
		//public abstract bool Emit (EmitContext ec);
		/*
		public static Expression ResolveBoolean (EmitContext ec, Expression e, Location loc) {
			e = e.Resolve (ec);
			if (e == null)
				return null;
			
			if (e.Type != TypeManager.bool_type){
				e = Expression.ConvertImplicit (ec, e, TypeManager.bool_type,
					new Location (-1));
			}

			if (e == null){
				Report.Error (
					31, loc, "Can not convert the expression to a boolean");
			}

			if (CodeGen.SymbolWriter != null)
				ec.Mark (loc);

			return e;
		}
		
		/// <remarks>
		///    Emits a bool expression.
		/// </remarks>
		public static void EmitBoolExpression (EmitContext ec, Expression bool_expr,
			Label target, bool isTrue) {
			ILGenerator ig = ec.ig;
			
			bool invert = false;
			if (bool_expr is Unary){
				Unary u = (Unary) bool_expr;
				
				if (u.Oper == Unary.Operator.LogicalNot){
					invert = true;

					u.EmitLogicalNot (ec);
				}
			} 

			if (!invert)
				bool_expr.Emit (ec);

			if (isTrue){
				if (invert)
					ig.Emit (OpCodes.Brfalse, target);
				else
					ig.Emit (OpCodes.Brtrue, target);
			} else {
				if (invert)
					ig.Emit (OpCodes.Brtrue, target);
				else
					ig.Emit (OpCodes.Brfalse, target);
			}
		}

		public static void Warning_DeadCodeFound (Location loc) {
			Report.Warning (162, loc, "Unreachable code detected");
		}
*/
		public abstract object execute(IStatementVisitor v, object input);

		public override object execute(IASTVisitor v, object inp) {
			return this.execute((IStatementVisitor)v, inp);
		}
	}

}
