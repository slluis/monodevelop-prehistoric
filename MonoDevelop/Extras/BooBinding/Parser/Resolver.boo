#region license
// Copyright (c) 2004-2005, Daniel Grunwald (daniel@danielgrunwald.de)
// Copyright (c) 2005, Peter Johanson (latexer@gentoo.org)
// All rights reserved.
//
// The BooBinding.Parser code is originally that of Daniel Grunwald
// (daniel@danielgrunwald.de) from the SharpDevelop BooBinding. The code has
// been imported here, and modified, including, but not limited to, changes
// to function with MonoDevelop, additions, refactorings, etc.
//
// BooBinding is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// BooBinding is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with BooBinding; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

namespace BooBinding.Parser

import BooBinding
import System
import System.Collections
import System.Diagnostics
import System.IO
import MonoDevelop.Services
import MonoDevelop.Internal.Parser
import MonoDevelop.Internal.Project
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast as AST
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps

class Resolver:
	[Getter(ParserService)]
	_parserService as IParserService

	_caretLine as int
	_caretColumn as int

	[Getter(Project)]
	_project as Project
	
	[Getter(CallingClass)]
	_callingClass as IClass
	_compilationUnit as ICompilationUnit

	[Property(ShowStatic)]
	_showStatic as bool
	
	_parentClass as IClass
	ParentClass as IClass:
		get:
			curClass = GetInnermostClass(_compilationUnit) as IClass
			return BaseClass(curClass)
	
	_resolvedMember = false
	_currentMember as IMember
	
	CurrentMember as IMember:
		get:
			if not _resolvedMember:
				_resolvedMember = true
				_currentMember = ResolveCurrentMember()
			return _currentMember
	

	def constructor ():
		pass

	def constructor (project as Project):
		_project = project

	#region Helper methods
	private def ResolveCurrentMember() as IMember:
		print "Getting current method... caretLine = ${_caretLine}, caretColumn = ${_caretColumn}"
		return null if _callingClass == null
		best as IMember = null
		line = 0
		for m as IMember in _callingClass.Methods:
			if m.Region != null:
				if m.Region.BeginLine <= _caretLine and m.Region.BeginLine > line:
					line = m.Region.BeginLine
					best = m
		for m as IMember in _callingClass.Properties:
			if m.Region != null:
				if m.Region.BeginLine <= _caretLine and m.Region.BeginLine > line:
					line = m.Region.BeginLine
					best = m
		if _callingClass.Region == null:
			for m as IMember in _callingClass.Methods:
				if m.Region == null:
					if best == null or best.Region.EndLine < _caretLine:
						return m
		return best
	
	_localTypes as Hashtable = {}
	
	def GetTypeFromLocal(name as string) as IReturnType:
		// gets the type of a local variable or method parameter
		print "Trying to get local variable ${name}..."
		return _localTypes[name] if _localTypes.ContainsKey(name)
		_localTypes[name] = null // prevent stack overflow by caching null first
		rt = InnerGetTypeFromLocal(name)
		_localTypes[name] = rt
		return rt
	
	def InnerGetTypeFromLocal(name as string) as IReturnType:
		member = self.CurrentMember
		Print("member", member)
		if member isa BooAbstractMethod:
			method as BooAbstractMethod = member
			for para as IParameter in method.Parameters:
				return para.ReturnType if para.Name == name
			if method.Node != null and method.Node.Body != null:
				varLookup = VariableLookupVisitor(Resolver: self, LookFor: name)
				print "Visiting method body..."
				varLookup.Visit(method.Node.Body)
				print "Finished visiting method body!"
				return varLookup.ReturnType
		elif member isa Property:
			print "name: ${name}"
			property as Property = member

			return property.ReturnType if name == "value"
			for para as IParameter in property.Parameters:
				return para.ReturnType if para.Name == name
			if property.Node != null:
				varLookup = VariableLookupVisitor(Resolver: self, LookFor: name)
				// TODO: visit only the correct body
				print "Visiting property body..."
				varLookup.Visit(property.Node.Getter) unless property.Node.Getter == null
				varLookup.Visit(property.Node.Setter) unless property.Node.Setter == null
				print "Finished visiting property body!"
				/*
				if varLookup.ReturnType is null:
					print "null return type!"
					return ReturnType("System.Object");
					*/
				print "ReturnType: ${varLookup.ReturnType}"
				return varLookup.ReturnType
		return null
	
	def SearchType(name as string) as IClass:
		expandedName = BooAmbience.ReverseTypeConversionTable[name]
		return _parserService.GetClass(_project, expandedName) if expandedName != null
		//return _parserService.SearchType(_project, name, _callingClass, _caretLine, _caretColumn)
		return _parserService.SearchType(_project, name, _callingClass, null)
	
	builtinClass as IClass
	
	BuiltinClass as IClass:
		get:
			builtinClass = _parserService.GetClass(_project, "Boo.Lang.Builtins") if builtinClass == null
			return builtinClass
	
	def IsNamespace(name as string) as bool:
		return _parserService.NamespaceExists(_project, name)
	
	#endregion
	
	#region CtrlSpace-Completion
	def CtrlSpace(parserService as IParserService, caretLine as int, caretColumn as int, fileName as string) as ArrayList:
		_parserService = parserService
		_caretLine = caretLine
		_caretColumn = caretColumn
		result = ArrayList(BooAmbience.TypeConversionTable.Values)
		result.Add("System") // system namespace can be used everywhere
		
		builtinClass = self.BuiltinClass
		if builtinClass != null:
			for method as IMethod in builtinClass.Methods:
				result.Add(method)
		
		parseInfo = parserService.GetParseInformation(fileName)
		cu = parseInfo.MostRecentCompilationUnit as CompilationUnit
		_compilationUnit = cu
		if cu != null:
			curClass = GetInnermostClass(cu) as IClass
			_callingClass = curClass
			if curClass != null:
				result = AddCurrentClassMembers(result, curClass)
				result.AddRange(parserService.GetNamespaceContents(_project, curClass.Namespace, true, true))
			for u as IUsing in cu.Usings:
				if u != null and (u.Region == null or u.Region.IsInside(caretLine, caretColumn)):
					for name as string in u.Usings:
						result.AddRange(parserService.GetNamespaceContents(_project, name, true, true))
					for alias as string in u.Aliases.Keys:
						result.Add(alias)
			member = self.CurrentMember
			Print("member", member)
			if member != null:
				varList as Hashtable = null
				if member isa BooAbstractMethod:
					method as BooAbstractMethod = member
					for para as IParameter in method.Parameters:
						result.Add(Field(para.ReturnType, para.Name, ModifierEnum.Private, null))
					if method.Node != null:
						varLookup = VariableListLookupVisitor(Resolver: self)
						print "Visiting method body..."
						varLookup.Visit(cast(BooAbstractMethod, member).Node.Body)
						print "Finished visiting method body!"
						varList = varLookup.Results
				elif member isa Property:
					property as Property = member
					if property.Node != null:
						varLookup = VariableListLookupVisitor(Resolver: self)
						// TODO: visit only the correct body
						print "Visiting property body..."
						varLookup.Visit(property.Node.Getter) unless property.Node.Getter == null
						varLookup.Visit(property.Node.Setter) unless property.Node.Setter == null
						print "Finished visiting property body!"
						varList = varLookup.Results
				if varList != null:
					for e as DictionaryEntry in varList:
						result.Add(Field(e.Value, e.Key, ModifierEnum.Private, null))
		result.AddRange(parserService.GetNamespaceContents(_project, "", true, true))
		return result
	
	def AddCurrentClassMembers(result as ArrayList, curClass as IClass) as ArrayList:
		if self.CurrentMember != null and self.CurrentMember.IsStatic == false:
			//result = ListMembers(result, curClass, curClass, false)
			result = ListMembers(result, curClass)
		// Add static members, but only from this class (not from base classes)
		for method as IMethod in curClass.Methods:
			result.Add(method) if (method.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		for field as IField in curClass.Fields:
			result.Add(field) if (field.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		for property as IProperty in curClass.Properties:
			result.Add(property) if (property.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		for e as Event in curClass.Events:
			result.Add(e) if (e.Modifiers & ModifierEnum.Static) == ModifierEnum.Static
		return result
	#endregion
	
	#region IsAsResolve

	def IsAsResolve(parserService as IParserService, expression as string, caretLine as int, caretColumn as int, fileName as string, fileContent as string) as ArrayList: 
		return null

	def MonodocResolver(parserService as IParserService, expression as string, caretLine as int, caretColumn as int, fileName as string, fileContent as string) as string: 
		return null

	#region Resolve CC
	def Initialize(parserService as IParserService, caretLine as int, caretColumn as int, fileName as string):
		_parserService = parserService
		_caretLine = caretLine
		_caretColumn = caretColumn
		
		parseInfo = parserService.GetParseInformation(fileName)
		cu = parseInfo.MostRecentCompilationUnit as CompilationUnit
		_compilationUnit = cu
		if _compilationUnit == null:
			print "BooResolver: No parse information!"
			return false
		_callingClass = GetInnermostClass(cu)
		if _callingClass == null:
			return false if cu.Classes.Count == 0
			_callingClass = cu.Classes[cu.Classes.Count - 1]
			if _callingClass.Region != null:
				return false if _callingClass.Region.BeginLine > caretLine
		return true
	
	def Resolve(parserService as IParserService, expression as string, caretLine as int, caretColumn as int, fileName as string, fileContent as string) as ResolveResult:
		if expression == null or expression == '':
			return null
		
		if expression.StartsWith("import "):
			expression = expression.Substring(7).Trim()
			if parserService.NamespaceExists(_project, expression):
				return ResolveResult(parserService.GetNamespaceList(_project, expression, true, true))
			return null
		
		if not Initialize(parserService, caretLine, caretColumn, fileName):
			return null
		callingClass = _callingClass
		returnClass as IClass = null
		if expression == "self":
			returnClass = callingClass
		elif expression == "this":
			// SharpDevelop uses "this" as expression when requesting method insight information
			// for a method on the current class
			returnClass = callingClass
		elif expression == "super":
			returnClass = self.ParentClass
		else:
			// try looking if the expression is the name of a class
			expressionClass = self.SearchType(expression)
			if expressionClass != null:
				//return ResolveResult(expressionClass, ListMembers(ArrayList(), expressionClass, callingClass, true))
				return ResolveResult(expressionClass, ListMembers(ArrayList(), expressionClass))
			
			// try if it is the name of a namespace
			if parserService.NamespaceExists(_project, expression):
				return ResolveResult(array(string, 0), parserService.GetNamespaceContents(_project, expression, true, true))
			
			expr = Boo.Lang.Parser.BooParser.ParseExpression("expression", expression)
			return null if expr isa AST.IntegerLiteralExpression
			visitor = ExpressionTypeVisitor(Resolver : self)
			visitor.Visit(expr)
			retType = visitor.ReturnType
			Print ("result", retType)
			if visitor.ReturnClass != null:
				returnClass = visitor.ReturnClass
			elif retType != null:
				if retType.ArrayDimensions != null and retType.ArrayDimensions.Length > 0:
					returnClass = self.SearchType("System.Array")
				else:
					returnClass = self.SearchType(retType.FullyQualifiedName)
		
		return null if returnClass == null
		//return ResolveResult(returnClass, ListMembers(ArrayList(), returnClass, callingClass, false))
		return ResolveResult(returnClass, ListMembers(ArrayList(), returnClass))
	
	private def Print(name as string, obj):
		Console.Write(name)
		Console.Write(' = ')
		if obj == null:
			print('null')
		else:
			print("${obj} (${obj.GetType().FullName})")
	#endregion

	#region Code converted from CSharpBinding/Parser/Resolver.cs
	def MustBeShowen(c as IClass, member as IDecoration) as bool:
		// FIXME: _showStatic should be coming from elsewhere... but where? (See CSharpBinding)
		_showStatic = false
//		print("member:" + member.Modifiers);
		if (((not _showStatic) and  ((member.Modifiers & ModifierEnum.Static) == ModifierEnum.Static)) or
		    ( _showStatic and not ((member.Modifiers & ModifierEnum.Static) == ModifierEnum.Static))):
			//// enum type fields are not shown here - there is no info in member about enum field
			return false
		
//		print("Testing Accessibility");
		return IsAccessible(c, member)
	
	def IsAccessible(c as IClass, member as IDecoration) as bool:
//		print("member.Modifiers = " + member.Modifiers);
		if ((member.Modifiers & ModifierEnum.Internal) == ModifierEnum.Internal):
			return true

		if ((member.Modifiers & ModifierEnum.Public) == ModifierEnum.Public):
//			print("IsAccessible")
			return true

		if ((member.Modifiers & ModifierEnum.Protected) == ModifierEnum.Protected and IsClassInInheritanceTree(c, _callingClass)):
//			print("IsAccessible")
			return true

		return c.FullyQualifiedName == _callingClass.FullyQualifiedName

	/// <remarks>
	/// Returns true, if class possibleBaseClass is in the inheritance tree from c
	/// </remarks>
	def IsClassInInheritanceTree(possibleBaseClass as IClass , c as IClass) as bool:
		if (possibleBaseClass == null or c == null):
			return false

		if (possibleBaseClass.FullyQualifiedName == c.FullyQualifiedName):
			return true

		for baseClass as string in c.BaseTypes:
			bc = _parserService.GetClass (_project, baseClass, true, true)
			if (IsClassInInheritanceTree(possibleBaseClass, bc)):
				return true

		return false

	def BaseClass(curClass as IClass) as IClass:
		for s as string in curClass.BaseTypes:
			baseClass = _parserService.GetClass (_project, s, true, true)
			if ((baseClass != null) and (baseClass.ClassType != ClassType.Interface)):
				return baseClass
		return null
	
	def ListMembers(members as ArrayList, curType as IClass) as ArrayList:
		// FIXME: _showStatic should be coming from elsewhere... but where? (See CSharpBinding)
		_showStatic = false
//		print("LIST MEMBERS!!!");
//		print("_showStatic = " + _showStatic);
//		print(curType.InnerClasses.Count + " classes");
//		print(curType.Properties.Count + " properties");
//		print(curType.Methods.Count + " methods");
//		print(curType.Events.Count + " events");
//		print(curType.Fields.Count + " fields");
		if _showStatic:
			for c as IClass in curType.InnerClasses:
				if IsAccessible(curType, c):
					members.Add(c)
//					print("Member added")

		for p as IProperty in curType.Properties:
			if (MustBeShowen(curType, p)):
				members.Add(p)
//				print("Member added")

//		print("ADDING METHODS!!!");
		for m as IMethod in curType.Methods:
//			print("Method : " + m)
			if (MustBeShowen(curType, m)):
				members.Add(m)
//				print("Member added");

		for e as IEvent in curType.Events:
			if (MustBeShowen(curType, e)):
				members.Add(e)
//				print("Member added");

		for f as IField in curType.Fields:
			if (MustBeShowen(curType, f)):
				members.Add(f)
//				print("Member added")
			else:
				//// enum fields must be shown here if present
				if (curType.ClassType == ClassType.Enum):
					members.Add(f) if (IsAccessible(curType,f))
//					print("Member {0} added", f.FullyQualifiedName);

//		print("ClassType = " + curType.ClassType);
		if (curType.ClassType == ClassType.Interface and not _showStatic):
			for s as string in curType.BaseTypes:
				baseClass = _parserService.GetClass (_project, s, true, true)
				if (baseClass != null and baseClass.ClassType == ClassType.Interface):
					ListMembers(members, baseClass)
		else:
			baseClass = BaseClass(curType)
			if (baseClass != null):
//				print("Base Class = " + baseClass.FullyQualifiedName)
				ListMembers(members, baseClass)

//		print("listing finished");
		return members;

	def GetResolvedClass (cls as IClass) as IClass:
		// Returns an IClass in which all type names have been properly resolved
		return _parserService.GetClass (_project, cls.FullyQualifiedName);

	def GetInnermostClass(cu as ICompilationUnit) as IClass:
		if (cu != null):
			for c as IClass in cu.Classes:
				if (c != null and c.Region != null and c.Region.IsInside(_caretLine, _caretColumn)):
					return GetInnermostClass(c)
		return null;
	
	def GetInnermostClass(curClass as IClass) as IClass:
		if (curClass == null):
			return null

		if (curClass.InnerClasses == null):
			return GetResolvedClass (curClass)

		for c as IClass in curClass.InnerClasses:
			if (c != null and c.Region != null and c.Region.IsInside(_caretLine, _caretColumn)):
				return GetInnermostClass(c)

		return GetResolvedClass (curClass)
