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
		IClass    GetClass(string typeName);
		string[]  GetNamespaceList(string subNameSpace);
		ArrayList GetNamespaceContents(string subNameSpace);
		bool      NamespaceExists(string name);
		
		IClass    GetClass(string typeName, bool caseSensitive);
		string[]  GetNamespaceList(string subNameSpace, bool caseSensitive);
		ArrayList GetNamespaceContents(string subNameSpace, bool caseSensitive);
		bool      NamespaceExists(string name, bool caseSensitive);
		////////////////////////////////////////////

		/// <summary>
		/// Resolves an expression.
		/// The caretLineNumber and caretColumn is 1 based.
		/// </summary>
		ResolveResult Resolve(string expression,
		                      int caretLineNumber,
		                      int caretColumn,
		                      string fileName,
		                      string fileContent);
		string MonodocResolver (string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent);
		ArrayList IsAsResolve (string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent);
		ArrayList CtrlSpace(IParserService parserService, int caretLine, int caretColumn, string fileName);
		void AddReferenceToCompletionLookup(IProject project, ProjectReference reference);
		string LoadAssemblyFromGac (string name);

		event ParseInformationEventHandler ParseInformationAdded;
		event ParseInformationEventHandler ParseInformationRemoved;
		event ParseInformationEventHandler ParseInformationChanged;
	}
}
