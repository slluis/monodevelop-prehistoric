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
	public abstract class InputElement : SelectableElement
	{
		public bool Disabled {
			get {
				return GetBooleanAttribute("disabled");
			}
			set {
				this.SetBooleanAttribute("disabled", value);
			}
		}
		
		public string Name {
			get {
				return GetStringAttribute("name");
			}
			set {
				SetStringAttribute("name", value);
			}
		}
		
		public string OnChange {
			get {
				return GetStringAttribute("onChange");
			}
			set {
				SetStringAttribute("onChange", value);
			}
		}
		
		public string Value {
			get {
				return GetStringAttribute("value");
			}
			set {
				SetStringAttribute("value", value);
			}
		}
		
		public abstract string Type {
			get;
		}
		
//		public InputElement(IHTMLElement htmlElement) : base(htmlElement)
//		{
//		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('<');
			sb.Append("input");
			sb.Append(" type=\"");
			sb.Append(Type);
			sb.Append("\">");
			return sb.ToString();
		}
	}
}
