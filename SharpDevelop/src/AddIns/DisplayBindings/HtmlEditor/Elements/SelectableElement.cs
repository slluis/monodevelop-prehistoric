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
using mshtml;

namespace HtmlBackendBinding
{
	public class SelectableElement : StyledElement
	{
		public string AccessKey {
			get {
				return GetStringAttribute("accessKey");
			}
			set {
				SetStringAttribute("accessKey", value);
			}
		}
		
		public string HideFocus {
			get {
				return GetStringAttribute("hideFocus");
			}
			set {
				SetStringAttribute("hideFocus", value);
			}
		}
		
		public string OnFocus {
			get {
				return GetStringAttribute("onFocus");
			}
			set {
				SetStringAttribute("onFocus", value);
			}
		}
		
		public string OnKeyPress {
			get {
				return GetStringAttribute("onKeyPress");
			}
			set {
				SetStringAttribute("onKeyPress", value);
			}
		}
		
		public bool RunAtServer {
			get {
				string local0;
				
				local0 = this.GetAttribute("runAt") as String;
				if (local0 != null)
					return local0 == "server";
				return false;
			}
			set {
				if (value) {
					this.SetAttribute("runAt", "server");
					return;
				}
				this.RemoveAttribute("runAt");
			}
		}
		
		public short TabIndex {
			get {
				object tabIndex = GetAttribute("tabIndex");
				return tabIndex == null ? (short)0 : (short)tabIndex;
			}
			set {
				SetAttribute("tabIndex", value);
			}
		}
		
//		public SelectableElement(IHTMLElement htmlElement) : base(htmlElement)
//		{
//		}
	}
}
