using System;
using System.Reflection;

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for Info.
	/// </summary>
	public abstract class Info {
		protected object val;
		public virtual object Value {
			get { return val; }
			set { val = value; }
		}

		protected Type type;
		public virtual Type Type { 
			get { return type; }
			set { type = value; }
		}

		public Info(object val) {
			this.val = val;
		}

		public Info(object val, Type t) {
			this.val = val;
			type = t;
		}


	}

	//	public class PrimInfo : Info {
	//		public PrimInfo(object val, Type t) : base (val, t) {}
	//	}

	public class NamespaceInfo : Info {
		public NamespaceInfo(string name) : base(name) {
		}

	}

	public class TypeInfo : Info {
		public TypeInfo(Type t) : base(t) {
		}
	}

	public class ObjectInfo : Info {

		private object currValue;
		public object CurrValue {
			get { return currValue; }
		}

		private Type currType;
		public Type CurrType {
			get { return currType; }
		}

		private MemberInfo info;
		public MemberInfo Info { 
			get { return info; }
			set { info = value; }
		}

		public ObjectInfo(object o, Type t, MemberInfo m) : this(o, t, m, o, t) {
		}

		public ObjectInfo(object pO, Type pT, MemberInfo m, object cO, Type cT) : base (pO, pT) {
			info = m;
			currValue = cO;
			currType = cT;
		}

	}

	public class ArrayInfo : ObjectInfo {
		private object[] indexers;
		public object[] Indexers {
			get { return indexers; }
			set { indexers = value; }
		}

		public ArrayInfo(object o, Type t, MemberInfo m, object[] i) : base (o, t, m ) {
			indexers = i;
		}

	}

	/// <summary>
	/// Class to hold variable instances and type info
	/// </summary>
	public class VariableInfo : Info {

		private bool readOnly;
		public bool ReadOnly {
			get { return readOnly; }
			set { readOnly = value; }
		}

		private string name;
		public string Name {
			get { return name; }
		}

		public override object Value {
			get {
				if(!initialized)
					throw new UninitializedVariableException(name);
				return val; 
			}
			set {
				if(!readOnly) {
					val = value;
					initialized = true;
				}
				else
					throw new ReadonlyVariableException(name + "is readonly");
			}
		}

		private bool initialized;
		public bool Initialized { 
			get { return initialized; }
		}
		
		//add field for const?

		public VariableInfo(string n, Type t) : this(n, t, null) {
			initialized = false;
		}

		public VariableInfo(string n, Type t, object v) : base(v, t) {
			name = n;
			initialized = true;
			readOnly = false;
		}

		public override string ToString() {
			return "<VariableInfo: Name = " + this.name + ", Type = " + this.type.FullName;
		}

	}
}
