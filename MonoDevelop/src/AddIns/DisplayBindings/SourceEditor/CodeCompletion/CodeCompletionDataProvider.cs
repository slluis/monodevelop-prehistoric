// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ?Â¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui;
using MonoDevelop.Internal.Templates;
using MonoDevelop.Services;
using MonoDevelop.Internal.Parser;

using MonoDevelop.SourceEditor.Gui;
using Stock = MonoDevelop.Gui.Stock;

using Gtk;

namespace MonoDevelop.SourceEditor.CodeCompletion
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CodeCompletionDataProvider : ICompletionDataProvider
	{
		static ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
//		static AmbienceService          ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
		Hashtable insertedElements           = new Hashtable();
		Hashtable insertedPropertiesElements = new Hashtable();
		Hashtable insertedEventElements      = new Hashtable();
		
		int caretLineNumber;
		int caretColumn;
		string fileName;
		bool ctrlspace;

		public CodeCompletionDataProvider() : this (false)
			{
			}
			
			public CodeCompletionDataProvider (bool ctrl) 
			{
				this.ctrlspace = ctrl;
			}
			
		
		ArrayList completionData = null;

		public ICompletionData[] GenerateCompletionData(string fileName, SourceEditorView textArea, char charTyped, TextMark triggerMark)
		{
			completionData = new ArrayList();
			this.fileName = fileName;
		
			Gtk.TextIter insertIter = textArea.Buffer.GetIterAtMark (triggerMark);
		
			// the parser works with 1 based coordinates
			
			caretLineNumber      = insertIter.Line + 1;
			caretColumn          = insertIter.LineOffset + 1;
			//string expression    = TextUtilities.GetExpressionBeforeOffset (textArea, insertIter.Offset);
			ResolveResult results;
			
			IParserService parserService = (IParserService)MonoDevelop.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			IExpressionFinder expressionFinder = parserService.GetExpressionFinder(fileName);
			string expression    = expressionFinder == null ? TextUtilities.GetExpressionBeforeOffset(textArea, insertIter.Offset) : expressionFinder.FindExpression(textArea.Buffer.GetText(textArea.Buffer.StartIter, insertIter, true), insertIter.Offset - 2);
			if (expression == null) return null;
			if (ctrlspace && charTyped != '.') {
				AddResolveResults (parserService.CtrlSpace (parserService, caretLineNumber, caretColumn, fileName));
				return (ICompletionData[])completionData.ToArray (typeof (ICompletionData));
			}
			if (charTyped == ' ') {
				if (expression == "using" || expression.EndsWith(" using") || expression.EndsWith("\tusing")|| expression.EndsWith("\nusing")|| expression.EndsWith("\rusing")) {
					string[] namespaces = parserService.GetNamespaceList("");
					AddResolveResults(new ResolveResult(namespaces));
				}
			} else {
				//FIXME: I added the null check, #D doesnt need it, why do we?
				if (fileName != null) {
					results = parserService.Resolve(expression, 
				                                caretLineNumber,
				                                caretColumn,
				                                fileName,
				                                textArea.Buffer.Text);
					AddResolveResults(results);
				}
			}
			
			return (ICompletionData[]) completionData.ToArray (typeof (ICompletionData));
		}

		void AddResolveResults(ICollection list) 
		{
			if (list == null) {
				return;
			}
			completionData.Capacity += list.Count;
			foreach (object o in list) {
				if (o is string) {
					completionData.Add(new CodeCompletionData(o.ToString(), Stock.NameSpace));
				} else if (o is IClass) {
					completionData.Add(new CodeCompletionData((IClass)o));
				} else if (o is IProperty) {
					IProperty property = (IProperty)o;
					if (property.Name != null && insertedPropertiesElements[property.Name] == null) {
						completionData.Add(new CodeCompletionData(property));
						insertedPropertiesElements[property.Name] = property;
					}
				} else if (o is IMethod) {
					IMethod method = (IMethod)o;
					
					if (method.Name != null &&!method.IsConstructor) {
						CodeCompletionData ccd = new CodeCompletionData(method);
						if (insertedElements[method.Name] == null) {
							completionData.Add(ccd);
							insertedElements[method.Name] = ccd;
						} else {
							CodeCompletionData oldMethod = (CodeCompletionData)insertedElements[method.Name];
							++oldMethod.Overloads;
						}
					}
				} else if (o is IField) {
					completionData.Add(new CodeCompletionData((IField)o));
				} else if (o is IEvent) {
					IEvent e = (IEvent)o;
					if (e.Name != null && insertedEventElements[e.Name] == null) {
						completionData.Add(new CodeCompletionData(e));
						insertedEventElements[e.Name] = e;
					}
				}
			}
		}
			
		void AddResolveResults(ResolveResult results)
		{
			if (results != null) {
				AddResolveResults(results.Namespaces);
				AddResolveResults(results.Members);
			}
		}
	}
}
