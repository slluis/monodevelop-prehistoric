using System;
using Rice.Drcsharp.Parser.AST.Visitors;
using Rice.Drcsharp.Interpreter;

namespace Rice.Drcsharp.Parser.AST.Expressions {
	/// <summary>
	/// Summary description for BoolLit.
	/// </summary>
	public class BoolLit : BoolConstant {
		public new static readonly bool TRUE = true;
		public new static readonly bool FALSE = false;
		
		private static BoolLit falseInstance = null;
		private static BoolLit trueInstance = null;

		public new static BoolLit FalseInstance {
			get {
				if(null == falseInstance) {
					falseInstance = new BoolLit(false);
				}
				return falseInstance;
			}
		}
		public new static BoolLit TrueInstance {
			get {
				if(null == trueInstance) {
					trueInstance = new BoolLit(true);
				}
				return trueInstance;
			}
		}
		private BoolLit(bool v) : base(v) {}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forBoolLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for CharLit.
	/// </summary>
	public class CharLit : CharConstant {
		public CharLit(char c) : base(c) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forCharLit(this, inp);
		}
	}


	/// <summary>
	/// Summary description for DecimalLit.
	/// </summary>
	public class DecimalLit : DecimalConstant {
		public DecimalLit(decimal d) : base(d){}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forDecimalLit(this, inp);
		}
	}


	/// <summary>
	/// Summary description for DoubleLit.
	/// </summary>
	public class DoubleLit : DoubleConstant {
		public DoubleLit(double d) : base(d) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forDoubleLit(this, inp);
		}

	}
	/// <summary>
	/// Summary description for FloatLit.
	/// </summary>
	public class FloatLit : FloatConstant {
		public FloatLit(float f) : base(f) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forFloatLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for IntLit.
	/// </summary>
	public class IntLit : IntConstant {
		public IntLit(int v) : base(v) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forIntLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for LongLit.
	/// </summary>
	public class LongLit : LongConstant {
		public LongLit(long l) : base(l) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forLongLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for NullLit.
	/// </summary>
	public class NullLit : Constant {
		private static NullLit instance;

		public object Value {
			get {
				return null;
			}
		}

		public override object GetValue() {
			return null;
		}

		public static NullLit Instance {
			get {
				if(instance == null)
					instance = new NullLit();
				return instance;
			}
		}
		private NullLit() {
			type = TypeManager.null_type;
			loc = Location.Null;
		}

		public override object execute(IExpressionVisitor v, object inp) {
			return v.forNullLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for StringLit.
	/// </summary>
	public class StringLit : StringConstant {
		public StringLit(string s, Location l) : base(s, l) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forStringLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for UIntLit.
	/// </summary>
	public class UIntLit : UIntConstant {
		public UIntLit(uint v) : base(v) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forUIntLit(this, inp);
		}
	}

	/// <summary>
	/// Summary description for ULongLit.
	/// </summary>
	public class ULongLit : ULongConstant {
		public ULongLit(ulong u) : base(u) {}
		public override object execute(IExpressionVisitor v, object inp) {
			return v.forULongLit(this, inp);
		}
	}
}
