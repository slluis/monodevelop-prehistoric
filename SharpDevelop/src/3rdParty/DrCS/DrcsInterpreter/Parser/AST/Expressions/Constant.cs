using System;
using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser.AST.Visitors;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Expressions
{
	/// <summary>
	/// Summary description for Constant.
	/// </summary>
	public abstract class Constant : Expression {
		/// <summary>
		///  This is used to obtain the actual value of the literal
		///  cast into an object.
		/// </summary>
		public abstract object GetValue ();

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forConstant(this, inp);
		}

		//public abstract static object operator+(Constant l, Constant r);
		//public abstract static object operator-(Constant l, Constant r);
		//public abstract static object operator
	}

	public class BoolConstant : Constant {
		public static readonly bool TRUE = true;
		public static readonly bool FALSE = false;
		
		private static BoolConstant falseInstance = null;
		private static BoolConstant trueInstance = null;

		public static BoolConstant FalseInstance {
			get {
				if(null == falseInstance) {
					falseInstance = new BoolConstant(false);
				}
				return falseInstance;
			}
		}
		public static BoolConstant TrueInstance {
			get {
				if(null == trueInstance) {
					trueInstance = new BoolConstant(true);
				}
				return trueInstance;
			}
		}
		public readonly bool Value;
		
		public BoolConstant (bool val) {
			type = TypeManager.bool_type;
			Value = val;
			loc = Location.Null;
		}

		public override object GetValue () {
			return (object) Value;
		}
	}

	public class ByteConstant : Constant {
		public readonly byte Value;

		public ByteConstant (byte v) {
			type = TypeManager.byte_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class CharConstant : Constant {
		public readonly char Value;

		public CharConstant (char v) {
			type = TypeManager.char_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class SByteConstant : Constant {
		public readonly sbyte Value;

		public SByteConstant (sbyte v) {
			type = TypeManager.sbyte_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class ShortConstant : Constant {
		public readonly short Value;

		public ShortConstant (short v) {
			type = TypeManager.short_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class UShortConstant : Constant {
		public readonly ushort Value;

		public UShortConstant (ushort v) {
			type = TypeManager.ushort_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class IntConstant : Constant {
		public readonly int Value;

		public IntConstant (int v) {
			type = TypeManager.int32_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class UIntConstant : Constant {
		public readonly uint Value;

		public UIntConstant (uint v) {
			type = TypeManager.uint32_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class LongConstant : Constant {
		public readonly long Value;

		public LongConstant (long v) {
			type = TypeManager.int64_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class ULongConstant : Constant {
		public readonly ulong Value;

		public ULongConstant (ulong v) {
			type = TypeManager.uint64_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class FloatConstant : Constant {
		public readonly float Value;

		public FloatConstant (float v) {
			type = TypeManager.float_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class DoubleConstant : Constant {
		public readonly double Value;

		public DoubleConstant (double v) {
			type = TypeManager.double_type;
			Value = v;
			loc = Location.Null;
		}

		public override object GetValue () {
			return Value;
		}
	}

	public class DecimalConstant : Constant {
		public readonly decimal Value;

		public DecimalConstant (decimal d) {
			type = TypeManager.decimal_type;
			Value = d;
			loc = Location.Null;
		}

		public override object GetValue () {
			return (object) Value;
		}
	}

	public class StringConstant : Constant {
		public readonly string Value;

		public StringConstant (string s, Location l) {
			type = TypeManager.string_type;
			Value = s;
			loc = l;
		}

		public override object GetValue () {
			return Value;
		}

		public override object execute(IExpressionVisitor v, object inp ) {
			return v.forStringConstant(this, inp);
		}
	}

	/// <summary>
	///  This class is used to wrap literals which belong inside Enums
	/// </summary>
	public class EnumConstant : Constant {
		public readonly Constant Child;

		public EnumConstant (Constant child, Type enum_type) {
			this.Child = child;
			type = enum_type;
		}

		public override object GetValue () {
			return Child.GetValue ();
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forEnumConstant(this, inp);
		}
		//
		// Converts from one of the valid underlying types for an enumeration
		// (int32, uint32, int64, uint64, short, ushort, byte, sbyte) to
		// one of the internal compiler literals: Int/UInt/Long/ULong Literals.
		//
		public Constant WidenToCompilerConstant () {
			Type t = TypeManager.EnumToUnderlying (Child.Type);
			object v = ((Constant) Child).GetValue ();;
			
			if (t == TypeManager.int32_type)
				return new IntConstant ((int) v);
			if (t == TypeManager.uint32_type)
				return new UIntConstant ((uint) v);
			if (t == TypeManager.int64_type)
				return new LongConstant ((long) v);
			if (t == TypeManager.uint64_type)
				return new ULongConstant ((ulong) v);
			if (t == TypeManager.short_type)
				return new ShortConstant ((short) v);
			if (t == TypeManager.ushort_type)
				return new UShortConstant ((ushort) v);
			if (t == TypeManager.byte_type)
				return new ByteConstant ((byte) v);
			if (t == TypeManager.sbyte_type)
				return new SByteConstant ((sbyte) v);

			throw new Exception ("Invalid enumeration underlying type: " + t);
		}

		//
		// Extracts the value in the enumeration on its native representation
		//
		public object GetPlainValue () {
			Type t = TypeManager.EnumToUnderlying (Child.Type);
			object v = ((Constant) Child).GetValue ();;
			
			if (t == TypeManager.int32_type)
				return (int) v;
			if (t == TypeManager.uint32_type)
				return (uint) v;
			if (t == TypeManager.int64_type)
				return (long) v;
			if (t == TypeManager.uint64_type)
				return (ulong) v;
			if (t == TypeManager.short_type)
				return (short) v;
			if (t == TypeManager.ushort_type)
				return (ushort) v;
			if (t == TypeManager.byte_type)
				return (byte) v;
			if (t == TypeManager.sbyte_type)
				return (sbyte) v;

			return null;
		}
		
	}


	
}
