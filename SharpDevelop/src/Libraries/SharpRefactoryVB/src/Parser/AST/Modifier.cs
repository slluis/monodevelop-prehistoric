using System;

namespace ICSharpCode.SharpRefactory.Parser.VB
{
	[Flags()]
	public enum Modifier
	{
		Shadows     = 0x0001,
		Public = 0x0002,
		Protected= 0x0004,
		Friend=0x0008,
		Private = 0x0010,
		Default = 0x0020,
		Shared   = 0x0040,
		Readonly=0x0080,
		Overridable= 0x0100,
		Overloads= 0x0200,
		NotInheritable   = 0x0400,
		Override=0x0800,
		MustInherit= 0x1000,
		NotOverridable = 0x2000,
		Writeonly = 0x3000,
		Static = 0x4000,
		None                            = 0x0000, //    0
		Constant                        = 0x001f, //   31
		StructsInterfacesEnumsDelegates = 0x007f, //   63
		Fields                          = 0x01ff, //  511 
		Classes                         = 0x143f, // 5183
		Destructors                     = 0x2020, // 8224
		Constructors                    = 0x203e, // 8254
		StaticConstructors              = 0x2060, // 8288
		Operators                       = 0x2062, // 8290
		Indexers                        = 0x3e3f, //15935
		PropertysEventsMethods          = 0x3e7f, //15999
		All                             = 0x3fff  //16383
	}
}
