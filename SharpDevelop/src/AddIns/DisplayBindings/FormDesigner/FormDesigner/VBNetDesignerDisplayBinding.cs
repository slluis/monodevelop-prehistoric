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

using ICSharpCode.SharpRefactory.Parser.VB;
using ICSharpCode.SharpRefactory.Parser.AST.VB;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class InitializeComponentsExtractorVisitor : ICSharpCode.SharpRefactory.Parser.VB.AbstractASTVisitor
	{
		public MethodDeclaration initializeComponents = null;
		
		public MethodDeclaration InitializeComponents {
			get {
				return initializeComponents;
			}
		}
		
		public override object Visit(TypeDeclaration typeDeclaration, object data)
		{
			Console.WriteLine("TYPE:" + typeDeclaration);
			
			if (typeDeclaration.BaseType != null) {

				if (typeDeclaration.BaseType == "System.Windows.Forms.Form" ||
				    typeDeclaration.BaseType == "Form" ||
				    typeDeclaration.BaseType == "System.Windows.Forms.UserControl" ||
				    typeDeclaration.BaseType == "UserControl") {
					Console.WriteLine("Children count: " + typeDeclaration.Children.Count);
					foreach (object o in typeDeclaration.Children) {
						Console.WriteLine(o);
					}
					typeDeclaration.AcceptChildren(this, data);
				}
			}
			return null;
		}
		
		public override object Visit(MethodDeclaration methodDeclaration, object data)
		{
				Console.WriteLine("VISIT METHOD !!!!" + methodDeclaration);
			if ((methodDeclaration.Name == "InitializeComponents" || methodDeclaration.Name == "InitializeComponent") && methodDeclaration.Parameters.Count == 0) {
				initializeComponents = methodDeclaration;
			}
			return null;
		}
	}
	
	public class VBNetDesignerDisplayBinding : ISecondaryDisplayBinding
	{
		public bool CanAttachTo(IViewContent viewContent)
		{
			Console.WriteLine("ASK ATTACH TO!!!" + viewContent);
			
			if (viewContent is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
				
				Lexer lexer = new Lexer(new ICSharpCode.SharpRefactory.Parser.VB.StringReader(textAreaControlProvider.TextEditorControl.Document.TextContent));
				Parser p = new Parser();
				p.Parse(lexer);
				InitializeComponentsExtractorVisitor initializeComponentsExtractorVisitor = new InitializeComponentsExtractorVisitor();
				initializeComponentsExtractorVisitor.Visit(p.compilationUnit, null);
				Console.WriteLine("InitializeComponentS: " + initializeComponentsExtractorVisitor.InitializeComponents);
				
//				IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
//				string content = textAreaControlProvider.TextEditorControl.Document.TextContent;
//				IParseInformation info        = parserService.ParseFile(@"C:\a.cs", content);
//				
//				if (info != null) {
//					ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
//					foreach (IClass c in cu.Classes) {
//						if (BaseClassIsFormOrControl(c)) {
//							IMethod method = GetInitializeComponents(c);
//							if (method == null) {
//								continue;
//							}
//							return true;
//						}
//					}
//				}
			}
			return false;
		}
		
		public ISecondaryViewContent CreateSecondaryViewContent(IViewContent viewContent)
		{
//			if (viewContent is ITextEditorControlProvider) {
//				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
//				
//				
//				IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
//				string content = textAreaControlProvider.TextEditorControl.Document.TextContent;
//				IParseInformation info        = parserService.ParseFile(@"C:\a.cs", content);
//				
//				ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
//				foreach (IClass c in cu.Classes) {
//					if (BaseClassIsFormOrControl(c)) {
//						IMethod method = GetInitializeComponents(c);
//						if (method == null) {
//							continue;
//						}
//						return new CSharpDesignerDisplayBindingWrapper(textAreaControlProvider, viewContent, c, method);
//					}
//				}
//				
//			}
			return null;
		}
	}
}
