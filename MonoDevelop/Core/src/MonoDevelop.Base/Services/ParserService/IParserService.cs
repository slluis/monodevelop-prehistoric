// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using MonoDevelop.Core.AddIns;

using MonoDevelop.Internal.Parser;
using MonoDevelop.Internal.Project;

using MonoDevelop.Gui;

namespace MonoDevelop.Services
{
	public interface IParseInformation
	{
		ICompilationUnitBase ValidCompilationUnit {
			get;
		}
		ICompilationUnitBase DirtyCompilationUnit {
			get;
		}

		ICompilationUnitBase BestCompilationUnit {
			get;
		}

		ICompilationUnitBase MostRecentCompilationUnit {
			get;
		}
	}

	public interface IParserService
	{
		IParseInformation ParseFile(string fileName);
		IParseInformation ParseFile(string fileName, string fileContent);
		
		IParseInformation GetParseInformation(string fileName);
		
		IParser GetParser(string fileName);
		IExpressionFinder GetExpressionFinder(string fileName);
		
		// Default Parser Layer dependent functions
		IClass    GetClass(Project project, string typeName);
		string[]  GetNamespaceList(Project project, string subNameSpace);
		ArrayList GetNamespaceContents(Project project, string subNameSpace, bool includeReferences);
		bool      NamespaceExists(Project project, string name);
		string    SearchNamespace(Project project, IUsing iusing, string partitialNamespaceName);
		IClass    SearchType(Project project, IUsing iusing, string partitialTypeName);
		
		IClass    GetClass(Project project, string typeName, bool deepSearchReferences, bool caseSensitive);
		string[]  GetNamespaceList(Project project, string subNameSpace, bool caseSensitive);
		ArrayList GetNamespaceContents(Project project, string subNameSpace, bool includeReferences, bool caseSensitive);
		bool      NamespaceExists(Project project, string name, bool caseSensitive);
		string    SearchNamespace(Project project, IUsing iusing, string partitialNamespaceName, bool caseSensitive);
		IClass    SearchType(Project project, IUsing iusing, string partitialTypeName, bool caseSensitive);
		IClass    SearchType (Project project, string name, IClass callingClass, ICompilationUnit unit);
		
		IEnumerable GetClassInheritanceTree (Project project, IClass cls);
		
		////////////////////////////////////////////

		/// <summary>
		/// Resolves an expression.
		/// The caretLineNumber and caretColumn is 1 based.
		/// </summary>
		ResolveResult Resolve(Project project,
							  string expression,
		                      int caretLineNumber,
		                      int caretColumn,
		                      string fileName,
		                      string fileContent);
		string MonodocResolver (Project project, string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent);
		ArrayList IsAsResolve (Project project, string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent);
		ArrayList CtrlSpace(IParserService parserService, Project project, int caretLine, int caretColumn, string fileName);
		string LoadAssemblyFromGac (string name);

		event ParseInformationEventHandler ParseInformationChanged;
		event ClassInformationEventHandler ClassInformationChanged;
	}
}
