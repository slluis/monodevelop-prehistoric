using System;
using Rice.Drcsharp.Parser.AST.Visitors;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	///   Unary expressions.  
	/// </summary>
	public class Unary : Expression {
		public enum UnaryOperator {
			UnaryPlus, 
			UnaryNegation, 
			LogicalNot, 
			OnesComplement,
			Indirection, 
			AddressOf,  
			TOP
		}

		private UnaryOperator oper;
		public UnaryOperator Oper {
			get {
				return oper;
			}
		}

		private Expression expr;
		public Expression Expr {
			get {
				return expr;
			}
		}

		public Unary (UnaryOperator op, Expression expr, Location l) {
			this.oper = op;
			this.expr = expr;
			loc = l;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forUnary(this, inp);
		}

		/// <summary>
		///   Returns a stringified representation of the UnaryOperator
		/// </summary>
		static public string OperName (UnaryOperator oper) {
			switch (oper){
				case UnaryOperator.UnaryPlus:
					return "+";
				case UnaryOperator.UnaryNegation:
					return "-";
				case UnaryOperator.LogicalNot:
					return "!";
				case UnaryOperator.OnesComplement:
					return "~";
				case UnaryOperator.AddressOf:
					return "&";
				case UnaryOperator.Indirection:
					return "*";
			}

			return oper.ToString ();
		}

		static string [] oper_names;

		static Unary () {
			oper_names = new string [(int)UnaryOperator.TOP];

			oper_names [(int) UnaryOperator.UnaryPlus] = "op_UnaryPlus";
			oper_names [(int) UnaryOperator.UnaryNegation] = "op_UnaryNegation";
			oper_names [(int) UnaryOperator.LogicalNot] = "op_LogicalNot";
			oper_names [(int) UnaryOperator.OnesComplement] = "op_OnesComplement";
			oper_names [(int) UnaryOperator.Indirection] = "op_Indirection";
			oper_names [(int) UnaryOperator.AddressOf] = "op_AddressOf";
		}

		/*
		void Error23 (Type t) {
			Report.Error (
				23, loc, "Operator " + OperName (Oper) +
				" cannot be applied to operand of type `" +
				TypeManager.CSharpName (t) + "'");
		}

		/// <remarks>
		///   The result has been already resolved:
		///
		///   FIXME: a minus constant -128 sbyte cant be turned into a
		///   constant byte.
		/// </remarks>
		static Expression TryReduceNegative (Expression expr) {
			Expression e = null;
			
			if (expr is IntConstant)
				e = new IntConstant (-((IntConstant) expr).Value);
			else if (expr is UIntConstant){
				uint value = ((UIntConstant) expr).Value;

				if (value < 2147483649)
					return new IntConstant (-(int)value);
				else
					e = new LongConstant (value);
			}
			else if (expr is LongConstant)
				e = new LongConstant (-((LongConstant) expr).Value);
			else if (expr is ULongConstant){
				ulong value = ((ULongConstant) expr).Value;

				if (value < 9223372036854775809)
					return new LongConstant(-(long)value);
			}
			else if (expr is FloatConstant)
				e = new FloatConstant (-((FloatConstant) expr).Value);
			else if (expr is DoubleConstant)
				e = new DoubleConstant (-((DoubleConstant) expr).Value);
			else if (expr is DecimalConstant)
				e = new DecimalConstant (-((DecimalConstant) expr).Value);
			else if (expr is ShortConstant)
				e = new IntConstant (-((ShortConstant) expr).Value);
			else if (expr is UShortConstant)
				e = new IntConstant (-((UShortConstant) expr).Value);
			return e;
		}
		
		Expression Reduce (EmitContext ec, Expression e) {
			Type expr_type = e.Type;
			
			switch (Oper){
				case Operator.UnaryPlus:
					return e;
				
				case Operator.UnaryNegation:
					return TryReduceNegative (e);
				
				case Operator.LogicalNot:
					if (expr_type != TypeManager.bool_type) {
						Error23 (expr_type);
						return null;
					}
				
					BoolConstant b = (BoolConstant) e;
					return new BoolConstant (!(b.Value));
				
				case Operator.OnesComplement:
					if (!((expr_type == TypeManager.int32_type) ||
						(expr_type == TypeManager.uint32_type) ||
						(expr_type == TypeManager.int64_type) ||
						(expr_type == TypeManager.uint64_type) ||
						(expr_type.IsSubclassOf (TypeManager.enum_type)))){
						Error23 (expr_type);
						return null;
					}

					if (e is EnumConstant){
						EnumConstant enum_constant = (EnumConstant) e;
					
						Expression reduced = Reduce (ec, enum_constant.Child);

						return new EnumConstant ((Constant) reduced, enum_constant.Type);
					}

					if (expr_type == TypeManager.int32_type)
						return new IntConstant (~ ((IntConstant) e).Value);
					if (expr_type == TypeManager.uint32_type)
						return new UIntConstant (~ ((UIntConstant) e).Value);
					if (expr_type == TypeManager.int64_type)
						return new LongConstant (~ ((LongConstant) e).Value);
					if (expr_type == TypeManager.uint64_type)
						return new ULongConstant (~ ((ULongConstant) e).Value);

					Error23 (expr_type);
					return null;
			}
			throw new Exception ("Can not constant fold");
		}

		Expression ResolveOperator (EmitContext ec) {
			Type expr_type = Expr.Type;

			//
			// Step 1: Perform Operator Overload location
			//
			Expression mg;
			string op_name;
			
			op_name = oper_names [(int) Oper];

			mg = MemberLookup (ec, expr_type, op_name, MemberTypes.Method, AllBindingFlags, loc);
			
			if (mg != null) {
				Expression e = StaticCallExpr.MakeSimpleCall (
					ec, (MethodGroupExpr) mg, Expr, loc);

				if (e == null){
					Error23 (expr_type);
					return null;
				}
				
				return e;
			}

			// Only perform numeric promotions on:
			// +, - 

			if (expr_type == null)
				return null;
			
			//
			// Step 2: Default operations on CLI native types.
			//
			if (Expr is Constant)
				return Reduce (ec, Expr);

			if (Oper == Operator.LogicalNot){
				if (expr_type != TypeManager.bool_type) {
					Error23 (Expr.Type);
					return null;
				}
				
				type = TypeManager.bool_type;
				return this;
			}

			if (Oper == Operator.OnesComplement) {
				if (!((expr_type == TypeManager.int32_type) ||
					(expr_type == TypeManager.uint32_type) ||
					(expr_type == TypeManager.int64_type) ||
					(expr_type == TypeManager.uint64_type) ||
					(expr_type.IsSubclassOf (TypeManager.enum_type)))){
					Expression e;

					e = ConvertImplicit (ec, Expr, TypeManager.int32_type, loc);
					if (e != null){
						type = TypeManager.int32_type;
						return this;
					}
					e = ConvertImplicit (ec, Expr, TypeManager.uint32_type, loc);
					if (e != null){
						type = TypeManager.uint32_type;
						return this;
					}
					e = ConvertImplicit (ec, Expr, TypeManager.int64_type, loc);
					if (e != null){
						type = TypeManager.int64_type;
						return this;
					}
					e = ConvertImplicit (ec, Expr, TypeManager.uint64_type, loc);
					if (e != null){
						type = TypeManager.uint64_type;
						return this;
					}
					Error23 (expr_type);
					return null;
				}
				type = expr_type;
				return this;
			}

			if (Oper == Operator.UnaryPlus) {
				//
				// A plus in front of something is just a no-op, so return the child.
				//
				return Expr;
			}

			//
			// Deals with -literals
			// int     operator- (int x)
			// long    operator- (long x)
			// float   operator- (float f)
			// double  operator- (double d)
			// decimal operator- (decimal d)
			//
			if (Oper == Operator.UnaryNegation){
				Expression e = null;

				//
				// transform - - expr into expr
				//
				if (Expr is Unary){
					Unary unary = (Unary) Expr;

					if (unary.Oper == Operator.UnaryNegation)
						return unary.Expr;
				}

				//
				// perform numeric promotions to int,
				// long, double.
				//
				//
				// The following is inneficient, because we call
				// ConvertImplicit too many times.
				//
				// It is also not clear if we should convert to Float
				// or Double initially.
				//
				if (expr_type == TypeManager.uint32_type){
					//
					// FIXME: handle exception to this rule that
					// permits the int value -2147483648 (-2^31) to
					// bt wrote as a decimal interger literal
					//
					type = TypeManager.int64_type;
					Expr = ConvertImplicit (ec, Expr, type, loc);
					return this;
				}

				if (expr_type == TypeManager.uint64_type){
					//
					// FIXME: Handle exception of `long value'
					// -92233720368547758087 (-2^63) to be wrote as
					// decimal integer literal.
					//
					Error23 (expr_type);
					return null;
				}

				if (expr_type == TypeManager.float_type){
					type = expr_type;
					return this;
				}
				
				e = ConvertImplicit (ec, Expr, TypeManager.int32_type, loc);
				if (e != null){
					Expr = e;
					type = e.Type;
					return this;
				} 

				e = ConvertImplicit (ec, Expr, TypeManager.int64_type, loc);
				if (e != null){
					Expr = e;
					type = e.Type;
					return this;
				}

				e = ConvertImplicit (ec, Expr, TypeManager.double_type, loc);
				if (e != null){
					Expr = e;
					type = e.Type;
					return this;
				}

				Error23 (expr_type);
				return null;
			}

			if (Oper == Operator.AddressOf){
				if (Expr.eclass != ExprClass.Variable){
					Error (211, loc, "Cannot take the address of non-variables");
					return null;
				}

				if (!ec.InUnsafe) {
					UnsafeError (loc); 
					return null;
				}

				if (!TypeManager.VerifyUnManaged (Expr.Type, loc)){
					return null;
				}
				
				//
				// This construct is needed because dynamic types
				// are not known by Type.GetType, so we have to try then to use
				// ModuleBuilder.GetType.
				//
				string ptr_type_name = Expr.Type.FullName + "*";
				type = Type.GetType (ptr_type_name);
				if (type == null)
					type = CodeGen.ModuleBuilder.GetType (ptr_type_name);
				
				return this;
			}

			if (Oper == Operator.Indirection){
				if (!ec.InUnsafe){
					UnsafeError (loc);
					return null;
				}

				if (!expr_type.IsPointer){
					Report.Error (
						193, loc,
						"The * or -> operator can only be applied to pointers");
					return null;
				}

				//
				// We create an Indirection expression, because
				// it can implement the IMemoryLocation.
				// 
				return new Indirection (Expr);
			}
			
			Error (187, loc, "No such operator '" + OperName (Oper) + "' defined for type '" +
				TypeManager.CSharpName (expr_type) + "'");
			return null;
		}

*/
		public override string ToString () {
			return "Unary (" + Oper + ", " + Expr + ")";
		}
		
	}

}
