
#line  1 "VBNET.ATG" 
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.SharpRefactory.Parser.AST.VB;
using ICSharpCode.SharpRefactory.Parser.VB;
using System;
using System.Reflection;

namespace ICSharpCode.SharpRefactory.Parser.VB {



public class Parser
{
	const int maxT = 187;

	const  bool   T            = true;
	const  bool   x            = false;
	const  int    minErrDist   = 2;
	const  string errMsgFormat = "-- line {0} col {1}: {2}";  // 0=line, 1=column, 2=text
	int    errDist             = minErrDist;
	Errors errors;
	Lexer  lexer;

	public Errors Errors {
		get {
			return errors;
		}
	}


#line  10 "VBNET.ATG" 
private string assemblyName = null;
public CompilationUnit compilationUnit;
private ArrayList importedNamespaces = null;
private Stack withStatements;

public string ContainingAssembly
{
	set { assemblyName = value; }
}

Token t
{
	get {
		return lexer.Token;
	}
}
Token la
{
	get {
		return lexer.LookAhead;
	}
}

/* Return the n-th token after the current lookahead token */
void StartPeek()
{
	lexer.StartPeek();
}

Token Peek()
{
	return lexer.Peek();
}

Token Peek (int n)
{
	lexer.StartPeek();
	Token x = la;
	while (n > 0) {
		x = lexer.Peek();
		n--;
	}
	return x;
}

public void Error(string s)
{
	if (errDist >= minErrDist) {
		errors.Error(la.line, la.col, s);
	}
	errDist = 0;
}

/*
	True, if "." is followed by an ident
*/
bool DotAndIdent () {
	return la.kind == Tokens.Dot && Peek(1).kind == Tokens.Identifier;
}

bool IsNotClosingParenthesis() {
	return la.kind != Tokens.CloseParenthesis;
}

/*
	True, if ident is followed by "="
*/
bool IdentAndAsgn () {
	if(la.kind == Tokens.Identifier) {
		if(Peek(1).kind == Tokens.Assign) return true;
		if(Peek(1).kind == Tokens.Colon && Peek(2).kind == Tokens.Assign) return true;
	}
	return false;
}

/*
	True, if ident is followed by "=" or by ":" and "="
*/
bool IsNamedAssign() {
	if(Peek(1).kind == Tokens.Assign) return true;
	if(Peek(1).kind == Tokens.NamedAssign) return true;
	return false;
}

bool IsObjectCreation() {
	return Peek(1).kind == Tokens.New;
}

/*
	True, if "<" is followed by the ident "assembly" or "module"
*/
bool IsGlobalAttrTarget () {
	Token pt = Peek(1);
	return la.kind == Tokens.LessThan && ( pt.val.ToLower() == "assembly" || pt.val.ToLower() == "module");
}

/*
	True if the next token is a "(" and is followed by "," or ")"
*/
bool IsDims()
{
	int peek = Peek(1).kind;
	return la.kind == Tokens.OpenParenthesis &&
		( peek == Tokens.CloseParenthesis || peek == Tokens.Comma);
}

/*
	True, if the comma is not a trailing one,
	like the last one in: a, b, c,
*/
bool NotFinalComma() {
	int peek = Peek(1).kind;
	return la.kind == Tokens.Comma &&
		   peek != Tokens.CloseCurlyBrace && peek != Tokens.CloseSquareBracket;
}

/*
	True, if the next token is "Else" and this one
	if followed by "If"
*/
bool IsElseIf()
{
	int peek = Peek(1).kind;
	return la.kind == Tokens.Else && peek == Tokens.If;
}

/*
	True if the next token is goto and this one is
	followed by minus ("-"), this is allowd in on
	error clauses
*/
bool IsNegativeLabelName()
{
	int peek = Peek(1).kind;
	return la.kind == Tokens.GoTo && peek == Tokens.Minus;
}

/*
	True if the next statement is a "Resume next" statement
*/
bool IsResumeNext()
{
	int peek = Peek(1).kind;
	return la.kind == Tokens.Resume && peek == Tokens.Next;
}

/*
	True, if ident/literal integer is followed by ":"
*/
bool IsLabel() {
	return (la.kind == Tokens.Identifier || la.kind == Tokens.LiteralInteger)
			&& Peek(1).kind == Tokens.Colon;
}

bool IsAssignment ()
{
	return IdentAndAsgn();
}

/*
	True, if lookahead is a local attribute target specifier,
	i.e. one of "event", "return", "field", "method",
	"module", "param", "property", or "type"
*/
bool IsLocalAttrTarget () {
	// TODO
	return false;
}



/*

*/
	void SynErr(int n)
	{
		if (errDist >= minErrDist) {
			errors.SynErr(lexer.LookAhead.line, lexer.LookAhead.col, n);
		}
		errDist = 0;
	}

	public void SemErr(string msg)
	{
		if (errDist >= minErrDist) {
			errors.Error(lexer.Token.line, lexer.Token.col, msg);
		}
		errDist = 0;
	}
	
	void Expect(int n)
	{
		if (lexer.LookAhead.kind == n) {
			lexer.NextToken();
		} else {
			SynErr(n);
		}
	}
	
	bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	void ExpectWeak(int n, int follow)
	{
		if (lexer.LookAhead.kind == n) {
			lexer.NextToken();
		} else {
			SynErr(n);
			while (!StartOf(follow)) {
				lexer.NextToken();
			}
		}
	}
	
	bool WeakSeparator(int n, int syFol, int repFol)
	{
		bool[] s = new bool[maxT + 1];
		
		if (lexer.LookAhead.kind == n) {
			lexer.NextToken();
			return true; 
		} else if (StartOf(repFol)) {
			return false;
		} else {
			for (int i = 0; i <= maxT; i++) {
				s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
			}
			SynErr(n);
			while (!s[lexer.LookAhead.kind]) {
				lexer.NextToken();
			}
			return StartOf(syFol);
		}
	}
	
	void VBNET() {

#line  384 "VBNET.ATG" 
		compilationUnit = new CompilationUnit();
		withStatements = new Stack();
		
		while (la.kind == 136) {
			OptionStmt();
		}
		while (la.kind == 108) {
			ImportsStmt();
		}
		while (
#line  389 "VBNET.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void OptionStmt() {

#line  394 "VBNET.ATG" 
		INode node = null; bool val = true; 
		Expect(136);

#line  395 "VBNET.ATG" 
		Point startPos = t.Location; 
		if (la.kind == 95) {
			lexer.NextToken();
			OptionValue(
#line  397 "VBNET.ATG" 
ref val);

#line  398 "VBNET.ATG" 
			node = new OptionExplicitDeclaration(val); 
		} else if (la.kind == 165) {
			lexer.NextToken();
			OptionValue(
#line  400 "VBNET.ATG" 
ref val);

#line  401 "VBNET.ATG" 
			node = new OptionStrictDeclaration(val); 
		} else if (la.kind == 70) {
			lexer.NextToken();
			if (la.kind == 51) {
				lexer.NextToken();

#line  403 "VBNET.ATG" 
				node = new OptionCompareDeclaration(CompareType.Binary); 
			} else if (la.kind == 170) {
				lexer.NextToken();

#line  404 "VBNET.ATG" 
				node = new OptionCompareDeclaration(CompareType.Text); 
			} else SynErr(188);
		} else SynErr(189);
		EndOfStmt();

#line  409 "VBNET.ATG" 
		node.StartLocation = startPos;
		node.EndLocation   = t.Location;
		compilationUnit.AddChild(node);
		
	}

	void ImportsStmt() {

#line  433 "VBNET.ATG" 
		ArrayList importClauses = new ArrayList();
		importedNamespaces = new ArrayList();
		object importClause;
		
		Expect(108);

#line  439 "VBNET.ATG" 
		Point startPos = t.Location;
		ImportsStatement importsStatement = new ImportsStatement(null);
		
		ImportClause(
#line  442 "VBNET.ATG" 
out importClause);

#line  442 "VBNET.ATG" 
		importClauses.Add(importClause); 
		while (la.kind == 13) {
			lexer.NextToken();
			ImportClause(
#line  444 "VBNET.ATG" 
out importClause);

#line  444 "VBNET.ATG" 
			importClauses.Add(importClause); 
		}
		EndOfStmt();

#line  448 "VBNET.ATG" 
		importsStatement.ImportClauses = importClauses;
		importsStatement.StartLocation = startPos;
		importsStatement.EndLocation   = t.Location;
		compilationUnit.AddChild(importsStatement);
		
	}

	void GlobalAttributeSection() {

#line  1548 "VBNET.ATG" 
		Point startPos = t.Location; 
		Expect(28);
		if (la.kind == 49) {
			lexer.NextToken();
		} else if (la.kind == 121) {
			lexer.NextToken();
		} else SynErr(190);

#line  1551 "VBNET.ATG" 
		string attributeTarget = t.val;
		ArrayList attributes = new ArrayList();
		ICSharpCode.SharpRefactory.Parser.AST.VB.Attribute attribute;
		
		Expect(14);
		Attribute(
#line  1555 "VBNET.ATG" 
out attribute);

#line  1555 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  1556 "VBNET.ATG" 
NotFinalComma()) {
			Expect(13);
			Attribute(
#line  1556 "VBNET.ATG" 
out attribute);

#line  1556 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 13) {
			lexer.NextToken();
		}
		Expect(27);
		EndOfStmt();

#line  1561 "VBNET.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  478 "VBNET.ATG" 
		Modifiers m = new Modifiers(this);
		AttributeSection section;
		ArrayList attributes = new ArrayList();
		string qualident;
		
		if (la.kind == 126) {
			lexer.NextToken();

#line  485 "VBNET.ATG" 
			Point startPos = t.Location;
			
			Qualident(
#line  487 "VBNET.ATG" 
out qualident);

#line  489 "VBNET.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(1);
			NamespaceBody();

#line  497 "VBNET.ATG" 
			node.EndLocation = t.Location;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 28) {
				AttributeSection(
#line  501 "VBNET.ATG" 
out section);

#line  501 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  502 "VBNET.ATG" 
m);
			}
			NonModuleDeclaration(
#line  502 "VBNET.ATG" 
m, attributes);
		} else SynErr(191);
	}

	void OptionValue(
#line  415 "VBNET.ATG" 
ref bool val) {
		if (la.kind == 135) {
			lexer.NextToken();

#line  417 "VBNET.ATG" 
			val = true; 
		} else if (la.kind == 134) {
			lexer.NextToken();

#line  419 "VBNET.ATG" 
			val = true; 
		} else SynErr(192);
	}

	void EndOfStmt() {
		if (la.kind == 1) {
			lexer.NextToken();
		} else if (la.kind == 14) {
			lexer.NextToken();
		} else SynErr(193);
	}

	void ImportClause(
#line  455 "VBNET.ATG" 
out object importClause) {

#line  457 "VBNET.ATG" 
		string qualident = null;
		string aliasident = null;
		importClause = null;
		
		if (
#line  461 "VBNET.ATG" 
IsAssignment()) {
			lexer.NextToken();

#line  461 "VBNET.ATG" 
			aliasident = t.val;  
			Expect(11);
		}
		Qualident(
#line  462 "VBNET.ATG" 
out qualident);

#line  464 "VBNET.ATG" 
		if (qualident != null && qualident.Length > 0) {
		if (aliasident != null) {
			importClause = new ImportsAliasDeclaration(aliasident, qualident);
		} else {
			importedNamespaces.Add(qualident);
			importClause = new ImportsDeclaration(qualident);
		}
		}
		
	}

	void Qualident(
#line  2184 "VBNET.ATG" 
out string qualident) {

#line  2185 "VBNET.ATG" 
		string name = String.Empty; 
		Expect(2);

#line  2186 "VBNET.ATG" 
		qualident = t.val; 
		while (
#line  2187 "VBNET.ATG" 
DotAndIdent()) {
			Expect(10);
			IdentifierOrKeyword(
#line  2187 "VBNET.ATG" 
out name);

#line  2187 "VBNET.ATG" 
			qualident += "." + name; 
		}
	}

	void NamespaceBody() {
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(88);
		Expect(126);
		Expect(1);
	}

	void AttributeSection(
#line  1619 "VBNET.ATG" 
out AttributeSection section) {

#line  1621 "VBNET.ATG" 
		string attributeTarget = "";
		ArrayList attributes = new ArrayList();
		ICSharpCode.SharpRefactory.Parser.AST.VB.Attribute attribute;
		
		
		Expect(28);

#line  1626 "VBNET.ATG" 
		Point startPos = t.Location; 
		if (
#line  1627 "VBNET.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 93) {
				lexer.NextToken();

#line  1628 "VBNET.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 155) {
				lexer.NextToken();

#line  1629 "VBNET.ATG" 
				attributeTarget = "return";
			} else {
				lexer.NextToken();

#line  1632 "VBNET.ATG" 
				string val = t.val.ToLower();
				if (val != "field"	|| val != "method" ||
					val != "module" || val != "param"  ||
					val != "property" || val != "type")
				Error("attribute target specifier (event, return, field," +
						"method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(14);
		}
		Attribute(
#line  1642 "VBNET.ATG" 
out attribute);

#line  1642 "VBNET.ATG" 
		attributes.Add(attribute); 
		while (
#line  1643 "VBNET.ATG" 
NotFinalComma()) {
			Expect(13);
			Attribute(
#line  1643 "VBNET.ATG" 
out attribute);

#line  1643 "VBNET.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 13) {
			lexer.NextToken();
		}
		Expect(27);

#line  1647 "VBNET.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  2352 "VBNET.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 149: {
			lexer.NextToken();

#line  2353 "VBNET.ATG" 
			m.Add(Modifier.Public); 
			break;
		}
		case 148: {
			lexer.NextToken();

#line  2354 "VBNET.ATG" 
			m.Add(Modifier.Protected); 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2355 "VBNET.ATG" 
			m.Add(Modifier.Friend); 
			break;
		}
		case 146: {
			lexer.NextToken();

#line  2356 "VBNET.ATG" 
			m.Add(Modifier.Private); 
			break;
		}
		case 159: {
			lexer.NextToken();

#line  2357 "VBNET.ATG" 
			m.Add(Modifier.Shared); 
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2358 "VBNET.ATG" 
			m.Add(Modifier.Shadows); 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  2359 "VBNET.ATG" 
			m.Add(Modifier.MustInherit); 
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2360 "VBNET.ATG" 
			m.Add(Modifier.NotInheritable); 
			break;
		}
		default: SynErr(194); break;
		}
	}

	void NonModuleDeclaration(
#line  506 "VBNET.ATG" 
Modifiers m, ArrayList attributes) {

#line  508 "VBNET.ATG" 
		string name = String.Empty;
		ArrayList names = null;
		
		switch (la.kind) {
		case 67: {

#line  511 "VBNET.ATG" 
			
			lexer.NextToken();

#line  514 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration();
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = t.Location;
			newType.Type = Types.Class;
			newType.Modifier = m.Modifier;
			newType.Attributes = attributes;
			
			Expect(2);

#line  522 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();
			if (la.kind == 110) {
				ClassBaseType(
#line  524 "VBNET.ATG" 
out name);

#line  524 "VBNET.ATG" 
				newType.BaseType = name; 
			}
			while (la.kind == 107) {
				TypeImplementsClause(
#line  525 "VBNET.ATG" 
out names);

#line  525 "VBNET.ATG" 
				newType.BaseInterfaces = names; 
			}
			ClassBody();

#line  528 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation;
			compilationUnit.BlockEnd();
			
			break;
		}
		case 121: {
			lexer.NextToken();

#line  532 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration();
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = t.Location;
			newType.Type = Types.Module;
			newType.Modifier = m.Modifier;
			newType.Attributes = attributes;
			
			Expect(2);

#line  540 "VBNET.ATG" 
			newType.Name = t.val; 
			Expect(1);
			ModuleBody();

#line  544 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation;
			compilationUnit.BlockEnd();
			
			break;
		}
		case 167: {
			lexer.NextToken();

#line  548 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration();
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = t.Location;
			newType.Type = Types.Structure;
			newType.Modifier = m.Modifier;
			newType.Attributes = attributes;
			ArrayList baseInterfaces = new ArrayList();
			
			Expect(2);

#line  557 "VBNET.ATG" 
			newType.Name = t.val; 
			Expect(1);
			while (la.kind == 107) {
				TypeImplementsClause(
#line  558 "VBNET.ATG" 
out baseInterfaces);
			}
			StructureBody();

#line  561 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation;
			compilationUnit.BlockEnd();
			
			break;
		}
		case 90: {
			lexer.NextToken();

#line  566 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration();
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = t.Location;
			newType.Type = Types.Enum;
			newType.Modifier = m.Modifier;
			newType.Attributes = attributes;
			
			Expect(2);

#line  574 "VBNET.ATG" 
			newType.Name = t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				PrimitiveTypeName(
#line  575 "VBNET.ATG" 
out name);

#line  575 "VBNET.ATG" 
				newType.BaseType = name; 
			}
			Expect(1);
			EnumBody();

#line  579 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation;
			compilationUnit.BlockEnd();
			
			break;
		}
		case 112: {
			lexer.NextToken();

#line  584 "VBNET.ATG" 
			TypeDeclaration newType = new TypeDeclaration();
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = t.Location;
			newType.Type = Types.Interface;
			newType.Modifier = m.Modifier;
			newType.Attributes = attributes;
			ArrayList baseInterfaces = new ArrayList();
			
			Expect(2);

#line  593 "VBNET.ATG" 
			newType.Name = t.val; 
			EndOfStmt();
			while (la.kind == 110) {
				InterfaceBase(
#line  594 "VBNET.ATG" 
out baseInterfaces);

#line  594 "VBNET.ATG" 
				newType.BaseInterfaces = baseInterfaces; 
			}
			InterfaceBody();

#line  597 "VBNET.ATG" 
			newType.EndLocation = t.EndLocation;
			compilationUnit.BlockEnd();
			
			break;
		}
		case 80: {
			lexer.NextToken();

#line  603 "VBNET.ATG" 
			DelegateDeclaration delegateDeclr = new DelegateDeclaration();
			ArrayList p = null;
			TypeReference type = null;
			delegateDeclr.StartLocation = t.Location;
			delegateDeclr.Modifier = m.Modifier;
			delegateDeclr.Attributes = attributes;
			
			if (la.kind == 168) {
				lexer.NextToken();
				Expect(2);

#line  611 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  612 "VBNET.ATG" 
out p);
					}
					Expect(26);

#line  612 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
			} else if (la.kind == 100) {
				lexer.NextToken();
				Expect(2);

#line  614 "VBNET.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  615 "VBNET.ATG" 
out p);
					}
					Expect(26);

#line  615 "VBNET.ATG" 
					delegateDeclr.Parameters = p; 
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  616 "VBNET.ATG" 
out type);

#line  616 "VBNET.ATG" 
					delegateDeclr.ReturnType = type; 
				}
			} else SynErr(195);
			Expect(1);

#line  620 "VBNET.ATG" 
			delegateDeclr.EndLocation = t.EndLocation;
			compilationUnit.AddChild(delegateDeclr);
			
			break;
		}
		default: SynErr(196); break;
		}
	}

	void ClassBaseType(
#line  799 "VBNET.ATG" 
out string name) {

#line  801 "VBNET.ATG" 
		TypeReference type;
		name = String.Empty;
		
		Expect(110);
		TypeName(
#line  805 "VBNET.ATG" 
out type);

#line  805 "VBNET.ATG" 
		name = type.Type; 
		Expect(1);
	}

	void TypeImplementsClause(
#line  1197 "VBNET.ATG" 
out ArrayList baseInterfaces) {

#line  1199 "VBNET.ATG" 
		baseInterfaces = new ArrayList();
		TypeReference type = null;
		
		Expect(107);
		TypeName(
#line  1202 "VBNET.ATG" 
out type);

#line  1204 "VBNET.ATG" 
		baseInterfaces.Add(type);
		
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  1207 "VBNET.ATG" 
out type);

#line  1208 "VBNET.ATG" 
			baseInterfaces.Add(type); 
		}
		EndOfStmt();
	}

	void ClassBody() {

#line  632 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(5)) {

#line  635 "VBNET.ATG" 
			ArrayList attributes = new ArrayList();
			Modifiers m = new Modifiers(this);
			
			while (la.kind == 28) {
				AttributeSection(
#line  638 "VBNET.ATG" 
out section);

#line  638 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(6)) {
				MemberModifier(
#line  639 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  640 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(67);
		Expect(1);
	}

	void ModuleBody() {

#line  663 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(5)) {

#line  666 "VBNET.ATG" 
			ArrayList attributes = new ArrayList();
			Modifiers m = new Modifiers(this);
			
			while (la.kind == 28) {
				AttributeSection(
#line  669 "VBNET.ATG" 
out section);

#line  669 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(6)) {
				MemberModifier(
#line  670 "VBNET.ATG" 
m);
			}
			ClassMemberDecl(
#line  671 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(121);
		Expect(1);
	}

	void StructureBody() {

#line  647 "VBNET.ATG" 
		AttributeSection section; 
		while (StartOf(5)) {

#line  650 "VBNET.ATG" 
			ArrayList attributes = new ArrayList();
			Modifiers m = new Modifiers(this);
			
			while (la.kind == 28) {
				AttributeSection(
#line  653 "VBNET.ATG" 
out section);

#line  653 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(6)) {
				MemberModifier(
#line  654 "VBNET.ATG" 
m);
			}
			StructureMemberDecl(
#line  655 "VBNET.ATG" 
m, attributes);
		}
		Expect(88);
		Expect(167);
		Expect(1);
	}

	void PrimitiveTypeName(
#line  2337 "VBNET.ATG" 
out string type) {

#line  2338 "VBNET.ATG" 
		type = String.Empty; 
		switch (la.kind) {
		case 52: {
			lexer.NextToken();

#line  2339 "VBNET.ATG" 
			type = "Boolean"; 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  2340 "VBNET.ATG" 
			type = "Date"; 
			break;
		}
		case 65: {
			lexer.NextToken();

#line  2341 "VBNET.ATG" 
			type = "Char"; 
			break;
		}
		case 166: {
			lexer.NextToken();

#line  2342 "VBNET.ATG" 
			type = "String"; 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  2343 "VBNET.ATG" 
			type = "Decimal"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  2344 "VBNET.ATG" 
			type = "Byte"; 
			break;
		}
		case 160: {
			lexer.NextToken();

#line  2345 "VBNET.ATG" 
			type = "Short"; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  2346 "VBNET.ATG" 
			type = "Integer"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  2347 "VBNET.ATG" 
			type = "Long"; 
			break;
		}
		case 161: {
			lexer.NextToken();

#line  2348 "VBNET.ATG" 
			type = "Single"; 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  2349 "VBNET.ATG" 
			type = "Double"; 
			break;
		}
		default: SynErr(197); break;
		}
	}

	void EnumBody() {

#line  678 "VBNET.ATG" 
		FieldDeclaration f; 
		while (la.kind == 2 || la.kind == 28) {
			EnumMemberDecl(
#line  680 "VBNET.ATG" 
out f);

#line  680 "VBNET.ATG" 
			compilationUnit.AddChild(f); 
		}
		Expect(88);
		Expect(90);
		Expect(1);
	}

	void InterfaceBase(
#line  1182 "VBNET.ATG" 
out ArrayList bases) {

#line  1184 "VBNET.ATG" 
		TypeReference type;
		bases = new ArrayList();
		
		Expect(110);
		TypeName(
#line  1188 "VBNET.ATG" 
out type);

#line  1188 "VBNET.ATG" 
		bases.Add(type); 
		while (la.kind == 13) {
			lexer.NextToken();
			TypeName(
#line  1191 "VBNET.ATG" 
out type);

#line  1191 "VBNET.ATG" 
			bases.Add(type); 
		}
		Expect(1);
	}

	void InterfaceBody() {
		while (StartOf(7)) {
			InterfaceMemberDecl();
		}
		Expect(88);
		Expect(112);
		Expect(1);
	}

	void FormalParameterList(
#line  1653 "VBNET.ATG" 
out ArrayList parameter) {

#line  1655 "VBNET.ATG" 
		parameter = new ArrayList();
		ParameterDeclarationExpression p;
		AttributeSection section;
		ArrayList attributes = new ArrayList();
		
		while (la.kind == 28) {
			AttributeSection(
#line  1660 "VBNET.ATG" 
out section);

#line  1660 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(8)) {
			FixedParameter(
#line  1662 "VBNET.ATG" 
out p);

#line  1664 "VBNET.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 13) {
				lexer.NextToken();

#line  1669 "VBNET.ATG" 
				attributes = new ArrayList(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 28) {
					AttributeSection(
#line  1670 "VBNET.ATG" 
out section);

#line  1670 "VBNET.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(8)) {
					FixedParameter(
#line  1672 "VBNET.ATG" 
out p);

#line  1672 "VBNET.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 144) {
					ParameterArray(
#line  1673 "VBNET.ATG" 
out p);

#line  1673 "VBNET.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(198);
			}
		} else if (la.kind == 144) {
			ParameterArray(
#line  1676 "VBNET.ATG" 
out p);

#line  1676 "VBNET.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(199);
	}

	void TypeName(
#line  1498 "VBNET.ATG" 
out TypeReference typeref) {

#line  1500 "VBNET.ATG" 
		int[] rank = null;
		
		NonArrayTypeName(
#line  1502 "VBNET.ATG" 
out typeref);
		ArrayTypeModifiers(
#line  1503 "VBNET.ATG" 
out rank);

#line  1505 "VBNET.ATG" 
		typeref = new TypeReference(typeref.Type, rank);
		
	}

	void MemberModifier(
#line  2363 "VBNET.ATG" 
Modifiers m) {
		switch (la.kind) {
		case 122: {
			lexer.NextToken();

#line  2364 "VBNET.ATG" 
			m.Add(Modifier.MustInherit);
			break;
		}
		case 79: {
			lexer.NextToken();

#line  2365 "VBNET.ATG" 
			m.Add(Modifier.Default);
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2366 "VBNET.ATG" 
			m.Add(Modifier.Friend);
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2367 "VBNET.ATG" 
			m.Add(Modifier.Shadows);
			break;
		}
		case 142: {
			lexer.NextToken();

#line  2368 "VBNET.ATG" 
			m.Add(Modifier.Override);
			break;
		}
		case 146: {
			lexer.NextToken();

#line  2369 "VBNET.ATG" 
			m.Add(Modifier.Private);
			break;
		}
		case 148: {
			lexer.NextToken();

#line  2370 "VBNET.ATG" 
			m.Add(Modifier.Protected);
			break;
		}
		case 149: {
			lexer.NextToken();

#line  2371 "VBNET.ATG" 
			m.Add(Modifier.Public);
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2372 "VBNET.ATG" 
			m.Add(Modifier.NotInheritable);
			break;
		}
		case 132: {
			lexer.NextToken();

#line  2373 "VBNET.ATG" 
			m.Add(Modifier.NotOverridable);
			break;
		}
		case 159: {
			lexer.NextToken();

#line  2374 "VBNET.ATG" 
			m.Add(Modifier.Shared);
			break;
		}
		case 141: {
			lexer.NextToken();

#line  2375 "VBNET.ATG" 
			m.Add(Modifier.Overridable);
			break;
		}
		case 140: {
			lexer.NextToken();

#line  2376 "VBNET.ATG" 
			m.Add(Modifier.Overloads);
			break;
		}
		case 151: {
			lexer.NextToken();

#line  2377 "VBNET.ATG" 
			m.Add(Modifier.Readonly);
			break;
		}
		case 185: {
			lexer.NextToken();

#line  2378 "VBNET.ATG" 
			m.Add(Modifier.Writeonly);
			break;
		}
		default: SynErr(200); break;
		}
	}

	void ClassMemberDecl(
#line  795 "VBNET.ATG" 
Modifiers m, ArrayList attributes) {
		StructureMemberDecl(
#line  796 "VBNET.ATG" 
m, attributes);
	}

	void StructureMemberDecl(
#line  810 "VBNET.ATG" 
Modifiers m, ArrayList attributes) {

#line  812 "VBNET.ATG" 
		TypeReference type = null;
		ArrayList p = null;
		Statement stmt = null;
		ArrayList variableDeclarators = new ArrayList();
		
		switch (la.kind) {
		case 67: case 80: case 90: case 112: case 121: case 167: {
			NonModuleDeclaration(
#line  817 "VBNET.ATG" 
m, attributes);
			break;
		}
		case 168: {
			lexer.NextToken();

#line  820 "VBNET.ATG" 
			Point startPos = t.Location; 
			if (la.kind == 2) {

#line  823 "VBNET.ATG" 
				string name = String.Empty;
				MethodDeclaration methodDeclaration;
				HandlesClause handlesClause = null;
				ImplementsClause implementsClause = null;
				
				lexer.NextToken();

#line  828 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  829 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				if (la.kind == 105 || la.kind == 107) {
					if (la.kind == 107) {
						ImplementsClause(
#line  832 "VBNET.ATG" 
out implementsClause);
					} else {
						HandlesClause(
#line  834 "VBNET.ATG" 
out handlesClause);
					}
				}
				Expect(1);

#line  839 "VBNET.ATG" 
				methodDeclaration = new MethodDeclaration(name, m.Modifier,  null, p, attributes);
				methodDeclaration.StartLocation = startPos;
				methodDeclaration.EndLocation   = t.EndLocation;
				
				methodDeclaration.HandlesClause = handlesClause;
				methodDeclaration.ImplementsClause = implementsClause;
				
				compilationUnit.AddChild(methodDeclaration);
				compilationUnit.BlockStart(methodDeclaration);
				
				Block(
#line  849 "VBNET.ATG" 
out stmt);

#line  851 "VBNET.ATG" 
				compilationUnit.BlockEnd();
				methodDeclaration.Body  = (BlockStatement)stmt;
				
				Expect(88);
				Expect(168);
				Expect(1);
			} else if (la.kind == 127) {
				lexer.NextToken();
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  856 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				Expect(1);
				Block(
#line  859 "VBNET.ATG" 
out stmt);
				Expect(88);
				Expect(168);
				Expect(1);

#line  862 "VBNET.ATG" 
				ConstructorDeclaration cd = new ConstructorDeclaration("New", m.Modifier, p, attributes); 
				cd.StartLocation = startPos;
				cd.Body = (BlockStatement)stmt;
				cd.EndLocation   = t.EndLocation;
				
			} else SynErr(201);
			break;
		}
		case 100: {
			lexer.NextToken();

#line  872 "VBNET.ATG" 
			string name = String.Empty;
			Point startPos = t.Location;
			MethodDeclaration methodDeclaration;
			HandlesClause handlesClause = null;
			ImplementsClause implementsClause = null;
			
			Expect(2);

#line  878 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  879 "VBNET.ATG" 
out p);
				}
				Expect(26);
			}
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  880 "VBNET.ATG" 
out type);
			}
			if (la.kind == 105 || la.kind == 107) {
				if (la.kind == 107) {
					ImplementsClause(
#line  883 "VBNET.ATG" 
out implementsClause);
				} else {
					HandlesClause(
#line  885 "VBNET.ATG" 
out handlesClause);
				}
			}
			Expect(1);

#line  890 "VBNET.ATG" 
			methodDeclaration = new MethodDeclaration(name, m.Modifier,  type, p, attributes);
			methodDeclaration.StartLocation = startPos;
			methodDeclaration.EndLocation   = t.EndLocation;
			
			methodDeclaration.HandlesClause = handlesClause;
			methodDeclaration.ImplementsClause = implementsClause;
			
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			Block(
#line  900 "VBNET.ATG" 
out stmt);

#line  902 "VBNET.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
			Expect(88);
			Expect(100);
			Expect(1);
			break;
		}
		case 78: {
			lexer.NextToken();

#line  910 "VBNET.ATG" 
			Point startPos = t.Location;
			CharsetModifier charsetModifer = CharsetModifier.None;
			string library = String.Empty;
			string alias = String.Empty;
			string name = String.Empty;
			
			if (StartOf(9)) {
				Charset(
#line  916 "VBNET.ATG" 
out charsetModifer);
			}
			if (la.kind == 168) {
				lexer.NextToken();
				Expect(2);

#line  919 "VBNET.ATG" 
				name = t.val; 
				Expect(115);
				Expect(3);

#line  920 "VBNET.ATG" 
				library = t.val; 
				if (la.kind == 44) {
					lexer.NextToken();
					Expect(3);

#line  921 "VBNET.ATG" 
					alias = t.val; 
				}
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  922 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				Expect(1);

#line  925 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, null, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = startPos;
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else if (la.kind == 100) {
				lexer.NextToken();
				Expect(2);

#line  932 "VBNET.ATG" 
				name = t.val; 
				Expect(115);
				Expect(3);

#line  933 "VBNET.ATG" 
				library = t.val; 
				if (la.kind == 44) {
					lexer.NextToken();
					Expect(3);

#line  934 "VBNET.ATG" 
					alias = t.val; 
				}
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  935 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  936 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  939 "VBNET.ATG" 
				DeclareDeclaration declareDeclaration = new DeclareDeclaration(name, m.Modifier, type, p, attributes, library, alias, charsetModifer);
				declareDeclaration.StartLocation = startPos;
				declareDeclaration.EndLocation   = t.EndLocation;
				compilationUnit.AddChild(declareDeclaration);
				
			} else SynErr(202);
			break;
		}
		case 93: {
			lexer.NextToken();

#line  949 "VBNET.ATG" 
			Point startPos = t.Location;
			EventDeclaration eventDeclaration;
			string name = String.Empty;
			ImplementsClause implementsClause = null;
			
			Expect(2);

#line  954 "VBNET.ATG" 
			name= t.val; 
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  956 "VBNET.ATG" 
out type);
			} else if (la.kind == 1 || la.kind == 25 || la.kind == 107) {
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  958 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
			} else SynErr(203);
			if (la.kind == 107) {
				ImplementsClause(
#line  960 "VBNET.ATG" 
out implementsClause);
			}

#line  962 "VBNET.ATG" 
			eventDeclaration = new EventDeclaration(type, m.Modifier, p, attributes, name, implementsClause);
			eventDeclaration.StartLocation = startPos;
			eventDeclaration.EndLocation = t.EndLocation;
			compilationUnit.AddChild(eventDeclaration);
			
			Expect(1);
			break;
		}
		case 2: case 81: {

#line  969 "VBNET.ATG" 
			Point startPos = t.Location; 
			if (la.kind == 81) {
				lexer.NextToken();
			}

#line  972 "VBNET.ATG" 
			m.Check(Modifier.Fields);
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = startPos; 
			
			VariableDeclarator(
#line  976 "VBNET.ATG" 
variableDeclarators);
			while (la.kind == 13) {
				lexer.NextToken();
				VariableDeclarator(
#line  977 "VBNET.ATG" 
variableDeclarators);
			}
			Expect(1);

#line  980 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			fd.Fields = variableDeclarators;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 71: {

#line  985 "VBNET.ATG" 
			m.Check(Modifier.Constant); 
			lexer.NextToken();

#line  988 "VBNET.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
			fd.StartLocation = t.Location;
			ArrayList constantDeclarators = new ArrayList();
			
			ConstantDeclarator(
#line  992 "VBNET.ATG" 
constantDeclarators);
			while (la.kind == 13) {
				lexer.NextToken();
				ConstantDeclarator(
#line  993 "VBNET.ATG" 
constantDeclarators);
			}

#line  995 "VBNET.ATG" 
			fd.EndLocation = t.Location;
			
			Expect(1);

#line  999 "VBNET.ATG" 
			fd.EndLocation = t.EndLocation;
			compilationUnit.AddChild(fd);
			
			break;
		}
		case 147: {
			lexer.NextToken();

#line  1005 "VBNET.ATG" 
			Point startPos = t.Location;
			ImplementsClause implementsClause = null;
			
			Expect(2);
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1009 "VBNET.ATG" 
out p);
				}
				Expect(26);
			}
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1010 "VBNET.ATG" 
out type);
			}
			if (la.kind == 107) {
				ImplementsClause(
#line  1011 "VBNET.ATG" 
out implementsClause);
			}
			Expect(1);

#line  1014 "VBNET.ATG" 
			PropertyDeclaration pDecl = new PropertyDeclaration(t.val, type, m.Modifier, attributes);
			pDecl.StartLocation = startPos;
			pDecl.EndLocation   = t.Location;
			pDecl.BodyStart   = t.Location;
			pDecl.TypeReference = type;
			pDecl.ImplementsClause = implementsClause;
			pDecl.Parameters = p;
			PropertyGetRegion getRegion;
			PropertySetRegion setRegion;
			
			AccessorDecls(
#line  1024 "VBNET.ATG" 
out getRegion, out setRegion);
			Expect(88);
			Expect(147);
			Expect(1);

#line  1028 "VBNET.ATG" 
			pDecl.GetRegion = getRegion;
			pDecl.SetRegion = setRegion;
			pDecl.BodyEnd = t.EndLocation;
			compilationUnit.AddChild(pDecl);
			
			break;
		}
		default: SynErr(204); break;
		}
	}

	void EnumMemberDecl(
#line  776 "VBNET.ATG" 
out FieldDeclaration f) {

#line  778 "VBNET.ATG" 
		Expression expr = null;
		ArrayList attributes = new ArrayList();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 28) {
			AttributeSection(
#line  783 "VBNET.ATG" 
out section);

#line  783 "VBNET.ATG" 
			attributes.Add(section); 
		}
		Expect(2);

#line  786 "VBNET.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 11) {
			lexer.NextToken();
			Expr(
#line  791 "VBNET.ATG" 
out expr);

#line  791 "VBNET.ATG" 
			varDecl.Initializer = expr; 
		}
		Expect(1);
	}

	void InterfaceMemberDecl() {

#line  698 "VBNET.ATG" 
		TypeReference type =null;
		ArrayList p = null;
		AttributeSection section;
		Modifiers mod = new Modifiers(this);
		ArrayList attributes = new ArrayList();
		ArrayList parameters = new ArrayList();
		string name;
		
		if (StartOf(10)) {
			while (la.kind == 28) {
				AttributeSection(
#line  706 "VBNET.ATG" 
out section);

#line  706 "VBNET.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(6)) {
				MemberModifier(
#line  710 "VBNET.ATG" 
mod);
			}
			if (la.kind == 93) {
				lexer.NextToken();
				Expect(2);

#line  713 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  714 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  715 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  718 "VBNET.ATG" 
				EventDeclaration ed = new EventDeclaration(type, mod.Modifier, p, attributes, name, null);
				compilationUnit.AddChild(ed);
				ed.EndLocation = t.EndLocation;
				
			} else if (la.kind == 168) {
				lexer.NextToken();
				Expect(2);

#line  723 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  724 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				Expect(1);

#line  727 "VBNET.ATG" 
				MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, null, p, attributes);
				md.EndLocation = t.EndLocation;
				compilationUnit.AddChild(md);
				
			} else if (la.kind == 100) {
				lexer.NextToken();
				Expect(2);

#line  732 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  733 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  734 "VBNET.ATG" 
out type);
				}

#line  736 "VBNET.ATG" 
				MethodDeclaration md = new MethodDeclaration(name, mod.Modifier, type, p, attributes);
				md.EndLocation = t.EndLocation;
				compilationUnit.AddChild(md);
				
				Expect(1);
			} else if (la.kind == 147) {
				lexer.NextToken();
				Expect(2);

#line  742 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 25) {
					lexer.NextToken();
					if (StartOf(4)) {
						FormalParameterList(
#line  743 "VBNET.ATG" 
out p);
					}
					Expect(26);
				}
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  744 "VBNET.ATG" 
out type);
				}
				Expect(1);

#line  747 "VBNET.ATG" 
				PropertyDeclaration pd = new PropertyDeclaration(name, type, mod.Modifier, attributes);
				pd.Parameters = p;
				pd.EndLocation = t.EndLocation;
				compilationUnit.AddChild(pd);
				
			} else if (la.kind == 90) {
				lexer.NextToken();

#line  754 "VBNET.ATG" 
				TypeDeclaration newType = new TypeDeclaration();
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = t.Location;
				newType.Type = Types.Enum;
				newType.Modifier = mod.Modifier;
				newType.Attributes = attributes;
				
				Expect(2);

#line  762 "VBNET.ATG" 
				newType.Name = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					PrimitiveTypeName(
#line  763 "VBNET.ATG" 
out name);

#line  763 "VBNET.ATG" 
					newType.BaseType = name; 
				}
				Expect(1);
				EnumBody();

#line  767 "VBNET.ATG" 
				newType.EndLocation = t.EndLocation;
				compilationUnit.BlockEnd();
				
			} else SynErr(205);
		} else if (StartOf(11)) {
			NonModuleDeclaration(
#line  772 "VBNET.ATG" 
mod, attributes);
		} else SynErr(206);
	}

	void Expr(
#line  1232 "VBNET.ATG" 
out Expression expr) {

#line  1233 "VBNET.ATG" 
		expr = new Expression(); 
		UnaryExpr(
#line  1234 "VBNET.ATG" 
out expr);
		if (StartOf(12)) {
			ConditionalOrExpr(
#line  1236 "VBNET.ATG" 
ref expr);
		} else if (StartOf(13)) {

#line  1237 "VBNET.ATG" 
			AssignmentOperatorType op; Expression val; 
			AssignmentOperator(
#line  1237 "VBNET.ATG" 
out op);
			Expr(
#line  1237 "VBNET.ATG" 
out val);

#line  1237 "VBNET.ATG" 
			expr = new AssignmentExpression(expr, op, val); 
		} else SynErr(207);
	}

	void ImplementsClause(
#line  1214 "VBNET.ATG" 
out ImplementsClause clause) {

#line  1216 "VBNET.ATG" 
		clause = new ImplementsClause();
		string typename = String.Empty;
		string first;
		
		Expect(107);
		Expect(2);

#line  1220 "VBNET.ATG" 
		first = t.val; 
		Expect(10);
		Qualident(
#line  1220 "VBNET.ATG" 
out typename);

#line  1220 "VBNET.ATG" 
		((ImplementsClause)clause).BaseMembers.Add(first + "." + typename); 
		while (la.kind == 13) {
			lexer.NextToken();
			Expect(2);

#line  1221 "VBNET.ATG" 
			first = t.val; 
			Expect(10);
			Qualident(
#line  1221 "VBNET.ATG" 
out typename);

#line  1221 "VBNET.ATG" 
			((ImplementsClause)clause).BaseMembers.Add(first + "." + typename); 
		}
	}

	void HandlesClause(
#line  1172 "VBNET.ATG" 
out HandlesClause handlesClause) {

#line  1174 "VBNET.ATG" 
		handlesClause = new HandlesClause();
		string name;
		
		Expect(105);
		EventMemberSpecifier(
#line  1177 "VBNET.ATG" 
out name);

#line  1177 "VBNET.ATG" 
		handlesClause.EventNames.Add(name); 
		while (la.kind == 13) {
			lexer.NextToken();
			EventMemberSpecifier(
#line  1178 "VBNET.ATG" 
out name);

#line  1178 "VBNET.ATG" 
			handlesClause.EventNames.Add(name); 
		}
	}

	void Block(
#line  1711 "VBNET.ATG" 
out Statement stmt) {

#line  1714 "VBNET.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		
		while (StartOf(14)) {
			Statement();
			EndOfStmt();
		}

#line  1720 "VBNET.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void Charset(
#line  1164 "VBNET.ATG" 
out CharsetModifier charsetModifier) {

#line  1165 "VBNET.ATG" 
		charsetModifier = CharsetModifier.None; 
		if (la.kind == 100 || la.kind == 168) {
		} else if (la.kind == 47) {
			lexer.NextToken();

#line  1166 "VBNET.ATG" 
			charsetModifier = CharsetModifier.ANSI; 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  1167 "VBNET.ATG" 
			charsetModifier = CharsetModifier.Auto; 
		} else if (la.kind == 177) {
			lexer.NextToken();

#line  1168 "VBNET.ATG" 
			charsetModifier = CharsetModifier.ANSI; 
		} else SynErr(208);
	}

	void VariableDeclarator(
#line  1110 "VBNET.ATG" 
ArrayList fieldDeclaration) {

#line  1112 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		ObjectCreateExpression oce = null;
		
		Expect(2);

#line  1118 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val);
		
		if (
#line  1121 "VBNET.ATG" 
IsObjectCreation()) {
			Expect(48);
			ObjectCreateExpression(
#line  1121 "VBNET.ATG" 
out oce);

#line  1123 "VBNET.ATG" 
			f.Initializer = oce;
			if(oce.CreateType != null) {
				f.Type = oce.CreateType;
			}
			
		} else if (StartOf(15)) {
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1129 "VBNET.ATG" 
out type);

#line  1129 "VBNET.ATG" 
				f.Type = type; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
				VariableInitializer(
#line  1130 "VBNET.ATG" 
out expr);

#line  1130 "VBNET.ATG" 
				f.Initializer = expr; 
			}
		} else SynErr(209);

#line  1132 "VBNET.ATG" 
		fieldDeclaration.Add(f); 
	}

	void ConstantDeclarator(
#line  1093 "VBNET.ATG" 
ArrayList constantDeclaration) {

#line  1095 "VBNET.ATG" 
		Expression expr = null;
		TypeReference type = null;
		string name = String.Empty;
		
		Expect(2);

#line  1099 "VBNET.ATG" 
		name = t.val; 
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  1100 "VBNET.ATG" 
out type);
		}
		Expect(11);
		Expr(
#line  1101 "VBNET.ATG" 
out expr);

#line  1103 "VBNET.ATG" 
		VariableDeclaration f = new VariableDeclaration(name, expr);
		f.Type = type;
		constantDeclaration.Add(f);
		
	}

	void AccessorDecls(
#line  1036 "VBNET.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1038 "VBNET.ATG" 
		ArrayList attributes = new ArrayList(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		
		while (la.kind == 28) {
			AttributeSection(
#line  1043 "VBNET.ATG" 
out section);

#line  1043 "VBNET.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 101) {
			GetAccessorDecl(
#line  1045 "VBNET.ATG" 
out getBlock, attributes);
			if (la.kind == 28 || la.kind == 157) {

#line  1047 "VBNET.ATG" 
				attributes = new ArrayList(); 
				while (la.kind == 28) {
					AttributeSection(
#line  1048 "VBNET.ATG" 
out section);

#line  1048 "VBNET.ATG" 
					attributes.Add(section); 
				}
				SetAccessorDecl(
#line  1049 "VBNET.ATG" 
out setBlock, attributes);
			}
		} else if (la.kind == 157) {
			SetAccessorDecl(
#line  1052 "VBNET.ATG" 
out setBlock, attributes);
			if (la.kind == 28 || la.kind == 101) {

#line  1054 "VBNET.ATG" 
				attributes = new ArrayList(); 
				while (la.kind == 28) {
					AttributeSection(
#line  1055 "VBNET.ATG" 
out section);

#line  1055 "VBNET.ATG" 
					attributes.Add(section); 
				}
				GetAccessorDecl(
#line  1056 "VBNET.ATG" 
out getBlock, attributes);
			}
		} else SynErr(210);
	}

	void GetAccessorDecl(
#line  1062 "VBNET.ATG" 
out PropertyGetRegion getBlock, ArrayList attributes) {

#line  1063 "VBNET.ATG" 
		Statement stmt = null; 
		Expect(101);
		Expect(1);
		Block(
#line  1066 "VBNET.ATG" 
out stmt);

#line  1068 "VBNET.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes);
		
		Expect(88);
		Expect(101);
		Expect(1);
	}

	void SetAccessorDecl(
#line  1075 "VBNET.ATG" 
out PropertySetRegion setBlock, ArrayList attributes) {

#line  1077 "VBNET.ATG" 
		Statement stmt = null;
		ArrayList p = null;
		
		Expect(157);
		if (la.kind == 25) {
			lexer.NextToken();
			if (StartOf(4)) {
				FormalParameterList(
#line  1081 "VBNET.ATG" 
out p);
			}
			Expect(26);
		}
		Expect(1);
		Block(
#line  1083 "VBNET.ATG" 
out stmt);

#line  1085 "VBNET.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes);
		setBlock.Parameters = p;
		
		Expect(88);
		Expect(157);
		Expect(1);
	}

	void ObjectCreateExpression(
#line  1451 "VBNET.ATG" 
out ObjectCreateExpression oce) {

#line  1453 "VBNET.ATG" 
		TypeReference type = null;
		ArrayList arguments = null;
		oce = null;
		
		Expect(127);
		TypeName(
#line  1457 "VBNET.ATG" 
out type);
		if (la.kind == 25) {
			lexer.NextToken();
			if (StartOf(16)) {
				ArgumentList(
#line  1459 "VBNET.ATG" 
out arguments);
			}
			Expect(26);
		}

#line  1463 "VBNET.ATG" 
		oce = new ObjectCreateExpression(type, arguments);
		
	}

	void VariableInitializer(
#line  1136 "VBNET.ATG" 
out Expression initializerExpression) {

#line  1138 "VBNET.ATG" 
		initializerExpression = null;
		
		if (StartOf(17)) {
			Expr(
#line  1140 "VBNET.ATG" 
out initializerExpression);
		} else if (la.kind == 21) {
			ArrayInitializer(
#line  1141 "VBNET.ATG" 
out initializerExpression);
		} else SynErr(211);
	}

	void ArrayInitializer(
#line  1145 "VBNET.ATG" 
out Expression outExpr) {

#line  1147 "VBNET.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(21);
		if (StartOf(18)) {
			VariableInitializer(
#line  1152 "VBNET.ATG" 
out expr);

#line  1154 "VBNET.ATG" 
			initializer.CreateExpressions.Add(expr);
			
			while (
#line  1157 "VBNET.ATG" 
NotFinalComma()) {
				Expect(13);
				VariableInitializer(
#line  1157 "VBNET.ATG" 
out expr);

#line  1158 "VBNET.ATG" 
				initializer.CreateExpressions.Add(expr); 
			}
		}
		Expect(22);

#line  1161 "VBNET.ATG" 
		outExpr = initializer; 
	}

	void EventMemberSpecifier(
#line  1224 "VBNET.ATG" 
out string name) {

#line  1225 "VBNET.ATG" 
		string type; name = String.Empty; 
		if (la.kind == 2) {
			lexer.NextToken();

#line  1226 "VBNET.ATG" 
			type = t.val; 
			Expect(10);
			Expect(2);

#line  1228 "VBNET.ATG" 
			name =  type + "." + t.val; 
		} else if (la.kind == 124) {
			lexer.NextToken();
			Expect(10);
			Expect(2);

#line  1229 "VBNET.ATG" 
			name = "MyBase." + t.val; 
		} else SynErr(212);
	}

	void UnaryExpr(
#line  1241 "VBNET.ATG" 
out Expression uExpr) {

#line  1243 "VBNET.ATG" 
		Expression expr;
		UnaryOperatorType uop = UnaryOperatorType.None;
		bool isUOp = false;
		
		while (StartOf(19)) {
			if (la.kind == 15) {
				lexer.NextToken();

#line  1248 "VBNET.ATG" 
				uop = UnaryOperatorType.Plus; isUOp = true; 
			} else if (la.kind == 16) {
				lexer.NextToken();

#line  1249 "VBNET.ATG" 
				uop = UnaryOperatorType.Minus; isUOp = true; 
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1250 "VBNET.ATG" 
				uop = UnaryOperatorType.Not;  isUOp = true;
			} else {
				lexer.NextToken();

#line  1251 "VBNET.ATG" 
				uop = UnaryOperatorType.Star;  isUOp = true;
			}
		}
		SimpleExpr(
#line  1253 "VBNET.ATG" 
out expr);

#line  1255 "VBNET.ATG" 
		if (isUOp) {
		uExpr = new UnaryOperatorExpression(expr, uop);
		} else {
			uExpr = expr;
		}
		
	}

	void ConditionalOrExpr(
#line  1342 "VBNET.ATG" 
ref Expression outExpr) {

#line  1343 "VBNET.ATG" 
		Expression expr; 
		ConditionalAndExpr(
#line  1344 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 139) {
			lexer.NextToken();
			UnaryExpr(
#line  1344 "VBNET.ATG" 
out expr);
			ConditionalAndExpr(
#line  1344 "VBNET.ATG" 
ref expr);

#line  1344 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BooleanOr, expr);  
		}
	}

	void AssignmentOperator(
#line  1263 "VBNET.ATG" 
out AssignmentOperatorType op) {

#line  1264 "VBNET.ATG" 
		op = AssignmentOperatorType.None; 
		switch (la.kind) {
		case 11: {
			lexer.NextToken();

#line  1265 "VBNET.ATG" 
			op = AssignmentOperatorType.Assign; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1266 "VBNET.ATG" 
			op = AssignmentOperatorType.Add; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1267 "VBNET.ATG" 
			op = AssignmentOperatorType.Subtract; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1268 "VBNET.ATG" 
			op = AssignmentOperatorType.Multiply; 
			break;
		}
		case 38: {
			lexer.NextToken();

#line  1269 "VBNET.ATG" 
			op = AssignmentOperatorType.Divide; 
			break;
		}
		case 39: {
			lexer.NextToken();

#line  1270 "VBNET.ATG" 
			op = AssignmentOperatorType.DivideInteger; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1271 "VBNET.ATG" 
			op = AssignmentOperatorType.Power; 
			break;
		}
		case 40: {
			lexer.NextToken();

#line  1272 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
			break;
		}
		case 41: {
			lexer.NextToken();

#line  1273 "VBNET.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
			break;
		}
		default: SynErr(213); break;
		}
	}

	void SimpleExpr(
#line  1277 "VBNET.ATG" 
out Expression pexpr) {

#line  1279 "VBNET.ATG" 
		Expression expr;
		TypeReference type = null;
		ObjectCreateExpression oce = null;
		string name = String.Empty;
		pexpr = null;
		
		switch (la.kind) {
		case 3: {
			lexer.NextToken();

#line  1287 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
			break;
		}
		case 4: {
			lexer.NextToken();

#line  1288 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1289 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1290 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1291 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
			break;
		}
		case 9: {
			lexer.NextToken();

#line  1292 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
			break;
		}
		case 174: {
			lexer.NextToken();

#line  1294 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
			break;
		}
		case 96: {
			lexer.NextToken();

#line  1295 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
			break;
		}
		case 130: {
			lexer.NextToken();

#line  1296 "VBNET.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
			break;
		}
		case 25: {
			lexer.NextToken();
			Expr(
#line  1297 "VBNET.ATG" 
out expr);
			Expect(26);

#line  1297 "VBNET.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
			break;
		}
		case 2: {
			lexer.NextToken();

#line  1298 "VBNET.ATG" 
			pexpr = new IdentifierExpression(t.val); 
			break;
		}
		case 52: case 54: case 65: case 76: case 77: case 84: case 111: case 117: case 160: case 161: case 166: {

#line  1299 "VBNET.ATG" 
			string val = String.Empty; 
			PrimitiveTypeName(
#line  1299 "VBNET.ATG" 
out val);
			Expect(10);
			Expect(2);

#line  1300 "VBNET.ATG" 
			pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  1301 "VBNET.ATG" 
			pexpr = new ThisReferenceExpression(); 
			break;
		}
		case 124: case 125: {

#line  1302 "VBNET.ATG" 
			Expression retExpr = null; 
			if (la.kind == 124) {
				lexer.NextToken();

#line  1303 "VBNET.ATG" 
				retExpr = new BaseReferenceExpression(); 
			} else {
				lexer.NextToken();

#line  1304 "VBNET.ATG" 
				retExpr = new ClassReferenceExpression(); 
			}
			Expect(10);
			Expect(2);

#line  1306 "VBNET.ATG" 
			retExpr = new FieldReferenceExpression(retExpr, t.val); 
			break;
		}
		case 127: {
			ObjectCreateExpression(
#line  1307 "VBNET.ATG" 
out oce);

#line  1307 "VBNET.ATG" 
			pexpr = oce; 
			break;
		}
		case 75: case 82: {
			if (la.kind == 82) {
				lexer.NextToken();
			} else {
				lexer.NextToken();
			}
			Expect(25);
			Expr(
#line  1308 "VBNET.ATG" 
out expr);
			Expect(13);
			TypeName(
#line  1308 "VBNET.ATG" 
out type);
			Expect(26);

#line  1308 "VBNET.ATG" 
			pexpr = new CastExpression(type, expr); 
			break;
		}
		case 59: case 60: case 61: case 62: case 63: case 64: case 66: case 68: case 69: case 72: case 73: case 74: {
			CastTarget(
#line  1309 "VBNET.ATG" 
out type);
			Expect(25);
			Expr(
#line  1309 "VBNET.ATG" 
out expr);
			Expect(26);

#line  1309 "VBNET.ATG" 
			pexpr = new CastExpression(type, expr); 
			break;
		}
		case 43: {
			lexer.NextToken();
			Expr(
#line  1310 "VBNET.ATG" 
out expr);

#line  1310 "VBNET.ATG" 
			pexpr = new AddressOfExpression(expr); 
			break;
		}
		case 102: {
			lexer.NextToken();
			Expect(25);
			TypeName(
#line  1311 "VBNET.ATG" 
out type);
			Expect(26);

#line  1311 "VBNET.ATG" 
			pexpr = new GetTypeExpression(type); 
			break;
		}
		case 176: {
			lexer.NextToken();
			Expr(
#line  1312 "VBNET.ATG" 
out expr);
			Expect(113);
			TypeName(
#line  1312 "VBNET.ATG" 
out type);

#line  1312 "VBNET.ATG" 
			pexpr = new TypeOfExpression(expr, type); 
			break;
		}
		default: SynErr(214); break;
		}
		while (la.kind == 10 || la.kind == 25) {
			if (la.kind == 10) {
				lexer.NextToken();
				IdentifierOrKeyword(
#line  1315 "VBNET.ATG" 
out name);

#line  1315 "VBNET.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, name);
			} else {
				lexer.NextToken();

#line  1316 "VBNET.ATG" 
				ArrayList parameters = new ArrayList(); 
				if (StartOf(17)) {
					Argument(
#line  1317 "VBNET.ATG" 
out expr);

#line  1317 "VBNET.ATG" 
					parameters.Add(expr); 
					while (la.kind == 13) {
						lexer.NextToken();
						Argument(
#line  1318 "VBNET.ATG" 
out expr);

#line  1318 "VBNET.ATG" 
						parameters.Add(expr); 
					}
				}
				Expect(26);

#line  1319 "VBNET.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			}
		}
	}

	void CastTarget(
#line  1324 "VBNET.ATG" 
out TypeReference type) {

#line  1326 "VBNET.ATG" 
		type = null;
		
		switch (la.kind) {
		case 59: {
			lexer.NextToken();

#line  1328 "VBNET.ATG" 
			type = new TypeReference("System.Boolean"); 
			break;
		}
		case 60: {
			lexer.NextToken();

#line  1329 "VBNET.ATG" 
			type = new TypeReference("System.Byte"); 
			break;
		}
		case 61: {
			lexer.NextToken();

#line  1330 "VBNET.ATG" 
			type = new TypeReference("System.Char"); 
			break;
		}
		case 62: {
			lexer.NextToken();

#line  1331 "VBNET.ATG" 
			type = new TypeReference("System.DateTime"); 
			break;
		}
		case 64: {
			lexer.NextToken();

#line  1332 "VBNET.ATG" 
			type = new TypeReference("System.Decimal"); 
			break;
		}
		case 63: {
			lexer.NextToken();

#line  1333 "VBNET.ATG" 
			type = new TypeReference("System.Double"); 
			break;
		}
		case 66: {
			lexer.NextToken();

#line  1334 "VBNET.ATG" 
			type = new TypeReference("System.Int32"); 
			break;
		}
		case 68: {
			lexer.NextToken();

#line  1335 "VBNET.ATG" 
			type = new TypeReference("System.64"); 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  1336 "VBNET.ATG" 
			type = new TypeReference("System.Object"); 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1337 "VBNET.ATG" 
			type = new TypeReference("System.Int16"); 
			break;
		}
		case 73: {
			lexer.NextToken();

#line  1338 "VBNET.ATG" 
			type = new TypeReference("System.Single"); 
			break;
		}
		case 74: {
			lexer.NextToken();

#line  1339 "VBNET.ATG" 
			type = new TypeReference("System.String"); 
			break;
		}
		default: SynErr(215); break;
		}
	}

	void IdentifierOrKeyword(
#line  2191 "VBNET.ATG" 
out string name) {

#line  2193 "VBNET.ATG" 
		name = String.Empty;
		
		switch (la.kind) {
		case 2: {
			lexer.NextToken();

#line  2195 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 42: {
			lexer.NextToken();

#line  2196 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 43: {
			lexer.NextToken();

#line  2197 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 44: {
			lexer.NextToken();

#line  2198 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 45: {
			lexer.NextToken();

#line  2199 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 46: {
			lexer.NextToken();

#line  2200 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 47: {
			lexer.NextToken();

#line  2201 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  2202 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  2203 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 50: {
			lexer.NextToken();

#line  2204 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 52: {
			lexer.NextToken();

#line  2205 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  2206 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  2207 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 55: {
			lexer.NextToken();

#line  2208 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 56: {
			lexer.NextToken();

#line  2209 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  2210 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 58: {
			lexer.NextToken();

#line  2211 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 59: {
			lexer.NextToken();

#line  2212 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 60: {
			lexer.NextToken();

#line  2213 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 61: {
			lexer.NextToken();

#line  2214 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 62: {
			lexer.NextToken();

#line  2215 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 63: {
			lexer.NextToken();

#line  2216 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 64: {
			lexer.NextToken();

#line  2217 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 65: {
			lexer.NextToken();

#line  2218 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 66: {
			lexer.NextToken();

#line  2219 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 67: {
			lexer.NextToken();

#line  2220 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 68: {
			lexer.NextToken();

#line  2221 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 69: {
			lexer.NextToken();

#line  2222 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 71: {
			lexer.NextToken();

#line  2223 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  2224 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 73: {
			lexer.NextToken();

#line  2225 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 74: {
			lexer.NextToken();

#line  2226 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 75: {
			lexer.NextToken();

#line  2227 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 76: {
			lexer.NextToken();

#line  2228 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 77: {
			lexer.NextToken();

#line  2229 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 78: {
			lexer.NextToken();

#line  2230 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 79: {
			lexer.NextToken();

#line  2231 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 80: {
			lexer.NextToken();

#line  2232 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  2233 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  2234 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  2235 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  2236 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 85: {
			lexer.NextToken();

#line  2237 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 86: {
			lexer.NextToken();

#line  2238 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  2239 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 88: {
			lexer.NextToken();

#line  2240 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 89: {
			lexer.NextToken();

#line  2241 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 90: {
			lexer.NextToken();

#line  2242 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 91: {
			lexer.NextToken();

#line  2243 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 92: {
			lexer.NextToken();

#line  2244 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 93: {
			lexer.NextToken();

#line  2245 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 94: {
			lexer.NextToken();

#line  2246 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  2247 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  2248 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  2249 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 99: {
			lexer.NextToken();

#line  2250 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 100: {
			lexer.NextToken();

#line  2251 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 101: {
			lexer.NextToken();

#line  2252 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  2253 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  2254 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 104: {
			lexer.NextToken();

#line  2255 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 105: {
			lexer.NextToken();

#line  2256 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 106: {
			lexer.NextToken();

#line  2257 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 107: {
			lexer.NextToken();

#line  2258 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 108: {
			lexer.NextToken();

#line  2259 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 109: {
			lexer.NextToken();

#line  2260 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 110: {
			lexer.NextToken();

#line  2261 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 111: {
			lexer.NextToken();

#line  2262 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 112: {
			lexer.NextToken();

#line  2263 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 113: {
			lexer.NextToken();

#line  2264 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 114: {
			lexer.NextToken();

#line  2265 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 115: {
			lexer.NextToken();

#line  2266 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  2267 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  2268 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 118: {
			lexer.NextToken();

#line  2269 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  2270 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  2271 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 121: {
			lexer.NextToken();

#line  2272 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 122: {
			lexer.NextToken();

#line  2273 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 123: {
			lexer.NextToken();

#line  2274 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 124: {
			lexer.NextToken();

#line  2275 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 125: {
			lexer.NextToken();

#line  2276 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  2277 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 127: {
			lexer.NextToken();

#line  2278 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 128: {
			lexer.NextToken();

#line  2279 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 129: {
			lexer.NextToken();

#line  2280 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 130: {
			lexer.NextToken();

#line  2281 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 131: {
			lexer.NextToken();

#line  2282 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 132: {
			lexer.NextToken();

#line  2283 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 133: {
			lexer.NextToken();

#line  2284 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 135: {
			lexer.NextToken();

#line  2285 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 136: {
			lexer.NextToken();

#line  2286 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 137: {
			lexer.NextToken();

#line  2287 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 138: {
			lexer.NextToken();

#line  2288 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 139: {
			lexer.NextToken();

#line  2289 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 140: {
			lexer.NextToken();

#line  2290 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 141: {
			lexer.NextToken();

#line  2291 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 143: {
			lexer.NextToken();

#line  2292 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 144: {
			lexer.NextToken();

#line  2293 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 145: {
			lexer.NextToken();

#line  2294 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 146: {
			lexer.NextToken();

#line  2295 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 147: {
			lexer.NextToken();

#line  2296 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 148: {
			lexer.NextToken();

#line  2297 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 149: {
			lexer.NextToken();

#line  2298 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 150: {
			lexer.NextToken();

#line  2299 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 151: {
			lexer.NextToken();

#line  2300 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 152: {
			lexer.NextToken();

#line  2301 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 153: {
			lexer.NextToken();

#line  2302 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 154: {
			lexer.NextToken();

#line  2303 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 155: {
			lexer.NextToken();

#line  2304 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 156: {
			lexer.NextToken();

#line  2305 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 157: {
			lexer.NextToken();

#line  2306 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 158: {
			lexer.NextToken();

#line  2307 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 159: {
			lexer.NextToken();

#line  2308 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 160: {
			lexer.NextToken();

#line  2309 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 161: {
			lexer.NextToken();

#line  2310 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 162: {
			lexer.NextToken();

#line  2311 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 163: {
			lexer.NextToken();

#line  2312 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 164: {
			lexer.NextToken();

#line  2313 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 166: {
			lexer.NextToken();

#line  2314 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 167: {
			lexer.NextToken();

#line  2315 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 168: {
			lexer.NextToken();

#line  2316 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 169: {
			lexer.NextToken();

#line  2317 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 171: {
			lexer.NextToken();

#line  2318 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 172: {
			lexer.NextToken();

#line  2319 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 173: {
			lexer.NextToken();

#line  2320 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 174: {
			lexer.NextToken();

#line  2321 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 175: {
			lexer.NextToken();

#line  2322 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 176: {
			lexer.NextToken();

#line  2323 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 177: {
			lexer.NextToken();

#line  2324 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 178: {
			lexer.NextToken();

#line  2325 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 179: {
			lexer.NextToken();

#line  2326 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 180: {
			lexer.NextToken();

#line  2327 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 181: {
			lexer.NextToken();

#line  2328 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 182: {
			lexer.NextToken();

#line  2329 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 183: {
			lexer.NextToken();

#line  2330 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 184: {
			lexer.NextToken();

#line  2331 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 185: {
			lexer.NextToken();

#line  2332 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		case 186: {
			lexer.NextToken();

#line  2333 "VBNET.ATG" 
			name = t.val; 
			break;
		}
		default: SynErr(216); break;
		}
	}

	void Argument(
#line  1483 "VBNET.ATG" 
out Expression argumentexpr) {

#line  1485 "VBNET.ATG" 
		Expression expr;
		argumentexpr = null;
		string name;
		
		if (
#line  1489 "VBNET.ATG" 
IsNamedAssign()) {
			Expect(2);

#line  1489 "VBNET.ATG" 
			name = t.val;  
			Expect(12);
			Expr(
#line  1489 "VBNET.ATG" 
out expr);

#line  1491 "VBNET.ATG" 
			argumentexpr = new NamedArgumentExpression(name, expr);
			
		} else if (StartOf(17)) {
			Expr(
#line  1494 "VBNET.ATG" 
out argumentexpr);
		} else SynErr(217);
	}

	void ConditionalAndExpr(
#line  1347 "VBNET.ATG" 
ref Expression outExpr) {

#line  1348 "VBNET.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  1349 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 46) {
			lexer.NextToken();
			UnaryExpr(
#line  1349 "VBNET.ATG" 
out expr);
			InclusiveOrExpr(
#line  1349 "VBNET.ATG" 
ref expr);

#line  1349 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BooleanAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  1352 "VBNET.ATG" 
ref Expression outExpr) {

#line  1353 "VBNET.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  1354 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 186) {
			lexer.NextToken();
			UnaryExpr(
#line  1354 "VBNET.ATG" 
out expr);
			ExclusiveOrExpr(
#line  1354 "VBNET.ATG" 
ref expr);

#line  1354 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  1357 "VBNET.ATG" 
ref Expression outExpr) {

#line  1358 "VBNET.ATG" 
		Expression expr; 
		AndExpr(
#line  1359 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 138) {
			lexer.NextToken();
			UnaryExpr(
#line  1359 "VBNET.ATG" 
out expr);
			AndExpr(
#line  1359 "VBNET.ATG" 
ref expr);

#line  1359 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  1362 "VBNET.ATG" 
ref Expression outExpr) {

#line  1363 "VBNET.ATG" 
		Expression expr; 
		EqualityExpr(
#line  1365 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 45) {
			lexer.NextToken();
			UnaryExpr(
#line  1365 "VBNET.ATG" 
out expr);
			EqualityExpr(
#line  1365 "VBNET.ATG" 
ref expr);

#line  1365 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  1368 "VBNET.ATG" 
ref Expression outExpr) {

#line  1370 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  1373 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 11 || la.kind == 29) {
			if (la.kind == 29) {
				lexer.NextToken();

#line  1376 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  1377 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  1379 "VBNET.ATG" 
out expr);
			RelationalExpr(
#line  1379 "VBNET.ATG" 
ref expr);

#line  1379 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  1383 "VBNET.ATG" 
ref Expression outExpr) {

#line  1385 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1389 "VBNET.ATG" 
ref outExpr);
		while (StartOf(20)) {
			if (StartOf(21)) {
				if (la.kind == 28) {
					lexer.NextToken();

#line  1392 "VBNET.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 27) {
					lexer.NextToken();

#line  1393 "VBNET.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 31) {
					lexer.NextToken();

#line  1394 "VBNET.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 30) {
					lexer.NextToken();

#line  1395 "VBNET.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(218);
				UnaryExpr(
#line  1397 "VBNET.ATG" 
out expr);
				ShiftExpr(
#line  1397 "VBNET.ATG" 
ref expr);

#line  1397 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
			} else {
				lexer.NextToken();

#line  1400 "VBNET.ATG" 
				op = BinaryOperatorType.IS; 
				Expr(
#line  1401 "VBNET.ATG" 
out expr);

#line  1401 "VBNET.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			}
		}
	}

	void ShiftExpr(
#line  1405 "VBNET.ATG" 
ref Expression outExpr) {

#line  1407 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  1410 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 32 || la.kind == 33) {
			if (la.kind == 32) {
				lexer.NextToken();

#line  1413 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				lexer.NextToken();

#line  1414 "VBNET.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  1416 "VBNET.ATG" 
out expr);
			AdditiveExpr(
#line  1416 "VBNET.ATG" 
ref expr);

#line  1416 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  1420 "VBNET.ATG" 
ref Expression outExpr) {

#line  1422 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  1425 "VBNET.ATG" 
ref outExpr);
		while (la.kind == 15 || la.kind == 16) {
			if (la.kind == 15) {
				lexer.NextToken();

#line  1428 "VBNET.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1429 "VBNET.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  1431 "VBNET.ATG" 
out expr);
			MultiplicativeExpr(
#line  1431 "VBNET.ATG" 
ref expr);

#line  1431 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1435 "VBNET.ATG" 
ref Expression outExpr) {

#line  1437 "VBNET.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (StartOf(22)) {
			if (la.kind == 17) {
				lexer.NextToken();

#line  1442 "VBNET.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 18) {
				lexer.NextToken();

#line  1443 "VBNET.ATG" 
				op = BinaryOperatorType.Divide; 
			} else if (la.kind == 19) {
				lexer.NextToken();

#line  1444 "VBNET.ATG" 
				op = BinaryOperatorType.DivideInteger; 
			} else {
				lexer.NextToken();

#line  1445 "VBNET.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  1447 "VBNET.ATG" 
out expr);

#line  1447 "VBNET.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void ArgumentList(
#line  1468 "VBNET.ATG" 
out ArrayList arguments) {

#line  1470 "VBNET.ATG" 
		arguments = new ArrayList();
		Expression expr = null;
		
		if (StartOf(17)) {
			Argument(
#line  1474 "VBNET.ATG" 
out expr);

#line  1474 "VBNET.ATG" 
			arguments.Add(expr); 
			while (la.kind == 13) {
				lexer.NextToken();
				Argument(
#line  1477 "VBNET.ATG" 
out expr);

#line  1477 "VBNET.ATG" 
				arguments.Add(expr); 
			}
		}
	}

	void NonArrayTypeName(
#line  1510 "VBNET.ATG" 
out TypeReference typeref) {

#line  1512 "VBNET.ATG" 
		string name;
		typeref = null;
		
		if (la.kind == 2) {
			Qualident(
#line  1515 "VBNET.ATG" 
out name);

#line  1515 "VBNET.ATG" 
			typeref = new TypeReference(name); 
		} else if (la.kind == 133) {
			lexer.NextToken();

#line  1516 "VBNET.ATG" 
			typeref = new TypeReference("Object"); 
		} else if (StartOf(23)) {
			PrimitiveTypeName(
#line  1517 "VBNET.ATG" 
out name);

#line  1517 "VBNET.ATG" 
			typeref = new TypeReference(name); 
		} else SynErr(219);
	}

	void ArrayTypeModifiers(
#line  1521 "VBNET.ATG" 
out int[] arrayModifiers) {

#line  1523 "VBNET.ATG" 
		ArrayList r = new ArrayList();
		int i = 0;
		
		while (
#line  1526 "VBNET.ATG" 
IsDims()) {
			Expect(25);
			if (la.kind == 13 || la.kind == 26) {
				RankList(
#line  1528 "VBNET.ATG" 
out i);
			}

#line  1530 "VBNET.ATG" 
			r.Add(i);
			
			Expect(26);
		}

#line  1535 "VBNET.ATG" 
		arrayModifiers = new int[r.Count];
		r.CopyTo(arrayModifiers);
		
	}

	void RankList(
#line  1541 "VBNET.ATG" 
out int i) {

#line  1542 "VBNET.ATG" 
		i = 0; 

#line  1543 "VBNET.ATG" 
		i = 1; 
		while (la.kind == 13) {
			lexer.NextToken();

#line  1544 "VBNET.ATG" 
			++i; 
		}
	}

	void Attribute(
#line  1569 "VBNET.ATG" 
out ICSharpCode.SharpRefactory.Parser.AST.VB.Attribute attribute) {

#line  1570 "VBNET.ATG" 
		string qualident; 
		Qualident(
#line  1571 "VBNET.ATG" 
out qualident);

#line  1573 "VBNET.ATG" 
		ArrayList positional = new ArrayList();
		ArrayList named      = new ArrayList();
		string name = qualident;
		
		if (la.kind == 25) {
			AttributeArguments(
#line  1577 "VBNET.ATG" 
ref positional, ref named);
		}

#line  1579 "VBNET.ATG" 
		attribute  = new ICSharpCode.SharpRefactory.Parser.AST.VB.Attribute(name, positional, named);
		
	}

	void AttributeArguments(
#line  1584 "VBNET.ATG" 
ref ArrayList positional, ref ArrayList named) {

#line  1586 "VBNET.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(25);
		if (
#line  1592 "VBNET.ATG" 
IsNotClosingParenthesis()) {
			if (
#line  1594 "VBNET.ATG" 
IsNamedAssign()) {

#line  1594 "VBNET.ATG" 
				nameFound = true; 
				IdentifierOrKeyword(
#line  1595 "VBNET.ATG" 
out name);
				if (la.kind == 12) {
					lexer.NextToken();
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(220);
			}
			Expr(
#line  1597 "VBNET.ATG" 
out expr);

#line  1599 "VBNET.ATG" 
			if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			
			while (la.kind == 13) {
				lexer.NextToken();
				if (
#line  1605 "VBNET.ATG" 
IsNamedAssign()) {

#line  1605 "VBNET.ATG" 
					nameFound = true; 
					IdentifierOrKeyword(
#line  1606 "VBNET.ATG" 
out name);
					if (la.kind == 12) {
						lexer.NextToken();
					} else if (la.kind == 11) {
						lexer.NextToken();
					} else SynErr(221);
				} else if (StartOf(17)) {

#line  1608 "VBNET.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(222);
				Expr(
#line  1609 "VBNET.ATG" 
out expr);

#line  1609 "VBNET.ATG" 
				if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				
			}
		}
		Expect(26);
	}

	void FixedParameter(
#line  1680 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  1682 "VBNET.ATG" 
		TypeReference type = null;
		ParamModifiers mod = ParamModifiers.ByVal;
		Expression expr = null;
		p = null;
		
		if (la.kind == 2 || la.kind == 53 || la.kind == 55) {
			if (la.kind == 53 || la.kind == 55) {
				if (la.kind == 53) {
					lexer.NextToken();

#line  1688 "VBNET.ATG" 
					mod = ParamModifiers.ByRef; 
				} else {
					lexer.NextToken();

#line  1689 "VBNET.ATG" 
					mod = ParamModifiers.ByVal; 
				}
			}
			Expect(2);
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1691 "VBNET.ATG" 
out type);
			}

#line  1693 "VBNET.ATG" 
			p = new ParameterDeclarationExpression(type, t.val, mod);
			
		} else if (la.kind == 137) {
			lexer.NextToken();

#line  1695 "VBNET.ATG" 
			mod = ParamModifiers.Optional; 
			Expect(2);
			if (la.kind == 48) {
				lexer.NextToken();
				TypeName(
#line  1696 "VBNET.ATG" 
out type);
			}
			if (la.kind == 11) {
				lexer.NextToken();
				Expr(
#line  1696 "VBNET.ATG" 
out expr);
			}

#line  1698 "VBNET.ATG" 
			p = new ParameterDeclarationExpression(type, t.val, mod, expr);
			
		} else SynErr(223);
	}

	void ParameterArray(
#line  1702 "VBNET.ATG" 
out ParameterDeclarationExpression p) {

#line  1703 "VBNET.ATG" 
		TypeReference type = null; 
		Expect(144);
		Expect(2);
		if (la.kind == 48) {
			lexer.NextToken();
			TypeName(
#line  1704 "VBNET.ATG" 
out type);
		}

#line  1706 "VBNET.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParamModifiers.Params);
		
	}

	void Statement() {

#line  1728 "VBNET.ATG" 
		Statement stmt;
		string label = String.Empty;
		
		if (
#line  1732 "VBNET.ATG" 
IsLabel()) {
			LabelName(
#line  1732 "VBNET.ATG" 
out label);

#line  1733 "VBNET.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(14);
			if (StartOf(14)) {
				Statement();
			}
		} else if (StartOf(24)) {
			EmbeddedStatement(
#line  1735 "VBNET.ATG" 
out stmt);

#line  1735 "VBNET.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(25)) {
			LocalDeclarationStatement(
#line  1736 "VBNET.ATG" 
out stmt);
		} else SynErr(224);
	}

	void LabelName(
#line  2038 "VBNET.ATG" 
out string name) {

#line  2040 "VBNET.ATG" 
		name = String.Empty;
		
		if (la.kind == 2) {
			lexer.NextToken();

#line  2042 "VBNET.ATG" 
			name = t.val; 
		} else if (la.kind == 5) {
			lexer.NextToken();

#line  2043 "VBNET.ATG" 
			name = t.val; 
		} else SynErr(225);
	}

	void EmbeddedStatement(
#line  1768 "VBNET.ATG" 
out Statement statement) {

#line  1770 "VBNET.ATG" 
		Statement embeddedStatement = null;
		statement = null;
		Expression expr = null;
		string name = String.Empty;
		TypeReference type = null;
		ArrayList p = null;
		
		switch (la.kind) {
		case 94: {
			lexer.NextToken();

#line  1778 "VBNET.ATG" 
			ExitType exitType = ExitType.None; 
			switch (la.kind) {
			case 168: {
				lexer.NextToken();

#line  1780 "VBNET.ATG" 
				exitType = ExitType.Sub; 
				break;
			}
			case 100: {
				lexer.NextToken();

#line  1782 "VBNET.ATG" 
				exitType = ExitType.Function; 
				break;
			}
			case 147: {
				lexer.NextToken();

#line  1784 "VBNET.ATG" 
				exitType = ExitType.Property; 
				break;
			}
			case 83: {
				lexer.NextToken();

#line  1786 "VBNET.ATG" 
				exitType = ExitType.Do; 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  1788 "VBNET.ATG" 
				exitType = ExitType.For; 
				break;
			}
			case 175: {
				lexer.NextToken();

#line  1790 "VBNET.ATG" 
				exitType = ExitType.Try; 
				break;
			}
			case 182: {
				lexer.NextToken();

#line  1792 "VBNET.ATG" 
				exitType = ExitType.While; 
				break;
			}
			default: SynErr(226); break;
			}

#line  1794 "VBNET.ATG" 
			statement = new ExitStatement(exitType); 
			break;
		}
		case 175: {
			TryStatement(
#line  1795 "VBNET.ATG" 
out statement);
			break;
		}
		case 172: {
			lexer.NextToken();
			if (StartOf(17)) {
				Expr(
#line  1797 "VBNET.ATG" 
out expr);
			}

#line  1797 "VBNET.ATG" 
			statement = new ThrowStatement(expr); 
			break;
		}
		case 155: {
			lexer.NextToken();
			if (StartOf(17)) {
				Expr(
#line  1799 "VBNET.ATG" 
out expr);
			}

#line  1799 "VBNET.ATG" 
			statement = new ReturnStatement(expr); 
			break;
		}
		case 169: {
			lexer.NextToken();
			Expr(
#line  1801 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  1801 "VBNET.ATG" 
out embeddedStatement);
			Expect(88);
			Expect(169);

#line  1802 "VBNET.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
			break;
		}
		case 150: {
			lexer.NextToken();
			Expect(2);

#line  1804 "VBNET.ATG" 
			name = t.val; 
			if (la.kind == 25) {
				lexer.NextToken();
				if (StartOf(4)) {
					FormalParameterList(
#line  1805 "VBNET.ATG" 
out p);
				}
				Expect(26);
			}

#line  1806 "VBNET.ATG" 
			RaiseEventStatement res = new RaiseEventStatement(name, p); 
			break;
		}
		case 183: {
			WithStatement(
#line  1808 "VBNET.ATG" 
out statement);
			break;
		}
		case 42: {
			lexer.NextToken();

#line  1810 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  1811 "VBNET.ATG" 
out expr);
			Expect(13);
			Expr(
#line  1811 "VBNET.ATG" 
out handlerExpr);

#line  1813 "VBNET.ATG" 
			statement = new AddHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 153: {
			lexer.NextToken();

#line  1816 "VBNET.ATG" 
			Expression handlerExpr = null; 
			Expr(
#line  1817 "VBNET.ATG" 
out expr);
			Expect(13);
			Expr(
#line  1817 "VBNET.ATG" 
out handlerExpr);

#line  1819 "VBNET.ATG" 
			statement = new RemoveHandlerStatement(expr, handlerExpr);
			
			break;
		}
		case 182: {
			lexer.NextToken();
			Expr(
#line  1822 "VBNET.ATG" 
out expr);
			EndOfStmt();
			Block(
#line  1823 "VBNET.ATG" 
out embeddedStatement);
			Expect(88);
			Expect(182);

#line  1825 "VBNET.ATG" 
			statement = new WhileStatement(expr, embeddedStatement);
			
			break;
		}
		case 83: {
			lexer.NextToken();

#line  1830 "VBNET.ATG" 
			ConditionType conditionType = ConditionType.None;
			
			if (la.kind == 178 || la.kind == 182) {
				WhileOrUntil(
#line  1833 "VBNET.ATG" 
out conditionType);
				Expr(
#line  1833 "VBNET.ATG" 
out expr);
				EndOfStmt();
				Block(
#line  1834 "VBNET.ATG" 
out embeddedStatement);
				Expect(118);

#line  1837 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.Start);
				
			} else if (la.kind == 1 || la.kind == 14) {
				EndOfStmt();
				Block(
#line  1841 "VBNET.ATG" 
out embeddedStatement);
				Expect(118);
				WhileOrUntil(
#line  1842 "VBNET.ATG" 
out conditionType);
				Expr(
#line  1842 "VBNET.ATG" 
out expr);

#line  1844 "VBNET.ATG" 
				statement = new DoLoopStatement(expr, embeddedStatement, conditionType, ConditionPosition.End);
				
			} else SynErr(227);
			break;
		}
		case 98: {
			lexer.NextToken();
			if (la.kind == 85) {

#line  1850 "VBNET.ATG" 
				Expression group = null;
				
				lexer.NextToken();
				Expect(2);

#line  1853 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  1853 "VBNET.ATG" 
out type);
				}
				Expect(109);
				Expr(
#line  1854 "VBNET.ATG" 
out group);
				EndOfStmt();
				Block(
#line  1855 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (StartOf(17)) {
					Expr(
#line  1856 "VBNET.ATG" 
out expr);
				}

#line  1858 "VBNET.ATG" 
				statement = new ForeachStatement(type, name, group, embeddedStatement, expr);
				
			} else if (la.kind == 2) {

#line  1862 "VBNET.ATG" 
				Expression start = null;
				Expression end = null;
				Expression step = null;
				Expression nextExpr = null;
				ArrayList nextExpressions = null;
				
				lexer.NextToken();

#line  1868 "VBNET.ATG" 
				name = t.val; 
				if (la.kind == 48) {
					lexer.NextToken();
					TypeName(
#line  1868 "VBNET.ATG" 
out type);
				}
				Expect(11);
				Expr(
#line  1869 "VBNET.ATG" 
out start);
				Expect(173);
				Expr(
#line  1869 "VBNET.ATG" 
out end);
				if (la.kind == 163) {
					lexer.NextToken();
					Expr(
#line  1869 "VBNET.ATG" 
out step);
				}
				EndOfStmt();
				Block(
#line  1870 "VBNET.ATG" 
out embeddedStatement);
				Expect(128);
				if (StartOf(17)) {
					Expr(
#line  1873 "VBNET.ATG" 
out nextExpr);

#line  1873 "VBNET.ATG" 
					nextExpressions = new ArrayList(); nextExpressions.Add(nextExpr); 
					while (la.kind == 13) {
						lexer.NextToken();
						Expr(
#line  1874 "VBNET.ATG" 
out nextExpr);

#line  1874 "VBNET.ATG" 
						nextExpressions.Add(nextExpr); 
					}
				}

#line  1877 "VBNET.ATG" 
				statement = new ForStatement(name, type, start, end, step, embeddedStatement, nextExpressions);
				
			} else SynErr(228);
			break;
		}
		case 92: {
			lexer.NextToken();
			Expr(
#line  1881 "VBNET.ATG" 
out expr);

#line  1881 "VBNET.ATG" 
			statement = new ErrorStatement(expr); 
			break;
		}
		case 152: {
			lexer.NextToken();

#line  1883 "VBNET.ATG" 
			ReDimClause clause = null; 
			if (la.kind == 145) {
				lexer.NextToken();
			}
			ReDimClause(
#line  1884 "VBNET.ATG" 
out clause);

#line  1886 "VBNET.ATG" 
			ArrayList clauses = new ArrayList();
			clauses.Add(clause);
			ReDimStatement reDimStatement = new ReDimStatement(clauses);
			
			while (la.kind == 13) {
				lexer.NextToken();
				ReDimClause(
#line  1890 "VBNET.ATG" 
out clause);

#line  1890 "VBNET.ATG" 
				clauses.Add(clause); 
			}
			break;
		}
		case 91: {
			lexer.NextToken();
			Expr(
#line  1893 "VBNET.ATG" 
out expr);

#line  1895 "VBNET.ATG" 
			ArrayList arrays = new ArrayList();
			arrays.Add(expr);
			EraseStatement eraseStatement = new EraseStatement(arrays);
			
			
			while (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1900 "VBNET.ATG" 
out expr);

#line  1900 "VBNET.ATG" 
				arrays.Add(expr); 
			}

#line  1901 "VBNET.ATG" 
			statement = eraseStatement; 
			break;
		}
		case 164: {
			lexer.NextToken();

#line  1903 "VBNET.ATG" 
			statement = new StopStatement(); 
			break;
		}
		case 106: {
			lexer.NextToken();
			Expr(
#line  1905 "VBNET.ATG" 
out expr);
			if (la.kind == 171) {
				lexer.NextToken();
			}
			if (la.kind == 1 || la.kind == 14) {
				EndOfStmt();
				Block(
#line  1907 "VBNET.ATG" 
out embeddedStatement);

#line  1909 "VBNET.ATG" 
				ArrayList elseIfSections = new ArrayList();
				IfStatement ifStatement = new IfStatement(expr, embeddedStatement);
				
				while (la.kind == 87 || 
#line  1914 "VBNET.ATG" 
IsElseIf()) {
					if (
#line  1914 "VBNET.ATG" 
IsElseIf()) {
						Expect(86);
						Expect(106);
					} else {
						lexer.NextToken();
					}

#line  1917 "VBNET.ATG" 
					Expression condition = null; Statement block = null; 
					Expr(
#line  1918 "VBNET.ATG" 
out condition);
					if (la.kind == 171) {
						lexer.NextToken();
					}
					EndOfStmt();
					Block(
#line  1919 "VBNET.ATG" 
out block);

#line  1921 "VBNET.ATG" 
					ElseIfSection elseIfSection = new ElseIfSection(condition, block);
					elseIfSections.Add(elseIfSection);
					
				}
				if (la.kind == 86) {
					lexer.NextToken();
					EndOfStmt();
					Block(
#line  1927 "VBNET.ATG" 
out embeddedStatement);

#line  1929 "VBNET.ATG" 
					ifStatement.EmbeddedElseStatement = embeddedStatement;
					
				}
				Expect(88);
				Expect(106);

#line  1933 "VBNET.ATG" 
				ifStatement.ElseIfStatements = elseIfSections;
				statement = ifStatement;
				
			} else if (StartOf(24)) {
				EmbeddedStatement(
#line  1937 "VBNET.ATG" 
out embeddedStatement);

#line  1939 "VBNET.ATG" 
				SimpleIfStatement ifStatement = new SimpleIfStatement(expr);
				ArrayList statements = new ArrayList();
				statements.Add(embeddedStatement);
				ifStatement.Statements = statements;
				
				while (la.kind == 14) {
					lexer.NextToken();
					EmbeddedStatement(
#line  1944 "VBNET.ATG" 
out embeddedStatement);

#line  1944 "VBNET.ATG" 
					statements.Add(embeddedStatement); 
				}
				if (la.kind == 86) {
					lexer.NextToken();
					EmbeddedStatement(
#line  1946 "VBNET.ATG" 
out embeddedStatement);

#line  1948 "VBNET.ATG" 
					ArrayList elseStatements = new ArrayList();
					elseStatements.Add(embeddedStatement);
					ifStatement.ElseStatements = elseStatements;
					
					while (la.kind == 14) {
						lexer.NextToken();
						EmbeddedStatement(
#line  1953 "VBNET.ATG" 
out embeddedStatement);

#line  1954 "VBNET.ATG" 
						elseStatements.Add(embeddedStatement); 
					}
				}

#line  1957 "VBNET.ATG" 
				statement = ifStatement; 
			} else SynErr(229);
			break;
		}
		case 156: {
			lexer.NextToken();
			if (la.kind == 57) {
				lexer.NextToken();
			}
			Expr(
#line  1960 "VBNET.ATG" 
out expr);
			EndOfStmt();

#line  1961 "VBNET.ATG" 
			ArrayList selectSections = new ArrayList(); 
			while (la.kind == 57) {

#line  1963 "VBNET.ATG" 
				ArrayList caseClauses = null; 
				lexer.NextToken();
				CaseClauses(
#line  1964 "VBNET.ATG" 
out caseClauses);
				EndOfStmt();

#line  1966 "VBNET.ATG" 
				SelectSection selectSection = new SelectSection();
				selectSection.CaseClauses = caseClauses;
				compilationUnit.BlockStart(selectSection);
				
				while (StartOf(14)) {
					Statement();
					EndOfStmt();
					while (StartOf(14)) {
						Statement();
						EndOfStmt();
					}
				}

#line  1972 "VBNET.ATG" 
				compilationUnit.BlockEnd();
				selectSections.Add(selectSection);
				
			}

#line  1976 "VBNET.ATG" 
			statement = new SelectStatement(expr, selectSections); 
			Expect(88);
			Expect(156);
			break;
		}
		case 135: {

#line  1978 "VBNET.ATG" 
			OnErrorStatement onErrorStatement = null; 
			OnErrorStatement(
#line  1979 "VBNET.ATG" 
out onErrorStatement);

#line  1979 "VBNET.ATG" 
			statement = onErrorStatement; 
			break;
		}
		case 104: {

#line  1980 "VBNET.ATG" 
			GoToStatement goToStatement = null; 
			GoToStatement(
#line  1981 "VBNET.ATG" 
out goToStatement);

#line  1981 "VBNET.ATG" 
			statement = goToStatement; 
			break;
		}
		case 154: {

#line  1982 "VBNET.ATG" 
			ResumeStatement resumeStatement = null; 
			ResumeStatement(
#line  1983 "VBNET.ATG" 
out resumeStatement);

#line  1983 "VBNET.ATG" 
			statement = resumeStatement; 
			break;
		}
		default: SynErr(230); break;
		}
	}

	void LocalDeclarationStatement(
#line  1740 "VBNET.ATG" 
out Statement statement) {

#line  1742 "VBNET.ATG" 
		Modifiers m = new Modifiers(this);
		ArrayList vars = new ArrayList();
		LocalVariableDeclaration localVariableDeclaration;
		bool dimfound = false;
		
		while (StartOf(26)) {
			if (StartOf(6)) {
				MemberModifier(
#line  1748 "VBNET.ATG" 
m);
			} else if (la.kind == 71) {
				lexer.NextToken();

#line  1749 "VBNET.ATG" 
				m.Add(Modifier.Constant); 
			} else if (la.kind == 162) {
				lexer.NextToken();

#line  1750 "VBNET.ATG" 
				m.Add(Modifier.Static); 
			} else {
				lexer.NextToken();

#line  1751 "VBNET.ATG" 
				dimfound = true;
			}
		}

#line  1754 "VBNET.ATG" 
		if(dimfound && (m.Modifier & Modifier.Constant) != 0) {
		Error("Dim is not allowed on constants.");
		}
		localVariableDeclaration = new LocalVariableDeclaration(m.Modifier);
		localVariableDeclaration.StartLocation = t.Location;
		
		VariableDeclarator(
#line  1760 "VBNET.ATG" 
vars);
		while (la.kind == 13) {
			lexer.NextToken();
			VariableDeclarator(
#line  1761 "VBNET.ATG" 
vars);
		}

#line  1763 "VBNET.ATG" 
		localVariableDeclaration.Variables = vars;
		statement = localVariableDeclaration;
		
	}

	void TryStatement(
#line  2146 "VBNET.ATG" 
out Statement tryStatement) {

#line  2148 "VBNET.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		ArrayList catchClauses = null;
		
		Expect(175);
		EndOfStmt();
		Block(
#line  2152 "VBNET.ATG" 
out blockStmt);
		if (la.kind == 58 || la.kind == 88 || la.kind == 97) {
			CatchClauses(
#line  2154 "VBNET.ATG" 
out catchClauses);
			if (la.kind == 97) {
				lexer.NextToken();
				EndOfStmt();
				Block(
#line  2155 "VBNET.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 97) {
			lexer.NextToken();
			EndOfStmt();
			Block(
#line  2156 "VBNET.ATG" 
out finallyStmt);
		} else SynErr(231);
		Expect(88);
		Expect(175);

#line  2160 "VBNET.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
		
	}

	void WithStatement(
#line  2128 "VBNET.ATG" 
out Statement withStatement) {

#line  2130 "VBNET.ATG" 
		Statement blockStmt = null;
		Expression expr = null;
		
		Expect(183);
		Expr(
#line  2133 "VBNET.ATG" 
out expr);
		EndOfStmt();

#line  2135 "VBNET.ATG" 
		withStatement = new WithStatement(expr);
		withStatements.Push(withStatement);
		
		Block(
#line  2138 "VBNET.ATG" 
out blockStmt);

#line  2140 "VBNET.ATG" 
		withStatements.Pop();
		
		Expect(88);
		Expect(183);
	}

	void WhileOrUntil(
#line  2121 "VBNET.ATG" 
out ConditionType conditionType) {

#line  2122 "VBNET.ATG" 
		conditionType = ConditionType.None; 
		if (la.kind == 182) {
			lexer.NextToken();

#line  2123 "VBNET.ATG" 
			conditionType = ConditionType.While; 
		} else if (la.kind == 178) {
			lexer.NextToken();

#line  2124 "VBNET.ATG" 
			conditionType = ConditionType.Until; 
		} else SynErr(232);
	}

	void ReDimClause(
#line  2047 "VBNET.ATG" 
out ReDimClause clause) {

#line  2049 "VBNET.ATG" 
		Expression initializer = null;
		string name = String.Empty;
		
		Qualident(
#line  2052 "VBNET.ATG" 
out name);

#line  2054 "VBNET.ATG" 
		clause = new ReDimClause(name);
		
		Expect(25);
		Expr(
#line  2056 "VBNET.ATG" 
out initializer);

#line  2058 "VBNET.ATG" 
		clause.Initializers = new ArrayList();
		clause.Initializers.Add(initializer);
		
		while (la.kind == 13) {
			lexer.NextToken();
			Expr(
#line  2061 "VBNET.ATG" 
out initializer);

#line  2061 "VBNET.ATG" 
			clause.Initializers.Add(initializer); 
		}
		Expect(26);
	}

	void CaseClauses(
#line  2077 "VBNET.ATG" 
out ArrayList caseClauses) {

#line  2079 "VBNET.ATG" 
		caseClauses = null;
		CaseClause caseClause = null;
		
		CaseClause(
#line  2082 "VBNET.ATG" 
out caseClause);

#line  2084 "VBNET.ATG" 
		caseClauses = new ArrayList();
		caseClauses.Add(caseClause);
		
		while (la.kind == 13) {
			lexer.NextToken();
			CaseClause(
#line  2087 "VBNET.ATG" 
out caseClause);

#line  2087 "VBNET.ATG" 
			caseClauses.Add(caseClause); 
		}
	}

	void OnErrorStatement(
#line  1987 "VBNET.ATG" 
out OnErrorStatement stmt) {

#line  1989 "VBNET.ATG" 
		stmt = null;
		GoToStatement goToStatement = null;
		
		Expect(135);
		Expect(92);
		if (
#line  1995 "VBNET.ATG" 
IsNegativeLabelName()) {
			Expect(104);
			Expect(16);
			Expect(5);

#line  1997 "VBNET.ATG" 
			long intLabel = Int64.Parse(t.val);
			if(intLabel != 1) {
				Error("invalid label in on error statement.");
			}
			stmt = new OnErrorStatement(new GoToStatement((intLabel * -1).ToString()));
			
		} else if (la.kind == 104) {
			GoToStatement(
#line  2003 "VBNET.ATG" 
out goToStatement);

#line  2005 "VBNET.ATG" 
			string val = goToStatement.LabelName;
			
			// check if val is numeric, if that is the case
			// make sure that it is 0
			try {
				long intLabel = Int64.Parse(val);
				if(intLabel != 0) {
					Error("invalid label in on error statement.");
				}
			} catch {
			}
			stmt = new OnErrorStatement(goToStatement);
			
		} else if (la.kind == 154) {
			lexer.NextToken();
			Expect(128);

#line  2020 "VBNET.ATG" 
			stmt = new OnErrorStatement(new ResumeStatement(true));
			
		} else SynErr(233);
	}

	void GoToStatement(
#line  2026 "VBNET.ATG" 
out GoToStatement goToStatement) {

#line  2028 "VBNET.ATG" 
		string label = String.Empty;
		
		Expect(104);
		LabelName(
#line  2031 "VBNET.ATG" 
out label);

#line  2033 "VBNET.ATG" 
		goToStatement = new GoToStatement(label);
		
	}

	void ResumeStatement(
#line  2066 "VBNET.ATG" 
out ResumeStatement resumeStatement) {

#line  2068 "VBNET.ATG" 
		resumeStatement = null;
		string label = String.Empty;
		
		if (
#line  2071 "VBNET.ATG" 
IsResumeNext()) {
			Expect(154);
			Expect(128);

#line  2072 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(true); 
		} else if (la.kind == 154) {
			lexer.NextToken();
			LabelName(
#line  2073 "VBNET.ATG" 
out label);

#line  2073 "VBNET.ATG" 
			resumeStatement = new ResumeStatement(label); 
		} else SynErr(234);
	}

	void CaseClause(
#line  2091 "VBNET.ATG" 
out CaseClause caseClause) {

#line  2093 "VBNET.ATG" 
		Expression expr = null;
		Expression sexpr = null;
		BinaryOperatorType op = BinaryOperatorType.None;
		caseClause = null;
		
		if (la.kind == 86) {
			lexer.NextToken();

#line  2099 "VBNET.ATG" 
			caseClause = new CaseClause(true); 
		} else if (StartOf(27)) {
			if (la.kind == 113) {
				lexer.NextToken();
			}
			switch (la.kind) {
			case 28: {
				lexer.NextToken();

#line  2103 "VBNET.ATG" 
				op = BinaryOperatorType.LessThan; 
				break;
			}
			case 27: {
				lexer.NextToken();

#line  2104 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThan; 
				break;
			}
			case 31: {
				lexer.NextToken();

#line  2105 "VBNET.ATG" 
				op = BinaryOperatorType.LessThanOrEqual; 
				break;
			}
			case 30: {
				lexer.NextToken();

#line  2106 "VBNET.ATG" 
				op = BinaryOperatorType.GreaterThanOrEqual; 
				break;
			}
			case 11: {
				lexer.NextToken();

#line  2107 "VBNET.ATG" 
				op = BinaryOperatorType.Equality; 
				break;
			}
			case 29: {
				lexer.NextToken();

#line  2108 "VBNET.ATG" 
				op = BinaryOperatorType.InEquality; 
				break;
			}
			default: SynErr(235); break;
			}
			Expr(
#line  2110 "VBNET.ATG" 
out expr);

#line  2112 "VBNET.ATG" 
			caseClause = new CaseClause(op, expr);
			
		} else if (StartOf(17)) {
			Expr(
#line  2114 "VBNET.ATG" 
out expr);
			if (la.kind == 173) {
				lexer.NextToken();
				Expr(
#line  2114 "VBNET.ATG" 
out sexpr);
			}

#line  2116 "VBNET.ATG" 
			caseClause = new CaseClause(expr, sexpr);
			
		} else SynErr(236);
	}

	void CatchClauses(
#line  2165 "VBNET.ATG" 
out ArrayList catchClauses) {

#line  2167 "VBNET.ATG" 
		catchClauses = new ArrayList();
		TypeReference type = null;
		Statement blockStmt = null;
		Expression expr = null;
		string name = String.Empty;
		
		while (la.kind == 58) {
			lexer.NextToken();
			if (la.kind == 2) {
				lexer.NextToken();

#line  2175 "VBNET.ATG" 
				name = t.val; 
				Expect(48);
				TypeName(
#line  2175 "VBNET.ATG" 
out type);
			}
			if (la.kind == 181) {
				lexer.NextToken();
				Expr(
#line  2176 "VBNET.ATG" 
out expr);
			}
			EndOfStmt();
			Block(
#line  2178 "VBNET.ATG" 
out blockStmt);

#line  2179 "VBNET.ATG" 
			catchClauses.Add(new CatchClause(type, name, blockStmt, expr)); 
		}
	}



	public void Parse(Lexer lexer)
	{
		this.errors = lexer.Errors;
		this.lexer = lexer;
		errors.SynErr = new ErrorCodeProc(SynErr);
		lexer.NextToken();
		VBNET();

	}

	void SynErr(int line, int col, int errorNumber)
	{
		errors.count++; 
		string s;
		switch (errorNumber) {
			case 0: s = "EOF expected"; break;
			case 1: s = "EOL expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "LiteralString expected"; break;
			case 4: s = "LiteralCharacter expected"; break;
			case 5: s = "LiteralInteger expected"; break;
			case 6: s = "LiteralDouble expected"; break;
			case 7: s = "LiteralSingle expected"; break;
			case 8: s = "LiteralDecimal expected"; break;
			case 9: s = "LiteralDate expected"; break;
			case 10: s = "\".\" expected"; break;
			case 11: s = "\"=\" expected"; break;
			case 12: s = "\":=\" expected"; break;
			case 13: s = "\",\" expected"; break;
			case 14: s = "\":\" expected"; break;
			case 15: s = "\"+\" expected"; break;
			case 16: s = "\"-\" expected"; break;
			case 17: s = "\"*\" expected"; break;
			case 18: s = "\"/\" expected"; break;
			case 19: s = "\"\\\\\" expected"; break;
			case 20: s = "\"&\" expected"; break;
			case 21: s = "\"{\" expected"; break;
			case 22: s = "\"}\" expected"; break;
			case 23: s = "\"[\" expected"; break;
			case 24: s = "\"]\" expected"; break;
			case 25: s = "\"(\" expected"; break;
			case 26: s = "\")\" expected"; break;
			case 27: s = "\">\" expected"; break;
			case 28: s = "\"<\" expected"; break;
			case 29: s = "\"<>\" expected"; break;
			case 30: s = "\">=\" expected"; break;
			case 31: s = "\"<=\" expected"; break;
			case 32: s = "\"<<\" expected"; break;
			case 33: s = "\">>\" expected"; break;
			case 34: s = "\"+=\" expected"; break;
			case 35: s = "\"^=\" expected"; break;
			case 36: s = "\"-=\" expected"; break;
			case 37: s = "\"*=\" expected"; break;
			case 38: s = "\"/=\" expected"; break;
			case 39: s = "\"\\\\=\" expected"; break;
			case 40: s = "\"<<=\" expected"; break;
			case 41: s = "\">>=\" expected"; break;
			case 42: s = "\"AddHandler\" expected"; break;
			case 43: s = "\"AddressOf\" expected"; break;
			case 44: s = "\"Alias\" expected"; break;
			case 45: s = "\"And\" expected"; break;
			case 46: s = "\"AndAlso\" expected"; break;
			case 47: s = "\"Ansi\" expected"; break;
			case 48: s = "\"As\" expected"; break;
			case 49: s = "\"Assembly\" expected"; break;
			case 50: s = "\"Auto\" expected"; break;
			case 51: s = "\"Binary\" expected"; break;
			case 52: s = "\"Boolean\" expected"; break;
			case 53: s = "\"ByRef\" expected"; break;
			case 54: s = "\"Byte\" expected"; break;
			case 55: s = "\"ByVal\" expected"; break;
			case 56: s = "\"Call\" expected"; break;
			case 57: s = "\"Case\" expected"; break;
			case 58: s = "\"Catch\" expected"; break;
			case 59: s = "\"CBool\" expected"; break;
			case 60: s = "\"CByte\" expected"; break;
			case 61: s = "\"CChar\" expected"; break;
			case 62: s = "\"CDate\" expected"; break;
			case 63: s = "\"CDbl\" expected"; break;
			case 64: s = "\"CDec\" expected"; break;
			case 65: s = "\"Char\" expected"; break;
			case 66: s = "\"CInt\" expected"; break;
			case 67: s = "\"Class\" expected"; break;
			case 68: s = "\"CLng\" expected"; break;
			case 69: s = "\"CObj\" expected"; break;
			case 70: s = "\"Compare\" expected"; break;
			case 71: s = "\"Const\" expected"; break;
			case 72: s = "\"CShort\" expected"; break;
			case 73: s = "\"CSng\" expected"; break;
			case 74: s = "\"CStr\" expected"; break;
			case 75: s = "\"CType\" expected"; break;
			case 76: s = "\"Date\" expected"; break;
			case 77: s = "\"Decimal\" expected"; break;
			case 78: s = "\"Declare\" expected"; break;
			case 79: s = "\"Default\" expected"; break;
			case 80: s = "\"Delegate\" expected"; break;
			case 81: s = "\"Dim\" expected"; break;
			case 82: s = "\"DirectCast\" expected"; break;
			case 83: s = "\"Do\" expected"; break;
			case 84: s = "\"Double\" expected"; break;
			case 85: s = "\"Each\" expected"; break;
			case 86: s = "\"Else\" expected"; break;
			case 87: s = "\"ElseIf\" expected"; break;
			case 88: s = "\"End\" expected"; break;
			case 89: s = "\"EndIf\" expected"; break;
			case 90: s = "\"Enum\" expected"; break;
			case 91: s = "\"Erase\" expected"; break;
			case 92: s = "\"Error\" expected"; break;
			case 93: s = "\"Event\" expected"; break;
			case 94: s = "\"Exit\" expected"; break;
			case 95: s = "\"Explicit\" expected"; break;
			case 96: s = "\"False\" expected"; break;
			case 97: s = "\"Finally\" expected"; break;
			case 98: s = "\"For\" expected"; break;
			case 99: s = "\"Friend\" expected"; break;
			case 100: s = "\"Function\" expected"; break;
			case 101: s = "\"Get\" expected"; break;
			case 102: s = "\"GetType\" expected"; break;
			case 103: s = "\"GoSub\" expected"; break;
			case 104: s = "\"GoTo\" expected"; break;
			case 105: s = "\"Handles\" expected"; break;
			case 106: s = "\"If\" expected"; break;
			case 107: s = "\"Implements\" expected"; break;
			case 108: s = "\"Imports\" expected"; break;
			case 109: s = "\"In\" expected"; break;
			case 110: s = "\"Inherits\" expected"; break;
			case 111: s = "\"Integer\" expected"; break;
			case 112: s = "\"Interface\" expected"; break;
			case 113: s = "\"Is\" expected"; break;
			case 114: s = "\"Let\" expected"; break;
			case 115: s = "\"Lib\" expected"; break;
			case 116: s = "\"Like\" expected"; break;
			case 117: s = "\"Long\" expected"; break;
			case 118: s = "\"Loop\" expected"; break;
			case 119: s = "\"Me\" expected"; break;
			case 120: s = "\"Mod\" expected"; break;
			case 121: s = "\"Module\" expected"; break;
			case 122: s = "\"MustInherit\" expected"; break;
			case 123: s = "\"MustOverride\" expected"; break;
			case 124: s = "\"MyBase\" expected"; break;
			case 125: s = "\"MyClass\" expected"; break;
			case 126: s = "\"Namespace\" expected"; break;
			case 127: s = "\"New\" expected"; break;
			case 128: s = "\"Next\" expected"; break;
			case 129: s = "\"Not\" expected"; break;
			case 130: s = "\"Nothing\" expected"; break;
			case 131: s = "\"NotInheritable\" expected"; break;
			case 132: s = "\"NotOverridable\" expected"; break;
			case 133: s = "\"Object\" expected"; break;
			case 134: s = "\"Off\" expected"; break;
			case 135: s = "\"On\" expected"; break;
			case 136: s = "\"Option\" expected"; break;
			case 137: s = "\"Optional\" expected"; break;
			case 138: s = "\"Or\" expected"; break;
			case 139: s = "\"OrElse\" expected"; break;
			case 140: s = "\"Overloads\" expected"; break;
			case 141: s = "\"Overridable\" expected"; break;
			case 142: s = "\"Override\" expected"; break;
			case 143: s = "\"Overrides\" expected"; break;
			case 144: s = "\"ParamArray\" expected"; break;
			case 145: s = "\"Preserve\" expected"; break;
			case 146: s = "\"Private\" expected"; break;
			case 147: s = "\"Property\" expected"; break;
			case 148: s = "\"Protected\" expected"; break;
			case 149: s = "\"Public\" expected"; break;
			case 150: s = "\"RaiseEvent\" expected"; break;
			case 151: s = "\"ReadOnly\" expected"; break;
			case 152: s = "\"ReDim\" expected"; break;
			case 153: s = "\"RemoveHandler\" expected"; break;
			case 154: s = "\"Resume\" expected"; break;
			case 155: s = "\"Return\" expected"; break;
			case 156: s = "\"Select\" expected"; break;
			case 157: s = "\"Set\" expected"; break;
			case 158: s = "\"Shadows\" expected"; break;
			case 159: s = "\"Shared\" expected"; break;
			case 160: s = "\"Short\" expected"; break;
			case 161: s = "\"Single\" expected"; break;
			case 162: s = "\"Static\" expected"; break;
			case 163: s = "\"Step\" expected"; break;
			case 164: s = "\"Stop\" expected"; break;
			case 165: s = "\"Strict\" expected"; break;
			case 166: s = "\"String\" expected"; break;
			case 167: s = "\"Structure\" expected"; break;
			case 168: s = "\"Sub\" expected"; break;
			case 169: s = "\"SyncLock\" expected"; break;
			case 170: s = "\"Text\" expected"; break;
			case 171: s = "\"Then\" expected"; break;
			case 172: s = "\"Throw\" expected"; break;
			case 173: s = "\"To\" expected"; break;
			case 174: s = "\"True\" expected"; break;
			case 175: s = "\"Try\" expected"; break;
			case 176: s = "\"TypeOf\" expected"; break;
			case 177: s = "\"Unicode\" expected"; break;
			case 178: s = "\"Until\" expected"; break;
			case 179: s = "\"Variant\" expected"; break;
			case 180: s = "\"Wend\" expected"; break;
			case 181: s = "\"When\" expected"; break;
			case 182: s = "\"While\" expected"; break;
			case 183: s = "\"With\" expected"; break;
			case 184: s = "\"WithEvents\" expected"; break;
			case 185: s = "\"WriteOnly\" expected"; break;
			case 186: s = "\"Xor\" expected"; break;
			case 187: s = "??? expected"; break;
			case 188: s = "invalid OptionStmt"; break;
			case 189: s = "invalid OptionStmt"; break;
			case 190: s = "invalid GlobalAttributeSection"; break;
			case 191: s = "invalid NamespaceMemberDecl"; break;
			case 192: s = "invalid OptionValue"; break;
			case 193: s = "invalid EndOfStmt"; break;
			case 194: s = "invalid TypeModifier"; break;
			case 195: s = "invalid NonModuleDeclaration"; break;
			case 196: s = "invalid NonModuleDeclaration"; break;
			case 197: s = "invalid PrimitiveTypeName"; break;
			case 198: s = "invalid FormalParameterList"; break;
			case 199: s = "invalid FormalParameterList"; break;
			case 200: s = "invalid MemberModifier"; break;
			case 201: s = "invalid StructureMemberDecl"; break;
			case 202: s = "invalid StructureMemberDecl"; break;
			case 203: s = "invalid StructureMemberDecl"; break;
			case 204: s = "invalid StructureMemberDecl"; break;
			case 205: s = "invalid InterfaceMemberDecl"; break;
			case 206: s = "invalid InterfaceMemberDecl"; break;
			case 207: s = "invalid Expr"; break;
			case 208: s = "invalid Charset"; break;
			case 209: s = "invalid VariableDeclarator"; break;
			case 210: s = "invalid AccessorDecls"; break;
			case 211: s = "invalid VariableInitializer"; break;
			case 212: s = "invalid EventMemberSpecifier"; break;
			case 213: s = "invalid AssignmentOperator"; break;
			case 214: s = "invalid SimpleExpr"; break;
			case 215: s = "invalid CastTarget"; break;
			case 216: s = "invalid IdentifierOrKeyword"; break;
			case 217: s = "invalid Argument"; break;
			case 218: s = "invalid RelationalExpr"; break;
			case 219: s = "invalid NonArrayTypeName"; break;
			case 220: s = "invalid AttributeArguments"; break;
			case 221: s = "invalid AttributeArguments"; break;
			case 222: s = "invalid AttributeArguments"; break;
			case 223: s = "invalid FixedParameter"; break;
			case 224: s = "invalid Statement"; break;
			case 225: s = "invalid LabelName"; break;
			case 226: s = "invalid EmbeddedStatement"; break;
			case 227: s = "invalid EmbeddedStatement"; break;
			case 228: s = "invalid EmbeddedStatement"; break;
			case 229: s = "invalid EmbeddedStatement"; break;
			case 230: s = "invalid EmbeddedStatement"; break;
			case 231: s = "invalid TryStatement"; break;
			case 232: s = "invalid WhileOrUntil"; break;
			case 233: s = "invalid OnErrorStatement"; break;
			case 234: s = "invalid ResumeStatement"; break;
			case 235: s = "invalid CaseClause"; break;
			case 236: s = "invalid CaseClause"; break;

			default: s = "error " + errorNumber; break;
		}
		errors.Error(line, col, s);
	}

	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,T, T,T,x,T, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,T, T,T,x,T, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,T, T,T,x,T, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,T,T, x,T,T,T, T,T,T,T, x,x,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,T, T,x,T,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, T,x,x,x, x,x,x,T, T,x,x,x, x,T,x,T, T,T,x,T, x,x,x,x, x,x,T,T, x,x,T,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,T, T,x,T,x, x,x,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, T,T,T,x, x,x,T,x, T,T,T,T, T,T,T,T, T,x,T,T, x,x,T,x, T,x,x,x, x,T,x,x, T,x,x,T, x,x,x,x, x,x,T,T, x,T,x,x, x},
	{x,T,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, x,T,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,x,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, x,T,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,x,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,T,T, T,T,T,T, x,T,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,T, T,T,T,T, T,T,T,x, T,T,x,x, T,T,T,T, T,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,T, x,x,x,x, T,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,T, T,x,T,x, x,x,T,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,T, T,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, T,x,x,T, x,x,x,x, x,x,T,T, x,x,x,x, x},
	{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,T,T,x, x,x,T,x, T,T,x,T, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x},
	{x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

	};
} // end Parser

}