// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃƒÂ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;

using MonoDevelop.SourceEditor.Gui;
using MonoDevelop.SourceEditor.CodeCompletion;

namespace MonoDevelop.SourceEditor.InsightWindow 
{
	public class MethodInsightDataProvider : IInsightDataProvider
	{
		ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
		AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		
		string              fileName = null;
		SourceEditorView    textArea  = null;
		MethodCollection    methods  = new MethodCollection();
		
		int caretLineNumber;
		int caretColumn;
		
		public int InsightDataCount {
			get {
				return methods.Count;
			}
		}
		
		public string GetInsightData(int number)
		{
			IMethod method = methods[number];
			IAmbience conv = ambienceService.CurrentAmbience;
			conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
			return conv.Convert(method);
			//       "\n" + 
			//       CodeCompletionData.GetDocumentation(method.Documentation); // new (by G.B.)
		}
		
		int initialOffset;
		public void SetupDataProvider(string fileName, SourceEditorView textArea)
		{
			this.fileName = fileName;
			this.textArea = textArea;
			Gtk.TextIter initialIter = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark);
			initialOffset = initialIter.Offset;
			string text = textArea.Buffer.Text;
			
			string word         = TextUtilities.GetExpressionBeforeOffset(textArea, initialOffset);
			string methodObject = word;
			string methodName   =  null;
			int idx = methodObject.LastIndexOf('.');
			if (idx >= 0) {
				methodName   = methodObject.Substring(idx + 1);
				methodObject = methodObject.Substring(0, idx);
			} else {
				methodObject = "this";
				methodName   = word;
			}
			
			if (methodName.Length == 0 || methodObject.Length == 0) {
				return;
			}
			
			// the parser works with 1 based coordinates
			caretLineNumber      = initialIter.Line + 1;
			caretColumn          = initialIter.LineOffset + 1;
			
			string[] words = word.Split(' ');
			bool contructorInsight = false;
			if (words.Length > 1) {
				contructorInsight = words[words.Length - 2] == "new";
				if (contructorInsight) {
					methodObject = words[words.Length - 1];
				}
			}
			IParserService parserService = (IParserService)ServiceManager.Services.GetService(typeof(IParserService));
			ResolveResult results = parserService.Resolve(methodObject, caretLineNumber, caretColumn, fileName, text);
			
			if (results != null && results.Type != null) {
				if (contructorInsight) {
					AddConstructors(results.Type);
				} else {
					foreach (IClass c in results.Type.ClassInheritanceTree) {
						AddMethods(c, methodName, false);
					}
				}
			}
		}
		
		bool IsAlreadyIncluded(IMethod newMethod) 
		{
			foreach (IMethod method in methods) {
				if (method.Name == newMethod.Name) {
					if (newMethod.Parameters.Count != method.Parameters.Count) {
						return false;
					}
					
					for (int i = 0; i < newMethod.Parameters.Count; ++i) {
						if (newMethod.Parameters[i].ReturnType != method.Parameters[i].ReturnType) {
							return false;
						}
					}
					
					// take out old method, when it isn't documented.
					if (method.Documentation == null || method.Documentation.Length == 0) {
						methods.Remove(method);
						return false;
					}
					return true;
				}
			}
			return false;
		}
		
		void AddConstructors(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if (method.IsConstructor && !method.IsStatic) {
					methods.Add(method);
				}
			}
		}
		
		void AddMethods(IClass c, string methodName, bool discardPrivate)
		{
			foreach (IMethod method in c.Methods) {
				if (!(method.IsPrivate && discardPrivate) && 
				    method.Name == methodName &&
				    !IsAlreadyIncluded(method)) {
					methods.Add(method);
				}
			}
		}
		
		public bool CaretOffsetChanged()
		{
			Gtk.TextIter insertIter = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark);
			bool closeDataProvider = insertIter.Offset <= initialOffset;
			int brackets = 0;
			int curlyBrackets = 0;
			string text = textArea.Buffer.Text;
			if (!closeDataProvider) {
				bool insideChar   = false;
				bool insideString = false;
				for (int offset = initialOffset; offset < Math.Min(insertIter.Offset, text.Length); ++offset) {
					char ch = text[offset];
					switch (ch) {
						case '\'':
							insideChar = !insideChar;
							break;
						case '(':
							if (!(insideChar || insideString)) {
								++brackets;
							}
							break;
						case ')':
							if (!(insideChar || insideString)) {
								--brackets;
							}
							if (brackets <= 0) {
								return true;
							}
							break;
						case '"':
							insideString = !insideString;
							break;
						case '}':
							if (!(insideChar || insideString)) {
								--curlyBrackets;
							}
							if (curlyBrackets < 0) {
								return true;
							}
							break;
						case '{':
							if (!(insideChar || insideString)) {
								++curlyBrackets;
							}
							break;
						case ';':
							if (!(insideChar || insideString)) {
								return true;
							}
							break;
					}
				}
			}
			
			return closeDataProvider;
		}
		
		public bool CharTyped()
		{
			int offset = textArea.Buffer.GetIterAtMark (textArea.Buffer.InsertMark).Offset - 1;
			if (offset >= 0) {
				return textArea.Buffer.Text [offset] == ')';
			}
			return false;
		}
	}
}
