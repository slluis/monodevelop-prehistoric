using System;
using Rice.Drcsharp.Parser.AST.Visitors;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	public class Binary : Expression {
		public enum BiOperator {
			Multiply, Division, 
			Modulus,
			Addition, Subtraction,
			LeftShift, RightShift,
			LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, 
			Equality, Inequality,
			BitwiseAnd,
			BitwiseOr,
			ExclusiveOr,
			LogicalAnd,
			LogicalOr,
			Is, As,
			TOP
		}

		public enum ENumPromotion {
			IntInt,
			UintUint,
			LongLong,
			UlongUlong,
			FloatFloat,
			DoubleDouble,
			DecimalDecimal,
			TOP
		}

		private BiOperator oper;
		public BiOperator Oper {
			get {
				return oper;
			}
			set {
				oper = value;
			}
		}
		Expression left, right;

		public Expression Left {
			get {
				return left;
			}
			set {
				left = value;
			}
		}

		public Expression Right {
			get {
				return right;
			}
			set {
				right = value;
			}
		}

		// This must be kept in sync with BiOperator!!!
		public static readonly string [] Oper_names;

		static Binary () {
			Oper_names = new string [(int) BiOperator.TOP];

			Oper_names [(int) BiOperator.Multiply] = "op_Multiply";
			Oper_names [(int) BiOperator.Division] = "op_Division";
			Oper_names [(int) BiOperator.Modulus] = "op_Modulus";
			Oper_names [(int) BiOperator.Addition] = "op_Addition";
			Oper_names [(int) BiOperator.Subtraction] = "op_Subtraction";
			Oper_names [(int) BiOperator.LeftShift] = "op_LeftShift";
			Oper_names [(int) BiOperator.RightShift] = "op_RightShift";
			Oper_names [(int) BiOperator.LessThan] = "op_LessThan";
			Oper_names [(int) BiOperator.GreaterThan] = "op_GreaterThan";
			Oper_names [(int) BiOperator.LessThanOrEqual] = "op_LessThanOrEqual";
			Oper_names [(int) BiOperator.GreaterThanOrEqual] = "op_GreaterThanOrEqual";
			Oper_names [(int) BiOperator.Equality] = "op_Equality";
			Oper_names [(int) BiOperator.Inequality] = "op_Inequality";
			Oper_names [(int) BiOperator.BitwiseAnd] = "op_BitwiseAnd";
			Oper_names [(int) BiOperator.BitwiseOr] = "op_BitwiseOr";
			Oper_names [(int) BiOperator.ExclusiveOr] = "op_ExclusiveOr";
			Oper_names [(int) BiOperator.LogicalOr] = "op_LogicalOr";
			Oper_names [(int) BiOperator.LogicalAnd] = "op_LogicalAnd";
			Oper_names [(int) BiOperator.Is] = "op_Is";
			Oper_names [(int) BiOperator.As] = "op_As";
		}

		
		public Binary (BiOperator oper, Expression left, Expression right, Location l) {
			this.oper = oper;
			this.left = left;
			this.right = right;
			this.loc = l;
		}

		/// <summary>
		///   Returns a stringified representation of the BiOperator
		/// </summary>
		public static string OperName (BiOperator oper) {
			switch (oper){
				case BiOperator.Multiply:
					return "*";
				case BiOperator.Division:
					return "/";
				case BiOperator.Modulus:
					return "%";
				case BiOperator.Addition:
					return "+";
				case BiOperator.Subtraction:
					return "-";
				case BiOperator.LeftShift:
					return "<<";
				case BiOperator.RightShift:
					return ">>";
				case BiOperator.LessThan:
					return "<";
				case BiOperator.GreaterThan:
					return ">";
				case BiOperator.LessThanOrEqual:
					return "<=";
				case BiOperator.GreaterThanOrEqual:
					return ">=";
				case BiOperator.Equality:
					return "==";
				case BiOperator.Inequality:
					return "!=";
				case BiOperator.BitwiseAnd:
					return "&";
				case BiOperator.BitwiseOr:
					return "|";
				case BiOperator.ExclusiveOr:
					return "^";
				case BiOperator.LogicalOr:
					return "||";
				case BiOperator.LogicalAnd:
					return "&&";
				case BiOperator.Is:
					return "is";
				case BiOperator.As:
					return "as";
			}

			return oper.ToString ();
		}

		
		public override string ToString () {
			return "operator " + OperName (oper) + "(" + left.ToString () + ", " +
				right.ToString () + ")";
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forBinary(this, inp);
		}

		public static object BiArithmetic(BiOperator bo, object x, object y, Location l) {
			DB.bp("<BiAritmetic>");
			if(x == null || y == null)
				throw new InterpreterException("cannot do binary arithmetic on null");

			DB.bp("left type == " + x.GetType().ToString());
			DB.bp("right type == " + y.GetType().ToString());
			DB.bp("operation == " + bo);

			if((!x.GetType().IsPrimitive && y.GetType().IsPrimitive)) {
			
				throw new InterpreterException("Binary operator may only be applied to primitive types", l);
			}
			// if boolean then....
			if(x.GetType() == TypeManager.bool_type || y.GetType() == TypeManager.bool_type) {
				if(x.GetType() == TypeManager.bool_type && y.GetType() == TypeManager.bool_type) {
					//do bool comparisons
					DB.bp("both are bools");
					switch(bo) {
						case(BiOperator.Equality):
							return Equality((bool)x,(bool)y);
						case(BiOperator.Inequality):
							return Inequality((bool)x,(bool)y);
						case(BiOperator.BitwiseAnd):
							return BitwiseAnd((bool)x, (bool)y);
						case(BiOperator.BitwiseOr):
							return BitwiseOr((bool)x, (bool)y);
						case(BiOperator.ExclusiveOr):
							return ExclusiveOr((bool)x, (bool)y);
						default:
							throw new InterpreterException("invalid bool operation: " + OperName(bo), l);
					}
				}
				else {
					throw new InterpreterException("bool types may only be compared to other bool types", l);
				}
			}

			//not bools so must be number type
			//first check shifts:
			if(bo == BiOperator.LeftShift || bo == BiOperator.RightShift) {
				DB.bp("we are shifting");
				if(y.GetType() != TypeManager.int32_type)
					throw new InterpreterException("second argument of a shift must be an int", l);

				if(bo == BiOperator.LeftShift) {
					if(x.GetType() == TypeManager.int32_type)
						return	LeftShift((int)x, (int)y);
					if(x.GetType() == TypeManager.uint32_type)
						return LeftShift((uint)x, (int)y);
					if(x.GetType() == TypeManager.int64_type)
						return LeftShift((uint)x, (int)y);
					if(x.GetType() == TypeManager.uint32_type)
						return LeftShift((ulong)x, (int)y);
				}
				else {
					if(x.GetType() == TypeManager.int32_type)
						return RightShift((int)x, (int)y);
					if(x.GetType() == TypeManager.uint32_type)
						return RightShift((uint)x, (int)y);
					if(x.GetType() == TypeManager.int64_type)
						return RightShift((uint)x, (int)y);
					if(x.GetType() == TypeManager.uint32_type)
						return RightShift((ulong)x, (int)y);
				}
				throw new InterpreterException("can only shift on integer type", l);
			}

			//then do normal biops
			
			ENumPromotion ep = DoNumericPromotion(ref x, ref y, l);
			DB.bp("we did a  numeric promotion to: " + ep.ToString());
			switch(ep) {
				case(ENumPromotion.DecimalDecimal):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((decimal)x, (decimal)y);
					case(BiOperator.Division):
						return Division((decimal)x, (decimal)y);
					case(BiOperator.Equality):
						return Equality((decimal)x, (decimal)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((decimal)x, (decimal)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((decimal)x, (decimal)y);
					case(BiOperator.Inequality):
						return Inequality((decimal)x, (decimal)y);
					case(BiOperator.LessThan):
						return LessThan((decimal)x, (decimal)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((decimal)x, (decimal)y);
					case(BiOperator.Modulus):
						return Modulus((decimal)x, (decimal)y);
					case(BiOperator.Multiply):
						return Multiply((decimal)x, (decimal)y);
					case(BiOperator.Subtraction):
						return Subtraction((decimal)x, (decimal)y);
					default:
						throw new InterpreterException("unsupported binary operation on decimals", l);
				}
				case(ENumPromotion.DoubleDouble):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((double)x, (double)y);
					case(BiOperator.Division):
						return Division((double)x, (double)y);
					case(BiOperator.Equality):
						return Equality((double)x, (double)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((double)x, (double)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((double)x, (double)y);
					case(BiOperator.Inequality):
						return Inequality((double)x, (double)y);
					case(BiOperator.LessThan):
						return LessThan((double)x, (double)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((double)x, (double)y);
					case(BiOperator.Modulus):
						return Modulus((double)x, (double)y);
					case(BiOperator.Multiply):
						return Multiply((double)x, (double)y);
					case(BiOperator.Subtraction):
						return Subtraction((double)x, (double)y);
					default:
						throw new InterpreterException("unsupported binary operation on double", l);
				}
				case(ENumPromotion.FloatFloat):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((float)x, (float)y);
					case(BiOperator.Division):
						return Division((float)x, (float)y);
					case(BiOperator.Equality):
						return Equality((float)x, (float)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((float)x, (float)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((float)x, (float)y);
					case(BiOperator.Inequality):
						return Inequality((float)x, (float)y);
					case(BiOperator.LessThan):
						return LessThan((float)x, (float)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((float)x, (float)y);
					case(BiOperator.Modulus):
						return Modulus((float)x, (float)y);
					case(BiOperator.Multiply):
						return Multiply((float)x, (float)y);
					case(BiOperator.Subtraction):
						return Subtraction((float)x, (float)y);
					default:
						throw new InterpreterException("unsupported binary operation on float", l);
				}
				case(ENumPromotion.IntInt):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((int)x, (int)y);
					case(BiOperator.BitwiseAnd):
						return BitwiseAnd((int)x, (int)y);
					case(BiOperator.BitwiseOr):
						return BitwiseOr((int)x, (int)y);
					case(BiOperator.Division):
						return Division((int)x, (int)y);
					case(BiOperator.Equality):
						return Equality((int)x, (int)y);
					case(BiOperator.ExclusiveOr):
						return ExclusiveOr((int)x, (int)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((int)x, (int)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((int)x, (int)y);
					case(BiOperator.Inequality):
						return Inequality((int)x, (int)y);
					case(BiOperator.LessThan):
						return LessThan((int)x, (int)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((int)x, (int)y);
					case(BiOperator.Modulus):
						return Modulus((int)x, (int)y);
					case(BiOperator.Multiply):
						return Multiply((int)x, (int)y);
					case(BiOperator.Subtraction):
						return Subtraction((int)x, (int)y);
					default:
						throw new InterpreterException("unsupported binary operation on int", l);
				}
				case(ENumPromotion.LongLong):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((long)x, (long)y);
					case(BiOperator.BitwiseAnd):
						return BitwiseAnd((long)x, (long)y);
					case(BiOperator.BitwiseOr):
						return BitwiseOr((long)x, (long)y);
					case(BiOperator.Division):
						return Division((long)x, (long)y);
					case(BiOperator.Equality):
						return Equality((long)x, (long)y);
					case(BiOperator.ExclusiveOr):
						return ExclusiveOr((long)x, (long)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((long)x, (long)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((long)x, (long)y);
					case(BiOperator.Inequality):
						return Inequality((long)x, (long)y);
					case(BiOperator.LessThan):
						return LessThan((long)x, (long)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((long)x, (long)y);
					case(BiOperator.Modulus):
						return Modulus((long)x, (long)y);
					case(BiOperator.Multiply):
						return Multiply((long)x, (long)y);
					case(BiOperator.Subtraction):
						return Subtraction((long)x, (long)y);
					default:
						throw new InterpreterException("unsupported binary operation on long", l);
				}
				case(ENumPromotion.UintUint):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((uint)x, (uint)y);
					case(BiOperator.BitwiseAnd):
						return BitwiseAnd((uint)x, (uint)y);
					case(BiOperator.BitwiseOr):
						return BitwiseOr((uint)x, (uint)y);
					case(BiOperator.Division):
						return Division((uint)x, (uint)y);
					case(BiOperator.Equality):
						return Equality((uint)x, (uint)y);
					case(BiOperator.ExclusiveOr):
						return ExclusiveOr((uint)x, (uint)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((uint)x, (uint)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((uint)x, (uint)y);
					case(BiOperator.Inequality):
						return Inequality((uint)x, (uint)y);
					case(BiOperator.LessThan):
						return LessThan((uint)x, (uint)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((uint)x, (uint)y);
					case(BiOperator.Modulus):
						return Modulus((uint)x, (uint)y);
					case(BiOperator.Multiply):
						return Multiply((uint)x, (uint)y);
					case(BiOperator.Subtraction):
						return Subtraction((uint)x, (uint)y);
					default:
						throw new InterpreterException("unsupported binary operation on uint", l);
				}
				case(ENumPromotion.UlongUlong):
				switch(bo) {
					case(BiOperator.Addition):
						return Addition((ulong)x, (ulong)y);
					case(BiOperator.BitwiseAnd):
						return BitwiseAnd((ulong)x, (ulong)y);
					case(BiOperator.BitwiseOr):
						return BitwiseOr((ulong)x, (ulong)y);
					case(BiOperator.Division):
						return Division((ulong)x, (ulong)y);
					case(BiOperator.Equality):
						return Equality((ulong)x, (ulong)y);
					case(BiOperator.ExclusiveOr):
						return ExclusiveOr((ulong)x, (ulong)y);
					case(BiOperator.GreaterThan):
						return GreaterThan((ulong)x, (ulong)y);
					case(BiOperator.GreaterThanOrEqual):
						return GreaterThanOrEqual((ulong)x, (ulong)y);
					case(BiOperator.Inequality):
						return Inequality((ulong)x, (ulong)y);
					case(BiOperator.LessThan):
						return LessThan((ulong)x, (ulong)y);
					case(BiOperator.LessThanOrEqual):
						return LessThanOrEqual((ulong)x, (ulong)y);
					case(BiOperator.Modulus):
						return Modulus((ulong)x, (ulong)y);
					case(BiOperator.Multiply):
						return Multiply((ulong)x, (ulong)y);
					case(BiOperator.Subtraction):
						return Subtraction((ulong)x, (ulong)y);
					default:
						throw new InterpreterException("unsupported binary operation on ulong", l);
				}
				default:
					throw new InterpreterException("unknown binary operator", l);
			}	
		}

		public static int Multiply(int x, int y) {
			return x * y;
		}
		public static uint Multiply(uint x, uint y) {
			return x * y;
		}
		public static long Multiply(long x, long y) {
			return x * y;
		}
		public static ulong Multiply( ulong x, ulong y) {
			return x * y;
		}
		public static float Multiply( float x, float y) {
			return x * y;
		}
		public static double Multiply(double x, double y) {
			return x * y;
		}
		public static decimal Multiply(decimal x, decimal y) {
			return x * y;
		}

		
			

		public static int Division(int x, int y) {
			return x / y;
		}
		public static uint Division(uint x, uint y) {
			return x / y;
		}
		public static long Division(long x, long y) {
			return x / y;
		}
		public static ulong Division( ulong x, ulong y) {
			return x / y;
		}
		public static float Division( float x, float y) {
			return x / y;
		}
		public static double Division(double x, double y) {
			return x / y;
		}
		public static decimal Division(decimal x, decimal y) {
			return x / y;
		}

		public static int Modulus(int x, int y) {
			return x % y;
		}
		public static uint Modulus(uint x, uint y) {
			return x % y;
		}
		public static long Modulus(long x, long y) {
			return x % y;
		}
		public static ulong Modulus( ulong x, ulong y) {
			return x % y;
		}
		public static float Modulus( float x, float y) {
			return x % y;
		}
		public static double Modulus(double x, double y) {
			return x % y;
		}
		public static decimal Modulus(decimal x, decimal y) {
			return x % y;
		}

		public static int Addition(int x, int y) {
			return x + y;
		}
		public static uint Addition(uint x, uint y) {
			return x + y;
		}
		public static long Addition(long x, long y) {
			return x + y;
		}
		public static ulong Addition(ulong x, ulong y) {
			return x + y;
		}
		public static float Addition( float x, float y) {
			return x + y;
		}
		public static double Addition(double x, double y) {
			return x + y;
		}
		public static decimal Addition(decimal x, decimal y) {
			return x + y;
		}

		public static int Subtraction(int x, int y) {
			return x - y;
		}
		public static uint Subtraction(uint x, uint y) {
			return x - y;
		}
		public static long Subtraction(long x, long y) {
			return x - y;
		}
		public static ulong Subtraction( ulong x, ulong y) {
			return x - y;
		}
		public static float Subtraction( float x, float y) {
			return x - y;
		}
		public static double Subtraction(double x, double y) {
			return x - y;
		}
		public static decimal Subtraction(decimal x, decimal y) {
			return x - y;
		}
		
		public static int LeftShift(int x, int count) {
			return x << count;
		}

		public static uint LeftShift(uint x, int count) {
			return x << count;
		}

		public static long LeftShift(long x, int count) {
			return x << count;
		}

		public static ulong LeftShift(ulong x, int count) {
			return x << count;
		}
	
		public static int RightShift(int x, int count) {
			return x >> count;
		}

		public static uint RightShift(uint x, int count) {
			return x >> count;
		}

		public static long RightShift(long x, int count) {
			return x >> count;
		}

		public static ulong RightShift(ulong x, int count) {
			return x >> count;
		}

		public static bool Equality(int x, int y) {
			return x == y;
		}
		public static bool Equality(uint x, uint y) {
			return x == y;
		}
		public static bool Equality(long x, long y) {
			return x == y;
		}
		public static bool Equality(ulong x, ulong y) {
			return x == y;
		}
		public static bool Equality(float x, float y) {
			return x == y;
		}
		public static bool Equality(double x, double y) {
			return x == y;
		}
		public static bool Equality(decimal x, decimal y) {
			return x == y;
		}
		
		public static bool Inequality(int x, int y) {
			return x != y;
		}
		public static bool Inequality(uint x, uint y) {
			return x != y;
		}
		public static bool Inequality(long x, long y) {
			return x != y;
		}
		public static bool Inequality(ulong x, ulong y) {
			return x != y;
		}
		public static bool Inequality(float x, float y) {
			return x != y;
		}
		public static bool Inequality(double x, double y) {
			return x != y;
		}
		public static bool Inequality(decimal x, decimal y) {
			return x != y;
		}
	
		public static bool LessThan(int x, int y) {
			return x < y;
		}
		public static bool LessThan(uint x, uint y) {
			return x < y;
		}
		public static bool LessThan(long x, long y) {
			return x < y;
		}
		public static bool LessThan(ulong x, ulong y) {
			return x < y;
		}
		public static bool LessThan(float x, float y) {
			return x < y;
		}
		public static bool LessThan(double x, double y) {
			return x < y;
		}
		public static bool LessThan(decimal x, decimal y) {
			return x < y;
		}

		public static bool GreaterThan(int x, int y) {
			return x > y;
		}
		public static bool GreaterThan(uint x, uint y) {
			return x > y;
		}
		public static bool GreaterThan(long x, long y) {
			return x > y;
		}
		public static bool GreaterThan(ulong x, ulong y) {
			return x > y;
		}
		public static bool GreaterThan(float x, float y) {
			return x > y;
		}
		public static bool GreaterThan(double x, double y) {
			return x > y;
		}
		public static bool GreaterThan(decimal x, decimal y) {
			return x > y;
		}

		public static bool LessThanOrEqual(int x, int y) {
			return x <= y;
		}
		public static bool LessThanOrEqual(uint x, uint y) {
			return x <= y;
		}
		public static bool LessThanOrEqual(long x, long y) {
			return x <= y;
		}
		public static bool LessThanOrEqual(ulong x, ulong y) {
			return x <= y;
		}
		public static bool LessThanOrEqual(float x, float y) {
			return x <= y;
		}
		public static bool LessThanOrEqual(double x, double y) {
			return x <= y;
		}
		public static bool LessThanOrEqual(decimal x, decimal y) {
			return x <= y;
		}

		public static bool GreaterThanOrEqual(int x, int y) {
			return x >= y;
		}
		public static bool GreaterThanOrEqual(uint x, uint y) {
			return x >= y;
		}
		public static bool GreaterThanOrEqual(long x, long y) {
			return x >= y;
		}
		public static bool GreaterThanOrEqual(ulong x, ulong y) {
			return x >= y;
		}
		public static bool GreaterThanOrEqual(float x, float y) {
			return x >= y;
		}
		public static bool GreaterThanOrEqual(double x, double y) {
			return x >= y;
		}
		public static bool GreaterThanOrEqual(decimal x, decimal y) {
			return x >= y;
		}

		//bool checking
		public static bool Equality(bool x, bool y) {
			return x == y;
		}

		public static bool Inequality(bool x, bool y) {
			return x != y;
		}
		public static bool BitwiseAnd(bool x, bool y) {
			return x & y;
		}
		public static bool BitwiseOr(bool x, bool y) {
			return x | y;
		}
		public static bool ExclusiveOr(bool x, bool y) {
			return x ^ y;
		}

		//logical operators
		public static int BitwiseAnd(int x, int y) {
			return x & y;
		}
		
		public static uint BitwiseAnd(uint x, uint y) {
			return x & y;
		}

		public static long BitwiseAnd(long x, long y) {
			return x & y;
		}

		public static ulong BitwiseAnd(ulong x, ulong y) {
			return x & y;
		}

		public static int BitwiseOr(int x, int y) {
			return x | y;
		}
		
		public static uint BitwiseOr(uint x, uint y) {
			return x | y;
		}

		public static long BitwiseOr(long x, long y) {
			return x | y;
		}

		public static ulong BitwiseOr(ulong x, ulong y) {
			return x | y;
		}


		public static int ExclusiveOr(int x, int y) {
			return x ^ y;
		}
		
		public static uint ExclusiveOr(uint x, uint y) {
			return x ^ y;
		}

		public static long ExclusiveOr(long x, long y) {
			return x ^ y;
		}

		public static ulong ExclusiveOr(ulong x, ulong y) {
			return x ^ y;
		}



		//enum checking



		protected static ENumPromotion DoNumericPromotion(ref object left, ref object right, Location l) {
			Type lType = left.GetType();
			Type rType = right.GetType();
			if(lType.IsPrimitive && rType.IsPrimitive) {
				if(lType == TypeManager.bool_type || rType == TypeManager.bool_type) {
					throw new InterpreterException("cannot perform numerical promotion on bool types", l);
				}
				
				if(lType == TypeManager.decimal_type) {
					if(rType == TypeManager.double_type || rType == TypeManager.float_type)
						throw new InterpreterException("cannot promote '" + right + "' to type decimal", l);
					right = Convert.ToDecimal(right);
					//rType = right.GetType();
					return ENumPromotion.DecimalDecimal;
				}
				else if(rType == TypeManager.decimal_type) {
					if(lType == TypeManager.double_type || lType == TypeManager.float_type)
						throw new InterpreterException("cannot promote '" + left + "' to type decimal", l);
					left = Convert.ToDecimal(left);
					//lType = left.GetType();
					return ENumPromotion.DecimalDecimal;
				}
				else if(lType == TypeManager.double_type) {
					right = Convert.ToDouble(right);
					//rType = right.GetType();
					return ENumPromotion.DoubleDouble;
				}
				else if(rType == TypeManager.double_type) {
					left = Convert.ToDouble(left);
					//lType = left.GetType();
					return ENumPromotion.DoubleDouble;
				}
				else if(lType == TypeManager.float_type) {
					right = Convert.ToSingle(right);
					//rType = right.GetType();
					return ENumPromotion.FloatFloat;
				}
				else if(rType == TypeManager.float_type) {
					left = Convert.ToSingle(left);
					//lType = left.GetType();
					return ENumPromotion.FloatFloat;
				}
				else if(lType == TypeManager.uint64_type) {
					if(rType == TypeManager.sbyte_type || rType == TypeManager.short_type || rType == TypeManager.int32_type || rType == TypeManager.int64_type)
						throw new InterpreterException("cannot promote '" + right + "' to type ulong", l);
					right = Convert.ToUInt64(right);
					//rType = right.GetType();
					return ENumPromotion.UlongUlong;
				}
				else if(rType == TypeManager.uint64_type) {
					if(lType == TypeManager.sbyte_type || lType == TypeManager.short_type || lType == TypeManager.int32_type || lType == TypeManager.int64_type)
						throw new InterpreterException("cannot promote '" + left + "' to type ulong", l);
					left = Convert.ToUInt64(left);
					//lType = left.GetType();
					return ENumPromotion.UlongUlong;
				}
				else if(lType == TypeManager.int64_type) {
					right = Convert.ToInt64(right);
					//rType = right.GetType();
					return ENumPromotion.LongLong;
				}
				else if(rType == TypeManager.int64_type) {
					left = Convert.ToInt64(left);
					//lType = left.GetType();
					return ENumPromotion.LongLong;
				}
				else if(lType == TypeManager.uint32_type) {
					if(rType == TypeManager.sbyte_type || rType == TypeManager.short_type || rType == TypeManager.int32_type) {
						left = Convert.ToInt64(left);
						//lType = left.GetType();
						right = Convert.ToInt64(right);
						//rType = right.GetType();
						return ENumPromotion.LongLong;
					}
					else {
						right = Convert.ToUInt32(right);
						//rType = right.GetType();
						return ENumPromotion.UintUint;
					}
				}
				else if(rType == TypeManager.uint32_type) {
					if(lType == TypeManager.sbyte_type || lType == TypeManager.short_type || lType == TypeManager.int32_type) {
						left = Convert.ToInt64(left);
						//lType = left.GetType();
						right = Convert.ToInt64(right);
						//rType = right.GetType();
						return ENumPromotion.LongLong;
					}
					else {
						left = Convert.ToUInt32(left);
						//lType = left.GetType();
						return ENumPromotion.UintUint;
					}
				}
				else {
					left = Convert.ToInt32(left);
					//lType = left.GetType();
					right = Convert.ToInt32(right);
					//rType = right.GetType();
					return ENumPromotion.IntInt;
				}

			}
			throw new InterpreterException("cannot perform numerical promotion on non-primitive types", l);
		}



//equality operators


		#region BINARY_STUFF

#if BINARY_STUFF
		Expression ForceConversion (EmitContext ec, Expression expr, Type target_type) {
			if (expr.Type == target_type)
				return expr;

			return ConvertImplicit (ec, expr, target_type, new Location (-1));
		}

		public static void Error_OperatorAmbiguous (Location loc, BiOperator oper, Type l, Type r) {
			Report.Error (
				34, loc, "BiOperator `" + OperName (oper) 
				+ "' is ambiguous on operands of type `"
				+ TypeManager.CSharpName (l) + "' "
				+ "and `" + TypeManager.CSharpName (r)
				+ "'");
		}

		//
		// Note that handling the case l == Decimal || r == Decimal
		// is taken care of by the Step 1 BiOperator Overload resolution.
		//
		bool DoNumericPromotions (EmitContext ec, Type l, Type r) {
			if (l == TypeManager.double_type || r == TypeManager.double_type){
				//
				// If either operand is of type double, the other operand is
				// conveted to type double.
				//
				if (r != TypeManager.double_type)
					right = ConvertImplicit (ec, right, TypeManager.double_type, loc);
				if (l != TypeManager.double_type)
					left = ConvertImplicit (ec, left, TypeManager.double_type, loc);
				
				type = TypeManager.double_type;
			} else if (l == TypeManager.float_type || r == TypeManager.float_type){
				//
				// if either operand is of type float, the other operand is
				// converted to type float.
				//
				if (r != TypeManager.double_type)
					right = ConvertImplicit (ec, right, TypeManager.float_type, loc);
				if (l != TypeManager.double_type)
					left = ConvertImplicit (ec, left, TypeManager.float_type, loc);
				type = TypeManager.float_type;
			} else if (l == TypeManager.uint64_type || r == TypeManager.uint64_type){
				Expression e;
				Type other;
				//
				// If either operand is of type ulong, the other operand is
				// converted to type ulong.  or an error ocurrs if the other
				// operand is of type sbyte, short, int or long
				//
				if (l == TypeManager.uint64_type){
					if (r != TypeManager.uint64_type){
						if (right is IntConstant){
							IntConstant ic = (IntConstant) right;
							
							e = TryImplicitIntConversion (l, ic);
							if (e != null)
								right = e;
						} else if (right is LongConstant){
							long ll = ((LongConstant) right).Value;

							if (ll > 0)
								right = new ULongConstant ((ulong) ll);
						} else {
							e = ImplicitNumericConversion (ec, right, l, loc);
							if (e != null)
								right = e;
						}
					}
					other = right.Type;
				} else {
					if (left is IntConstant){
						e = TryImplicitIntConversion (r, (IntConstant) left);
						if (e != null)
							left = e;
					} else if (left is LongConstant){
						long ll = ((LongConstant) left).Value;
						
						if (ll > 0)
							left = new ULongConstant ((ulong) ll);
					} else {
						e = ImplicitNumericConversion (ec, left, r, loc);
						if (e != null)
							left = e;
					}
					other = left.Type;
				}

				if ((other == TypeManager.sbyte_type) ||
					(other == TypeManager.short_type) ||
					(other == TypeManager.int32_type) ||
					(other == TypeManager.int64_type))
					Error_OperatorAmbiguous (loc, oper, l, r);
				type = TypeManager.uint64_type;
			} else if (l == TypeManager.int64_type || r == TypeManager.int64_type){
				//
				// If either operand is of type long, the other operand is converted
				// to type long.
				//
				if (l != TypeManager.int64_type)
					left = ConvertImplicit (ec, left, TypeManager.int64_type, loc);
				if (r != TypeManager.int64_type)
					right = ConvertImplicit (ec, right, TypeManager.int64_type, loc);

				type = TypeManager.int64_type;
			} else if (l == TypeManager.uint32_type || r == TypeManager.uint32_type){
				//
				// If either operand is of type uint, and the other
				// operand is of type sbyte, short or int, othe operands are
				// converted to type long.
				//
				Type other = null;
				
				if (l == TypeManager.uint32_type){
					if (right is IntConstant){
						IntConstant ic = (IntConstant) right;
						int val = ic.Value;
						
						if (val >= 0)
							right = new UIntConstant ((uint) val);

						type = l;
						return true;
					}
					other = r;
				} 
				else if (r == TypeManager.uint32_type){
					if (left is IntConstant){
						IntConstant ic = (IntConstant) left;
						int val = ic.Value;
						
						if (val >= 0)
							left = new UIntConstant ((uint) val);

						type = r;
						return true;
					}
					
					other = l;
				}

				if ((other == TypeManager.sbyte_type) ||
					(other == TypeManager.short_type) ||
					(other == TypeManager.int32_type)){
					left = ForceConversion (ec, left, TypeManager.int64_type);
					right = ForceConversion (ec, right, TypeManager.int64_type);
					type = TypeManager.int64_type;
				} else {
					//
					// if either operand is of type uint, the other
					// operand is converd to type uint
					//
					left = ForceConversion (ec, left, TypeManager.uint32_type);
					right = ForceConversion (ec, right, TypeManager.uint32_type);
					type = TypeManager.uint32_type;
				} 
			} else if (l == TypeManager.decimal_type || r == TypeManager.decimal_type){
				if (l != TypeManager.decimal_type)
					left = ConvertImplicit (ec, left, TypeManager.decimal_type, loc);
				if (r != TypeManager.decimal_type)
					right = ConvertImplicit (ec, right, TypeManager.decimal_type, loc);

				type = TypeManager.decimal_type;
			} else {
				Expression l_tmp, r_tmp;

				l_tmp = ForceConversion (ec, left, TypeManager.int32_type);
				if (l_tmp == null)
					return false;
				
				r_tmp = ForceConversion (ec, right, TypeManager.int32_type);
				if (r_tmp == null)
					return false;

				left = l_tmp;
				right = r_tmp;
				
				type = TypeManager.int32_type;
			}

			return true;
		}

		static public void Error_OperatorCannotBeApplied (Location loc, string name, Type l, Type r) {
			Error (19, loc,
				"BiOperator " + name + " cannot be applied to operands of type `" +
				TypeManager.CSharpName (l) + "' and `" +
				TypeManager.CSharpName (r) + "'");
		}
		
		void Error_OperatorCannotBeApplied () {
			Error_OperatorCannotBeApplied (loc, OperName (oper), left.Type, right.Type);
		}

		static bool is_32_or_64 (Type t) {
			return (t == TypeManager.int32_type || t == TypeManager.uint32_type ||
				t == TypeManager.int64_type || t == TypeManager.uint64_type);
		}
					
		Expression CheckShiftArguments (EmitContext ec) {
			Expression e;
			Type l = left.Type;
			Type r = right.Type;

			e = ForceConversion (ec, right, TypeManager.int32_type);
			if (e == null){
				Error_OperatorCannotBeApplied ();
				return null;
			}
			right = e;

			if (((e = ConvertImplicit (ec, left, TypeManager.int32_type, loc)) != null) ||
				((e = ConvertImplicit (ec, left, TypeManager.uint32_type, loc)) != null) ||
				((e = ConvertImplicit (ec, left, TypeManager.int64_type, loc)) != null) ||
				((e = ConvertImplicit (ec, left, TypeManager.uint64_type, loc)) != null)){
				left = e;
				type = e.Type;

				return this;
			}
			Error_OperatorCannotBeApplied ();
			return null;
		}

		Expression ResolveOperator (EmitContext ec) {
			Type l = left.Type;
			Type r = right.Type;

			bool overload_failed = false;

			//
			// Step 1: Perform BiOperator Overload location
			//
			Expression left_expr, right_expr;
				
			string op = oper_names [(int) oper];
				
			MethodGroupExpr union;
			left_expr = MemberLookup (ec, l, op, MemberTypes.Method, AllBindingFlags, loc);
			if (r != l){
				right_expr = MemberLookup (
					ec, r, op, MemberTypes.Method, AllBindingFlags, loc);
				union = Invocation.MakeUnionSet (left_expr, right_expr, loc);
			} else
				union = (MethodGroupExpr) left_expr;
				
			if (union != null) {
				Arguments = new ArrayList ();
				Arguments.Add (new Argument (left, Argument.AType.Expression));
				Arguments.Add (new Argument (right, Argument.AType.Expression));
				
				method = Invocation.OverloadResolve (ec, union, Arguments, Location.Null);
				if (method != null) {
					MethodInfo mi = (MethodInfo) method;
					
					type = mi.ReturnType;
					return this;
				} else {
					overload_failed = true;
				}
			}	
			
			//
			// Step 2: Default operations on CLI native types.
			//

			//
			// Step 0: String concatenation (because overloading will get this wrong)
			//
			if (oper == BiOperator.Addition){
				//
				// If any of the arguments is a string, cast to string
				//
				
				if (l == TypeManager.string_type){
					
					if (r == TypeManager.void_type) {
						Error_OperatorCannotBeApplied ();
						return null;
					}
					
					if (r == TypeManager.string_type){
						if (left is Constant && right is Constant){
							StringConstant ls = (StringConstant) left;
							StringConstant rs = (StringConstant) right;
							
							return new StringConstant (
								ls.Value + rs.Value);
						}
						
						// string + string
						method = TypeManager.string_concat_string_string;
					} else {
						// string + object
						method = TypeManager.string_concat_object_object;
						right = ConvertImplicit (ec, right,
							TypeManager.object_type, loc);
					}
					type = TypeManager.string_type;

					Arguments = new ArrayList ();
					Arguments.Add (new Argument (left, Argument.AType.Expression));
					Arguments.Add (new Argument (right, Argument.AType.Expression));

					return this;
					
				} else if (r == TypeManager.string_type){
					// object + string

					if (l == TypeManager.void_type) {
						Error_OperatorCannotBeApplied ();
						return null;
					}
					
					method = TypeManager.string_concat_object_object;
					left = ConvertImplicit (ec, left, TypeManager.object_type, loc);
					Arguments = new ArrayList ();
					Arguments.Add (new Argument (left, Argument.AType.Expression));
					Arguments.Add (new Argument (right, Argument.AType.Expression));

					type = TypeManager.string_type;

					return this;
				}

				//
				// Transform a + ( - b) into a - b
				//
				if (right is Unary){
					Unary right_unary = (Unary) right;

					if (right_unary.Oper == Unary.BiOperator.UnaryNegation){
						oper = BiOperator.Subtraction;
						right = right_unary.Expr;
						r = right.Type;
					}
				}
			}

			if (oper == BiOperator.Equality || oper == BiOperator.Inequality){
				if (l == TypeManager.bool_type || r == TypeManager.bool_type){
					if (r != TypeManager.bool_type || l != TypeManager.bool_type){
						Error_OperatorCannotBeApplied ();
						return null;
					}
					
					type = TypeManager.bool_type;
					return this;
				}

				//
				// operator != (object a, object b)
				// operator == (object a, object b)
				//
				// For this to be used, both arguments have to be reference-types.
				// Read the rationale on the spec (14.9.6)
				//
				// Also, if at compile time we know that the classes do not inherit
				// one from the other, then we catch the error there.
				//
				if (!(l.IsValueType || r.IsValueType)){
					type = TypeManager.bool_type;

					if (l == r)
						return this;
					
					if (l.IsSubclassOf (r) || r.IsSubclassOf (l))
						return this;

					//
					// Also, a standard conversion must exist from either one
					//
					if (!(StandardConversionExists (left, r) ||
						StandardConversionExists (right, l))){
						Error_OperatorCannotBeApplied ();
						return null;
					}
					//
					// We are going to have to convert to an object to compare
					//
					if (l != TypeManager.object_type)
						left = new EmptyCast (left, TypeManager.object_type);
					if (r != TypeManager.object_type)
						right = new EmptyCast (right, TypeManager.object_type);

					//
					// FIXME: CSC here catches errors cs254 and cs252
					//
					return this;
				}
			}

			// Only perform numeric promotions on:
			// +, -, *, /, %, &, |, ^, ==, !=, <, >, <=, >=
			//
			if (oper == BiOperator.Addition || oper == BiOperator.Subtraction) {
				if (l.IsSubclassOf (TypeManager.delegate_type) &&
					r.IsSubclassOf (TypeManager.delegate_type)) {
					
					Arguments = new ArrayList ();
					Arguments.Add (new Argument (left, Argument.AType.Expression));
					Arguments.Add (new Argument (right, Argument.AType.Expression));
					
					if (oper == BiOperator.Addition)
						method = TypeManager.delegate_combine_delegate_delegate;
					else
						method = TypeManager.delegate_remove_delegate_delegate;
					
					DelegateOperation = true;
					type = l;
					return this;
				}

				//
				// Pointer arithmetic:
				//
				// T* operator + (T* x, int y);
				// T* operator + (T* x, uint y);
				// T* operator + (T* x, long y);
				// T* operator + (T* x, ulong y);
				//
				// T* operator + (int y,   T* x);
				// T* operator + (uint y,  T *x);
				// T* operator + (long y,  T *x);
				// T* operator + (ulong y, T *x);
				//
				// T* operator - (T* x, int y);
				// T* operator - (T* x, uint y);
				// T* operator - (T* x, long y);
				// T* operator - (T* x, ulong y);
				//
				// long operator - (T* x, T *y)
				//
				if (l.IsPointer){
					if (r.IsPointer && oper == BiOperator.Subtraction){
						if (r == l)
							return new PointerArithmetic (
								false, left, right, TypeManager.int64_type);
					} else if (is_32_or_64 (r))
						return new PointerArithmetic (
							oper == BiOperator.Addition, left, right, l);
				} else if (r.IsPointer && is_32_or_64 (l) && oper == BiOperator.Addition)
					return new PointerArithmetic (
						true, right, left, r);
			}
			
			//
			// Enumeration operators
			//
			bool lie = TypeManager.IsEnumType (l);
			bool rie = TypeManager.IsEnumType (r);
			if (lie || rie){
				Expression temp;

				//
				// operator + (E e, U x)
				//
				if (oper == BiOperator.Addition){
					if (lie && rie){
						Error_OperatorCannotBeApplied ();
						return null;
					}

					Type enum_type = lie ? l : r;
					Type other_type = lie ? r : l;
					Type underlying_type = TypeManager.EnumToUnderlying (enum_type);
					;
					
					if (underlying_type != other_type){
						Error_OperatorCannotBeApplied ();
						return null;
					}

					type = enum_type;
					return this;
				}
				
				if (!rie){
					temp = ConvertImplicit (ec, right, l, loc);
					if (temp != null)
						right = temp;
				} if (!lie){
					  temp = ConvertImplicit (ec, left, r, loc);
					  if (temp != null){
						  left = temp;
						  l = r;
					  }
				  }
				 
				if (oper == BiOperator.Equality || oper == BiOperator.Inequality ||
					oper == BiOperator.LessThanOrEqual || oper == BiOperator.LessThan ||
					oper == BiOperator.GreaterThanOrEqual || oper == BiOperator.GreaterThan){
					type = TypeManager.bool_type;
					return this;
				}

				if (oper == BiOperator.BitwiseAnd ||
					oper == BiOperator.BitwiseOr ||
					oper == BiOperator.Xor){
					type = l;
					return this;
				}
				return null;
			}
			
			if (oper == BiOperator.LeftShift || oper == BiOperator.RightShift)
				return CheckShiftArguments (ec);

			if (oper == BiOperator.LogicalOr || oper == BiOperator.LogicalAnd){
				if (l != TypeManager.bool_type || r != TypeManager.bool_type){
					Error_OperatorCannotBeApplied ();
					return null;
				}

				type = TypeManager.bool_type;
				return this;
			} 

			//
			// operator & (bool x, bool y)
			// operator | (bool x, bool y)
			// operator ^ (bool x, bool y)
			//
			if (l == TypeManager.bool_type && r == TypeManager.bool_type){
				if (oper == BiOperator.BitwiseAnd ||
					oper == BiOperator.BitwiseOr ||
					oper == BiOperator.Xor){
					type = l;
					return this;
				}
			}
			
			//
			// Pointer comparison
			//
			if (l.IsPointer && r.IsPointer){
				if (oper == BiOperator.Equality || oper == BiOperator.Inequality ||
					oper == BiOperator.LessThan || oper == BiOperator.LessThanOrEqual ||
					oper == BiOperator.GreaterThan || oper == BiOperator.GreaterThanOrEqual){
					type = TypeManager.bool_type;
					return this;
				}
			}
			
			//
			// We are dealing with numbers
			//
			if (overload_failed){
				Error_OperatorCannotBeApplied ();
				return null;
			}

			if (!DoNumericPromotions (ec, l, r)){
				Error_OperatorCannotBeApplied ();
				return null;
			}

			if (left == null || right == null)
				return null;

			//
			// reload our cached types if required
			//
			l = left.Type;
			r = right.Type;
			
			if (oper == BiOperator.BitwiseAnd ||
				oper == BiOperator.BitwiseOr ||
				oper == BiOperator.Xor){
				if (l == r){
					if (!((l == TypeManager.int32_type) ||
						(l == TypeManager.uint32_type) ||
						(l == TypeManager.int64_type) ||
						(l == TypeManager.uint64_type)))
						type = l;
				} else {
					Error_OperatorCannotBeApplied ();
					return null;
				}
			}

			if (oper == BiOperator.Equality ||
				oper == BiOperator.Inequality ||
				oper == BiOperator.LessThanOrEqual ||
				oper == BiOperator.LessThan ||
				oper == BiOperator.GreaterThanOrEqual ||
				oper == BiOperator.GreaterThan){
				type = TypeManager.bool_type;
			}

			return this;
		}

		public override Expression DoResolve (EmitContext ec) {
			left = left.Resolve (ec);
			right = right.Resolve (ec);

			if (left == null || right == null)
				return null;

			if (left.Type == null)
				throw new Exception (
					"Resolve returned non null, but did not set the type! (" +
					left + ") at Line: " + loc.Row);
			if (right.Type == null)
				throw new Exception (
					"Resolve returned non null, but did not set the type! (" +
					right + ") at Line: "+ loc.Row);

			eclass = ExprClass.Value;

			if (left is Constant && right is Constant){
				Expression e = ConstantFold.BinaryFold (
					ec, oper, (Constant) left, (Constant) right, loc);
				if (e != null)
					return e;
			}

			return ResolveOperator (ec);
		}

		public bool IsBranchable () {
			if (oper == BiOperator.Equality ||
				oper == BiOperator.Inequality ||
				oper == BiOperator.LessThan ||
				oper == BiOperator.GreaterThan ||
				oper == BiOperator.LessThanOrEqual ||
				oper == BiOperator.GreaterThanOrEqual){
				return true;
			} else
				return false;
		}

	
		public bool IsBuiltinOperator {
			get {
				return method == null;
			}
		}
#endif
		#endregion
	}

}
