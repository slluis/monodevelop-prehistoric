using System;
using System.IO;
using System.Runtime.InteropServices;

using MonoDevelop.Gui;
using MonoDevelop.Internal.Project;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core.AddIns;
using MonoDevelop.Core.Services;
using MonoDevelop.Core.AddIns.Codons;
using MonoDevelop.Gui.Utils;
using MonoDevelop.EditorBindings.Properties;
using MonoDevelop.EditorBindings.FormattingStrategy;
using MonoDevelop.Services;

using Gtk;
using GtkSourceView;

namespace MonoDevelop.SourceEditor.Gui
{
	public class SourceEditorDisplayBinding : IDisplayBinding
	{
		StringParserService sps = (StringParserService) ServiceManager.GetService (typeof (StringParserService));
		
		static SourceEditorDisplayBinding ()
		{
			GtkSourceViewManager.Init ();
		}

		public virtual bool CanCreateContentForFile (string fileName)
		{
			return false;
		}

		public virtual bool CanCreateContentForMimeType (string mimetype)
		{
			if (mimetype == null)
				return false;
			if (mimetype.StartsWith ("text"))
				return true;
			if (mimetype == "application/x-python")
				return true;
			if (mimetype == "application/x-config")
				return true;
			if (mimetype == "application/x-aspx")
				return true;
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
			return CreateContentForLanguage (language, content, null);
		}
		
		public virtual IViewContent CreateContentForLanguage (string language, string content, string new_file_name)
		{
			SourceEditorDisplayBindingWrapper w = new SourceEditorDisplayBindingWrapper ();
			
			switch (language.ToUpper ()) {
				case "C#":
					language = "text/x-csharp";
					break;
				case "JAVA":
					language = "text/x-java";
					break;
				//case language "VBNET":
				//	language = "text/x-vbnet";
				//	break;
				case "NEMERLE":
					language = "text/x-nemerle";
					break;
				default:
					language = "text/plain";
					break;
			}
			
			w.LoadString (language, sps.Parse (content));
			return w;
		}	
	}
	
	public class SourceEditorDisplayBindingWrapper : AbstractViewContent,
		IEditable, IPositionable, IBookmarkOperations, IDebuggableEditor, ICodeStyleOperations
	{

		internal FileSystemWatcher fsw;
	
		internal SourceEditor se;

		object fileSaveLock = new object ();
		DateTime lastSaveTime;
		
		void UpdateFSW (object o, EventArgs e)
		{
			if (ContentName == null || ContentName.Length == 0)
				return;

			fsw.EnableRaisingEvents = false;
			lastSaveTime = File.GetLastWriteTime (ContentName);
			fsw.Path = Path.GetDirectoryName (ContentName);
			fsw.Filter = Path.GetFileName (ContentName);
			fsw.EnableRaisingEvents = true;
		}

		private void OnFileChanged (object o, FileSystemEventArgs e)
		{
			lock (fileSaveLock) {
				if (lastSaveTime == File.GetLastWriteTime (ContentName))
					return;
			}
			DispatchService dispatcher = (DispatchService)ServiceManager.GetService (typeof (DispatchService));
			dispatcher.GuiDispatch (new StatefulMessageHandler (realFileChanged), e);
		}

		MessageDialog ReloadFileDialog;
		void realFileChanged (object evnt)
		{
			FileSystemEventArgs e = (FileSystemEventArgs)evnt;
			if (e.ChangeType == WatcherChangeTypes.Changed) {
				//To prevent more than one being shown at a
				//time, check if this is not null.
				if (ReloadFileDialog != null)
					return;
				ReloadFileDialog = new MessageDialog ((Gtk.Window)WorkbenchSingleton.Workbench, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, String.Format (GettextCatalog.GetString ("The file {0} has been changed outside of MonoDevelop. Would you like to reload the file?"), ContentName));
				ReloadFileDialog.Response += new Gtk.ResponseHandler (Responded);
				ReloadFileDialog.ShowAll ();
			}
		}

		void Responded (object o, ResponseArgs e)
		{
			if (e.ResponseId == ResponseType.Yes)
				Load (ContentName);
			ReloadFileDialog.Hide ();
			ReloadFileDialog.Response -= new Gtk.ResponseHandler (Responded);
			ReloadFileDialog.Destroy ();
			ReloadFileDialog = null;
		}

		public void ExecutingAt (int line)
		{
			se.ExecutingAt (line);
		}

		public void ClearExecutingAt (int line)
		{
			se.ClearExecutingAt (line);
		}
		
		public override Gtk.Widget Control {
			get {
				return se;
			}
		}
		
		public override string TabPageLabel {
			get {
				return GettextCatalog.GetString ("Source Editor");
			}
		}
		
		public SourceEditorDisplayBindingWrapper ()
		{
			se = new SourceEditor (this);
			se.Buffer.ModifiedChanged += new EventHandler (OnModifiedChanged);
			se.Buffer.MarkSet += new MarkSetHandler (OnMarkSet);
			se.Buffer.Changed += new EventHandler (OnChanged);
			se.View.ToggleOverwrite += new EventHandler (CaretModeChanged);
			ContentNameChanged += new EventHandler (UpdateFSW);
			
			CaretModeChanged (null, null);
			PropertiesChanged (null, null);
			
			PropertyService propertyService = (PropertyService) ServiceManager.GetService (typeof (PropertyService));
			IProperties properties2 = ((IProperties) propertyService.GetProperty("MonoDevelop.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			properties2.PropertyChanged += new PropertyEventHandler (PropertiesChanged);
			fsw = new FileSystemWatcher ();
			fsw.Changed += new FileSystemEventHandler (OnFileChanged);
			UpdateFSW (null, null);
		}

		public void JumpTo (int line, int column)
		{
			// NOTE: 0 based!			
			TextIter itr = se.Buffer.GetIterAtLine (line);
			itr.LineOffset = column;
			
			se.Buffer.PlaceCursor (itr);		
			se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
			Gtk.Timeout.Add (20, new Gtk.Function (changeFocus));
		}

		//This code exists to workaround a gtk+ 2.4 regression/bug
		//
		//The gtk+ 2.4 treeview steals focus with double clicked
		//row_activated.
		// http://bugzilla.gnome.org/show_bug.cgi?id=138458
		bool changeFocus ()
		{
			se.View.GrabFocus ();
			return false;
		}
		
		public void GotoMatchingBrace ()
		{
			TextIter iter = se.Buffer.GetIterAtMark (se.Buffer.InsertMark);
			if (Source.IterFindMatchingBracket (ref iter)) {
				iter.ForwardChar ();
				se.Buffer.PlaceCursor (iter);
				se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
			}
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
			fsw.Dispose ();
		}
		
		void OnModifiedChanged (object o, EventArgs e)
		{
			this.IsDirty = se.Buffer.Modified;
		}
		
		public override bool IsReadOnly
		{
			get {
				return !se.View.Editable;
			}
		}
		
		public override void Save (string fileName)
		{
			lock (fileSaveLock) {
				se.Buffer.Save (fileName);
				lastSaveTime = File.GetLastWriteTime (fileName);
			}
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
		
		string cachedText;
		GLib.IdleHandler bouncingDelegate;
		
		public string Text {
			get {
				if (bouncingDelegate == null)
					bouncingDelegate = new GLib.IdleHandler (BounceAndGrab);
				if (needsUpdate) {
					GLib.Idle.Add (bouncingDelegate);
				}
				return cachedText;
			}
			set { se.Buffer.Text = value; }
		}

		bool needsUpdate;
		bool BounceAndGrab ()
		{
			if (needsUpdate) {
				cachedText = se.Buffer.Text;
				needsUpdate = false;
			}
			return false;
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
		IStatusBarService statusBarService = (IStatusBarService) ServiceManager.GetService (typeof (IStatusBarService));
		
		void OnMarkSet (object o, MarkSetArgs args)
		{
			// 99% of the time, this is the insertion point
			UpdateLineCol ();
		}
		
		void OnChanged (object o, EventArgs e)
		{
			// gedit also hooks this event, but do we need it?
			UpdateLineCol ();
			OnContentChanged (null);
			needsUpdate = true;
		}
		
		void UpdateLineCol ()
		{
			int col = 1; // first char == 1
			int chr = 1;
			bool found_non_ws = false;
			int tab_size = (int) se.View.TabsWidth;
			
			TextIter iter = se.Buffer.GetIterAtMark (se.Buffer.InsertMark);
			TextIter start = iter;
			
			iter.LineOffset = 0;
			
			while (!iter.Equal (start))
			{
				char c = iter.Char.ToCharArray ()[0];
				
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
			
			statusBarService.SetCaretPosition (iter.Line + 1, col, chr);
		}
		
		// This is false because we at first `toggle' it to set it to true
		bool insert_mode = false; // TODO: is this always the default
		void CaretModeChanged (object sender, EventArgs e)
		{
			statusBarService.SetInsertMode (insert_mode = ! insert_mode);
		}
#endregion
#region ICodeStyleOperations
		void ICodeStyleOperations.CommentCode ()
		{
			se.Buffer.CommentCode ();
		}
		void ICodeStyleOperations.UncommentCode ()
		{
			se.Buffer.UncommentCode ();
		}
		
		void ICodeStyleOperations.IndentSelection ()
		{
			se.View.IndentSelection ();
		}
		
		void ICodeStyleOperations.UnIndentSelection ()
		{
			se.View.UnIndentSelection ();
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
			se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
		}
		
		void IBookmarkOperations.NextBookmark ()
		{
			se.Buffer.NextBookmark ();
			se.View.ScrollMarkOnscreen (se.Buffer.InsertMark);
		}
		
		void IBookmarkOperations.ClearBookmarks ()
		{
			se.Buffer.ClearBookmarks ();
		}
#endregion
		
		void PropertiesChanged (object sender, PropertyEventArgs e)
 		{
 			// FIXME: do these seperately
			se.View.ModifyFont (TextEditorProperties.Font);
			se.View.ShowLineNumbers = TextEditorProperties.ShowLineNumbers;
			se.Buffer.CheckBrackets = TextEditorProperties.ShowMatchingBracket;
			se.View.ShowMargin = TextEditorProperties.ShowVerticalRuler;
			se.View.EnableCodeCompletion = TextEditorProperties.EnableCodeCompletion;
			
			if (TextEditorProperties.VerticalRulerRow > -1) {
				se.View.Margin = (uint) TextEditorProperties.VerticalRulerRow;
			} else {
				se.View.Margin = (uint) 80;		// FIXME: should i be doing this on a bad vruller setting?
			}
			
			if (TextEditorProperties.TabIndent > -1) {
				se.View.TabsWidth = (uint) TextEditorProperties.TabIndent;
			} else {
				se.View.TabsWidth = (uint) 4;	// FIXME: should i be doing this on a bad tabindent setting?
			}
			
			se.View.InsertSpacesInsteadOfTabs = TextEditorProperties.ConvertTabsToSpaces;
			se.View.AutoIndent = (TextEditorProperties.IndentStyle == IndentStyle.Auto);
			
			//System.Console.WriteLine(e.Key + " = " + e.NewValue + "(from " + e.OldValue + ")" );
					// The items below can't be done (since there is no support for it in gtksourceview)
					// CANTDO: show spaces				Key = "ShowSpaces"
					// CANTDO: show tabs				Key = "ShowTabs"
					// CANTDO eol makers				Key = "ShowEOLMarkers"
					// CANTDO: show horizontal ruler	Key = "ShowHRuler"		
					// CANDO in pango1.4: underline errors			Key = "ShowErrors"
					// DONOTDO: auto insert braces		Key = "AutoInsertCurlyBracket"
					// TODO: Show Invalid Lines			Key = "ShowInvalidLines"
					// TODO: Code Folding				Key = "EnableFolding"
					// TODO: Double Buffering			Key = "DoubleBuffer"
					// TODO: can move past EOL 			Key = "CursorBehindEOL"
					// TODO: auto insert template		Key = "AutoInsertTemplates"	
 		}
	}
}
