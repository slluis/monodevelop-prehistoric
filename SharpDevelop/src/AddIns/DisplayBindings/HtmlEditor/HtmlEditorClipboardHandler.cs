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
using SharpDevelop.Internal.Parser;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace HtmlBackendBinding
{
	public class HtmlEditorClipboardHandler : IClipboardHandler
	{
		onlyconnect.HtmlEditor htmlEditor;
		public bool EnableCut {
			get {
				return true;
			}
		}
		
		public bool EnableCopy {
			get {
				return true;
			}
		}
		
		public bool EnablePaste {
			get {
				return true;
			}
		}
		
		public bool EnableDelete {
			get {
				return true;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return true;
			}
		}		
		public HtmlEditorClipboardHandler(onlyconnect.HtmlEditor htmlEditor)
		{
			this.htmlEditor = htmlEditor;
		}
		
		public void Cut(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("Cut", false, null);
		}
		
		public void Copy(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("Copy", false, null);
		}
		
		public void Paste(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("Paste", false, null);
		}
		
		public void Delete(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("Delete", false, null);
		}
		
		public void SelectAll(object sender, EventArgs e)
		{
			this.htmlEditor.Document.execCommand("SelectAll", false, null);
		}

	}
}
