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
	public class StyledElement : Element
	{
		public string ClassName {
			get {
				return HtmlElement.className;
			}
			set {
				HtmlElement.className = value;
			}
		}
		
		public string ID {
			get {
				return HtmlElement.id;
			}
			set {
				HtmlElement.id = value;
			}
		}
		
		public string OnClick {
			get {
				return GetStringAttribute("onClick");
			}
			set {
				SetStringAttribute("onClick", value);
			}
		}
		
		public string OnMouseOut {
			get {
				return GetStringAttribute("onMouseOut");
			}
			set {
				SetStringAttribute("onMouseOut", value);
			}
		}
		
		public string OnMouseOver {
			get {
				return GetStringAttribute("onMouseOver");
			}
			set {
				SetStringAttribute("onMouseOver", value);
			}
		}
		
		public string Title {
			get {
				return GetStringAttribute("title");
			}
			set {
				SetStringAttribute("title", value);
			}
		}
		
//		public StyledElement(IHTMLElement htmlElement) : base(htmlElement)
//		{
//		}
		
		
		
	}
}
