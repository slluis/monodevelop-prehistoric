using System;

namespace ICSharpCode.CsVbRefactory.Parser.AST
{
	
	[Flags]
	public enum Modifier
	{
		// Access 
		Private   = 0x0001,
		Internal  = 0x0002,
		Protected = 0x0004,
		Public    = 0x0008,
		Dim	      = 0x0010,	// VB.NET SPECIFIC
	 
		// Scope
		Abstract  = 0x0010, 
		Virtual   = 0x0020,
		Sealed    = 0x0040,
		Static    = 0x0080,
		Override  = 0x0100,
		Readonly  = 0x0200,
		Const	  = 0X0400,
		New       = 0x0800,
	 	 
		// Special 
		Extern    = 0x1000,
		Volatile  = 0x2000,
		Unsafe    = 0x4000,
		
		// Modifier scopes
		None      = 0x0000,
		
		Classes                         = New | Public | Protected | Internal | Private | Abstract | Sealed,
		Fields                          = New | Public | Protected | Internal | Private | Static   | Readonly | Volatile,
		PropertysEventsMethods          = New | Public | Protected | Internal | Private | Static   | Virtual  | Sealed   | Override | Abstract | Extern,
		Indexers                        = New | Public | Protected | Internal | Private | Virtual  | Sealed   | Override | Abstract | Extern,
		Operators                       = Public | Static | Extern,
		Constants                       = New | Public | Protected | Internal | Private,
		StructsInterfacesEnumsDelegates = New | Public | Protected | Internal | Private,
		StaticConstructors              = Extern | Static | Unsafe,
		Destructors                     = Extern | Unsafe,
		Constructors                    = Public | Protected | Internal | Private | Extern,
		
		All       = Private  | Internal | Protected | Public |
		            Abstract | Virtual  | Sealed    | Static | 
		            Override | Readonly | Const     | New    |
		            Extern   | Volatile | Unsafe
	}
	
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public enum Types
	{
		Class,
		Interface,
		Struct,
		Enum
	}
	
	public enum ParentType
	{
		ClassOrStruct,
		InterfaceOrEnum,
		Namespace,
		Unknown
	}
	
	public enum FieldDirection {
		None,
		In,
		Out,
		Ref
	}
	
	public enum Members
	{
		Constant,
		Field,
		Method,
		Property,
		Event,
		Indexer,
		Operator,
		Constructor,
		StaticConstructor,
		Destructor,
		NestedType
	}
	
	public enum ParamModifiers
	{
		In,
		Out,
		Ref,
		Params
	}
	public enum UnaryOperatorType
	{
		None,
		Not,
		BitNot,
		
		Minus,
		Plus,
		
		Increment,
		Decrement,
		
		PostIncrement,
		PostDecrement,
		
		Star,
		BitWiseAnd
	}
	
	public enum AssignmentOperatorType
	{
		None,
		Assign,
		
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		
		ShiftLeft,
		ShiftRight,
		
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
	}
	
	public enum BinaryOperatorType
	{
		None,
		Add,
		BitwiseAnd,
		BitwiseOr,
		LogicalAnd,
		LogicalOr,
		Divide,
		GreaterThan,
		GreaterThanOrEqual,
		
		Equality,
		InEquality,
		
		LessThan,
		LessThanOrEqual,
		Modulus,
		Multiply,
		Subtract,
		ValueEquality,
		
		// additional
		ShiftLeft,
		ShiftRight,
		IS,
		AS,
		ExclusiveOr,
	}
	
	
}
