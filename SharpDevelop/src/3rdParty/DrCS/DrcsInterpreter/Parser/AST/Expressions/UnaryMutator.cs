using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions {
 /// <summary>
	///   Unary Mutator expressions (pre and post ++ and --)
	/// </summary>
	///
	/// <remarks>
	///   UnaryMutator implements ++ and -- expressions.   It derives from
	///   ExpressionStatement becuase the pre/post increment/decrement
	///   operators can be used in a statement context.
	///
	/// FIXME: Idea, we could split this up in two classes, one simpler
	/// for the common case, and one with the extra fields for more complex
	/// classes (indexers require temporary access;  overloaded require method)
	///
	/// Maybe we should have classes PreIncrement, PostIncrement, PreDecrement,
	/// PostDecrement, that way we could save the `Mode' byte as well.  
	/// </remarks>
	public class UnaryMutator : StatementExpression {
		public enum UMode : byte {
			PreIncrement, PreDecrement, PostIncrement, PostDecrement
		}
		
		private UMode mode;
		public UMode Mode {
			get {
				return mode;
			}
		}
		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}



			
		public UnaryMutator (UMode m, Expression e, Location l) {
			mode = m;
			loc = l;
			expr = e;
		}

		static string OperName (UMode mode) {
			return (mode == UMode.PreIncrement || mode == UMode.PostIncrement) ?
				"++" : "--";
		}

		/*
		/// <summary>
		///   Returns whether an object of type `t' can be incremented
		///   or decremented with add/sub (ie, basically whether we can
		///   use pre-post incr-decr operations on it, but it is not a
		///   System.Decimal, which we require operator overloading to catch)
		/// </summary>
		static bool IsIncrementableNumber (Type t) {
			return (t == TypeManager.sbyte_type) ||
				(t == TypeManager.byte_type) ||
				(t == TypeManager.short_type) ||
				(t == TypeManager.ushort_type) ||
				(t == TypeManager.int32_type) ||
				(t == TypeManager.uint32_type) ||
				(t == TypeManager.int64_type) ||
				(t == TypeManager.uint64_type) ||
				(t == TypeManager.char_type) ||
				(t.IsSubclassOf (TypeManager.enum_type)) ||
				(t == TypeManager.float_type) ||
				(t == TypeManager.double_type) ||
				(t.IsPointer && t != TypeManager.void_ptr_type);
		}

		Expression ResolveOperator (EmitContext ec) {
			Type expr_type = expr.Type;

			//
			// Step 1: Perform UnaryOperator Overload location
			//
			Expression mg;
			string op_name;
			
			if (mode == Mode.PreIncrement || mode == Mode.PostIncrement)
				op_name = "op_Increment";
			else 
				op_name = "op_Decrement";

			mg = MemberLookup (ec, expr_type, op_name, MemberTypes.Method, AllBindingFlags, loc);

			if (mg == null && expr_type.BaseType != null)
				mg = MemberLookup (ec, expr_type.BaseType, op_name,
					MemberTypes.Method, AllBindingFlags, loc);
			
			if (mg != null) {
				method = StaticCallExpr.MakeSimpleCall (
					ec, (MethodGroupExpr) mg, expr, loc);

				type = method.Type;
				return this;
			}

			//
			// The operand of the prefix/postfix increment decrement operators
			// should be an expression that is classified as a variable,
			// a property access or an indexer access
			//
			type = expr_type;
			if (expr.eclass == ExprClass.Variable){
				if (IsIncrementableNumber (expr_type) ||
					expr_type == TypeManager.decimal_type){
					return this;
				}
			} else if (expr.eclass == ExprClass.IndexerAccess){
				IndexerAccess ia = (IndexerAccess) expr;
				
				temp_storage = new LocalTemporary (ec, expr.Type);
				
				expr = ia.ResolveLValue (ec, temp_storage);
				if (expr == null)
					return null;

				return this;
			} else if (expr.eclass == ExprClass.PropertyAccess){
				PropertyExpr pe = (PropertyExpr) expr;

				if (pe.VerifyAssignable ())
					return this;

				return null;
			} else {
				report118 (loc, expr, "variable, indexer or property access");
				return null;
			}

			Error (187, loc, "No such operator '" + OperName (mode) + "' defined for type '" +
				TypeManager.CSharpName (expr_type) + "'");
			return null;
		}

		
*/
	
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forUnaryMutator(this, inp);
		}

	}

}
