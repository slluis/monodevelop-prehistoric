// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <owner name="Lluis Sanchez" email="lluis@novell.com"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using MonoDevelop.Services;

using MonoDevelop.Core.Properties;
using MonoDevelop.Core.Services;
using MonoDevelop.Gui;

using Gtk;
using Pango;

namespace MonoDevelop.EditorBindings.Gui.Pads
{	
	public class DefaultMonitorPad : IPadContent
	{
		Gtk.TextBuffer buffer;
		Gtk.TextView textEditorControl;
		Gtk.ScrolledWindow scroller;
		
		TextTag tag;
		TextTag bold;
		int ident = 0;
		ArrayList tags = new ArrayList ();

		string markupTitle;
		string title;
		string icon;
		string id;
		
		public DefaultMonitorPad (string title, string icon)
		{
			buffer = new Gtk.TextBuffer (new Gtk.TextTagTable ());
			textEditorControl = new Gtk.TextView (buffer);
			textEditorControl.Editable = false;
			scroller = new Gtk.ScrolledWindow ();
			scroller.ShadowType = ShadowType.In;
			scroller.Add (textEditorControl);
			
			bold = new TextTag ("bold");
			bold.Weight = Pango.Weight.Bold;
			buffer.TagTable.Add (bold);
			
			tag = new TextTag ("0");
			tag.Indent = 10;
			buffer.TagTable.Add (tag);
			tags.Add (tag);
			
			this.title = title;
			this.icon = icon;
			this.markupTitle = title;
		}
		
		public void BeginProgress (string title)
		{
			this.title = title;
			this.markupTitle = "<span foreground=\"blue\">" + title + "</span>";
			
			buffer.Clear ();
			OnTitleChanged (null);
		}
		
		public void BeginTask (string name, int totalWork)
		{
			Indent ();
			TextIter it = buffer.EndIter;
			string txt = "\n" + name + "\n";
			buffer.InsertWithTags (ref it, txt, tag, bold);
		}
		
		public void EndTask ()
		{
			Unindent ();
		}
		
		public void WriteText (string text)
		{
			AddText (text);
//			buffer.MoveMark (buffer.InsertMark, buffer.EndIter);
			if (text.EndsWith ("\n"))
				textEditorControl.ScrollMarkOnscreen (buffer.InsertMark);
		}
		
		public Gtk.Widget Control {
			get { return scroller; }
		}
		
		public string Title {
			get { return markupTitle; }
		}
		
		public string Icon {
			get { return icon; }
		}
		
		public string Id {
			get { return id; }
			set { id = value; }
		}
		
		public override string ToString ()
		{
			return base.ToString () + id;
		}
		
		public void EndProgress ()
		{
			markupTitle = title;
			OnTitleChanged (null);
		}
		
		void AddText (string s)
		{
			TextIter it = buffer.EndIter;
			buffer.InsertWithTags (ref it, s, tag);
		}
		
		void Indent ()
		{
			ident++;
			if (ident >= tags.Count) {
				tag = new TextTag (ident.ToString ());
				tag.Indent = 10 + 15 * (ident - 1);
				buffer.TagTable.Add (tag);
				tags.Add (tag);
			} else {
				tag = (TextTag) tags [ident];
			}
		}
		
		void Unindent ()
		{
			if (ident >= 0) {
				ident--;
				tag = (TextTag) tags [ident];
			}
		}
		
		public virtual void Dispose ()
		{
		}
	
		public void RedrawContent()
		{
			OnTitleChanged(null);
			OnIconChanged(null);
		}
		
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}

		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;

		public void BringToFront()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) {
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
			}
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
		}

	}
}
