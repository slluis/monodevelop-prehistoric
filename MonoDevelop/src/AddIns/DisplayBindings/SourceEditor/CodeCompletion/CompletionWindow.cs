// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;

using Gtk;
using MonoDevelop.SourceEditor.Gui;

namespace MonoDevelop.SourceEditor.CodeCompletion
{
	public class CompletionWindow : Window
	{
		const  int  DeclarationIndent  = 1;
		private static GLib.GType gtype;
		TreeViewColumn complete_column;
		
		ICompletionDataProvider completionDataProvider;
		SourceEditorView control;
		TreeView listView;
		TreeStore store;
		TextMark triggeringMark;
		int origOffset;
		int num_in = 0;
		DeclarationViewWindow declarationviewwindow = new DeclarationViewWindow ();
		
		string GetTypedString ()
		{
			TextIter startIter = control.Buffer.GetIterAtMark (control.Buffer.InsertMark);
			TextIter offsetIter = control.Buffer.GetIterAtMark (triggeringMark);
			return control.Buffer.GetText (offsetIter, startIter, true);
		}

		int insertLength {
			get {
				TextIter startIter = control.Buffer.GetIterAtMark (control.Buffer.InsertMark);
				return startIter.Offset - origOffset;
			}
		}

		void DeleteInsertion()
		{
			TextIter startIter = control.Buffer.GetIterAtMark (control.Buffer.InsertMark);
			TextIter offsetIter = control.Buffer.GetIterAtMark (triggeringMark);
			if (startIter.Offset > offsetIter.Offset) {
				int newPos = offsetIter.Offset;
				control.Buffer.Delete (offsetIter, startIter);
				control.Buffer.MoveMark (control.Buffer.InsertMark, control.Buffer.GetIterAtOffset (newPos));
			}
		}

		protected override bool OnKeyPressEvent (Gdk.EventKey e)
		{
			uint state = (uint)e.State;
			state &= 1101u;

			switch (state) {
			case 0: //NORMAL
				switch ((char)e.Key) {
				case '.':
				case ' ':
				case ';':
				case '(':
				case '[':
					control.SimulateKeyPress (ref e);
					LostFocusListView (null, null);
					return true;
				case (char) Gdk.Key.BackSpace:
					num_in--;
					control.SimulateKeyPress (ref e);
					if (insertLength <= -1) {
						LostFocusListView (null, null);
					}
					RowActivated (null, null);
					return true;
				}
				break;
			case 1: //SHIFT
				switch ((char)e.Key) {
				case 'P':
				case 'N':
					KeyPressEventArgs fake_args = new KeyPressEventArgs ();
					fake_args.Args = new object[] { e };
					ListKeypressEvent (null, fake_args);
					return true;
				}
				break;
			}
			return base.OnKeyPressEvent (e);
		}
		
		void ListKeypressEvent (object sender, KeyPressEventArgs ex)
		{
			Gdk.Key key = ex.Event.Key;
			char val = (char) key;
			
			switch (key) {
				case Gdk.Key.Shift_L:
				case Gdk.Key.Shift_R:
				case Gdk.Key.Control_L:
				case Gdk.Key.Control_R:
					ex.RetVal = true;
					return;
					
				case Gdk.Key.Escape:
					LostFocusListView (null, null);
					ex.RetVal = true;
					return;

				default:
					if (val != '_' && !Char.IsLetterOrDigit (val)) {
						if (listView.Selection.CountSelectedRows () > 0) {
							ActivateItem (null, null);
						} else {
							LostFocusListView (null, null);
						}
						
						//control.Buffer.InsertAtCursor (val.ToString ());
						ex.RetVal = true;
						return;
					} else {
						control.Buffer.InsertAtCursor (val.ToString ());
					}
					break;
			}

			num_in++;

			ShuffleSelection (false);
	
			ex.RetVal = true;
		}

		public bool ShuffleSelection (bool magic)
		{
			// select the current typed word
			int lastSelected = -1;
			int capitalizationIndex = -1;
			int numOfHits = 0;
			
			string typedString = GetTypedString ();
			TreeIter iter;
			int i = 0;
			store.GetIterFirst (out iter);
			do {
				string text = (string) store.GetValue (iter, 0);
				
				if (text.ToUpper ().StartsWith (typedString.ToUpper ())) {
					int currentCapitalizationIndex = 0;
					for (int j = 0; j < typedString.Length && j < text.Length; ++j) {
						if (typedString[j] == text[j]) {
							++currentCapitalizationIndex;
						}
					}
					if (currentCapitalizationIndex >= capitalizationIndex) {
						numOfHits++;
					}
					
					if (currentCapitalizationIndex > capitalizationIndex) {
						lastSelected = i;
						capitalizationIndex = currentCapitalizationIndex;
					}
				}
				i++;
			} while (store.IterNext (out iter) == true);

			if (lastSelected != -1) {
				listView.Selection.UnselectAll ();
				TreePath path = new TreePath ("" + (lastSelected));
				listView.Selection.SelectPath (path);
				listView.SetCursor (path, complete_column, false);
				listView.ScrollToCell (path, null, false, 0, 0);
				if (magic && numOfHits == 1) {
					ActivateItem (null, null);
					Destroy ();
					return true;
				}
			}
			return false;
		}
		
		void InitializeControls ()
		{
			Decorated = false;
			SkipPagerHint = true;
			SkipTaskbarHint = true;
			TypeHint = Gdk.WindowTypeHint.Dialog;
			
			store = new Gtk.TreeStore (typeof (string), typeof (Gdk.Pixbuf), typeof(ICompletionData));
			listView = new Gtk.TreeView (store);
			listView.HeadersVisible = false;

			complete_column = new TreeViewColumn ();
			complete_column.Title = "completion";

			Gtk.CellRendererPixbuf pix_render = new Gtk.CellRendererPixbuf ();
			complete_column.PackStart (pix_render, false);
			complete_column.AddAttribute (pix_render, "pixbuf", 1);
			
			Gtk.CellRendererText text_render = new Gtk.CellRendererText ();
			complete_column.PackStart (text_render, true);
			complete_column.AddAttribute (text_render, "text", 0);
	
			listView.AppendColumn (complete_column);

			Gtk.ScrolledWindow scroller = new Gtk.ScrolledWindow ();
			scroller.HscrollbarPolicy = Gtk.PolicyType.Never;
			scroller.Add (listView);

			Gtk.Frame frame = new Gtk.Frame ();
			frame.Add (scroller);
			this.Add (frame);
			
			listView.KeyPressEvent += new KeyPressEventHandler (ListKeypressEvent);
			//listView.KeyReleaseEvent += new KeyReleaseEventHandler(ListKeyreleaseEvent);
			listView.FocusOutEvent += new FocusOutEventHandler (LostFocusListView);
			listView.RowActivated += new RowActivatedHandler (ActivateItem);
			listView.AddEvents ((int) (Gdk.EventMask.KeyPressMask));
		}
	
		/// <remarks>
		/// Shows the filled completion window, if it has no items it isn't shown.
		/// </remarks>
		public void ShowCompletionWindow (char firstChar, TextIter trigIter, bool magic)
		{
			control.buf.StartAtomicUndo ();
			triggeringMark = control.Buffer.CreateMark (null, trigIter, true);
			origOffset = trigIter.Offset;
			FillList (true, firstChar);

			TreeIter iter;
			if (store.GetIterFirst (out iter) == false) {
				control.GrabFocus ();
				return;
			}

			if (magic) {
				if (ShuffleSelection (true))
					return;
			}

			Gdk.Rectangle rect = control.GetIterLocation (control.Buffer.GetIterAtMark (triggeringMark));

			int wx, wy;
			control.BufferToWindowCoords (Gtk.TextWindowType.Widget, rect.X /*+ rect.Width*/, rect.Y + rect.Height, out wx, out wy);
			
			int tx, ty;
			control.GdkWindow.GetOrigin (out tx, out ty);
			//Console.WriteLine ("Moving to: " + (tx + wx) + " . " + (ty + wy));
			//System.Threading.Thread.Sleep (50);
			ShowAll ();
			Move (tx + wx, ty + wy);
			Present ();
			listView.Selection.Changed += new EventHandler (RowActivated);
			RowActivated (null, null);
			//GdkWindow.Move (tx + wx, ty + wy);
		}
		
		string fileName;
		
		public static new GLib.GType GType
		{
			get
			{
				if (gtype == GLib.GType.Invalid)
					gtype = RegisterGType (typeof (CompletionWindow));
				return gtype;
			}
		}
		
		/// <remarks>
		/// Creates a new Completion window and puts it location under the caret
		/// </remarks>
		public CompletionWindow (SourceEditorView control, string fileName, ICompletionDataProvider completionDataProvider) : base (GType)
		{
			this.fileName = fileName;
			this.completionDataProvider = completionDataProvider;
			this.control = control;

			InitializeControls ();
		}
		
		/// <remarks>
		/// Creates a new Completion window at a given location
		/// </remarks>
		CompletionWindow (SourceEditorView control, Point location, ICompletionDataProvider completionDataProvider) : base (GType)
		{
			this.completionDataProvider = completionDataProvider;
			this.control = control;

			InitializeControls ();
		}
		
		void ActivateItem (object sender, RowActivatedArgs e)
		{
			if (listView.Selection.CountSelectedRows () > 0) {
				TreeModel foo;
				TreeIter iter;
				listView.Selection.GetSelected (out foo, out iter);
				ICompletionData data = (ICompletionData) store.GetValue (iter, 2);
				control.buf.DropCompleteAhead ();
				DeleteInsertion ();
				data.InsertAction (control);
				LostFocusListView (null, null);
			}
		}
		
		void LostFocusListView (object sender, FocusOutEventArgs e)
		{
			control.HasFocus = true;
			declarationviewwindow.HideAll ();
			this.Hide ();
			control.buf.DropCompleteAhead ();
			control.buf.EndAtomicUndo ();
		}
		
		void FillList (bool firstTime, char ch)
		{
			ICompletionData[] completionData = completionDataProvider.GenerateCompletionData(fileName, control, ch, triggeringMark);
			if (completionData == null || completionData.Length == 0) {
				return;
			}
			
			foreach (ICompletionData data in completionData) {
				store.AppendValues (data.Text[0], RenderIcon (data.Image, Gtk.IconSize.Menu, ""), data);
			}
			// sort here
			store.SetSortColumnId (0, SortType.Ascending);
		}
		
		void RowActivated (object sender, EventArgs a)
		{
			Gtk.TreeIter iter;
			Gtk.TreeModel model;
			
			if (listView.Selection.GetSelected (out model, out iter)){
				ICompletionData data = (ICompletionData) store.GetValue (iter, 2);
				if (data == null)
					return;
				
				int inst = insertLength;
				if (inst == -1) {
					LostFocusListView (null, null);
					return;
				}
				if (inst >= 1) {
					DeleteInsertion ();
					int l = inst > data.CompletionString.Length ? data.CompletionString.Length : inst;
					control.buf.InsertAtCursor (data.CompletionString.Substring (0, l));
				}
				control.buf.DropCompleteAhead ();
				if (data.CompletionString.Length > inst) {
					control.buf.DropCompleteAhead ();
					control.buf.CompleteAhead (data.CompletionString.Substring(inst));
				}
					
				// This code is for sizing the treeview properly.
				Gtk.TreePath path = store.GetPath (iter);
				Gdk.Rectangle backRect = listView.GetBackgroundArea (path, (Gtk.TreeViewColumn)listView.Columns[0]);
				
				listView.HeightRequest = (backRect.Height * 5) + 2;

				// FIXME: This code is buggy, and generates a bad placement sometimes when you jump a lot.
				// but it is better than 0,0
				// This code is for sizing the treeview properly.
				
				Gdk.Rectangle rect = listView.GetCellArea (path, (Gtk.TreeViewColumn)listView.Columns[0]);
				int listpos_x = 0, listpos_y = 0;
				while (listpos_x == 0)
					GetPosition (out listpos_x, out listpos_y);
				int vert = listpos_y + rect.Y;
				
				int lvWidth, lvHeight;
				listView.GdkWindow.GetSize (out lvWidth, out lvHeight);
				if (vert >= listpos_y + lvHeight - 2) {
					vert = listpos_y + lvHeight - rect.Height;
				} else if (vert < listpos_y) {
					vert = listpos_y;
				}
				// FIXME: This is a bad calc, its always on the right,
				// it needs to test if thats too big, and if so, place on the left;
				int horiz = listpos_x + lvWidth + 30;
				ICompletionDataWithMarkup wMarkup = data as ICompletionDataWithMarkup;
				declarationviewwindow.Destroy ();
				
				if (wMarkup != null) {
					declarationviewwindow = new DeclarationViewWindow ();
					declarationviewwindow.DescriptionMarkup = wMarkup.DescriptionPango;
				} else {
					declarationviewwindow = new DeclarationViewWindow ();
					declarationviewwindow.DescriptionMarkup = data.Description;
				}
			
				if (declarationviewwindow.DescriptionMarkup.Length == 0)
					return;
	
				declarationviewwindow.ShowAll ();

				int dvwWidth, dvwHeight;
	
				declarationviewwindow.GdkWindow.GetSize (out dvwWidth, out dvwHeight);
				if (listView.Screen.Width <= horiz + dvwWidth) {
					horiz = listpos_x - dvwWidth - 10;
				}
			
				declarationviewwindow.Move (horiz, vert);
			}
		}
	}
}
