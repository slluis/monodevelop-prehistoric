using System;

namespace ICSharpCode.SharpRefactory.Parser.VB
{
	public sealed class Tokens
	{
		/*----- terminal classes -----*/
		public const int EOF                = 0;
		public const int EOL                = 1;
		public const int Identifier         = 2;
		public const int LiteralString      = 3;
		public const int LiteralCharacter   = 4;
		public const int LiteralInteger     = 5;
		public const int LiteralDouble      = 6;
		public const int LiteralSingle      = 7;
		public const int LiteralDecimal     = 8;
		public const int LiteralDate        = 9;
		
		/*----- special character -----*/
		public const int Dot                = 10;
		public const int Assign             = 11;
		public const int NamedAssign        = 12;
		public const int Comma              = 13;
		public const int Colon              = 14;
		public const int Plus               = 15;
		public const int Minus              = 16;
		public const int Times              = 17;
		public const int Div                = 18;
		public const int DivInteger         = 19;
		public const int ConcatString       = 20;
		
		public const int OpenCurlyBrace     = 21;
		public const int CloseCurlyBrace    = 22;
		
		public const int OpenSquareBracket  = 23;
		public const int CloseSquareBracket = 24;
		
		public const int OpenParenthesis    = 25;
		public const int CloseParenthesis   = 26;
		
		public const int GreaterThan        = 27;
		public const int LessThan           = 28;
		
		public const int NotEqual           = 29;
		public const int GreaterEqual       = 30;
		public const int LessEqual          = 31;
		
		public const int ShiftLeft          = 32;
		public const int ShiftRight         = 33;

		public const int PlusAssign         = 34;
		public const int PowerAssign        = 35;
		public const int MinusAssign        = 36;
		public const int TimesAssign        = 37;
		public const int DivAssign          = 38;
		public const int DivIntegerAssign   = 39;
		public const int ShiftLeftAssign    = 40;
		public const int ShiftRightAssign   = 41;
		
		/*----- VB.NET keywords -----*/
		public const int AddHandler         = 42;
		public const int AddressOf          = 43;
		public const int Alias              = 44;
		public const int And                = 45;
		public const int AndAlso            = 46;
		public const int Ansi               = 47;
		public const int As                 = 48;
		public const int Assembly           = 49;
		public const int Auto               = 50;
		public const int Binary             = 51;
		public const int Boolean            = 52;
		public const int ByRef              = 53;
		public const int Byte               = 54;
		public const int ByVal              = 55;
		public const int Call               = 56;
		public const int Case               = 57;
		public const int Catch              = 58;
		public const int CBool              = 59;
		public const int CByte              = 60;
		public const int CChar              = 61;
		public const int CDate              = 62;
		public const int CDbl               = 63;
		public const int CDec               = 64;
		public const int Char               = 65;
		public const int CInt               = 66;
		public const int Class              = 67;
		public const int CLng               = 68;
		public const int CObj               = 69;
		public const int Compare            = 70;
		public const int Const              = 71;
		public const int CShort             = 72;
		public const int CSng               = 73;
		public const int CStr               = 74;
		public const int CType              = 75;
		public const int Date               = 76;
		public const int Decimal            = 77;
		public const int Declare            = 78;
		public const int Default            = 79;
		public const int Delegate           = 80;
		public const int Dim                = 81;
		public const int DirectCast         = 82;
		public const int Do                 = 83;
		public const int Double             = 84;
		public const int Each               = 85;
		public const int Else               = 86;
		public const int ElseIf             = 87;
		public const int End                = 88;
		public const int EndIf              = 89;
		public const int Enum               = 90;
		public const int Erase              = 91;
		public const int Error              = 92;
		public const int Event              = 93;
		public const int Exit               = 94;
		public const int Explicit           = 95;
		public const int False              = 96;
		public const int Finally            = 97;
		public const int For                = 98;
		public const int Friend             = 99;
		public const int Function           = 100;
		public const int Get                = 101;
		public new const int GetType            = 102;
		public const int GoSub              = 103;
		public const int GoTo               = 104;
		public const int Handles            = 105;
		public const int If                 = 106;
		public const int Implements         = 107;
		public const int Imports            = 108;
		public const int In                 = 109;
		public const int Inherits           = 110;
		public const int Integer            = 111;
		public const int Interface          = 112;
		public const int Is                 = 113;
		public const int Let                = 114;
		public const int Lib                = 115;
		public const int Like               = 116;
		public const int Long               = 117;
		public const int Loop               = 118;
		public const int Me                 = 119;
		public const int Mod                = 120;
		public const int Module             = 121;
		public const int MustInherit        = 122;
		public const int MustOverride       = 123;
		public const int MyBase             = 124;
		public const int MyClass            = 125;
		public const int Namespace          = 126;
		public const int New                = 127;
		public const int Next               = 128;
		public const int Not                = 129;
		public const int Nothing            = 130;
		public const int NotInheritable     = 131;
		public const int NotOverridable     = 132;
		public const int Object             = 133;
		public const int Off                = 134;
		public const int On                 = 135;
		public const int Option             = 136;
		public const int Optional           = 137;
		public const int Or                 = 138;
		public const int OrElse             = 139;
		public const int Overloads          = 140;
		public const int Overridable        = 141;
		public const int Override           = 142;
		public const int Overrides          = 143;
		public const int ParamArray         = 144;
		public const int Preserve           = 145;
		public const int Private            = 146;
		public const int Property           = 147;
		public const int Protected          = 148;
		public const int Public             = 149;
		public const int RaiseEvent         = 150;
		public const int ReadOnly           = 151;
		public const int ReDim              = 152;
		public const int RemoveHandler      = 153;
		public const int Resume             = 154;
		public const int Return             = 155;
		public const int Select             = 156;
		public const int Set                = 157;
		public const int Shadows            = 158;
		public const int Shared             = 159;
		public const int Short              = 160;
		public const int Single             = 161;
		public const int Static             = 162;
		public const int Step               = 163;
		public const int Stop               = 164;
		public const int Strict             = 165;
		public const int String             = 166;
		public const int Structure          = 167;
		public const int Sub                = 168;
		public const int SyncLock           = 169;
		public const int Text               = 170;
		public const int Then               = 171;
		public const int Throw              = 172;
		public const int To                 = 173;
		public const int True               = 174;
		public const int Try                = 175;
		public const int TypeOf             = 176;
		public const int Unicode            = 177;
		public const int Until              = 178;
		public const int Variant            = 179;
		public const int Wend               = 180;
		public const int When               = 181;
		public const int While              = 182;
		public const int With               = 183;
		public const int WithEvents         = 184;
		public const int WriteOnly          = 185;
		public const int Xor                = 186;

	}
}
