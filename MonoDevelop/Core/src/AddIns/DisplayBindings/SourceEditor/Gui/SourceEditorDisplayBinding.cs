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
		VBox mainBox;
		HBox editorBar;
		HBox reloadBar;
		internal FileSystemWatcher fsw;
		IProperties properties;
		
		BreakpointEventHandler breakpointAddedHandler;
		BreakpointEventHandler breakpointRemovedHandler;
		EventHandler executionChangedHandler;
		int currentExecutionLine = -1;
	
		internal SourceEditor se;

		object fileSaveLock = new object ();
		DateTime lastSaveTime;
		bool warnOverwrite = false;
		
		void UpdateFSW (object o, EventArgs e)
		{
			if (ContentName == null || ContentName.Length == 0 || !File.Exists (ContentName))
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
				ShowFileChangedWarning ();
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
				return mainBox;
			}
		}
		
		public SourceEditor Editor {
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
			mainBox = new VBox ();
			editorBar = new HBox ();
			mainBox.PackStart (editorBar, false, false, 0);
			se = new SourceEditor (this);
			se.Buffer.ModifiedChanged += new EventHandler (OnModifiedChanged);
			se.Buffer.MarkSet += new MarkSetHandler (OnMarkSet);
			se.Buffer.Changed += new EventHandler (OnChanged);
			se.View.ToggleOverwrite += new EventHandler (CaretModeChanged);
			ContentNameChanged += new EventHandler (UpdateFSW);
			
			CaretModeChanged (null, null);
			SetInitialValues ();
			
			PropertyService propertyService = (PropertyService) ServiceManager.GetService (typeof (PropertyService));
			properties = ((IProperties) propertyService.GetProperty("MonoDevelop.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
			properties.PropertyChanged += new PropertyEventHandler (PropertiesChanged);
			fsw = new FileSystemWatcher ();
			fsw.Changed += new FileSystemEventHandler (OnFileChanged);
			UpdateFSW (null, null);
			mainBox.PackStart (se, true, true, 0);
			
			if (Runtime.DebuggingService != null) {
				breakpointAddedHandler = (BreakpointEventHandler) Runtime.DispatchService.GuiDispatch (new BreakpointEventHandler (OnBreakpointAdded));
				breakpointRemovedHandler = (BreakpointEventHandler) Runtime.DispatchService.GuiDispatch (new BreakpointEventHandler (OnBreakpointRemoved));
				executionChangedHandler = (EventHandler) Runtime.DispatchService.GuiDispatch (new EventHandler (OnExecutionLocationChanged));
				
				Runtime.DebuggingService.BreakpointAdded += breakpointAddedHandler;
				Runtime.DebuggingService.BreakpointRemoved += breakpointRemovedHandler;
				Runtime.DebuggingService.ExecutionLocationChanged += executionChangedHandler;
			}
		}

		public void JumpTo (int line, int column)
		{
			// NOTE: 0 based!			
			TextIter itr = se.Buffer.GetIterAtLine (line);
			itr.LineOffset = column;

			se.Buffer.PlaceCursor (itr);		
			se.Buffer.HighlightLine (line);	
			se.View.ScrollToMark (se.Buffer.InsertMark, 0.3, false, 0, 0);
			GLib.Timeout.Add (20, new GLib.TimeoutHandler (changeFocus));
		}

		//This code exists to workaround a gtk+ 2.4 regression/bug
		//
		//The gtk+ 2.4 treeview steals focus with double clicked
		//row_activated.
		// http://bugzilla.gnome.org/show_bug.cgi?id=138458
		bool changeFocus ()
		{
			se.View.GrabFocus ();
			se.View.ScrollToMark (se.Buffer.InsertMark, 0.3, false, 0, 0);
			return false;
		}
		
		public override void RedrawContent()
		{
		}
		
		public override void Dispose()
		{
			if (Runtime.DebuggingService != null) {
				Runtime.DebuggingService.BreakpointAdded -= breakpointAddedHandler;
				Runtime.DebuggingService.BreakpointRemoved -= breakpointRemovedHandler;
				Runtime.DebuggingService.ExecutionLocationChanged -= executionChangedHandler;
			}

			mainBox.Remove (se);
			properties.PropertyChanged -= new PropertyEventHandler (PropertiesChanged);
			se.Buffer.ModifiedChanged -= new EventHandler (OnModifiedChanged);
			se.Buffer.MarkSet -= new MarkSetHandler (OnMarkSet);
			se.Buffer.Changed -= new EventHandler (OnChanged);
			se.View.ToggleOverwrite -= new EventHandler (CaretModeChanged);
			ContentNameChanged -= new EventHandler (UpdateFSW);
			se.Dispose ();
			fsw.Dispose ();
			se = null;
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
			if (warnOverwrite) {
				if (fileName == ContentName) {
					if (!Runtime.MessageService.AskQuestion (string.Format (GettextCatalog.GetString ("The file {0} has been changed outside of MonoDevelop. Are you sure you want to overwrite the file?"), fileName),"MonoDevelop"))
						return;
				}
				warnOverwrite = false;
				editorBar.Remove (reloadBar);
			}

			lock (fileSaveLock) {
				se.Buffer.Save (fileName);
				lastSaveTime = File.GetLastWriteTime (fileName);
			}
			ContentName = fileName;
			InitializeFormatter ();
		}
		
		public override void Load (string fileName)
		{
			if (warnOverwrite) {
				warnOverwrite = false;
				editorBar.Remove (reloadBar);
			}
			se.Buffer.LoadFile (fileName, Gnome.Vfs.MimeType.GetMimeTypeForUri (fileName));
			ContentName = fileName;
			InitializeFormatter ();
			
			if (Runtime.DebuggingService != null) {
				foreach (IBreakpoint b in Runtime.DebuggingService.GetBreakpointsAtFile (fileName))
					se.View.ShowBreakpointAt (b.Line - 1);
					
				UpdateExecutionLocation ();
			}
		}
		
		void OnBreakpointAdded (object sender, BreakpointEventArgs args)
		{
			se.View.ShowBreakpointAt (args.Breakpoint.Line - 1);
		}
		
		void OnBreakpointRemoved (object sender, BreakpointEventArgs args)
		{
			se.View.ClearBreakpointAt (args.Breakpoint.Line - 1);
		}
		
		void OnExecutionLocationChanged (object sender, EventArgs args)
		{
			UpdateExecutionLocation ();
		}
		
		void UpdateExecutionLocation ()
		{
			if (currentExecutionLine != -1)
				se.View.ClearExecutingAt (currentExecutionLine - 1);

			if (Runtime.DebuggingService.CurrentFilename == ContentName) {
				currentExecutionLine = Runtime.DebuggingService.CurrentLineNumber;
				se.View.ExecutingAt (currentExecutionLine - 1);
				
				TextIter itr = se.Buffer.GetIterAtLine (currentExecutionLine - 1);
				itr.LineOffset = 0;
				se.Buffer.PlaceCursor (itr);		
				se.View.ScrollToMark (se.Buffer.InsertMark, 0.3, false, 0, 0);
				GLib.Timeout.Add (200, new GLib.TimeoutHandler (changeFocus));
			}
			else
				currentExecutionLine = -1;
		}
		
		void ShowFileChangedWarning ()
		{
			if (reloadBar == null) {
				reloadBar = new HBox ();
				reloadBar.BorderWidth = 3;
				Gtk.Image img = Runtime.Gui.Resources.GetImage ("Icons.32x32.Warning", IconSize.Menu);
				reloadBar.PackStart (img, false, false, 2);
				reloadBar.PackStart (new Gtk.Label (GettextCatalog.GetString ("This file has been changed outside of MonoDevelop")), false, false, 5);
				HBox box = new HBox ();
				reloadBar.PackStart (box, true, true, 10);
				
				Button b1 = new Button ("Reload");
				box.PackStart (b1, false, false, 5);
				b1.Clicked += new EventHandler (ClickedReload);
				
				Button b2 = new Button ("Ignore");
				box.PackStart (b2, false, false, 5);
				b2.Clicked += new EventHandler (ClickedIgnore);

				reloadBar.ShowAll ();
			}
			warnOverwrite = true;
			editorBar.PackStart (reloadBar);
			reloadBar.ShowAll ();
			WorkbenchWindow.ShowNotification = true;
		}
		
		void ClickedReload (object sender, EventArgs args)
		{
			editorBar.Remove (reloadBar);
			Load (ContentName);
			WorkbenchWindow.ShowNotification = false;
		}
		
		void ClickedIgnore (object sender, EventArgs args)
		{
			editorBar.Remove (reloadBar);
			WorkbenchWindow.ShowNotification = false;
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
			if (needsUpdate && se != null) {
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
				char c = iter.Char[0];
				
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
			se.ToggleBookmark ();
		}
		
		void IBookmarkOperations.PrevBookmark ()
		{
			se.PrevBookmark ();
		}
		
		void IBookmarkOperations.NextBookmark ()
		{
			se.NextBookmark ();
		}
		
		void IBookmarkOperations.ClearBookmarks ()
		{
			se.ClearBookmarks ();
		}
#endregion

		void SetInitialValues ()
		{
			se.View.ModifyFont (TextEditorProperties.Font);
			se.View.ShowLineNumbers = TextEditorProperties.ShowLineNumbers;
			se.Buffer.CheckBrackets = TextEditorProperties.ShowMatchingBracket;
			se.View.ShowMargin = TextEditorProperties.ShowVerticalRuler;
			se.View.EnableCodeCompletion = TextEditorProperties.EnableCodeCompletion;
			se.View.InsertSpacesInsteadOfTabs = TextEditorProperties.ConvertTabsToSpaces;
			se.View.AutoIndent = (TextEditorProperties.IndentStyle == IndentStyle.Auto);
			se.View.AutoInsertTemplates = TextEditorProperties.AutoInsertTemplates;
			se.Buffer.UnderlineErrors = TextEditorProperties.UnderlineErrors;
			se.Buffer.Highlight = TextEditorProperties.SyntaxHighlight;

			if (TextEditorProperties.VerticalRulerRow > -1)
				se.View.Margin = (uint) TextEditorProperties.VerticalRulerRow;
			else
				se.View.Margin = (uint) 80;

			if (TextEditorProperties.TabIndent > -1)
				se.View.TabsWidth = (uint) TextEditorProperties.TabIndent;
			else
				se.View.TabsWidth = (uint) 4;
		}
		
		void PropertiesChanged (object sender, PropertyEventArgs e)
 		{
			switch (e.Key) {
				case "DefaultFont":
					se.View.ModifyFont (TextEditorProperties.Font);
					break;
				case "ShowLineNumbers":
					se.View.ShowLineNumbers = TextEditorProperties.ShowLineNumbers;
					break;
				case "ShowBracketHighlight":
					se.Buffer.CheckBrackets = TextEditorProperties.ShowMatchingBracket;
					break;
				case "ShowVRuler":
					se.View.ShowMargin = TextEditorProperties.ShowVerticalRuler;
					break;
				case "EnableCodeCompletion":
					se.View.EnableCodeCompletion = TextEditorProperties.EnableCodeCompletion;
					break;
				case "ConvertTabsToSpaces":
					se.View.InsertSpacesInsteadOfTabs = TextEditorProperties.ConvertTabsToSpaces;
					break;
				case "IndentStyle":
					se.View.AutoIndent = (TextEditorProperties.IndentStyle == IndentStyle.Auto);
					break;
				case "AutoInsertTemplates":
					se.View.AutoInsertTemplates = TextEditorProperties.AutoInsertTemplates;
					break;
				case "ShowErrors":
					se.Buffer.UnderlineErrors = TextEditorProperties.UnderlineErrors;
					break;
				case "SyntaxHighlight":
					se.Buffer.Highlight = TextEditorProperties.SyntaxHighlight;
					break;
				case "VRulerRow":
					if (TextEditorProperties.VerticalRulerRow > -1)
						se.View.Margin = (uint) TextEditorProperties.VerticalRulerRow;
					else
						se.View.Margin = (uint) 80;
					break;
				case "TabIndent":
					if (TextEditorProperties.TabIndent > -1)
						se.View.TabsWidth = (uint) TextEditorProperties.TabIndent;
					else
						se.View.TabsWidth = (uint) 4;
					break;
				case "EnableFolding":
					// TODO
					break;
				default:
					Console.WriteLine ("unhandled property change: {0}", e.Key);
					break;
			}
 		}
	}
}

