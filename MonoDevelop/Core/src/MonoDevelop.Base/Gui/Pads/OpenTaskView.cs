// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;

using MonoDevelop.Core.Services;
using MonoDevelop.Services;
using MonoDevelop.Core.Properties;
using MonoDevelop.Internal.Project;

using Gtk;

namespace MonoDevelop.Gui.Pads
{
	public class OpenTaskView : IPadContent
	{
		ScrolledWindow sw;
		Gtk.TreeView view;
		ListStore store;
		Clipboard clipboard;
		Hashtable tasks = new Hashtable ();
		
		public Gtk.Widget Control {
			get {
				return sw;
			}
		}
		
		public string Title {
			get {
				return GettextCatalog.GetString ("Task List");
			}
		}
		
		public string Icon {
			get {
				return MonoDevelop.Gui.Stock.TaskListIcon;
			}
		}
		
		public void RedrawContent()
		{
			// FIXME
		}

		const int COL_TYPE = 0, COL_LINE = 1, COL_DESC = 2, COL_PATH = 3, COL_FILE = 4, COL_TASK = 5, COL_READ = 6, COL_MARKED = 7, COL_READ_WEIGHT = 8;
		
		public OpenTaskView()
		{
			store = new Gtk.ListStore (
				typeof (Gdk.Pixbuf), // image
				typeof (int),        // line
				typeof (string),     // desc
				typeof (string),     // path
				typeof (string),     // file
				typeof (Task),       // task
				typeof (bool),       // read?
				typeof (bool),       // marked?
				typeof (int));       // read? -- use Pango weight
				
			view = new Gtk.TreeView (store);
			view.RulesHint = true;
			view.PopupMenu += OnPopupMenu;
			view.ButtonPressEvent += OnButtonPressed;
			AddColumns ();
			
			sw = new Gtk.ScrolledWindow ();
			sw.ShadowType = ShadowType.In;
			sw.Add (view);
			
			Runtime.TaskService.TasksChanged     += (EventHandler) Runtime.DispatchService.GuiDispatch (new EventHandler (ShowResults));
			Runtime.TaskService.TaskAdded        += (TaskEventHandler) Runtime.DispatchService.GuiDispatch (new TaskEventHandler (TaskAdded));
			Runtime.ProjectService.EndBuild      += (ProjectCompileEventHandler) Runtime.DispatchService.GuiDispatch (new ProjectCompileEventHandler (SelectTaskView));
			Runtime.ProjectService.CombineOpened += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler (OnCombineOpen));
			Runtime.ProjectService.CombineClosed += (CombineEventHandler) Runtime.DispatchService.GuiDispatch (new CombineEventHandler (OnCombineClosed));
			view.RowActivated            += new RowActivatedHandler (OnRowActivated);
		}

		[GLib.ConnectBefore]
		void OnButtonPressed (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3)
				ShowPopup ();
		}

		void OnPopupMenu (object o, PopupMenuArgs args)
		{
			ShowPopup ();
		}

		void ShowPopup ()
		{
			Menu menu = new Menu ();
			menu.AccelGroup = new AccelGroup ();
                        ImageMenuItem copy = new ImageMenuItem (Gtk.Stock.Copy, menu.AccelGroup);
                        copy.Activated += OnTaskCopied;
			menu.Append (copy);
			menu.Popup (null, null, null, IntPtr.Zero, 3, Global.CurrentEventTime);
			menu.ShowAll ();
		}

		void OnTaskCopied (object o, EventArgs args)
		{
			Task task;
			TreeModel model;
			TreeIter iter;

			if (view.Selection.GetSelected (out model, out iter))
			{
				task = (Task) model.GetValue (iter, 5);
			}
			else
			{
				// no selection
				return;
			}

			clipboard = Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false));
			clipboard.SetText (task.ToString ());
			clipboard = Clipboard.Get (Gdk.Atom.Intern ("PRIMARY", false));
			clipboard.SetText (task.ToString ());
		}
		
		void MarkupCol (Gtk.TreeViewColumn col)
		{
		}
		
		void AddColumns ()
		{
			Gtk.CellRendererPixbuf iconRender = new Gtk.CellRendererPixbuf ();
			
			Gtk.CellRendererToggle toggleRender = new Gtk.CellRendererToggle ();
			toggleRender.Toggled += new ToggledHandler (ItemToggled);
			
			Gtk.CellRendererText line = new Gtk.CellRendererText (), desc = new Gtk.CellRendererText () , path = new Gtk.CellRendererText (),
			  file = new Gtk.CellRendererText ();
			
			view.AppendColumn ("!"                                        , iconRender   , "pixbuf", COL_TYPE);
			view.AppendColumn (""                                         , toggleRender , "active"  , COL_MARKED, "activatable", COL_READ);
			view.AppendColumn (GettextCatalog.GetString ("Line")        , line         , "text"    , COL_LINE, "weight", COL_READ_WEIGHT);
			view.AppendColumn (GettextCatalog.GetString ("Description") , desc         , "text"    , COL_DESC, "weight", COL_READ_WEIGHT, "strikethrough", COL_MARKED);
			view.AppendColumn (GettextCatalog.GetString ("Path")        , path         , "text"    , COL_PATH, "weight", COL_READ_WEIGHT);
			view.AppendColumn (GettextCatalog.GetString ("File")        , file         , "text"    , COL_FILE, "weight", COL_READ_WEIGHT);
		}
		
		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			store.Clear ();
		}
		
		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			store.Clear ();
		}
		
		public void Dispose ()
		{
		}
		
		void SelectTaskView (bool success)
		{
			if (Runtime.TaskService.Tasks.Count > 0) {
				try {
					if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible (this)) {
						WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
					} else if ((bool) Runtime.Properties.GetProperty ("SharpDevelop.ShowTaskListAfterBuild", true)) {
						WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad (this);
						WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
					}
				} catch {}
			}
		}
		
		void OnRowActivated (object o, RowActivatedArgs args)
		{
			Gtk.TreeIter iter;
			if (store.GetIter (out iter, args.Path)) {
				store.SetValue (iter, COL_READ, true);
				store.SetValue (iter, COL_READ_WEIGHT, (int) Pango.Weight.Normal);
				
				((Task) store.GetValue (iter, COL_TASK)).JumpToPosition ();
			}
		}
		
		public CompilerResults CompilerResults = null;
		
		public void ShowResults (object sender, EventArgs e)
		{
			store.Clear ();
			tasks.Clear ();
			
			foreach (Task t in Runtime.TaskService.Tasks) {
				AddTask (t);
			}
			SelectTaskView(true);
		}
		
		void TaskAdded (object sender, TaskEventArgs e)
		{
			AddTask (e.Task);
		}
		
		public void AddTask (Task t)
		{
			if (tasks.Contains (t)) return;
			tasks [t] = t;
			
			Gdk.Pixbuf stock;
			switch (t.TaskType) {
				case TaskType.Warning:
					stock = sw.RenderIcon (Gtk.Stock.DialogWarning, Gtk.IconSize.SmallToolbar, "");
					break;
				case TaskType.Error:
					stock = sw.RenderIcon (Gtk.Stock.DialogError, Gtk.IconSize.SmallToolbar, "");
					break;
				case TaskType.Comment:
					stock = sw.RenderIcon (Gtk.Stock.DialogInfo, Gtk.IconSize.SmallToolbar, "");
					break;
				case TaskType.SearchResult:
					stock = sw.RenderIcon (Gtk.Stock.DialogQuestion, Gtk.IconSize.SmallToolbar, "");
					break;
				default:
					stock = null;
					break;
			}
			
			string tmpPath = t.FileName;
			if (t.Project != null)
				tmpPath = Runtime.FileUtilityService.AbsoluteToRelativePath (t.Project.BaseDirectory, t.FileName);
			
			string fileName = tmpPath;
			string path     = tmpPath;
			
			try {
				fileName = Path.GetFileName(tmpPath);
			} catch (Exception) {}
			
			try {
				path = Path.GetDirectoryName(tmpPath);
			} catch (Exception) {}
			
			TreeIter iter = store.AppendValues (
				stock,
				t.Line + 1,
				t.Description,
				path,
				fileName,
				t, false, false, (int) Pango.Weight.Bold);
		}
		
		protected virtual void OnTitleChanged (EventArgs e)
		{
			if (TitleChanged != null)
				TitleChanged(this, e);
		}
		
		protected virtual void OnIconChanged (EventArgs e)
		{
			if (IconChanged != null)
				IconChanged (this, e);
		}
		
		public event EventHandler TitleChanged, IconChanged;
		
		public void BringToFront ()
		{
			if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible (this))
				WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad (this);
			
			WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
		}
		
		private void ItemToggled (object o, ToggledArgs args)
		{
			Gtk.TreeIter iter;
			if (store.GetIterFromString(out iter, args.Path)) {
				bool val = (bool) store.GetValue(iter, COL_MARKED);
				store.SetValue(iter, COL_MARKED, !val);
			}
		}

	}
}
