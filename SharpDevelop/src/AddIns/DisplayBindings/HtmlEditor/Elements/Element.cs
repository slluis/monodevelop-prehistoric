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
	public class Element
	{
		IHTMLElement htmlElement;
		
		public IHTMLElement HtmlElement {
			get {
				return htmlElement;
			}
			set {
				htmlElement = value;
			}
		}
		
		public string InnerHtml {
			get {
				try {
					return htmlElement.innerHTML;
				} catch (Exception) {
					return String.Empty;
				}
			}
			set {
				try {
					htmlElement.innerHTML = value;
				} catch (Exception) {}
			}
		}
		
		public string TagName {
			get {
				try {
					return htmlElement.tagName;
				} catch (Exception) {
					return String.Empty;
				}
			}
		}
		
//		public Element(IHTMLElement htmlElement)
//		{
//			this.htmlElement = htmlElement;
//		}
		
		public object GetAttribute(string attribute)
		{
			try {
				object val = this.htmlElement.getAttribute(attribute, 0);
				if (val is DBNull) {
					return null;
				}
				return val;
			} catch (Exception) {
				return null;
			}
		}
		
		public string GetStringAttribute(string attribute)
		{
			return this.GetStringAttribute(attribute, String.Empty);
		}
		
		public string GetStringAttribute(string attribute, string defaultValue)
		{
			object val = this.GetAttribute(attribute);
			if (val == null) {
				return defaultValue;
			}
			
			if (val is string) {
				return val.ToString();
			}
			return defaultValue;
		}
		
		public void SetStringAttribute(string attribute, string val)
		{
			this.SetStringAttribute(attribute, val, String.Empty);
		}
		
		public void SetStringAttribute(string attribute, string val, string defaultValue)
		{
			if (val == null || val == defaultValue) {
				this.RemoveAttribute(attribute);
				return;
			}
			this.SetAttribute(attribute, val);
		}
		
		
		public bool GetBooleanAttribute(string attribute) {
			object local0;
		
			local0 = this.GetAttribute(attribute);
			if (local0 == null)
				return false;
			if (local0 is bool)
				return (bool) local0;
			return false;
		}

		
		public void SetBooleanAttribute(string attribute, bool val) 
		{
			if (val) {
				this.SetAttribute(attribute, true);
				return;
			}
			this.RemoveAttribute(attribute);
		}

		
		public void SetAttribute(string attribute, object val) 
		{
			try {
				this.htmlElement.setAttribute(attribute, val, 0);
			} catch (Exception) {}
		}
		
		public void RemoveAttribute(string attribute)
		{
			try {
				this.htmlElement.removeAttribute(attribute, 0);
			} catch (Exception) {}
		}
	}
}
