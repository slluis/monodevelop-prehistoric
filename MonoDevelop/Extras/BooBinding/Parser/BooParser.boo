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

import System
import System.Collections
import System.Diagnostics
import System.IO
import MonoDevelop.Services
import MonoDevelop.Internal.Project
import MonoDevelop.Internal.Parser
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

class BooParser(IParser):
	private _lexerTags as (string)

	private cuCache = Hashtable()
	
	LexerTags as (string):
		get:
			return _lexerTags
		set:
			_lexerTags = value
	
	ExpressionFinder as IExpressionFinder:
		get:
			return BooBinding.Parser.ExpressionFinder()
	
	def CanParse(fileName as string):
		return Path.GetExtension(fileName).ToLower() == ".boo"
	
	def CanParse(project as Project):
		return project.ProjectType == BooBinding.BooLanguageBinding.LanguageName
	
	def Parse(fileName as string) as ICompilationUnitBase:
		content as string
		using r = StreamReader(fileName):
			content = r.ReadToEnd()
		return Parse(fileName, content)
	
	def Parse(fileName as string, fileContent as string) as ICompilationUnitBase:
		//print "Parse ${fileName} with content"
		
		cr = '\r'[0]
		ln = '\n'[0]
		linecount = 1
		for c as Char in fileContent:
			linecount += 1 if c == ln
		lineLength = array(int, linecount)
		length = 0
		i = 0
		for c as Char in fileContent:
			if c == ln:
				lineLength[i] = length
				i += 1
				length = 0
			elif c != cr:
				length += 1
		lineLength[i] = length
		
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput(fileName, fileContent))
		project as Project
		for entry as Project in MonoDevelop.Services.Runtime.ProjectService.CurrentOpenCombine.GetAllProjects():
			if entry.IsFileInProject(fileName):
				project = entry
				
		return Parse(fileName, lineLength, compiler, project)
	
	private def Parse(fileName as string, lineLength as (int), compiler as BooCompiler, project as Project):
		compiler.Parameters.OutputWriter = StringWriter()
		compiler.Parameters.TraceSwitch.Level = TraceLevel.Warning;
		
		compilePipe = Compile()
		parsingStep as Boo.Lang.Parser.BooParsingStep = compilePipe[0]
		parsingStep.TabSize = 1
		//num = compilePipe.Find(typeof(ProcessMethodBodiesWithDuckTyping))
		// Include ProcessMethodBodies step now, as it solves issue
		// with [Propert(foo)] with an untyped 'foo'
		num = compilePipe.Find(typeof(StricterErrorChecking))
		visitor = Visitor(LineLength:lineLength)
		compilePipe[num] = visitor
		// Remove unneccessary compiler steps
		while compilePipe.Count > num + 1:
			compilePipe.RemoveAt(compilePipe.Count - 1)
		num = compilePipe.Find(typeof(TransformCallableDefinitions))
		compilePipe.RemoveAt(num)
		
		//for i in range(compilePipe.Count):
		//	print compilePipe[i].ToString()
		
		compilePipe.BreakOnErrors = false
		compiler.Parameters.Pipeline = compilePipe
		for projectRef as ProjectReference in project.ProjectReferences:
			compiler.Parameters.References.Add(System.Reflection.Assembly.LoadFile(projectRef.GetReferencedFileName()))
		
		try:
			compiler.Run()
			// somehow the SD parser thread goes into an endless loop if this flag is not set
			visitor.Cu.ErrorsDuringCompile = true //context.Errors.Count > 0
		except e:
			//ShowException(e)
			print "ShowException ${e}"

		for c as IClass in visitor.Cu.Classes:
			if c.Region is not null:
				c.Region.FileName = fileName

		// The following returns the "last known good" parse results
		// for a given file. Keeps our parse info from disappearing
		// when there is a parsing error in a file.
		if visitor.HadErrors:
			if cuCache[fileName] is null:
				return CompilationUnit()

			return cuCache[fileName] as ICompilationUnitBase
		
		cuCache[fileName] = visitor.Cu
		return visitor.Cu
	
	def CtrlSpace(parserService as IParserService, project as Project, caretLine as int, caretColumn as int, fileName as string) as ArrayList:
		//print "Ctrl-Space (${caretLine}/${caretColumn})"
		try:
			return Resolver(project).CtrlSpace(parserService, caretLine, caretColumn, fileName)
		except e:
			//ShowException(e)
			return null
	
	def IsAsResolve (parserService as IParserService , project as Project , expression as string , caretLineNumber as int , caretColumn as int , fileName as string , fileContent as string ) as ArrayList:
		return Resolver (project).IsAsResolve (parserService, expression, caretLineNumber, caretColumn, fileName, fileContent)

	def Resolve(parserService as IParserService, project as Project, expression as string, caretLineNumber as int, caretColumn as int, fileName as string, fileContent as string) as ResolveResult:
		//print "Resolve ${expression} (${caretLineNumber}/${caretColumn})"
		try:
			return Resolver(project).Resolve(parserService, expression, caretLineNumber, caretColumn, fileName, fileContent)
		except e:
			//ShowException(e)
			return null

	def MonodocResolver(parserService as IParserService, project as Project, expression as string, caretLineNumber as int, caretColumn as int, fileName as string, fileContent as string) as string:
		//print "MonodocResolver ${expression} (${caretLineNumber}/${caretColumn})"
		try:
			return Resolver(project).MonodocResolver(parserService, expression, caretLineNumber, caretColumn, fileName, fileContent)
		except e:
			//ShowException(e)
			return null
	
	/*
	static def ShowException(e as Exception):
		//messageService as IMessageService = ServiceManager.Services.GetService(typeof(IMessageService))
		//messageService.ShowError(e.ToString())
		retur
	*/


