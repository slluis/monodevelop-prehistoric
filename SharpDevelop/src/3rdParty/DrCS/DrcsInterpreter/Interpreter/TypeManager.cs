using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;

using Rice.Drcsharp.Parser.AST.Expressions;
using Rice.Drcsharp.Parser;

/*
 * TODO:
 * seperate system and user Types so that we don't have to reinitialize everything each time we reset
 */

namespace Rice.Drcsharp.Interpreter {
	/// <summary>
	/// Summary description for TypeManager.
	/// </summary>
	public class TypeManager : MarshalByRefObject {
#region STATIC_TYPES
		//static private bool staticInit = false;
		static public Type object_type;
		static public Type value_type;
		static public Type string_type;
		static public Type int32_type;
		static public Type uint32_type;
		static public Type int64_type; 
		static public Type uint64_type;
		static public Type float_type;
		static public Type double_type;
		static public Type char_type;
		static public Type short_type;
		static public Type decimal_type;
		static public Type bool_type;
		static public Type sbyte_type;
		static public Type byte_type;
		static public Type ushort_type;
		static public Type enum_type;
		static public Type multicast_delegate_type;
		static public Type delegate_type;
		static public Type void_type;
		static public Type array_type;
		static public Type type_type;
		static public Type ienumerator_type;
		static public Type idisposable_type;
		static public Type icloneable_type;
		static public Type param_array_type;
		static public Type null_type;
		
		static private string[] SystemAssemblies = { "System.Configuration.Install", "System.Data", "System.Design", "System.DirectoryServices",
													   "System.Drawing.Design", "System.Drawing", "System.EnterpriseServices", "System.Management",
													   "System.Messageing", "System.Runtime.Remoting", "System.Runtime.Serialization", 
													   "System.Security", "System.ServiceProcess", "System.Web", "System.Web.RegularExpression",
													   "System.Web.Services", "System.Windows.Forms", "System.Xml" };
#endregion
		
#region PROPERTIES
		//key: object's short name, value: fully qualified names
		protected Hashtable shortNameCache;
		public Hashtable ShortNameCache {
			get { return shortNameCache; }
		}

		//key: fully-qualified name, value: type
		protected Hashtable typeCache;
		public Hashtable TypeCache {
			get { return typeCache; }
		}

		//once a namespace is added, it cannot be removed
		protected ArrayList usedNamespaces;
		public virtual ArrayList UsedNamespaces {
			get { return usedNamespaces; }
		}
		
		//processed list of available namespaces
		//a Hashtable because we want a set
		protected Hashtable validNamespaces;
		public Hashtable ValidNamespaces {
			get { return validNamespaces; }
		}

		//key: assembly name, value: Assembly object
		protected Hashtable loadedAssemblies;
		public Hashtable LoadedAssemblies {
			get { return loadedAssemblies; }
		}

		private const string defaultSystemAssemblyDir = @"c:\WINNT\Microsoft.NET\Framework\v1.0.3705\";

		private static string systemAssemblyDir;
		public static string SystemAssemblyDir {
			get { return systemAssemblyDir; }
			set { systemAssemblyDir = value; }
		}

		private string userAssemblyDir;
		public string UserAssemblyDir {
			get { return userAssemblyDir; }
			set { userAssemblyDir = value; }
		}
#endregion

		

#region CONSTRUCTOR

		public TypeManager() {
			Debug("<TypeManager()>");
			//AppDomain ad = AppDomain.CurrentDomain;
			//ad.ShadowCopyFiles = "true";
			shortNameCache = new Hashtable();
			typeCache = new Hashtable();
			usedNamespaces = new ArrayList();	
			validNamespaces = new Hashtable();
			loadedAssemblies = new Hashtable();
			SetSystemAssemblyDir();
			LoadDefaultSystemAssemblies();
			InitCoreTypes();
			Debug("</TypeManager()>");
		}
		
		private void Debug(string s) {
			DB.tm(s);
		}

		private void InitCoreTypes() {			
			object_type   = CoreLookupType ("System.Object", "object");
			value_type    = CoreLookupType ("System.ValueType");

			int32_type    = CoreLookupType ("System.Int32", "int");
			int64_type    = CoreLookupType ("System.Int64", "long");
			uint32_type   = CoreLookupType ("System.UInt32", "uint"); 
			uint64_type   = CoreLookupType ("System.UInt64", "ulong"); 
			byte_type     = CoreLookupType ("System.Byte", "byte");
			sbyte_type    = CoreLookupType ("System.SByte", "sbyte");
			short_type    = CoreLookupType ("System.Int16", "short");
			ushort_type   = CoreLookupType ("System.UInt16", "ushort");
			char_type     = CoreLookupType ("System.Char", "char");
			string_type   = CoreLookupType ("System.String", "string");
			float_type    = CoreLookupType ("System.Single", "float");
			double_type   = CoreLookupType ("System.Double", "double");
			decimal_type  = CoreLookupType ("System.Decimal", "decimal");
			bool_type     = CoreLookupType ("System.Boolean", "bool");
			enum_type     = CoreLookupType ("System.Enum", "enum");

			multicast_delegate_type = CoreLookupType ("System.MulticastDelegate");
			delegate_type           = CoreLookupType ("System.Delegate", "delegate");

			array_type    = CoreLookupType ("System.Array");
			void_type     = CoreLookupType ("System.Void", "void");
			type_type     = CoreLookupType ("System.Type");

			ienumerator_type     = CoreLookupType ("System.Collections.IEnumerator");
			idisposable_type     = CoreLookupType ("System.IDisposable");
			icloneable_type      = CoreLookupType ("System.ICloneable");
			param_array_type      = CoreLookupType ("System.ParamArrayAttribute");
	
			null_type = NullType.Instance;
			//staticInit = true; 
		}

		/// <summary>
		///   Looks up a type, and aborts if it is not found.  This is used
		///   by types required by the compiler
		/// </summary>
		private Type CoreLookupType (string name) {
			string shortName = name.Substring(name.LastIndexOf('.') + 1);
			return CoreLookupType(name, shortName);
		}
			
		private Type CoreLookupType(string name, string shortName) {
			Type t = LookupType (name);
			if (t == null){
				Console.WriteLine("Error loading up core types. Must abort");
				//should change this to throw exception. allow user to point to system assembly dir
				Environment.Exit (0);
			}
			typeCache[name] = t;
			shortNameCache[shortName] = name;
			return t;
		}

		private void LoadDefaultSystemAssemblies() {
			//ExtractTypesAndNamespaces(LoadSystemAssembly("mscorlib.dll"), false);
			//ExtractTypesAndNamespaces(LoadSystemAssembly("System.dll"), false);
			LoadSystemAssembly("mscorlib.dll");
			LoadSystemAssembly("System.dll");
			AddUsedNamespace("System");
		}

		private static void SetSystemAssemblyDir () {
			Assembly [] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
			foreach (Assembly a in assemblies){
				string codebase = a.CodeBase;
				if (codebase.EndsWith ("corlib.dll")){
					systemAssemblyDir = codebase.Substring (0, codebase.LastIndexOf ("/")) + "/";
					return;
				}
			}
			systemAssemblyDir = defaultSystemAssemblyDir;	
		}

		//		public void Reset() {
		////can't just reset the hashtables cuz then i have to reinitialize all the static types to refill the haashtable.
		//			//perhaps need seperate hash for user types
		//		}
#endregion
		
		/// <summary>
		///   Returns the Type associated with name
		/// </summary>
		public Type LookupType(string name) {
			Debug("<LookupType name=" + name + ">");
			name = name.Replace('+', '.'); //get rid of internal types

			string fullName = (string)shortNameCache[name];
			if(fullName == null)
				fullName = name;

			//if we have the fullname, find it
			Type t = (Type) typeCache [fullName];
			if(t != null) {
				Debug("found in typeCache: " + fullName);
				Debug("</LookupType>");
				return t;
			}

			//didn't have the shortname cached
			foreach(string s in usedNamespaces) {
				string possibleName = s + "." + name;
				if(typeCache[possibleName] != null) {
					shortNameCache[name] = possibleName;
					Debug("</LookupType>");
					return (Type)typeCache[possibleName];
				}
			}
			
			Debug("</LookupType> ERROR THROWN");
			throw new MissingTypeOrNamespaceException(name);
		}

#region IS_Stuff
		public static bool IsBuiltinType (Type t) {
			if (t == object_type || t == string_type || t == int32_type || t == uint32_type ||
				t == int64_type || t == uint64_type || t == float_type || t == double_type ||
				t == char_type || t == short_type || t == decimal_type || t == bool_type ||
				t == sbyte_type || t == byte_type || t == ushort_type)
				return true;
			else
				return false;
		}

		public static bool IsWholeNumber(Type t) {
			if(t == int32_type || t == uint32_type || t == int64_type || t == uint64_type || t == char_type ||
				t == short_type || t == ushort_type || t == sbyte_type || t == byte_type)
				return true;
			else
				return false;
		}

		public static bool IsDelegateType (Type t) {
			if (t.IsSubclassOf (TypeManager.delegate_type))
				return true;
			else
				return false;
		}
	
		public static bool IsEnumType (Type t) {
			if (t.IsSubclassOf (TypeManager.enum_type))
				return true;
			else
				return false;
		}

		/// <remarks>
		///  The following is used to check if a given type implements an interface.
		///  The cache helps us reduce the expense of hitting Type.GetInterfaces everytime.
		/// </remarks>
		public static bool ImplementsInterface (Type t, Type iface) {
			Type [] interfaces;
			//
			// FIXME OPTIMIZATION:
			// as soon as we hit a non-TypeBuiler in the interface
			// chain, we could return, as the `Type.GetInterfaces'
			// will return all the interfaces implement by the type
			// or its parents.
			//
			do {
				interfaces = GetInterfaces (t);
				if (interfaces != null){
					foreach (Type i in interfaces){
						if (i == iface)
							return true;
					}
				}
				t = t.BaseType;
			} while (t != null);
			return false;
		}

		public virtual bool IsValidNamespace(string ns) {
			return validNamespaces.Contains(ns);
		}

		public virtual bool IsValidType(string s) {
			try {
				Type t = LookupType(s);
				return true;
			}
			catch(MissingTypeOrNamespaceException) {
				return false;
			}
		}
			
#endregion

#region ADD_Stuff
		public virtual void AddUsedNamespace(string ns) {
			Debug("<AddUsedNamespace>");
			Debug("trying to add namespace: " + ns);
			int system = Array.IndexOf(SystemAssemblies, ns);
			if(system != -1) {
				Debug("have system assembly we didn't load");
				SystemAssemblies[system] = "";
				//ExtractTypesAndNamespaces(LoadSystemAssembly(ns + ".dll"), false);
				LoadSystemAssembly(ns + ".dll");
				if(validNamespaces.Contains(ns)) {
					if(!usedNamespaces.Contains(ns)) {
						usedNamespaces.Add(ns);
					}
				}
			}
			else if(validNamespaces.Contains(ns)) {
				Debug("ns is a valid namespace");
				if(!usedNamespaces.Contains(ns)) {
					Debug("adding namespace: " + ns);
					usedNamespaces.Add(ns);
				}
			}
				/*
			else if(Array.IndexOf(SystemAssemblies, ns) != -1) {
				//have a system assembly that we don't load
				Debug("have system assembly we didn't load");
				ExtractTypesAndNamespaces(LoadSystemAssembly(ns + ".dll"), false);
				if(validNamespaces.Contains(ns)) {
					if(!usedNamespaces.Contains(ns)) {
						usedNamespaces.Add(ns);
					}
				}
			}
			*/
			else {
				throw new MissingTypeOrNamespaceException(ns);
			}
			Debug("</AddUSedNamespace>");
		}

		public virtual void AddValidNamespace(string ns) {
			//Debug("adding valid namespace == " + ns);
			this.validNamespaces[ns] = null;
		}
#endregion
		
#region GET_Stuff
		
		public string GetFullyQualifiedName(string name) {
			if(shortNameCache[name] != null) {
				string retName = (string)shortNameCache[name];
				return retName;
			}
			LookupType(name);
			return (string)shortNameCache[name];
		}

		public bool ValidQualifiedName(string name) {
			if(typeCache[name] != null) {
				return true;
			}
			return false;
		}
		
		/// <summary>
		///   This function returns the interfaces in the type `t'.
		/// </summary>
		public static Type [] GetInterfaces (Type t) {
			//
			// The reason for catching the Array case is that Reflection.Emit
			// will not return a TypeBuilder for Array types of TypeBuilder types,
			// but will still throw an exception if we try to call GetInterfaces
			// on the type.
			//
			// Since the array interfaces are always constant, we return those for
			// the System.Array
			//
			if (t.IsArray)
				t = TypeManager.array_type;
			return t.GetInterfaces ();
		}
		//
		// This is needed, because enumerations from assemblies
		// do not report their underlyingtype, but they report
		// themselves
		//
		public static Type EnumToUnderlying (Type t) {
			t = t.UnderlyingSystemType;
			if (!TypeManager.IsEnumType (t))
				return t;
		
			TypeCode tc = Type.GetTypeCode (t);

			switch (tc){
				case TypeCode.Boolean:
					return TypeManager.bool_type;
				case TypeCode.Byte:
					return TypeManager.byte_type;
				case TypeCode.SByte:
					return TypeManager.sbyte_type;
				case TypeCode.Char:
					return TypeManager.char_type;
				case TypeCode.Int16:
					return TypeManager.short_type;
				case TypeCode.UInt16:
					return TypeManager.ushort_type;
				case TypeCode.Int32:
					return TypeManager.int32_type;
				case TypeCode.UInt32:
					return TypeManager.uint32_type;
				case TypeCode.Int64:
					return TypeManager.int64_type;
				case TypeCode.UInt64:
					return TypeManager.uint64_type;
			}
			throw new Exception ("Unhandled typecode in enum" + tc);
		}

		
#endregion

#region ASSEMBLY_Stuff
		/// <summary>
		/// Loads an assembly from the fully specified path
		/// </summary>
		/// <param name="assemPath">path to the assembly</param> 
		public virtual Assembly LoadAssemblyPath(string assemFullPath) {
			Debug("<LoadAssemblyPath>");
			Debug("assemFullPath = " + assemFullPath);
			
			
			Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
			
			//Debug("CurrentDomain == " + AppDomain.CurrentDomain);
			//Debug("setup privatebinprobe == " + AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe);
			//Debug("shadow copy == " + AppDomain.CurrentDomain.ShadowCopyFiles);
			Debug("***************************");
			Debug("loaded[] == ");
			foreach(Assembly ass in loaded) {
				Debug("assembly == " + ass);
				//Debug("codebase == " + ass.CodeBase);
				//Debug("location == "+ ass.Location);
			}
			Debug("***************************");
			

			//Assembly assem = Assembly.LoadFrom(assemFullPath);
			Assembly assem = null;

			bool loadedAlready = false;
			foreach(Assembly ass in loaded) {
				Debug("previous loaded assembly: " + ass);
				if(ass.CodeBase.Equals(assemFullPath) || ass.Location.Equals(assemFullPath)) {
					Debug("found a match! loadedAlready = true");
					loadedAlready = true;
					assem = ass;
					break;
				}
			}
			if(loadedAlready) {
				Debug("loadedAlready == true");
				//loaded in to AppDomain but not into our array of Assemblies
				Debug("loaded into AppDomain but not into loadedAssemblies[]");
				loadedAssemblies[assemFullPath] = assem;
			}
			else {
				Debug("loadedAlready == false");
				//need to load into AppDomain and load in to our array of Assemblies
				Debug("need to load into AppDomain and add to loadedAssemblies[]");

				Debug("calling AppDomain.CurrentDomain.Load("+assemFullPath+");");
				try {
					//assem = AppDomain.CurrentDomain.Load(assemFullPath);
					assem = Assembly.LoadFrom(assemFullPath);
					Debug("returned from call");
				}
				catch(Exception e) {
					Debug("EXCEPTION!" + e);
				}

				Debug("adding to LoadedAssemblies table");
				loadedAssemblies[assemFullPath] = assem;
			}

			loaded = AppDomain.CurrentDomain.GetAssemblies();
			Debug("===========================");
			Debug("about to leave LoadAssemblypath");
			Debug("loaded[] == ");
			foreach(Assembly ass in loaded) {
				Debug("assembly == " + ass);
				//Debug("codebase == " + ass.CodeBase);
				//Debug("location == "+ ass.Location);
			}
			Debug("===========================");

			Debug("</LoadAssemblyPath>");
			return assem;
		}
		
		/// <summary>
		/// Loads an system assembly from the default system assembly directory.
		/// </summary>
		/// <param name="assemName">name of system assembly (note, it should not be the full path.)</param>
		public void LoadSystemAssembly(string assemName) {
			Debug("<LoadSystemAssemlby>");
			string assemFullPath = systemAssemblyDir + assemName;
			//Assembly assem = LoadAssemblyPath(systemAssemblyDir + assemName);
			if(loadedAssemblies[assemFullPath] != null) {
				Debug("already loaded: " + assemFullPath);
				Debug("</LoadSystemAssemlby>");
				return;
				//return (Assembly)loadedAssemblies[assemFullPath];
			}
			/*
						Assembly assem = Assembly.LoadFrom(assemFullPath);
						Assembly[] loaded = AppDomain.CurrentDomain.GetAssemblies();
						bool loadedAlready = false;
						foreach(Assembly ass in loaded) {
							if(ass.Equals(assem)) {
								loadedAlready = true;
								assem = ass;
							}
						}
						if(loadedAlready) {
							//loaded in to AppDomain but not into our array of Assemblies
							loadedAssemblies[assemFullPath] = assem;
						}
						else {
							//need to load into AppDomain and load in to our array of Assemblies
							assem = LoadAssemblyPath(assemFullPath);
						}
			*/
			Assembly assem = LoadAssemblyPath(assemFullPath);
			ExtractTypesAndNamespaces(assem, false);

			Debug("</LoadSystemAssemlby>");
			
		}

		/// <summary>
		/// Loads an assembly specified by the user in the specified user assembly directory.
		/// </summary>
		/// <param name="assemName">name of assembly to load (note, it shoudl not be the full path.)</param>
		public void LoadUserAssembly(string assemName) {
			LoadUserAssemblyFullPath(userAssemblyDir + assemName);
		}

		public void LoadUserAssemblyFullPath(string assemFullPath) {
			Debug("<LoadUserAssemblyFullPath>");
			Debug("assemFullPath = " + assemFullPath);

			if(loadedAssemblies[assemFullPath] != null) {
				//already loaded
				Debug("already loaded: " + assemFullPath);
				Debug("</LoadUserAssemblyFullPath>");
				return;
				//return (Assembly)loadedAssemblies[assemFullPath];
			}

			Assembly assem = LoadAssemblyPath(assemFullPath);

			Debug("LoadUserAssemblyFullPath... extracting types");
			ExtractTypesAndNamespaces(assem, true);

			Debug("</LoadUserAssemblyFullPath>");
		}
		/*
		public virtual Assembly LoadExecutingAssembly() {
			Debug("<LoadExecutingAssembly>");
			Assembly assem = Assembly.GetExecutingAssembly();
			LoadAssembly(assem);
			return assem;
		}

		protected virtual void LoadAssembly(Assembly assem) {
			Debug("<LoadAssembly>");

			string assemName = assem.FullName.Substring(0, assem.FullName.IndexOf(','));

			Debug("assemName = "+assemName);

			if(loadedAssemblies[assemName] == null) {
				Debug("adding to LoadedAssemblies table");
				loadedAssemblies[assemName] = assem;
			}

			Debug("</LoadAssembly>");
		}
*/
#endregion

		/// <summary>
		/// Finds and adds the Types in an Assembly to the typeCache. 
		/// Also finds the list of valid namespaces loaded.
		/// </summary>
		/// <param name="assem">Assembly to extract data from</param>
		/// <param name="addNS">true if extracted namespaces are added to the using list</param>
		protected virtual void ExtractTypesAndNamespaces(Assembly assem, bool addNS) {
			Debug("<ExtractTypesAndNamespaces>");
			Debug("assembly == " + assem.FullName);
			Debug("addNS == " + addNS);

			//InterpreterProxy.writeToFile("ExtractTypesAndNamespaces: assem = "+assem);
			Debug("assem.GetTypes()");
			Type[] types = assem.GetTypes();
			//InterpreterProxy.writeToFile("ExtractTypesAndNamespaces: types = "+types);
			Debug("got types");

			foreach(Type t in types) {
				string tName = t.FullName;

				//InterpreterProxy.writeToFile("ExtractTypesAndNamespaces: type = "+tName);
				//Debug("extracting type == " + tName);

				//cache type info
				if(!tName.StartsWith("<PrivateImplementationDetails>")) { //also enum and value types, check ClassSemantics
					string plainName = tName.Replace('+', '.'); //get rid of internal types
					typeCache[plainName] = t;
					//Debug("extracted type == " + tName);
					//get namespace info
					int start = 0;
					int last = tName.LastIndexOf('.');
					/*int last = tName.IndexOf('.',start);
					while(last != -1) {
						string ns = tName.Substring(start, last);
						if(DB.adb) {
							if(ns == "System.Drawing") {
								Debug("aseembly : " + assem + " contains System.Drawing");
								Debug("type == " + t);
							}
						}
						AddValidNamespace(ns);
						if(addNS) {
							AddUsedNamespace(ns);
						}
						last = tName.IndexOf('.', last+1);
					}
					*/
					if(last != -1) {
						string ns = tName.Substring(start, last);
						if(ns.Length > 0) {
							AddValidNamespace(ns);
						}
						if(addNS) {
							AddUsedNamespace(ns);
						}
					}
				}

			}
			Debug("</ExtractTypesAndNamespaces>");

			//InterpreterProxy.writeToFile("ExtractTypesAndNamespaces: done");
		}



#region CONVERSION_STUFF
		
		/*******************************************************************
		 * 
		 * Conversion Stuff below
		 * Note: We do not support User Defined Conversions
		 * 
		 ********************************************************************
		 */
		
		/// <summary>
		///  Determines if a standard implicit conversion exists from
		///  expr_type to target_type
		/// </summary>
		public static bool StandardConversionExists (Expression expr, Type target_type) {
			Type expr_type = expr.Type;
			
			if (expr_type == target_type)
				return true;

			// First numeric conversions 
			if (expr_type == TypeManager.sbyte_type){
				// From sbyte to short, int, long, float, double, decimal
				if ((target_type == TypeManager.int32_type) || 
					(target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.float_type)  ||
					(target_type == TypeManager.short_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.byte_type){
				// From byte to short, ushort, int, uint, long, ulong, float, double, decimal
				if ((target_type == TypeManager.short_type) ||
					(target_type == TypeManager.ushort_type) ||
					(target_type == TypeManager.int32_type) ||
					(target_type == TypeManager.uint32_type) ||
					(target_type == TypeManager.uint64_type) ||
					(target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.short_type){
				// From short to int, long, float, double , decimal
				if ((target_type == TypeManager.int32_type) ||
					(target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.ushort_type){
				// From ushort to int, uint, long, ulong, float, double, decimal
				if ((target_type == TypeManager.uint32_type) ||
					(target_type == TypeManager.uint64_type) ||
					(target_type == TypeManager.int32_type) ||
					(target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.int32_type){
				// From int to long, float, double, decimal
				if ((target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.uint32_type){
				// From uint to long, ulong, float, double, decimal
				if ((target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.uint64_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.decimal_type))
					return true;				
			} 
			else if ((expr_type == TypeManager.uint64_type) || (expr_type == TypeManager.int64_type)) {
				// From long/ulong to float, double
				if ((target_type == TypeManager.double_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.char_type){
				// From char to ushort, int, uint, long, ulong, float, double, decimal
				if ((target_type == TypeManager.ushort_type) ||
					(target_type == TypeManager.int32_type) ||
					(target_type == TypeManager.uint32_type) ||
					(target_type == TypeManager.uint64_type) ||
					(target_type == TypeManager.int64_type) ||
					(target_type == TypeManager.float_type) ||
					(target_type == TypeManager.double_type) ||
					(target_type == TypeManager.decimal_type))
					return true;
			} 
			else if (expr_type == TypeManager.float_type){
				// float to double
				if (target_type == TypeManager.double_type)
					return true;
			}	
			
			if (ImplicitReferenceConversionExists (expr, target_type))
				return true;
			
			if (expr is IntConstant){
				int value = ((IntConstant) expr).Value;
				if (target_type == TypeManager.sbyte_type){
					if (value >= SByte.MinValue && value <= SByte.MaxValue)
						return true;
				} else if (target_type == TypeManager.byte_type){
					if (Byte.MinValue >= 0 && value <= Byte.MaxValue)
						return true;
				} else if (target_type == TypeManager.short_type){
					if (value >= Int16.MinValue && value <= Int16.MaxValue)
						return true;
				} else if (target_type == TypeManager.ushort_type){
					if (value >= UInt16.MinValue && value <= UInt16.MaxValue)
						return true;
				} else if (target_type == TypeManager.uint32_type){
					if (value >= 0)
						return true;
				} else if (target_type == TypeManager.uint64_type){
					//
					// we can optimize this case: a positive int32
					// always fits on a uint64.  But we need an opcode
					// to do it.
					//
					if (value >= 0)
						return true;
				}
				
				if (value == 0 && expr is IntLit && TypeManager.IsEnumType (target_type))
					return true;
			}

			if (expr is LongConstant && target_type == TypeManager.uint64_type){
				//
				// Try the implicit constant expression conversion
				// from long to ulong, instead of a nice routine,
				// we just inline it
				//
				long v = ((LongConstant) expr).Value;
				if (v >= 0)
					return true;
			}

			if (target_type.IsSubclassOf (TypeManager.enum_type) && expr is IntLit){
				IntLit i = (IntLit) expr;
				if (i.Value == 0)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Tests whether an implicit reference conversion exists between expr_type
		/// and target_type
		/// </summary>
		/// <param name="expr"></param>
		/// <param name="target_type"></param>
		/// <returns></returns>
		public static bool ImplicitReferenceConversionExists (Expression expr, Type target_type) {
			Type expr_type = expr.Type;
			return ImplicitReferenceConversionExists(expr_type, target_type);
		}

		public static bool ImplicitReferenceConversionExists (Type expr_type, Type target_type) {
			// This is the boxed case.
			if (target_type == TypeManager.object_type) {
				if ((expr_type.IsClass) ||
					(expr_type.IsValueType) ||
					(expr_type.IsInterface))
					return true;	
			} 
			else if (expr_type.IsSubclassOf (target_type)) {
				return true;
				
			} 
			else {
				// Please remember that all code below actually comes
				// from ImplicitReferenceConversion so make sure code remains in sync
				
				// from any class-type S to any interface-type T.
				if (expr_type.IsClass && target_type.IsInterface) {
					if (TypeManager.ImplementsInterface (expr_type, target_type))
						return true;
				}
				
				// from any interface type S to interface-type T.
				if (expr_type.IsInterface && target_type.IsInterface)
					if (TypeManager.ImplementsInterface (expr_type, target_type))
						return true;
				
				// from an array-type S to an array-type of type T
				if (expr_type.IsArray && target_type.IsArray) {
					if (expr_type.GetArrayRank () == target_type.GetArrayRank ()) {
						Type expr_element_type = expr_type.GetElementType ();
						EmptyExpression MyEmptyExpr = new EmptyExpression ();
						
						MyEmptyExpr.Type = expr_element_type;
						Type target_element_type = target_type.GetElementType ();
						
						if (!expr_element_type.IsValueType && !target_element_type.IsValueType)
							//??? should be ImplicitReferenceConversionExists? 6.1.4
							//was StandardConversionExists
							if (ImplicitReferenceConversionExists (MyEmptyExpr, target_element_type))
								return true;
					}
				}
				
				// from an array-type to System.Array
				if (expr_type.IsArray && target_type.IsAssignableFrom (expr_type))
					return true;
				
				// from any delegate type to System.Delegate
				if (expr_type.IsSubclassOf (TypeManager.delegate_type) && target_type == TypeManager.delegate_type)
					if (target_type.IsAssignableFrom (expr_type))
						return true;
					
				// from any array-type or delegate type into System.ICloneable.
				if (expr_type.IsArray || expr_type.IsSubclassOf (TypeManager.delegate_type))
					if (target_type == TypeManager.icloneable_type)
						return true;
				
				// from the null type to any reference-type.
				if (expr_type is NullType && !target_type.IsValueType)
					return true;
				
			}

			return false;
		}

		/// <summary>
		/// Implements Implicit Reference Conversions
		/// </summary>
		/// <param name="expr">expression to convert</param>
		/// <param name="target_type">target type</param>
		/// <returns>EmptyCast if success, otherwise null</returns>
		public static Expression ImplicitReferenceConversion (Expression expr, Type target_type) {
			Type expr_type = expr.Type;

			if (expr_type == null){
				throw new InvalidCastException("unable to convert from unknown type to: " + target_type);
			}
			
			if (target_type == TypeManager.object_type) {
				if (expr_type.IsPointer)
					throw new UnsupportedException("pointer types are not supported");
				if (expr_type.IsValueType)
					return new BoxedCast (expr);
				if (expr_type.IsClass || expr_type.IsInterface)
					return new EmptyCast (expr, target_type);
			} 
			else if (expr_type.IsSubclassOf (target_type)) {
				return new EmptyCast (expr, target_type);
			}
			else {

				// This code is kind of mirrored inside StandardConversionExists
				// with the small distinction that we only probe there
				//
				// Always ensure that the code here and there is in sync
				
				// from the null type to any reference-type.
				if (expr is NullLit && !target_type.IsValueType)
					return new EmptyCast (expr, target_type);

				// from any class-type S to any interface-type T.
				if (expr_type.IsClass && target_type.IsInterface) {
					if (TypeManager.ImplementsInterface (expr_type, target_type))
						return new EmptyCast (expr, target_type);
					else
						return null;
				}

				// from any interface type S to interface-type T.
				if (expr_type.IsInterface && target_type.IsInterface) {
					if (TypeManager.ImplementsInterface (expr_type, target_type))
						return new EmptyCast (expr, target_type);
					else
						return null;
				}
				
				// from an array-type S to an array-type of type T
				if (expr_type.IsArray && target_type.IsArray) {
					if (expr_type.GetArrayRank () == target_type.GetArrayRank ()) {
						Type expr_element_type = expr_type.GetElementType ();

						EmptyExpression MyEmptyExpr = new EmptyExpression ();
						
						MyEmptyExpr.Type = expr_element_type;
						Type target_element_type = target_type.GetElementType ();

						if (!expr_element_type.IsValueType && !target_element_type.IsValueType)
							//changed from StandardConversionExisits
							if (ImplicitReferenceConversionExists (MyEmptyExpr,	target_element_type)) //should be implicit ref exists???
								return new EmptyCast (expr, target_type);
					}
				}				
				
				// from an array-type to System.Array
				if (expr_type.IsArray && target_type == TypeManager.array_type)
					return new EmptyCast (expr, target_type);
				
				// from any delegate type to System.Delegate
				if (expr_type.IsSubclassOf (TypeManager.delegate_type) && target_type == TypeManager.delegate_type)
					return new EmptyCast (expr, target_type);
					
				// from any array-type or delegate type into System.ICloneable.
				if (expr_type.IsArray || expr_type.IsSubclassOf (TypeManager.delegate_type))
					if (target_type == TypeManager.icloneable_type)
						return new EmptyCast (expr, target_type);
				return null;

			}
			return null;
		}

		
		/// <summary>
		///   Implicit Numeric Conversions.
		///
		///   expr is the expression to convert, returns a new expression of type
		///   target_type or null if an implicit conversion is not possible.
		/// </summary>
		public static Expression ImplicitNumericConversion (Expression expr, Type target_type) {
			Type expr_type = expr.Type;
			
			//
			// Attempt to do the implicit constant expression conversions
			if (expr is IntConstant){
				Expression e;
				
				e = TryImplicitConstantExpressionConversion (target_type, (IntConstant) expr);

				if (e != null)
					return e;
			} 
			else if (expr is LongConstant && target_type == TypeManager.uint64_type){
				//
				// Try the implicit constant expression conversion
				// from long to ulong, instead of a nice routine,
				// we just inline it
				//
				long v = ((LongConstant) expr).Value;
				if (v >= 0)
					return new ULongConstant ((ulong) v);
			}

			//
			// If we have an enumeration, extract the underlying type,
			// use this during the comparission, but wrap around the original
			// target_type
			//
			Type real_target_type = target_type;

			if (TypeManager.IsEnumType (real_target_type))
				real_target_type = TypeManager.EnumToUnderlying (real_target_type);

			if (expr_type == real_target_type)
				return new EmptyCast (expr, target_type);
			
			if (expr_type == TypeManager.sbyte_type){
				// From sbyte to short, int, long, float, double, decimal
				if ((real_target_type == TypeManager.int32_type) || 
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.float_type)  ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast(expr, target_type);
			} 
			else if (expr_type == TypeManager.byte_type){
				//
				// From byte to short, ushort, int, uint, long, ulong, float, double
				// 
				if ((real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			} 
			else if (expr_type == TypeManager.short_type){
				//
				// From short to int, long, float, double
				// 
				if ((real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.decimal_type) )
					return new EmptyCast (expr, target_type);
			} 
			else if (expr_type == TypeManager.ushort_type){
				//
				// From ushort to int, uint, long, ulong, float, double
				//
				if ((real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.int32_type){
				//
				// From int to long, float, double
				//
				if ((real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			} 
			else if (expr_type == TypeManager.uint32_type){
				//
				// From uint to long, ulong, float, double
				//
				if ((real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			}
			else if ((expr_type == TypeManager.uint64_type) || (expr_type == TypeManager.int64_type)){
				//
				// From long/ulong to float, double
				//
				if ((real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.char_type){
				//
				// From char to ushort, int, uint, long, ulong, float, double
				// 
				if ((real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.double_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.float_type){
				//
				// float to double
				//
				if (real_target_type == TypeManager.double_type)
					return new EmptyCast (expr, target_type);
			}

			return null;
		}

	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="expr"></param>
		/// <param name="target_type"></param>
		/// <returns></returns>
		/// <remarks>We do not support user defined conversions.</remarks>
		public static bool ImplicitConversionExists (Expression expr, Type target_type) {
			if (StandardConversionExists (expr, target_type))
				return true;
			return false;
		}
	
	
		/// <summary>
		///   Converts implicitly the resolved expression `expr' into the
		///   `target_type'.  It returns a new expression that can be used
		///   in a context that expects a `target_type'. 
		/// </summary>
		public static Expression ConvertImplicit (Expression expr, Type target_type) {
			Type expr_type = expr.Type;
			Expression e;

			if (expr_type == target_type)
				return expr;

			if (target_type == null)
				throw new InvalidCastException("Target type is null");
			
			if (TypeManager.IsEnumType(target_type) && expr is IntLit){
				IntLit i = (IntLit) expr;

				if (i.Value == 0)
					return new EnumConstant ((IntLit)expr, target_type);
			}
			
			e = ImplicitNumericConversion (expr, target_type);
			if (e != null)
				return e;

			e = ImplicitReferenceConversion (expr, target_type);
			if (e != null)
				return e;

			return null;
		}

		/// <summary>
		///   Attemps to perform an implict constant conversion of the IntConstant
		///   into a different data type using casts (See Implicit Constant
		///   Expression Conversions)
		/// </summary>
		static protected Expression TryImplicitConstantExpressionConversion (Type target_type, IntConstant ic) {
			int value = ic.Value;
			if (target_type == TypeManager.sbyte_type){
				if (value >= SByte.MinValue && value <= SByte.MaxValue)
					return new SByteConstant ((sbyte) value);
			} else if (target_type == TypeManager.byte_type){
				if (Byte.MinValue >= 0 && value <= Byte.MaxValue)
					return new ByteConstant ((byte) value);
			} else if (target_type == TypeManager.short_type){
				if (value >= Int16.MinValue && value <= Int16.MaxValue)
					return new ShortConstant ((short) value);
			} else if (target_type == TypeManager.ushort_type){
				if (value >= UInt16.MinValue && value <= UInt16.MaxValue)
					return new UShortConstant ((ushort) value);
			} else if (target_type == TypeManager.uint32_type){
				if (value >= 0)
					return new UIntConstant ((uint) value);
			} else if (target_type == TypeManager.uint64_type){
				//
				// we can optimize this case: a positive int32
				// always fits on a uint64.  But we need an opcode
				// to do it.
				//
				if (value >= 0)
					return new ULongConstant ((ulong) value);
			}
			
			//			if (value == 0 && ic is IntLiteral && TypeManager.IsEnumType (target_type))
			//				return new EnumConstant (ic, target_type);

			return null;
		}


		/// <summary>
		///  Returns whether an explicit reference conversion can be performed
		///  from source_type to target_type
		/// </summary>
		public static bool ExplicitReferenceConversionExists (Type source_type, Type target_type) {
			bool target_is_value_type = target_type.IsValueType;
			
			if (source_type == target_type)
				return true;
			
			//
			// From object to any reference type
			//
			if (source_type == TypeManager.object_type && !target_is_value_type)
				return true;
					
			//
			// From any class S to any class-type T, provided S is a base class of T
			//
			if (target_type.IsSubclassOf (source_type))
				return true;

			//
			// From any interface type S to any interface T provided S is not derived from T
			//
			if (source_type.IsInterface && target_type.IsInterface){
				if (!target_type.IsSubclassOf (source_type))
					return true;
			}
			    
			//
			// From any class type S to any interface T, provided S is not sealed
			// and provided S does not implement T.
			//
			if (target_type.IsInterface && !source_type.IsSealed &&
				!TypeManager.ImplementsInterface (source_type, target_type))
				return true;

			//
			// From any interface-type S to to any class type T, provided T is not
			// sealed, or provided T implements S.
			//
			if (source_type.IsInterface &&
				(!target_type.IsSealed || TypeManager.ImplementsInterface (target_type, source_type)))
				return true;
			
			
			// From an array type S with an element type Se to an array type T with an 
			// element type Te provided all the following are true:
			//     * S and T differe only in element type, in other words, S and T
			//       have the same number of dimensions.
			//     * Both Se and Te are reference types
			//     * An explicit referenc conversions exist from Se to Te
			//
			if (source_type.IsArray && target_type.IsArray) {
				if (source_type.GetArrayRank () == target_type.GetArrayRank ()) {
					
					Type source_element_type = source_type.GetElementType ();
					Type target_element_type = target_type.GetElementType ();
					
					if (!source_element_type.IsValueType && !target_element_type.IsValueType)
						if (ExplicitReferenceConversionExists (source_element_type,	target_element_type))
							return true;
				}
			}
			

			// From System.Array to any array-type
			if (source_type == TypeManager.array_type &&
				target_type.IsSubclassOf (TypeManager.array_type)){
				return true;
			}

			//
			// From System delegate to any delegate-type
			//
			if (source_type == TypeManager.delegate_type &&
				target_type.IsSubclassOf (TypeManager.delegate_type))
				return true;

			//
			// From ICloneable to Array or Delegate types
			//
			if (source_type == TypeManager.icloneable_type &&
				(target_type == TypeManager.array_type ||
				target_type == TypeManager.delegate_type))
				return true;
			
			return false;
		}
		
				
		/// <summary>
		///   Implements Explicit Reference conversions
		/// </summary>
		static Expression ConvertReferenceExplicit (Expression source, Type target_type) {
			Type source_type = source.Type;
			bool target_is_value_type = target_type.IsValueType;

			//
			// From object to any reference type
			//
			if (source_type == TypeManager.object_type && !target_is_value_type)
				return new ClassCast (source, target_type);

			//
			// From any class S to any class-type T, provided S is a base class of T
			//
			if (target_type.IsSubclassOf (source_type))
				return new ClassCast (source, target_type);

			//
			// From any interface type S to any interface T provided S is not derived from T
			//
			if (source_type.IsInterface && target_type.IsInterface){
				if (TypeManager.ImplementsInterface (source_type, target_type))
					return null;
				else
					return new ClassCast (source, target_type);
			}
			    
			//
			// From any class type S to any interface T, provides S is not sealed
			// and provided S does not implement T.
			//
			if (target_type.IsInterface && !source_type.IsSealed) {
				if (TypeManager.ImplementsInterface (source_type, target_type))
					return null;
				else
					return new ClassCast (source, target_type);				
			}

			//
			// From any interface-type S to to any class type T, provided T is not
			// sealed, or provided T implements S.
			//
			if (source_type.IsInterface) {
				if (!target_type.IsSealed || TypeManager.ImplementsInterface (target_type, source_type))
					return new ClassCast (source, target_type);
				else
					return null;
			}
			
			// From an array type S with an element type Se to an array type T with an 
			// element type Te provided all the following are true:
			//     * S and T differe only in element type, in other words, S and T
			//       have the same number of dimensions.
			//     * Both Se and Te are reference types
			//     * An explicit referenc conversions exist from Se to Te
			//
			if (source_type.IsArray && target_type.IsArray) {
				if (source_type.GetArrayRank () == target_type.GetArrayRank ()) {
					
					Type source_element_type = source_type.GetElementType ();
					Type target_element_type = target_type.GetElementType ();
					
					if (!source_element_type.IsValueType && !target_element_type.IsValueType)
						if (ExplicitReferenceConversionExists (source_element_type, target_element_type))
							return new ClassCast (source, target_type);
				}
			}
			
			// From System.Array to any array-type
			if (source_type == TypeManager.array_type && target_type.IsSubclassOf (TypeManager.array_type)){
				return new ClassCast (source, target_type);
			}

			//
			// From System delegate to any delegate-type
			//
			if (source_type == TypeManager.delegate_type &&	target_type.IsSubclassOf (TypeManager.delegate_type))
				return new ClassCast (source, target_type);

			//
			// From ICloneable to Array or Delegate types
			//
			if (source_type == TypeManager.icloneable_type &&
				(target_type == TypeManager.array_type || target_type == TypeManager.delegate_type))
				return new ClassCast (source, target_type);
			
			return null;
		}
		
		/// <summary>
		///   Performs the explicit numeric conversions
		/// </summary>
		static Expression ConvertNumericExplicit (Expression expr,	Type target_type) {
			Type expr_type = expr.Type;

			//
			// If we have an enumeration, extract the underlying type,
			// use this during the comparission, but wrap around the original
			// target_type
			//
			Type real_target_type = target_type;

			if (TypeManager.IsEnumType (real_target_type))
				real_target_type = TypeManager.EnumToUnderlying (real_target_type);

			if (expr_type == TypeManager.sbyte_type){
				//
				// From sbyte to byte, ushort, uint, ulong, char
				//
				if ((real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.sbyte_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.byte_type){
				//
				// From byte to sbyte and char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.char_type))
					return new EmptyCast (expr, target_type);
			} 
			else if (expr_type == TypeManager.short_type) {
				//
				// From short to sbyte, byte, ushort, uint, ulong, char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.char_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.ushort_type){
				//
				// From ushort to sbyte, byte, short, char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.char_type)) 
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.int32_type) {
				//
				// From int to sbyte, byte, short, ushort, uint, ulong, char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.char_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.uint32_type){
				//
				// From uint to sbyte, byte, short, ushort, int, char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.char_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.int64_type){
				//
				// From long to sbyte, byte, short, ushort, int, uint, ulong, char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.char_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.uint64_type){
				//
				// From ulong to sbyte, byte, short, ushort, int, uint, long, char
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.char_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.char_type){
				//
				// From char to sbyte, byte, short
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.float_type){
				//
				// From float to sbyte, byte, short,
				// ushort, int, uint, long, ulong, char
				// or decimal
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.char_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			}
			else if (expr_type == TypeManager.double_type) {
				//
				// From double or decimal to byte, sbyte, short,
				// ushort, int, uint, long, ulong,
				// char, float or decimal
				//
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.char_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.decimal_type))
					return new EmptyCast (expr, target_type);
			}
			else if(expr_type == TypeManager.decimal_type) {
				if ((real_target_type == TypeManager.sbyte_type) ||
					(real_target_type == TypeManager.byte_type) ||
					(real_target_type == TypeManager.short_type) ||
					(real_target_type == TypeManager.ushort_type) ||
					(real_target_type == TypeManager.int32_type) ||
					(real_target_type == TypeManager.uint32_type) ||
					(real_target_type == TypeManager.int64_type) ||
					(real_target_type == TypeManager.uint64_type) ||
					(real_target_type == TypeManager.char_type) ||
					(real_target_type == TypeManager.float_type) ||
					(real_target_type == TypeManager.double_type))
					return new EmptyCast (expr, target_type);
			}
			return null;
		}

		/// <summary>
		///   Performs an explicit conversion of the expression `expr' whose
		///   type is expr.Type to `target_type'.
		/// </summary>
		static public Expression ConvertExplicit (Expression expr, Type target_type) {
			Type expr_type = expr.Type;
			Expression ne = ConvertImplicit (expr, target_type);

			//check if implicit conversion
			if (ne != null)
				return ne;

			ne = ConvertNumericExplicit (expr, target_type);
			if (ne != null)
				return ne;

			//
			// Unboxing conversion.
			//
			if (expr_type == TypeManager.object_type && target_type.IsValueType)
				return new UnboxCast (expr, target_type);

			//
			// Enum types
			//
			if (expr_type.IsSubclassOf (TypeManager.enum_type)) {
				Expression e;

				//
				// FIXME: Is there any reason we should have EnumConstant
				// dealt with here instead of just using always the
				// UnderlyingSystemType to wrap the type?
				//
				if (expr is EnumConstant)
					e = ((EnumConstant) expr).Child;
				else {
					e = new EmptyCast (expr, TypeManager.EnumToUnderlying (expr_type));
				}
				
				Expression t = ConvertImplicit (e, target_type);
				if (t != null)
					return t;
				
				return ConvertNumericExplicit (e, target_type);
			}
			
			ne = ConvertReferenceExplicit (expr, target_type);
			if (ne != null)
				return ne;

			//Error_CannotConvertType (loc, expr_type, target_type);
			return null;
		}
		
		/// <summary>
		///   Converts the IntConstant, UIntConstant, LongConstant or
		///   ULongConstant into the integral target_type.   Notice
		///   that we do not return an `Expression' we do return
		///   a boxed integral type.
		///
		///   FIXME: Since I added the new constants, we need to
		///   also support conversions from CharConstant, ByteConstant,
		///   SByteConstant, UShortConstant, ShortConstant
		///
		///   This is used by the switch statement, so the domain
		///   of work is restricted to the literals above, and the
		///   targets are int32, uint32, char, byte, sbyte, ushort,
		///   short, uint64 and int64
		/// </summary>
		public static object ConvertIntLiteral (Constant c, Type target_type) {
			if (c.Type == target_type)
				return ((Constant) c).GetValue ();

			//
			// Make into one of the literals we handle, we dont really care
			// about this value as we will just return a few limited types
			// 
			if (c is EnumConstant)
				c = ((EnumConstant)c).WidenToCompilerConstant ();

			if (c is IntConstant){
				int v = ((IntConstant) c).Value;
				
				if (target_type == TypeManager.uint32_type){
					if (v >= 0)
						return (uint) v;
				}
				else if (target_type == TypeManager.char_type){
					if (v >= Char.MinValue && v <= Char.MaxValue)
						return (char) v;
				}
				else if (target_type == TypeManager.byte_type){
					if (v >= Byte.MinValue && v <= Byte.MaxValue)
						return (byte) v;
				}
				else if (target_type == TypeManager.sbyte_type){
					if (v >= SByte.MinValue && v <= SByte.MaxValue)
						return (sbyte) v;
				}
				else if (target_type == TypeManager.short_type){
					if (v >= Int16.MinValue && v <= UInt16.MaxValue)
						return (short) v;
				}
				else if (target_type == TypeManager.ushort_type){
					if (v >= UInt16.MinValue && v <= UInt16.MaxValue)
						return (ushort) v;
				} 
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type){
					if (v > 0)
						return (ulong) v;
				}
			} 
			else if (c is	UIntConstant){
				uint v = ((UIntConstant) c).Value;

				if (target_type == TypeManager.int32_type){
					if (v <= Int32.MaxValue)
						return (int) v;
				} 
				else if (target_type == TypeManager.char_type){
					if (v >= Char.MinValue && v <= Char.MaxValue)
						return (char) v;
				} 
				else if (target_type == TypeManager.byte_type){
					if (v <= Byte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.sbyte_type){
					if (v <= SByte.MaxValue)
						return (sbyte) v;
				} 
				else if (target_type == TypeManager.short_type){
					if (v <= UInt16.MaxValue)
						return (short) v;
				} 
				else if (target_type == TypeManager.ushort_type){
					if (v <= UInt16.MaxValue)
						return (ushort) v;
				} 
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type)
					return (ulong) v;
			} 
			else if (c is	LongConstant){ 
				long v = ((LongConstant) c).Value;

				if (target_type == TypeManager.int32_type){
					if (v >= UInt32.MinValue && v <= UInt32.MaxValue)
						return (int) v;
				} 
				else if (target_type == TypeManager.uint32_type){
					if (v >= 0 && v <= UInt32.MaxValue)
						return (uint) v;
				} 
				else if (target_type == TypeManager.char_type){
					if (v >= Char.MinValue && v <= Char.MaxValue)
						return (char) v;
				} 
				else if (target_type == TypeManager.byte_type){
					if (v >= Byte.MinValue && v <= Byte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.sbyte_type){
					if (v >= SByte.MinValue && v <= SByte.MaxValue)
						return (sbyte) v;
				} 
				else if (target_type == TypeManager.short_type){
					if (v >= Int16.MinValue && v <= UInt16.MaxValue)
						return (short) v;
				} 
				else if (target_type == TypeManager.ushort_type){
					if (v >= UInt16.MinValue && v <= UInt16.MaxValue)
						return (ushort) v;
				} 
				else if (target_type == TypeManager.uint64_type){
					if (v > 0)
						return (ulong) v;
				}
			} 
			else if (c is	ULongConstant){
				ulong v = ((ULongConstant) c).Value;

				if (target_type == TypeManager.int32_type){
					if (v <= Int32.MaxValue)
						return (int) v;
				} 
				else if (target_type == TypeManager.uint32_type){
					if (v <= UInt32.MaxValue)
						return (uint) v;
				} 
				else if (target_type == TypeManager.char_type){
					if (v >= Char.MinValue && v <= Char.MaxValue)
						return (char) v;
				} 
				else if (target_type == TypeManager.byte_type){
					if (v >= Byte.MinValue && v <= Byte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.sbyte_type){
					if (v <= (int) SByte.MaxValue)
						return (sbyte) v;
				} 
				else if (target_type == TypeManager.short_type){
					if (v <= UInt16.MaxValue)
						return (short) v;
				} 
				else if (target_type == TypeManager.ushort_type){
					if (v <= UInt16.MaxValue)
						return (ushort) v;
				} 
				else if (target_type == TypeManager.int64_type){
					if (v <= Int64.MaxValue)
						return (long) v;
				}
			} 
			else if (c is ByteConstant){
				byte v = ((ByteConstant) c).Value;
				
				if (target_type == TypeManager.int32_type)
					return (int) v;
				else if (target_type == TypeManager.uint32_type)
					return (uint) v;
				else if (target_type == TypeManager.char_type)
					return (char) v;
				else if (target_type == TypeManager.sbyte_type){
					if (v <= SByte.MaxValue)
						return (sbyte) v;
				} 
				else if (target_type == TypeManager.short_type)
					return (short) v;
				else if (target_type == TypeManager.ushort_type)
					return (ushort) v;
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type)
					return (ulong) v;
			} 
			else if (c is SByteConstant){
				sbyte v = ((SByteConstant) c).Value;
				
				if (target_type == TypeManager.int32_type)
					return (int) v;
				else if (target_type == TypeManager.uint32_type){
					if (v >= 0)
						return (uint) v;
				} 
				else if (target_type == TypeManager.char_type){
					if (v >= 0)
						return (char) v;
				} 
				else if (target_type == TypeManager.byte_type){
					if (v >= 0)
						return (byte) v;
				} 
				else if (target_type == TypeManager.short_type)
					return (short) v;
				else if (target_type == TypeManager.ushort_type){
					if (v >= 0)
						return (ushort) v;
				} 
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type){
					if (v >= 0)
						return (ulong) v;
				}
			} 
			else if (c is ShortConstant){
				short v = ((ShortConstant) c).Value;
				
				if (target_type == TypeManager.int32_type){
					return (int) v;
				} 
				else if (target_type == TypeManager.uint32_type){
					if (v >= 0)
						return (uint) v;
				} 
				else if (target_type == TypeManager.char_type){
					if (v >= 0)
						return (char) v;
				}
				else if (target_type == TypeManager.byte_type){
					if (v >= Byte.MinValue && v <= Byte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.sbyte_type){
					if (v >= SByte.MinValue && v <= SByte.MaxValue)
						return (sbyte) v;
				} 
				else if (target_type == TypeManager.ushort_type){
					if (v >= 0)
						return (ushort) v;
				} 
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type)
					return (ulong) v;
			} 
			else if (c is UShortConstant){
				ushort v = ((UShortConstant) c).Value;
				
				if (target_type == TypeManager.int32_type)
					return (int) v;
				else if (target_type == TypeManager.uint32_type)
					return (uint) v;
				else if (target_type == TypeManager.char_type){
					if (v >= Char.MinValue && v <= Char.MaxValue)
						return (char) v;
				} 
				else if (target_type == TypeManager.byte_type){
					if (v >= Byte.MinValue && v <= Byte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.sbyte_type){
					if (v <= SByte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.short_type){
					if (v <= Int16.MaxValue)
						return (short) v;
				} 
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type)
					return (ulong) v;
			} 
			else if (c is CharConstant){
				char v = ((CharConstant) c).Value;
				
				if (target_type == TypeManager.int32_type)
					return (int) v;
				else if (target_type == TypeManager.uint32_type)
					return (uint) v;
				else if (target_type == TypeManager.byte_type){
					if (v >= Byte.MinValue && v <= Byte.MaxValue)
						return (byte) v;
				} 
				else if (target_type == TypeManager.sbyte_type){
					if (v <= SByte.MaxValue)
						return (sbyte) v;
				} 
				else if (target_type == TypeManager.short_type){
					if (v <= Int16.MaxValue)
						return (short) v;
				} 
				else if (target_type == TypeManager.ushort_type)
					return (short) v;
				else if (target_type == TypeManager.int64_type)
					return (long) v;
				else if (target_type == TypeManager.uint64_type)
					return (ulong) v;
			}
			return null;
		}		
	
	#endregion
	}

	

	#region NULLTYPE
	public class NullType : Type {
		
		private static NullType instance = null;
		
		public static NullType Instance {
			get{
				if(instance == null)
					instance = new NullType();
				return instance;
			}
		}

		public NullType() : base () {}

		public override Guid GUID {
			get { return Guid.NewGuid(); }
		}
		public override object InvokeMember(string name,
			BindingFlags invokeAttr,
			Binder binder,
			object target,
			object[] args,
			ParameterModifier[] modifiers,
			CultureInfo culture,
			string[] namedParameters) {
			return null;
		}

		public override Module Module {
			get { return null; }
		}

		public override Assembly Assembly {
			get { return null; }
		}

		public override RuntimeTypeHandle TypeHandle {
			get { return typeof(void).TypeHandle; }
		}

		public override string FullName {
			get { return "null"; }
		}
		
		public override string Namespace {
			get { return "null"; }
		}

		public override string AssemblyQualifiedName {
			get { return "null"; }
		}

		public override Type BaseType {
			get { return null; }
		}
		
		protected override ConstructorInfo GetConstructorImpl( BindingFlags b, Binder b2, CallingConventions c, Type[] t, ParameterModifier[] m) {
			return null;
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags b) {
			return null;
		}
		
		protected override MethodInfo GetMethodImpl(string n, BindingFlags b, Binder b2, CallingConventions c, Type[] t, ParameterModifier[] m) {
			return null;
		}

		public override MethodInfo[] GetMethods (BindingFlags b) {
			return null;
		}

		public override FieldInfo GetField( string n, BindingFlags b) {
			return null;
		}

		public override FieldInfo[] GetFields(BindingFlags b) {
			return null;
		}

		public override Type GetInterface(string n, bool i) {
			return null;
		}

		public override Type[] GetInterfaces() {
			return null;
		}

		public override EventInfo GetEvent( string n, BindingFlags b) {
			return null;
		}

		public override EventInfo[] GetEvents(BindingFlags b) {
			return null;
		}

		protected override PropertyInfo GetPropertyImpl(string n, BindingFlags b, Binder b2, Type t, Type[] t2, ParameterModifier[] m) {
			return null;
		}
		
		public override PropertyInfo[] GetProperties(BindingFlags b) {
			return null;
		}

		public override Type GetNestedType(string n, BindingFlags b) {
			return null;
		}

		public override Type[] GetNestedTypes(BindingFlags b) {
			return null;
		}

		protected override TypeAttributes GetAttributeFlagsImpl() {
			return TypeAttributes.Class;
		}

		protected override bool IsArrayImpl() {
			return false;
		}

		protected override bool IsByRefImpl() {
			return false;
		}

		protected override bool IsPointerImpl() {
			return false;
		}
		
		protected override bool IsPrimitiveImpl() {
			return true;
		}

		protected override bool IsCOMObjectImpl() {
			return false;
		}

		public override Type GetElementType() {
			return null;
		}

		protected override bool HasElementTypeImpl() {
			return false;
		}

		public override Type UnderlyingSystemType {
			get { return null;	}
		}

		public override string Name {
			get { return "null"; }
		}
	
		public override bool IsDefined(Type t, bool b) {
			return false;
		}

		public override object[] GetCustomAttributes(bool i) {
			return null;
		}

		public override object[] GetCustomAttributes(Type a, bool b) {
			return null;
		}	

		public override MemberInfo[] GetMembers(BindingFlags b) {
			return null;
		}

	}
	#endregion
}
