using System;
using System.IO;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.AddIns.Codons;
using System.Runtime.InteropServices;

using Gtk;
using GtkSharp;

using MonoDevelop.Gui.Utils;
using MonoDevelop.EditorBindings.Properties;
using MonoDevelop.EditorBindings.FormattingStrategy;

namespace MonoDevelop.SourceEditor.Gui {
	public class SourceEditorDisplayBinding : IDisplayBinding {

		static SourceEditorDisplayBinding ()
		{
			GtkSourceView.Init ();
		}

		public virtual bool CanCreateContentForFile (string fileName)
		{
			return false;
		}

		public virtual bool CanCreateContentForMimeType (string mimetype)
		{
			if (mimetype.StartsWith ("text")) return true;
			return false;
		}
		
		public virtual bool CanCreateContentForLanguage (string language)
		{
			return true;
		}
		
		public virtual IViewContent CreateContentForFile (string fileName)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			w.Load (fileName);
			return w;
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			// HACK HACK
			if (language == "C#")
				language = "text/x-csharp";
			else
				language = "text/plain";
			
			w.LoadString (language, content);
			return w;
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content, string new_file_name)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			// HACK HACK
			if (language == "C#")
				language = "text/x-csharp";
			else
				language = "text/plain";
			
			StringParserService sps = (StringParserService)ServiceManager.Services.GetService (typeof (StringParserService));
			
			w.LoadString (language, sps.Parse (content));
			return w;
		}	
	}
	
	public class SourceEditorDisplayBindingWrapper : AbstractViewContent,
		IEditable, IPositionable, IBookmarkOperations
	{
		internal SourceEditor se;
		
		public override Gtk.Widget Control {
			get {
				return se;
			}
		}
		
		public override string TabPageText {
			get {
				return "${res:FormsDesigner.DesignTabPages.SourceTabPage}";
			}
		}
		
		public SourceEditorDisplayBindingWrapper ()
		{
			se = new SourceEditor (this);
			se.Buffer.ModifiedChanged += new EventHandler (OnModifiedChanged);
			se.Buffer.MarkSet += new MarkSetHandler (OnMarkSet);
			se.Buffer.Changed += new EventHandler (OnChanged);
			se.View.ToggleOverwrite += new EventHandler (CaretModeChanged);
			
			CaretModeChanged (null, null);
			PropertiesChanged (null, null);
			
			PropertyService propertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));
			IProperties properties2 = ((IProperties) propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			properties2.PropertyChanged += new PropertyEventHandler (PropertiesChanged);
		}
		
		public void JumpTo (int line, int column)
		{
			// NOTE: 0 based!
			
			TextIter itr = se.Buffer.GetIterAtLine (line);
			itr.LineOffset = column;
			
			se.Buffer.PlaceCursor (itr);
			
			se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
			se.View.GrabFocus ();
		}
		
		public void GotoMatchingBrace ()
		{
			TextIter iter = se.Buffer.GetIterAtMark (se.Buffer.InsertMark);
			if (Source.IterFindMatchingBracket (ref iter)) {
				se.Buffer.PlaceCursor (iter);
				se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
			}
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
		}
		
		void OnModifiedChanged (object o, EventArgs e)
		{
			this.IsDirty = se.Buffer.Modified;
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override void Save (string fileName)
		{
			se.Buffer.Save (fileName);
			ContentName = fileName;
			InitializeFormatter ();
		}
		
		public override void Load (string fileName)
		{
			se.Buffer.LoadFile (fileName, Vfs.GetMimeType (fileName));

			ContentName = fileName;
			InitializeFormatter ();
		}
		
		public void InitializeFormatter()
		{
			/*IFormattingStrategy[] formater = (IFormattingStrategy[])(AddInTreeSingleton.AddInTree.GetTreeNode("/AddIns/DefaultTextEditor/Formater").BuildChildItems(this)).ToArray(typeof(IFormattingStrategy));
			Console.WriteLine("SET FORMATTER : " + formater[0]);
			if (formater != null && formater.Length > 0) {
//					formater[0].Document = Document;
				se.View.fmtr = formater[0];
			}*/

		}
		
		public void InsertAtCursor (string s)
		{
			se.Buffer.InsertAtCursor (s);
			se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
			
		}
		
		public void LoadString (string mime, string val)
		{
			if (mime != null)
				se.Buffer.LoadText (val, mime);
			else
				se.Buffer.LoadText (val);
		}
		
#region IEditable
		public IClipboardHandler ClipboardHandler {
			get { return se.Buffer; }
		}
		
		public string Text {
			get { return se.Buffer.Text; }
			set { se.Buffer.Text = value; }
		}
		
		public void Undo ()
		{
			se.Buffer.Undo ();
		}
		
		public void Redo ()
		{
			se.Buffer.Redo ();
		}
#endregion
#region Status Bar Handling
		IStatusBarService statusBarService = (IStatusBarService) ServiceManager.Services.GetService (typeof (IStatusBarService));
		
		void OnMarkSet (object o, MarkSetArgs args)
		{
			// 99% of the time, this is the insertion point
			UpdateLineCol ();
		}
		
		void OnChanged (object o, EventArgs e)
		{
			// gedit also hooks this event, but do we need it?
			UpdateLineCol ();
		}
		
		// WORKAROUND until we get this method returning char in gtk#
		[DllImport("libgtk-win32-2.0-0.dll")]
		static extern char gtk_text_iter_get_char (ref Gtk.TextIter raw);
		
		void UpdateLineCol ()
		{
			int col = 1; // first char == 1
			int chr = 1;
			bool found_non_ws = false;
			int tab_size = (int) se.View.TabsWidth;
			
			TextIter iter = se.Buffer.GetIterAtMark (se.Buffer.InsertMark);
			TextIter start = iter;
			
			iter.LineOffset = 0;
			
			while (! iter.Equal (start)) {
				char c = gtk_text_iter_get_char (ref iter);
				
				if (c == '\t')
					col += (tab_size - (col % tab_size));
				else
					col ++;
				
				if (c != '\t' && c != ' ')
					found_non_ws = true;
				
				if (found_non_ws ) {
					if (c == '\t')
						chr += (tab_size - (col % tab_size));
					else
						chr ++;
				}
				
				iter.ForwardChar ();
			}
			
			// NOTE: this is absurd, *I* should tell the status bar which numbers
			// to print.
			statusBarService.SetCaretPosition (col - 1, iter.Line, chr - 1);
		}
		
		// This is false because we at first `toggle' it to set it to true
		bool insert_mode = false; // TODO: is this always the default
		void CaretModeChanged (object sender, EventArgs e)
		{
			statusBarService.SetInsertMode (insert_mode = ! insert_mode);
		}
#endregion

#region IBookmarkOperations
		void IBookmarkOperations.ToggleBookmark ()
		{
			se.Buffer.ToggleBookmark ();
		}
		
		void IBookmarkOperations.PrevBookmark ()
		{
			se.Buffer.PrevBookmark ();
		}
		
		void IBookmarkOperations.NextBookmark ()
		{
			se.Buffer.NextBookmark ();
		}
		
		void IBookmarkOperations.ClearBookmarks ()
		{
			se.Buffer.ClearBookmarks ();
		}
#endregion
		
		void PropertiesChanged (object sender, PropertyEventArgs e)
 		{
			se.View.ModifyFont (TextEditorProperties.Font);
			se.View.ShowLineNumbers = TextEditorProperties.ShowLineNumbers;
			se.Buffer.CheckBrackets = TextEditorProperties.ShowMatchingBracket;
			se.View.ShowMargin = TextEditorProperties.ShowVerticalRuler;
			if (TextEditorProperties.VerticalRulerRow > -1) {
				se.View.Margin = (uint) TextEditorProperties.VerticalRulerRow;
			} else {
				se.View.Margin = (uint) 80;		// FIXME: should i be doing this on a bad vruller setting?
			}
			if (TextEditorProperties.TabIndent > -1) {
				se.View.TabsWidth = (uint) TextEditorProperties.TabIndent;
			} else {
				se.View.TabsWidth = (uint) 8;	// FIXME: should i be doing this on a bad tabindent setting?
			}
			se.View.InsertSpacesInsteadOfTabs = TextEditorProperties.ConvertTabsToSpaces;
			se.View.AutoIndent = (TextEditorProperties.IndentStyle == IndentStyle.Auto);
			//System.Console.WriteLine(e.Key + " = " + e.NewValue + "(from " + e.OldValue + ")" );
					// The items below can't be done (since there is no support for it in gtksourceview)
					// CANTDO: show spaces				Key = "ShowSpaces"
					// CANTDO: show tabs				Key = "ShowTabs"
					// CANTDO eol makers				Key = "ShowEOLMarkers"
					// CANTDO: show horizontal ruler	Key = "ShowHRuler"		
					// CANTDO: underline errors			Key = "ShowErrors"					
					// DONOTDO: auto insert braces		Key = "AutoInsertCurlyBracket"
					// TODO: Show Invalid Lines			Key = "ShowInvalidLines"
					// TODO: CodeCompletion				Key = "EnableCodeCompletion"
					// TODO: Code Folding				Key = "EnableFolding"
					// TODO: Double Buffering			Key = "DoubleBuffer"
					// TODO: can move past EOL 			Key = "CursorBehindEOL"
					// TODO: auot insert template		Key = "AutoInsertTemplates"	
					// TODO: hide mouse while typing 	Key = "HideMouseCursor"
 		}

	}
}
