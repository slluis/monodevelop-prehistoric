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
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using Gtk;
using GtkSharp;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui.Pads {
	public class OpenTaskView : IPadContent {
		
		ResourceService resourceService = (ResourceService) ServiceManager.Services.GetService (typeof (IResourceService));
		Gtk.ScrolledWindow sw;
		Gtk.TreeView view;
		Gtk.ListStore store;
		
		public Gtk.Widget Control {
			get {
				return sw;
			}
		}
		
		public string Title {
			get {
				return resourceService.GetString ("MainWindow.Windows.TaskList");
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
			TaskService taskService        = (TaskService) ServiceManager.Services.GetService (typeof(TaskService));
			IProjectService projectService = (IProjectService) ServiceManager.Services.GetService (typeof(IProjectService));
			

			
			
			store = new Gtk.ListStore (
				typeof (string),     // stock id
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
			AddColumns ();
			
			sw = new Gtk.ScrolledWindow ();
			sw.ShadowType = ShadowType.In;
			sw.Add (view);
			
			taskService.TasksChanged     += new EventHandler (ShowResults);
			projectService.EndBuild      += new EventHandler (SelectTaskView);
			projectService.CombineOpened += new CombineEventHandler (OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler (OnCombineClosed);
			view.RowActivated            += new RowActivatedHandler (OnRowActivated);
		}
		
		string res (string s)
		{
			return resourceService.GetString (s);
		}
		
		void MarkupCol (Gtk.TreeViewColumn col)
		{
		}
		
		void AddColumns ()
		{
			Gtk.CellRendererPixbuf iconRender = new Gtk.CellRendererPixbuf ();
			iconRender.StockSize = Gtk.IconSize.SmallToolbar;
			
			Gtk.CellRendererToggle toggleRender = new Gtk.CellRendererToggle ();
			toggleRender.Toggled += new ToggledHandler (ItemToggled);
			
			Gtk.CellRendererText line = new Gtk.CellRendererText (), desc = new Gtk.CellRendererText () , path = new Gtk.CellRendererText (),
			  file = new Gtk.CellRendererText ();
			
			view.AppendColumn ("!"                                        , iconRender   , "stock-id", COL_TYPE);
			view.AppendColumn (""                                         , toggleRender , "active"  , COL_MARKED, "activatable", COL_READ);
			view.AppendColumn (res ("CompilerResultView.LineText")        , line         , "text"    , COL_LINE, "weight", COL_READ_WEIGHT);
			view.AppendColumn (res ("CompilerResultView.DescriptionText") , desc         , "text"    , COL_DESC, "weight", COL_READ_WEIGHT, "strikethrough", COL_MARKED);
			view.AppendColumn (res ("CompilerResultView.PathText")        , path         , "text"    , COL_PATH, "weight", COL_READ_WEIGHT);
			view.AppendColumn (res ("CompilerResultView.FileText")        , file         , "text"    , COL_FILE, "weight", COL_READ_WEIGHT);
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
		
		void SelectTaskView (object sender, EventArgs e)
		{
			TaskService taskService = (TaskService) ServiceManager.Services.GetService (typeof (TaskService));
			if (taskService.Tasks.Count > 0) {
				try {
					PropertyService propertyService = (PropertyService) ServiceManager.Services.GetService (typeof (PropertyService));
					if (WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible (this)) {
						WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad (this);
					} else if ((bool) propertyService.GetProperty ("SharpDevelop.ShowTaskListAfterBuild", true)) {
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
		
		public void ShowResults(object sender, EventArgs e)
		{
			store.Clear ();
			FileUtilityService fileUtilityService = (FileUtilityService) ServiceManager.Services.GetService (typeof (FileUtilityService));
			TaskService taskService = (TaskService) ServiceManager.Services.GetService (typeof (TaskService));
			
			foreach (Task t in taskService.Tasks) {
				string stock;
				switch (t.TaskType) {
					case TaskType.Warning      : stock = Gtk.Stock.DialogWarning  ; break;
					case TaskType.Error        : stock = Gtk.Stock.DialogError    ; break;
					case TaskType.Comment      : stock = Gtk.Stock.DialogInfo     ; break;
					case TaskType.SearchResult : stock = Gtk.Stock.DialogQuestion ; break;
					default                    : stock = null                     ; break;
				}
				
				string tmpPath = t.FileName;
				if (t.Project != null)
					tmpPath = fileUtilityService.AbsoluteToRelativePath (t.Project.BaseDirectory, t.FileName);
				
				string fileName = tmpPath;
				string path     = tmpPath;
				
				try {
					fileName = Path.GetFileName(tmpPath);
				} catch (Exception) {}
				
				try {
					path = Path.GetDirectoryName(tmpPath);
				} catch (Exception) {}
				
				store.AppendValues (
					stock,
					t.Line + 1,
					t.Description,
					path,
					fileName,
					t, false, false, (int) Pango.Weight.Bold);
			}
			SelectTaskView(null, null);
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
