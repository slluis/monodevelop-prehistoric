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
	public class CSharpDesignerDisplayBindingWrapper : FormDesignerDisplayBindingBase, ISecondaryViewContent
	{
		protected bool failedDesignerInitialize;
		
		IClass                   c;
		IMethod                  initializeComponents;
		IViewContent             viewContent;
		
		ITextEditorControlProvider textAreaControlProvider;
		
		string compilationErrors;
		
		public override Control Control {
			get {
				return base.designPanel;
			}
		}
		
		public override bool IsDirty {
			get {
				if (viewContent == null) {
					return false;
				}
				return viewContent.IsDirty;
			}
			set {
				if (viewContent != null) {
					viewContent.IsDirty = value;
				}
			}
		}
		
		IDocument Document {
			get {
				return textAreaControlProvider.TextEditorControl.Document;
			}
		}
		
		public CSharpDesignerDisplayBindingWrapper(ITextEditorControlProvider textAreaControlProvider, IViewContent viewContent, IClass c, IMethod initializeComponents)
		{
			this.viewContent             = viewContent;
			this.textAreaControlProvider = textAreaControlProvider;
			this.c                       = c;
			this.initializeComponents    = initializeComponents;
			
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			failedDesignerInitialize = false;
			undoHandler.Reset();
			Reload();
			UpdateSelectableObjects();
			if (designPanel != null) {
				base.designPanel.Disable();
			}
		}
		
		public override void Initialize()
		{
			base.Initialize();
			host.AddService(typeof(CodeDomProvider), new CSharpCodeProvider());
		}
		
		public override void Reload()
		{
			if (host != null) {
				base.host.SetRootFullName(c.FullyQualifiedName);
			}
			try {
				CodeDOMVisitor visitor = new CodeDOMVisitor();
				// parse the source file first to find errors ...
				
				Parser p = new Parser();
				p.Parse(new Lexer(new ICSharpCode.SharpRefactory.Parser.StringReader(Document.TextContent)));
				
				failedDesignerInitialize = p.Errors.count != 0;
				if (failedDesignerInitialize) {
					compilationErrors = p.Errors.ErrorOutput;
					return;
				}
				
				// now parse the generated file ...
				p = new Parser();
				p.Parse(new Lexer(new ICSharpCode.SharpRefactory.Parser.StringReader(GenerateClassString(Document))));
				
//				new DebugASTVisitor().Visit(Parser.compilationUnit, null);
				
				Initialize();
//				host.DesignerLoader.BeginLoad(host);
				
				CodeDomDesignerSerializetionManager serializationManager = (CodeDomDesignerSerializetionManager)host.GetService(typeof(IDesignerSerializationManager));
				serializationManager.Initialize();
				
				CodeDomSerializer rootSerializer = serializationManager.GetRootSerializer(typeof(System.Windows.Forms.Form));
				
				if (rootSerializer == null) {
					throw new Exception("No root serializer found");
				}
				
				visitor.Visit(p.compilationUnit, null);
				/* 
				// output generated CodeDOM to the console : 
				Microsoft.CSharp.CSharpCodeProvider provider = new CSharpCodeProvider();
				System.CodeDom.Compiler.ICodeGenerator generator = provider.CreateGenerator();
				generator.GenerateCodeFromCompileUnit(visitor.codeCompileUnit, Console.Out, null);
				*/
				foreach (CodeNamespace codeNamespace in visitor.codeCompileUnit.Namespaces) {
					if (codeNamespace.Types.Count > 0) {
						//Console.WriteLine("Try to deserialize type : " + codeNamespace.Types[0].Name);
						DesignerResourceService designerResourceService = (DesignerResourceService)host.GetService(typeof(System.ComponentModel.Design.IResourceService));
						designerResourceService.NameSpace = codeNamespace.Name;
						designerResourceService.RootType  = codeNamespace.Types[0].Name;
						designerResourceService.LoadResources();
						rootSerializer.Deserialize(serializationManager, codeNamespace.Types[0]);
						serializationManager.OnSerializationComplete();
 						designPanel.SetRootDesigner();
						break;
					}
				}
				failedDesignerInitialize = false;
//				host.DesignerLoader.EndLoad();
			} catch (Exception e) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e);
				failedDesignerInitialize = true;
			}
		}
		
		
		protected virtual void AppendUsings(StringBuilder builder, IUsingCollection usings)
		{
			foreach (IUsing u in c.CompilationUnit.Usings)  {
				foreach (string usingString in u.Usings)  {
					if (usingString.StartsWith("System"))  {
						builder.Append("using " + usingString + ";\n");
					}
				}
			}
		}
		
		string GenerateClassString(IDocument document)
		{
			Reparse("a.cs", document.TextContent);
			
			StringBuilder builder = new StringBuilder();
			// generate usings
			AppendUsings(builder, c.CompilationUnit.Usings);
			string className = c.Name;
			builder.Append("class ");
			builder.Append(className);
			builder.Append(" : ");
			builder.Append(ExtractBaseClass(c));
			builder.Append(" {\n");
			ArrayList fields = GetUsedFields(document, c, initializeComponents);
			foreach (IField field in fields) {
				LineSegment fieldLine = document.GetLineSegment(field.Region.BeginLine - 1);
				builder.Append(document.GetText(fieldLine.Offset, fieldLine.Length));
				builder.Append("\n");
			}
			
			builder.Append("\tpublic ");
			builder.Append(className);
			builder.Append("() {\n\t\t");
			builder.Append(initializeComponents.Name);
			builder.Append("();\n");
			builder.Append("\t}\n");
			string initializeComponentsString = GetInitializeComponentsString(document, initializeComponents);
			builder.Append(initializeComponentsString);
			
			builder.Append("}");
			return builder.ToString();
		}
		
		string GetInitializeComponentsString(IDocument doc, IMethod initializeComponents)
		{
			LineSegment beginLine = doc.GetLineSegment(initializeComponents.Region.BeginLine - 1);
			LineSegment endLine   = doc.GetLineSegment(initializeComponents.BodyRegion.EndLine - 1);
			
			int startOffset = beginLine.Offset + initializeComponents.Region.BeginColumn - 1;
			int endOffset   = endLine.Offset   + initializeComponents.BodyRegion.EndColumn - 1;
			
			string initializeComponentsString = doc.GetText(startOffset, endOffset - startOffset);
			int idx = initializeComponentsString.LastIndexOf('}');
			if (idx > 0) {
				initializeComponentsString = initializeComponentsString.Substring(0, idx + 1);
			}
			return initializeComponentsString;
		}
		
		protected virtual string ExtractBaseClass(IClass c)
		{
			foreach (string baseType in c.BaseTypes) {
				if (baseType == "System.Windows.Forms.Form" ||
				    baseType == "Form" ||
				    baseType == "System.Windows.Forms.UserControl" ||
				    baseType == "UserControl") {
					
					return baseType;
				}
			}
			return String.Empty;
		}
		
		ArrayList GetUsedFields(IDocument doc, IClass c, IMethod initializeComponents)
		{
			string InitializeComponentsString = GetInitializeComponentsString(doc, initializeComponents);
			ArrayList fields = new ArrayList();
			foreach (IField field in c.Fields) {
				if (field.IsPrivate) {
					if (InitializeComponentsString.IndexOf(field.Name) >= 0) {
						fields.Add(field);
					}
				}
			}
			return fields;
		}
		
		void DeleteFormFields(IDocument doc)
		{
			ArrayList fields = GetUsedFields(doc, c, initializeComponents);
			for (int i = fields.Count - 1; i >= 0; --i) {
				IField field = (IField)fields[i];
				LineSegment fieldLine = doc.GetLineSegment(field.Region.BeginLine - 1);
				doc.Remove(fieldLine.Offset, fieldLine.TotalLength);
			}
		}
		
		protected virtual void MergeFormChanges()
		{
			if (this.failedDesignerInitialize) {
				return;
			}
			bool dirty = viewContent.IsDirty;
			IParserService parserService    = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			
			// generate file and get initialize components string
			string currentForm = GetDataAs("C#");
			IParseInformation generatedInfo = parserService.ParseFile("a.cs", currentForm);
			ICompilationUnit cu = (ICompilationUnit)generatedInfo.BestCompilationUnit;
			IClass generatedClass = cu.Classes[0];
			IMethod generatedInitializeComponents = GetInitializeComponents(cu.Classes[0]);
			IDocument newDoc = new DocumentFactory().CreateDocument();
			newDoc.TextContent = currentForm;
			string newInitializeComponents = GetInitializeComponentsString(newDoc, generatedInitializeComponents);
			
			TextEditorControl textArea = textAreaControlProvider.TextEditorControl;
			textArea.BeginUpdate();
			IDocument oldDoc = new DocumentFactory().CreateDocument();
			oldDoc.TextContent = textArea.Document.TextContent;
			Reparse("a.cs", oldDoc.TextContent);
			DeleteFormFields(oldDoc);
			
			// replace the old initialize components method with the new one
			Reparse("a.cs", oldDoc.TextContent);
			LineSegment beginLine = oldDoc.GetLineSegment(initializeComponents.Region.BeginLine - 1);
			int startOffset = beginLine.Offset + initializeComponents.Region.BeginColumn - 1;
			oldDoc.Replace(startOffset, GetInitializeComponentsString(oldDoc, initializeComponents).Length, newInitializeComponents);
			Reparse("a.cs", oldDoc.TextContent);
			
			// insert new fields
			int lineNr = c.Region.BeginLine - 1;
			while (true) {
				if (lineNr >= textArea.Document.TotalNumberOfLines - 2) {
					break;
				}
				LineSegment curLine = oldDoc.GetLineSegment(lineNr);
				if (oldDoc.GetText(curLine.Offset, curLine.Length).Trim().EndsWith("{")) {
					break;
				}
				++lineNr;
			}
			
			beginLine = oldDoc.GetLineSegment(lineNr + 1);
			int insertOffset = beginLine.Offset;
			foreach (IField field in generatedClass.Fields) {
				LineSegment fieldLine = newDoc.GetLineSegment(field.Region.BeginLine - 1);
				oldDoc.Insert(insertOffset,newDoc.GetText(fieldLine.Offset, fieldLine.TotalLength));
			}
			textArea.Document.TextContent = oldDoc.TextContent;
			textArea.EndUpdate();
			viewContent.IsDirty = dirty;
		}
		
		void Reparse(string fileName, string content)
		{
			if (fileName == null) {
				fileName = "a.cs";
			}
			// get new initialize components
			IParserService parserService    = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			IParseInformation info = parserService.ParseFile(fileName, content);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (IsBaseClassDesignable(c)) {
					initializeComponents = GetInitializeComponents(c);
					if (initializeComponents != null) {
						this.c = c;
						break;
					}
				}
			}
		}

		protected virtual bool IsBaseClassDesignable(IClass c)
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
		
		public override void ShowSourceCode()
		{
			WorkbenchWindow.SwitchView(0);
		}
		
		public override void ShowSourceCode(int lineNumber)
		{
			textAreaControlProvider.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber, 0);
			ShowSourceCode();
		}
		
		protected static string GenerateParams(EventDescriptor edesc, bool paramNames)
		{
			System.Type type =  edesc.EventType;
			MethodInfo mInfo = type.GetMethod("Invoke");
			string param = "";
			IAmbience csa = null;
			try {
				csa = (IAmbience)AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences").BuildChildItem("CSharp", typeof(CSharpDesignerDisplayBindingWrapper));
			} catch {}
			
			for (int i = 0; i < mInfo.GetParameters().Length; ++i)  {
				ParameterInfo pInfo  = mInfo.GetParameters()[i];
				
				string typeStr = pInfo.ParameterType.ToString();
				if (csa != null) {
					typeStr = csa.GetIntrinsicTypeName(typeStr);
				}
				param += typeStr;
				if (paramNames == true) {
						param += " ";
						param += pInfo.Name;
				}
				if (i + 1 < mInfo.GetParameters().Length) {
					param += ", ";
				}
			}
			return param;
		}
		
		/// <summary>
		/// If found return true and int as position
		/// </summary>
		/// <param name="component"></param>
		/// <param name="edesc"></param>
		/// <returns></returns>
		protected bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out int position)
		{
			if (this.failedDesignerInitialize) {
				position = 0;
				return false;
			}

			Reparse("a.cs", Document.TextContent);
			
			foreach (IMethod method in c.Methods) {
				if (method.Name == eventMethodName) {
					position = method.Region.BeginLine;
					return true;
				}
			}
			position = c.Region.EndLine - 1;
			
			int offset = Document.GetLineSegment(c.Region.EndLine - 1).Offset;
			
			string param = GenerateParams(edesc, true);
			
			string text = "void " + eventMethodName + "(" + param + ")\n" +
			"{\n" + body +
			"\n}\n\n";
			Document.Insert(offset, text);
			Document.FormattingStrategy.IndentLines(this.textAreaControlProvider.TextEditorControl.ActiveTextAreaControl.TextArea, c.Region.EndLine - 1, c.Region.EndLine + 3);
			return false;
		}
		
		public override void ShowSourceCode(IComponent component, EventDescriptor edesc, string eventMethodName)
		{
			int position;
			if (InsertComponentEvent(component, edesc, eventMethodName, "", out position) == true) {
				ShowSourceCode(position);
			} else {
				ShowSourceCode(c.Region.EndLine);
			}
		}
		
		public override ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			Reparse("a.cs", Document.TextContent);
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == methodInfo.GetParameters().Length) {
					bool found = true;
					for (int i = 0; i < methodInfo.GetParameters().Length; ++i) {
						ParameterInfo pInfo = methodInfo.GetParameters()[i];
						IParameter p = method.Parameters[i];
						if (p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString()) {
							found = false;
							break;
						}
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			
			return compatibleMethods;
		}
		
		public void Selected()
		{
			isFormDesignerVisible = true;
			Reload();
			if (!failedDesignerInitialize) {
				base.designPanel.Enable();
				base.SelectMe(this, EventArgs.Empty);
			} else {
				base.designPanel.SetErrorState(compilationErrors);
			}
		}
		
		public void Deselected()
		{
			isFormDesignerVisible = false;
			base.designPanel.Disable();
			if (!failedDesignerInitialize) {
				MergeFormChanges();
				textAreaControlProvider.TextEditorControl.Refresh();
				base.DeSelectMe(this, EventArgs.Empty);
			}
			DeselectAllComponents();
		}
		
		public void NotifyBeforeSave()
		{
			MergeFormChanges();
		}
	}
}
