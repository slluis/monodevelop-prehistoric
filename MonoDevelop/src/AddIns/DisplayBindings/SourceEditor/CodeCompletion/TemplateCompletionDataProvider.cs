// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Reflection;
using System.Collections;

using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Templates;

using MonoDevelop.SourceEditor.Gui;
using Stock = MonoDevelop.Gui.Stock;

namespace MonoDevelop.SourceEditor.CodeCompletion {
	public class TemplateCompletionDataProvider : ICompletionDataProvider {
		public Gdk.Pixbuf[] ImageList {
			get {
				return null;
			}
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, SourceEditorView textArea, char charTyped)
		{
			CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(fileName);
			if (templateGroup == null) {
				return null;
			}
			ArrayList completionData = new ArrayList();
			foreach (CodeTemplate template in templateGroup.Templates) {
				completionData.Add(new TemplateCompletionData(template));
			}
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		class TemplateCompletionData : ICompletionData
		{
			CodeTemplate template;
			
			public string Image {
				get {
					return Stock.Method;
				}
			}
			
			public string[] Text {
				get {
					return new string[] { template.Shortcut, template.Description };
				}
			}
			
			public string Description {
				get {
					return template.Text;
				}
			}
			
			public void InsertAction(SourceEditorView control)
			{
				//((SharpDevelopTextAreaControl)control).InsertTemplate(template);
			}
			
			public TemplateCompletionData(CodeTemplate template) 
			{
				this.template = template;
			}
		}
	}
}
