// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
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

using ICSharpCode.SharpDevelop.FormDesigner.Services;
using ICSharpCode.SharpDevelop.FormDesigner.Hosts;
using ICSharpCode.SharpDevelop.FormDesigner.Util;
using ICSharpCode.Core.AddIns.Codons;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.SharpDevelop.FormDesigner
{
	public class FormDesignerDisplayBindingWrapper : FormDesignerDisplayBindingBase
	{
		TabControl   tabControl;
		TabPage      designerPage;
		
		IViewContent xmlTextEditor;
		IViewContent csharpTextEditor;
		IViewContent vbnetTextEditor;
		
		public override IClipboardHandler ClipboardHandler {
			get {
				if (tabControl.SelectedTab.Controls[0] is IEditable) {
					return ((IEditable)tabControl.SelectedTab.Controls[0]).ClipboardHandler;
				}
				return base.ClipboardHandler;
			}
		}
		
		public override Control Control {
			get {
				return tabControl;
			}
		}
		
		public FormDesignerDisplayBindingWrapper(string fileName, string xmlContent)
		{
			InitializeFrom(fileName, xmlContent);
			tabControl = new TabControl();
			tabControl.SelectedIndexChanged += new EventHandler(TabIndexChanged);
			
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			tabControl.ImageList = new ImageList();
			tabControl.ImageList.Images.Add(iconService.GetBitmap("Icons.16x16.DesignPanel"));
			tabControl.ImageList.Images.Add(iconService.GetBitmap("C#.FileIcon"));
			tabControl.ImageList.Images.Add(iconService.GetBitmap("VB.FileIcon"));
			tabControl.ImageList.Images.Add(iconService.GetBitmap("Icons.16x16.XMLFileIcon"));

			designerPage = new TabPage("Design");
			designerPage.Controls.Add(designPanel);
			designerPage.ImageIndex = 0;
			tabControl.TabPages.Add(designerPage);
			
			TabPage sourcePage = new TabPage("XML");
			sourcePage.ImageIndex = 3;
			TextEditorDisplayBinding tdb = new TextEditorDisplayBinding();
			xmlTextEditor = tdb.CreateContentForLanguage("XML", String.Empty);
			((SharpDevelopTextAreaControl)xmlTextEditor.Control).Document.ReadOnly = true;
			xmlTextEditor.Control.Dock = DockStyle.Fill;
			sourcePage.Controls.Add(xmlTextEditor.Control);
			tabControl.TabPages.Add(sourcePage);
			tabControl.Alignment = TabAlignment.Bottom;
			
			sourcePage = new TabPage("C#");
			sourcePage.ImageIndex = 1;
			csharpTextEditor = tdb.CreateContentForLanguage("C#", String.Empty);
			csharpTextEditor.Control.Dock = DockStyle.Fill;
			((SharpDevelopTextAreaControl)csharpTextEditor.Control).Document.ReadOnly = true;
			sourcePage.Controls.Add(csharpTextEditor.Control);
			tabControl.TabPages.Add(sourcePage);
			tabControl.Alignment = TabAlignment.Bottom;
			
			sourcePage = new TabPage("VB.NET");
			sourcePage.ImageIndex = 2;
			vbnetTextEditor = tdb.CreateContentForLanguage("VBNET", String.Empty);
			vbnetTextEditor.Control.Dock = DockStyle.Fill;
			((SharpDevelopTextAreaControl)vbnetTextEditor.Control).Document.ReadOnly = true;
			sourcePage.Controls.Add(vbnetTextEditor.Control);
			tabControl.TabPages.Add(sourcePage);
			tabControl.Alignment = TabAlignment.Bottom;
			isFormDesignerVisible = true;
			undoHandler.Reset();
		}
		
		void TabIndexChanged(object sender, EventArgs e)
		{
			switch (tabControl.SelectedIndex) {
				case 1:
					((IEditable)xmlTextEditor).Text = GetDataAs("XML");
					break;
				case 2:
					((IEditable)csharpTextEditor).Text = GetDataAs("C#");
					break;
				case 3:
					((IEditable)vbnetTextEditor).Text = GetDataAs("VB.NET");
					break;
			}
			isFormDesignerVisible = tabControl.SelectedIndex == 0;
		}
		public override void ShowSourceCode()
		{
			tabControl.SelectedIndex = 1;
		}
	}
	
	public class FormDesignerDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName) == ".xfrm";
		}
		
		public bool CanCreateContentForLanguage(string languageName)
		{
			return languageName == "XmlForm";
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			StreamReader sr = File.OpenText(fileName);
			string content = sr.ReadToEnd();
			sr.Close();
			return new FormDesignerDisplayBindingWrapper(fileName, content);
		}
		
		public IViewContent CreateContentForLanguage(string languageName, string content)
		{
			return new FormDesignerDisplayBindingWrapper(null, content);
		}
	}
}
