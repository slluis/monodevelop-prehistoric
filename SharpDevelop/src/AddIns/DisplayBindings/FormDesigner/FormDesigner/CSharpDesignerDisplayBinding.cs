// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.SharpRefactory.Parser;
using ICSharpCode.SharpRefactory.Parser.AST;
using ICSharpCode.SharpRefactory.PrettyPrinter;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class CSharpDesignerDisplayBinding : ISecondaryDisplayBinding
	{
		IMethod GetInitializeComponents(IClass c)
		{
			foreach (IMethod method in c.Methods) {
				if ((method.Name == "InitializeComponents" ||
				     method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
					return method;
				}
			}
			return null;
		}
		
		bool BaseClassIsFormOrControl(IClass c)
		{
			foreach (string baseType in c.BaseTypes) {
				if (baseType == "System.Windows.Forms.Form" ||
				    baseType == "Form" ||
				    baseType == "System.Windows.Forms.UserControl" ||
				    baseType == "UserControl") {
					return true;
				}
			}
			return false;
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
				IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
				string content = textAreaControlProvider.TextEditorControl.Document.TextContent;
				IParseInformation info        = parserService.ParseFile(@"C:\a.cs", content);
				
				if (info != null) {
					ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
					foreach (IClass c in cu.Classes) {
						if (BaseClassIsFormOrControl(c)) {
							IMethod method = GetInitializeComponents(c);
							if (method == null) {
								continue;
							}
							return true;
						}
					}
				}
			}
			return false;
		}
		
		public ISecondaryViewContent CreateSecondaryViewContent(IViewContent viewContent)
		{
			if (viewContent is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
				IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
				string content = textAreaControlProvider.TextEditorControl.Document.TextContent;
				IParseInformation info        = parserService.ParseFile(@"C:\a.cs", content);
				
				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					if (BaseClassIsFormOrControl(c)) {
						IMethod method = GetInitializeComponents(c);
						if (method == null) {
							continue;
						}
						return new CSharpDesignerDisplayBindingWrapper(textAreaControlProvider, viewContent, c, method);
					}
				}
				
			}
			return null;
		}
	}
}
