using System;
using Rice.Drcsharp.Parser.AST.Statements;
using Rice.Drcsharp.Parser.AST.Visitors;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Expressions {
	/// <summary>
	/// Summary description for Expression.
	/// </summary>
	public abstract class Expression : ASTNode {
		
		
		protected Location loc;
		public Location Loc {
			get {
				return loc;
			}
			set {
				loc = value;
			}
		}

		
		protected Type type;
		public Type Type {
			get { return type; }
			set { type = value; }
		}

		public override object execute(IASTVisitor v, object inp) {
			return execute((IExpressionVisitor)v, inp);
		}

		public abstract object execute(IExpressionVisitor v, object inp);
	
		#region LValue
		/*
			/// <summary>
			///   Utility wrapper routine for Error, just to beautify the code
			/// </summary>
			static protected void Error (int error, string s) {
				Report.Error (error, s);
			}

			static protected void Error (int error, Location loc, string s) {
				Report.Error (error, loc, s);
			}
		
			/// <summary>
			///   Utility wrapper routine for Warning, just to beautify the code
			/// </summary>
			static protected void Warning (int warning, string s) {
				Report.Warning (warning, s);
			}

			static public void Error_CannotConvertType (Location loc, Type source, Type target) {
				Report.Error (30, loc, "Cannot convert type '" +
					TypeManager.CSharpName (source) + "' to '" +
					TypeManager.CSharpName (target) + "'");
			}

			/// <summary>
			///   Performs semantic analysis on the Expression
			/// </summary>
			///
			/// <remarks>
			///   The Resolve method is invoked to perform the semantic analysis
			///   on the node.
			///
			///   The return value is an expression (it can be the
			///   same expression in some cases) or a new
			///   expression that better represents this node.
			///   
			///   For example, optimizations of Unary (LiteralInt)
			///   would return a new LiteralInt with a negated
			///   value.
			///   
			///   If there is an error during semantic analysis,
			///   then an error should be reported (using Report)
			///   and a null value should be returned.
			///   
			///   There are two side effects expected from calling
			///   Resolve(): the the field variable "eclass" should
			///   be set to any value of the enumeration
			///   `ExprClass' and the type variable should be set
			///   to a valid type (this is the type of the
			///   expression).
			/// </remarks>
			public abstract Expression DoResolve (EmitContext ec);

			public virtual Expression DoResolveLValue (EmitContext ec, Expression right_side) {
				return DoResolve (ec);
			}
		
			/// <summary>
			///   Resolves an expression and performs semantic analysis on it.
			/// </summary>
			///
			/// <remarks>
			///   Currently Resolve wraps DoResolve to perform sanity
			///   checking and assertion checking on what we expect from Resolve.
			/// </remarks>
			public Expression Resolve (EmitContext ec) {
				Expression e = DoResolve (ec);

				if (e != null){

					if (e is SimpleName){
						SimpleName s = (SimpleName) e;

						Report.Error (
							103, s.Location,
							"The name `" + s.Name + "' could not be found in `" +
							ec.DeclSpace.Name + "'");
						return null;
					}
				
					if (e.eclass == ExprClass.Invalid)
						throw new Exception ("Expression " + e.GetType () +
							" ExprClass is Invalid after resolve");

					if (e.eclass != ExprClass.MethodGroup)
						if (e.type == null)
							throw new Exception (
								"Expression " + e.GetType () +
								" did not set its type after Resolve\n" +
								"called from: " + this.GetType ());
				}

				return e;
			}

			/// <summary>
			///   Performs expression resolution and semantic analysis, but
			///   allows SimpleNames to be returned.
			/// </summary>
			///
			/// <remarks>
			///   This is used by MemberAccess to construct long names that can not be
			///   partially resolved (namespace-qualified names for example).
			/// </remarks>
			public Expression ResolveWithSimpleName (EmitContext ec) {
				Expression e;

				if (this is SimpleName)
					e = ((SimpleName) this).DoResolveAllowStatic (ec);
				else 
					e = DoResolve (ec);

				if (e != null){
					if (e is SimpleName)
						return e;

					if (e.eclass == ExprClass.Invalid)
						throw new Exception ("Expression " + e +
							" ExprClass is Invalid after resolve");

					if (e.eclass != ExprClass.MethodGroup)
						if (e.type == null)
							throw new Exception ("Expression " + e +
								" did not set its type after Resolve");
				}

				return e;
			}
		
			/// <summary>
			///   Resolves an expression for LValue assignment
			/// </summary>
			///
			/// <remarks>
			///   Currently ResolveLValue wraps DoResolveLValue to perform sanity
			///   checking and assertion checking on what we expect from Resolve
			/// </remarks>
			public Expression ResolveLValue (EmitContext ec, Expression right_side) {
				Expression e = DoResolveLValue (ec, right_side);

				if (e != null){
					if (e is SimpleName){
						SimpleName s = (SimpleName) e;

						Report.Error (
							103, s.Location,
							"The name `" + s.Name + "' could not be found in `" +
							ec.DeclSpace.Name + "'");
						return null;
					}

					if (e.eclass == ExprClass.Invalid)
						throw new Exception ("Expression " + e +
							" ExprClass is Invalid after resolve");

					if (e.eclass != ExprClass.MethodGroup)
						if (e.type == null)
							throw new Exception ("Expression " + e +
								" did not set its type after Resolve");
				}

				return e;
			}

			/// <summary>
			///   Returns a literalized version of a literal FieldInfo
			/// </summary>
			///
			/// <remarks>
			///   The possible return values are:
			///      IntConstant, UIntConstant
			///      LongLiteral, ULongConstant
			///      FloatConstant, DoubleConstant
			///      StringConstant
			///
			///   The value returned is already resolved.
			/// </remarks>
			public static Constant Constantify (object v, Type t) {
				if (t == TypeManager.int32_type)
					return new IntConstant ((int) v);
				else if (t == TypeManager.uint32_type)
					return new UIntConstant ((uint) v);
				else if (t == TypeManager.int64_type)
					return new LongConstant ((long) v);
				else if (t == TypeManager.uint64_type)
					return new ULongConstant ((ulong) v);
				else if (t == TypeManager.float_type)
					return new FloatConstant ((float) v);
				else if (t == TypeManager.double_type)
					return new DoubleConstant ((double) v);
				else if (t == TypeManager.string_type)
					return new StringConstant ((string) v);
				else if (t == TypeManager.short_type)
					return new ShortConstant ((short)v);
				else if (t == TypeManager.ushort_type)
					return new UShortConstant ((ushort)v);
				else if (t == TypeManager.sbyte_type)
					return new SByteConstant (((sbyte)v));
				else if (t == TypeManager.byte_type)
					return new ByteConstant ((byte)v);
				else if (t == TypeManager.char_type)
					return new CharConstant ((char)v);
				else if (t == TypeManager.bool_type)
					return new BoolConstant ((bool) v);
				else if (TypeManager.IsEnumType (t)){
					Constant e = Constantify (v, v.GetType ());

					return new EnumConstant (e, t);
				} else
					throw new Exception ("Unknown type for constant (" + t +
						"), details: " + v);
			}
	*/
		#endregion

	

		
		//
		// Returns the size of type `t' if known, otherwise, 0
		//
		public static int GetTypeSize (Type t) {
			if (t == TypeManager.int32_type ||
				t == TypeManager.uint32_type ||
				t == TypeManager.float_type)
				return 4;
			else if (t == TypeManager.int64_type ||
				t == TypeManager.uint64_type ||
				t == TypeManager.double_type)
				return 8;
			else if (t == TypeManager.byte_type ||
				t == TypeManager.sbyte_type ||
				t == TypeManager.bool_type) 	
				return 1;
			else if (t == TypeManager.short_type ||
				t == TypeManager.char_type ||
				t == TypeManager.ushort_type)
				return 2;
			else
				return 0;
		}

	}
}